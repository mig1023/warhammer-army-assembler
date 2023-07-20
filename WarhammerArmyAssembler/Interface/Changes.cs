using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Linq;

using static WarhammerArmyAssembler.ArmyBook.Parsers;
using static WarhammerArmyAssembler.ArmyBook.Services;

namespace WarhammerArmyAssembler.Interface
{
    class Changes
    {
        public static MainWindow main = null;
        public static ChangeArmybookWindow changeArmybook = new ChangeArmybookWindow();
        public static ObservableCollection<Unit> ArmyInInterface = new ObservableCollection<Unit>();

        public static object DragSender = null;
        public static string CurrentSelectedArmy = null;
        public static int CurrentSelectedUnit = -1;
        public static Brush CurrentSelectedArmyBackColor = null;

        public static Image lastImage = null;
        public static string lastArmy = String.Empty;
        public static Dictionary<Image, string> allImages = new Dictionary<Image, string>();
        public static Dictionary<string, string> allArmies = new Dictionary<string, string>();

        public enum MovingType { ToMain, ToRight, ToLeft, ToTop, ToMainMenu }

        public static List<Label> MainMenuButtons = new List<Label>();

        public static bool mainMenuIsOpen = false;
        public static bool armybookDetailIsOpen = false;
        public static bool unitTestIsOpen = false;

        public static Thickness Thick(object element, double? left = null, double? top = null, double? right = null, double? bottom = null)
        {
            FrameworkElement control = element as FrameworkElement;

            double newLeft = left ?? control.Margin.Left;
            double newTop = top ?? control.Margin.Top;
            double newRight = right ?? control.Margin.Right;
            double newBottom = bottom ?? control.Margin.Bottom;

            return new Thickness(newLeft, newTop, newRight, newBottom);
        }

        public static void ArmyGridDrop(int id, DataGridRow container = null, double points = 0, int unit = 0)
        {
            if (ArmyBook.Data.Artefact.ContainsKey(id))
                ArmyGridDropArtefact(id, container);

            else if (ArmyBook.Data.Mounts.ContainsKey(id))
                ArmyGridDropMount(id, points, unit);

            else
                ArmyGridDropUnit(id);

            Army.Mod.ChangeGeneralIfNeed();
        }

        public static void ArmyGridDropArtefact(int id, DataGridRow container)
        {
            if (container == null)
                return;

            Unit unit = container.DataContext as Unit;
            ArmyGridDropArtefact(id, unit.ID);
        }

        public static void ArmyGridDropArtefact(int id, int unitID)
        {
            Option prevRunicItem = null;
            double prevRunicPointsPenalty = 0;

            Unit unit = Army.Data.Units[unitID];
            Option artefact = ArmyBook.Data.Artefact[id];

            if (!String.IsNullOrEmpty(artefact.RandomGroup))
            {
                List<Option> group = artefact.AllRandomByGroup();

                do id = group[WarhammerArmyAssembler.Test.Data.rand.Next(0, group.Count)].ID;
                while (Army.Data.Units[unitID].Options.Where(x => x.ID == id).Count() > 0);
            }
            else if (artefact.Runic > 0)
            {
                Dictionary<int, Option> versions = artefact.AllRunicVersions();

                Option currentItem = unit.GetCurrentRunicItemByName(artefact.Name);

                if ((currentItem != null) && (currentItem.Runic >= versions.Count))
                    return;

                else if (currentItem != null)
                {
                    prevRunicItem = currentItem;
                    prevRunicPointsPenalty = currentItem.Points;

                    id = versions[currentItem.Runic + 1].ID;
                }

                if (Army.Checks.IsRunicCombinationAlreadyExist(unit, ArmyBook.Data.Artefact[id]))
                {
                    Error("No more than one item may carry the same combination of runes");
                    return;
                }
            }

            if (artefact.Virtue)
                artefact.Points = Army.Params.GetVirtuePoints(id);

            if (!Checks.EnoughPointsForAddArtefact(id, prevRunicPointsPenalty))
            {
                Error("Not enough points add an item");
            }
            else if (!Checks.EnoughUnitPointsForAddArtefact(id, unit, pointsPenalty: prevRunicPointsPenalty))
            {
                string type = unit.MagicItemCount > 0 ? "slots" : "points";
                Error($"Not enough magic item {type} to add an item");
            }
            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(unit.Type, artefact.Points, 0))
            {
                Error("For this type, a point cost limit has been reached");
            }
            else if (artefact.TypeUnitIncrese && Army.Checks.IsArmyFullForTypeIcrease(unit))
            {
                Error("Cant upgrade unit type: the army already has many such units");
            }
            else
            {
                if (prevRunicItem != null)
                    unit.Options.Remove(prevRunicItem);

                if (ArmyBook.Data.Artefact[id].TypeUnitIncrese && (unit.Type == Unit.UnitType.Core))
                    unit.Type = Unit.UnitType.Special;

                else if (ArmyBook.Data.Artefact[id].TypeUnitIncrese && (unit.Type == Unit.UnitType.Special))
                    unit.Type = Unit.UnitType.Rare;

                unit.AddAmmunition(id);
                Reload.ReloadArmyData();
                Details.UpdateUnitDescription(unitID, unit);

                bool multiple = artefact.Multiple || artefact.Virtue || (artefact.Runic > 0);
                bool honours = artefact.Honours && (artefact.Points > 0);

                if (!multiple && !honours && (artefact.Type != Option.OptionType.Powers) && String.IsNullOrEmpty(artefact.RandomGroup))
                    Mod.SetArtefactAlreadyUsed(id, true);

                Army.Mod.ChangeGeneralIfNeed();
            }
        }

        public static void ArmyGridDropUnit(int id)
        {
            Unit unit = ArmyBook.Data.Units[id];

            bool slotExists = Army.Params.GetArmyUnitsNumber(unit.Type) < Army.Params.GetArmyMaxUnitsNumber(unit.Type);
            bool coreUnit = unit.Type == Unit.UnitType.Core;

            int allHeroes = Army.Params.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.Params.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (unit.Type == Unit.UnitType.Hero) && (allHeroes >= Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if (unit.Character && Army.Checks.IsUnitExistInArmyByArmyBookID(id))
            {
                Error("Personalities cannot be repeated");
            }
            else if ((!slotExists && !coreUnit) || lordInHeroSlot)
            {
                string type = unit.IsHero() ? "heroes" : "units";
                Error($"The number of {type} of this type has been exhausted.");
            }
            else if (!Checks.EnoughPointsForAddUnit(id))
            {
                string type = unit.IsHero() ? "hero" : "unit";
                Error($"Not enough points to add a {type}");
            }
            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(unit.Type, unit.Points, unit.StaticPoints))
            {
                string type = unit.UnitTypeName();
                Error($"The {type} has reached a point cost limit");
            }
            else if (!Army.Checks.IsArmyDublicationOk(unit))
            {
                string type = unit.UnitTypeName();
                Error($"Army can't include as many duplicates of {type}");
            }
            else if (!Army.Checks.IsArmyUnitMaxLimitOk(unit))
            {
                Error("Only one unit of this type can be in the army");
            }
            else
            {
                CurrentSelectedUnit = Army.Mod.AddUnitByID(id);
                Reload.ReloadArmyData();
            }
        }

        public static void ArmyGridDropMount(int id, double points, int unit)
        {
            Unit currentUnit = Army.Data.Units[unit];

            if (!Checks.EnoughUnitPointsForAddOption(points))
            {
                Error("Not enough points to add a mount");
            }
            else if (currentUnit.MountOn > 0)
            {
                Error("The hero already has a mount");
            }
            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(currentUnit.Type, points, currentUnit.StaticPoints))
            {
                string type = currentUnit.UnitTypeName();
                Error($"The {type} has reached a point cost limit");
            }
            else
            {
                Army.Mod.AddMountByID(id, unit);
                Reload.ReloadArmyData();
            }
        }

        public static void UnitDeleteDrop(int id)
        {
            DetailResize(open: false);
            Army.Mod.DeleteUnitByID(id);
            Reload.ReloadArmyData();
        }

        public static void AllUnitDelete()
        {
            DetailResize(open: false);
            Army.Mod.DeleteAllUnits();
            Reload.ReloadArmyData();
        }

        public static void Error(string text)
        {
            main.errorText.Content = text;
            Move(MovingType.ToTop, err: true);
        }

        public static void CheckAndExportTo(bool toPDF = false, bool fullRules = false)
        {
            if (!Army.Checks.IsArmyValid())
                MessageBox.Show(Army.Checks.ArmyProblems());

            else if (toPDF)
                Export.PDF.SaveArmy(fullRules);

            else
                Export.Text.SaveArmy();

            Move(MovingType.ToMain, menu: true);
        }

        public static void MainMenu()
        {
            Dictionary<string, MouseButtonEventHandler> buttons = new Dictionary<string, MouseButtonEventHandler>
            {
                ["Change Armybook"] = main.toNewArmy_MouseDown,
                ["Export Army to PDF"] = main.saveArmyToPDF_MouseDown,
                ["Export Army to PDF (light)"] = main.saveArmyToPDF_light_MouseDown,
                ["Export Army to TXT"] = main.saveArmyToTXT_MouseDown,
                ["Exit"] = main.closeWindow_MouseLeftButtonDown,
                ["Close"] = main.closeMainMenu_MouseDown,
            };

            foreach (Label button in MainMenuButtons)
                main.mainMenu.Children.Remove(button);

            MainMenuButtons.Clear();

            double buttonXPosition = 15;
            double buttonYPosition = 10;

            foreach (string name in buttons.Keys)
            {
                Label newButton = new Label
                {
                    Content = name,
                    Height = 30,
                    Width = Double.NaN,
                    Foreground = Brushes.White,
                    Background = (name == "Close" ? Brushes.DarkGray : ArmyBook.Data.FrontColor),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                };

                newButton.MouseDown += buttons[name];
                newButton.Margin = Thick(newButton, buttonXPosition, buttonYPosition);

                main.mainMenu.Children.Add(newButton);
                MainMenuButtons.Add(newButton);

                main.UpdateLayout();

                buttonXPosition += newButton.ActualWidth + 10;

                if (buttonXPosition + 10 >= main.mainMenu.ActualWidth)
                {
                    buttonXPosition = 15;
                    buttonYPosition += newButton.Height + 10;

                    newButton.Margin = Thick(newButton, buttonXPosition, buttonYPosition);
                    buttonXPosition += newButton.ActualWidth + 10;
                }
            }

            Move(MovingType.ToMainMenu, menu: true, height: buttonYPosition + 40);
        }

        public static void DetailResize(bool open)
        {
            if (open)
            {
                main.unitDetailScroll.Visibility = Visibility.Visible;
                main.mainGrid.RowDefinitions[2].Height = new GridLength(270);
                main.mainGrid.RowDefinitions[2].MinHeight = 170;
                main.unitDetailScrollSplitter.IsEnabled = true;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(5);
                main.unitDetailScrollSplitter.Height = 5;
            }
            else
            {
                main.unitDetailScroll.Visibility = Visibility.Hidden;
                main.mainGrid.RowDefinitions[2].MinHeight = 0;
                main.mainGrid.RowDefinitions[2].Height = new GridLength(0);
                main.unitDetailScrollSplitter.IsEnabled = false;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(0);
                main.unitDetailScrollSplitter.Height = 0;
            }
        }

        public static void Move(MovingType moveTo, EventHandler secondAnimation = null,
            bool err = false, bool menu = false, bool detail = false, double height = -1)
        {
            Thickness newPosition = new Thickness(0, 0, 0, 0);

            main.ResizeMode = detail ? ResizeMode.NoResize : ResizeMode.CanResizeWithGrip;

            if (menu)
                Mod.View(canvasToShow: main.mainMenu, top: true);

            if (err)
                Mod.View(canvasToShow: main.errorDetail, top: true);

            if (moveTo == MovingType.ToLeft)
            {
                Mod.View(left: true);
                newPosition = new Thickness(main.ActualWidth, 0, 0, 0);
            }
                
            if (moveTo == MovingType.ToRight)
            {
                Mod.View(right: true);
                newPosition = new Thickness((main.ActualWidth * -1), 0, 0, 0);
            }

            if (moveTo == MovingType.ToTop)
                newPosition = new Thickness(0, main.errorDetail.Height, 0, 0);

            if (moveTo == MovingType.ToMainMenu)
                newPosition = new Thickness(0, height, 0, 0);

            if ((moveTo == MovingType.ToMain) && unitTestIsOpen)
                Mod.UnitTestClose();

            bool mainCanvasMoving = (err || menu);

            ThicknessAnimation move = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2),
                From = (mainCanvasMoving ? main.mainPlaceCanvas.Margin : main.mainGrid.Margin),
                To = newPosition
            };

            if (secondAnimation != null)
                move.Completed += secondAnimation;

            if (mainCanvasMoving)
                main.mainPlaceCanvas.BeginAnimation(FrameworkElement.MarginProperty, move);
            else
                main.mainGrid.BeginAnimation(FrameworkElement.MarginProperty, move);

            if (menu && (moveTo == MovingType.ToMainMenu))
                mainMenuIsOpen = true;
            else if (menu)
                mainMenuIsOpen = false;
        }

        private static BitmapImage GetImage(string imageName, string armyName)
        {
            string path = Path.GetDirectoryName(armyName);
            return new BitmapImage(new Uri($"{path}\\Images\\{imageName}"));
        }

        public static void LoadArmyImage(XmlNode imageName, string armyName)
        {
            if (imageName != null)
                main.armySymbol.Source = GetImage(imageName.InnerText, armyName);
            else
                main.armySymbol.Source = null;
        }

        public static bool ConfirmedDataCleaning()
        {
            if (Army.Data.Units.Count <= 0)
                return true;

            string text = "All army data will be lost.\nContinue?";
            return MessageBox.Show(text, String.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        private static void ArmyChangesColors(Label label)
        {
            label.MouseEnter += (sender, e) => label.Background = CurrentSelectedArmyBackColor;
            label.MouseLeave += (sender, e) => label.Background = Brushes.White;
        }

        private static void InputPountsColors(Control button, Brush mainColor)
        {
            button.MouseEnter += (sender, e) =>
            {
                button.BorderBrush = mainColor;
                button.Foreground = mainColor;
                button.Background = Brushes.White;
            };

            button.MouseLeave += (sender, e) =>
            {
                button.BorderBrush = mainColor;
                button.Foreground = Brushes.White;
                button.Background = mainColor;
            };
        }

        private static void PreviewLoadCurrentSelectedArmy(string armyName)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(armyName);

            string armyFile = Intro(xmlFile, "Styles/Images/Files/Cover").InnerText;
            changeArmybook.imageArmybook.Source = GetImage(armyFile, armyName);
            changeArmybook.listArmybookVer.Content = String.Format("{0}th edition", Intro(xmlFile, "Info/Edition").InnerText);
            changeArmybook.UpdateLayout();

            Brush mainColor = Services.BrushFromXml(StyleColor(xmlFile, "Front"));

            CurrentSelectedArmyBackColor = Services.BrushFromXml(StyleColor(xmlFile, "Grid"));
            ArmyChangesColors(changeArmybook.prev);
            ArmyChangesColors(changeArmybook.next);

            changeArmybook.pointesTumbler.Stroke = mainColor;
            changeArmybook.pointesTumblerMark.Stroke = mainColor;

            List<Control> marks = new List<Control>
            {
                changeArmybook.mark500,
                changeArmybook.mark1000,
                changeArmybook.mark2000,
                changeArmybook.mark2500,
                changeArmybook.mark3000,
            };

            foreach (Control label in marks)
            {
                label.Foreground = mainColor;
            }

            List<Control> labels = new List<Control>
            {
                changeArmybook.next,
                changeArmybook.prev,
                changeArmybook.listArmybookPoints,
                changeArmybook.armyAdditionalName,
                changeArmybook.randomArmy,
                changeArmybook.resetSelection,
                changeArmybook.sortedBy,
            };

            foreach (Control label in labels)
            {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            labels = new List<Control>
            {
                changeArmybook.listArmybookVer,
                changeArmybook.buttonArmybook,
                changeArmybook.closeArmybook,
                main.closeArmybookDetail,
            };

            foreach (Control label in labels)
                label.Background = mainColor;

            InputPountsColors(changeArmybook.buttonArmybook, mainColor);
            changeArmybook.buttonArmybook.BorderBrush = mainColor;

            main.mainWindowHeader.Background = mainColor;
            changeArmybook.gridCloseArmybook.Background = mainColor;
            changeArmybook.showArmyAdditionalName.Foreground = mainColor;
        }

        public static void PreviewArmy(string armyName)
        {
            CurrentSelectedArmy = armyName;
            PreviewLoadCurrentSelectedArmy(armyName);
        }

        public static void PreviewArmyList(bool next = false, bool prev = false, bool reset = false)
        {
            string army = ArmyBook.XmlBook.GetXmlArmyBooks(next, prev);
            SetArmySelected(allArmies[army]);
            PreviewArmy(army);
        }

        public static void RandomArmy()
        {
            string randomArmy = allArmies.ElementAt(WarhammerArmyAssembler.Test.Data.rand.Next(0, allArmies.Count)).Key;

            SetArmySelected(allArmies[randomArmy]);
            PreviewArmy(randomArmy);
        }

        public static void LoadAllArmy(List<string> allXmlFiles, bool reload = false)
        {
            int left = -1, top = 0;

            Image lastImage = null;
            allImages.Clear();
            allArmies.Clear();

            foreach (string armyName in allXmlFiles)
            {
                XmlDocument xmlFile = new XmlDocument();
                xmlFile.Load(armyName);

                XmlNode armyFile = Intro(xmlFile, "Styles/Images/Files/Cover");
                string source = String.Format("{0}\\Images\\{1}", Path.GetDirectoryName(armyName), armyFile.InnerText);

                Image newImage = new Image()
                {
                    Source = new BitmapImage(new Uri(source)),
                    Margin = new Thickness(2),
                    Stretch = Stretch.UniformToFill,
                };

                string head = StringParse(Intro(xmlFile, "Info/Army")).ToUpper();
                string edition = StringParse(Intro(xmlFile, "Info/Edition"));
                string description = StringParse(Intro(xmlFile, "Info/Description"));
                string authors = StringParse(Intro(xmlFile, "Info/Authors"));
                int released = IntParse(Intro(xmlFile, "Info/Released"));
                string illustration = StringParse(Intro(xmlFile, "Styles/Images/Files/Illustration"));

                Brush backColor = Services.BrushFromXml(StyleColor(xmlFile, "Tooltip"));
                Brush lineColor = Services.BrushFromXml(StyleColor(xmlFile, "Front"));

                if (String.IsNullOrEmpty(illustration))
                    illustration = source;
                else
                    illustration = String.Format("{0}\\Images\\{1}", Path.GetDirectoryName(armyName), illustration);

                int armyEdition = int.Parse(edition);

                newImage.ToolTip = new ToolTip
                {
                    MaxWidth = 600,
                    Background = backColor,
                    Content = new Border
                    {
                        Padding = new Thickness(5),
                        Child = TooltipBlock(head, armyEdition, description, illustration, released, authors, lineColor),
                    }
                };

                left += 1;

                if (left > 3)
                {
                    left = 0;
                    top += 1;

                    if (!reload)
                    {
                        ColumnDefinition column = new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) };
                        changeArmybook.armybookList.ColumnDefinitions.Add(column);
                    }
                }

                if (changeArmybook.armybookList.ColumnDefinitions.Count <= left)
                    changeArmybook.armybookList.ColumnDefinitions.Add(new ColumnDefinition());

                if (changeArmybook.armybookList.RowDefinitions.Count <= top)
                    changeArmybook.armybookList.RowDefinitions.Add(new RowDefinition());

                newImage.SetValue(Grid.RowProperty, top);
                newImage.SetValue(Grid.ColumnProperty, left);

                newImage.MouseDown += (sender, e) => Armybook_Click(armyName, source);

                changeArmybook.armybookList.Children.Add(newImage);
                allImages.Add(newImage, source);
                allArmies.Add(armyName, source);

                lastImage = newImage;
            }

            changeArmybook.UpdateLayout();

            Grid.SetRow(changeArmybook.armybookList, (int)lastImage.ActualHeight);           
        }

        private static void Armybook_Click(string armyName, string source)
        {
            PreviewArmy(armyName);
            SetArmySelected(source);
        }

        public static void SetArmySelected(string armySource)
        {
            foreach (KeyValuePair<Image, string> image in allImages)
            {
                if (String.IsNullOrEmpty(armySource) || (armySource == image.Value))
                    image.Key.Source = new BitmapImage(new Uri(image.Value));

                else
                {
                    FormatConvertedBitmap bwImage = new FormatConvertedBitmap();
                    bwImage.BeginInit();
                    bwImage.Source = new BitmapImage(new Uri(image.Value));
                    bwImage.DestinationFormat = PixelFormats.Gray8;
                    bwImage.EndInit();

                    image.Key.Source = bwImage;
                }
            }
        }

        public static double ZeroFuse(double currentParam) =>
            currentParam < 0 ? 0 : currentParam;

        private static StackPanel TooltipBlock(string head, int edition, string description,
            string image, int released, string authors, Brush lineColor) => new StackPanel
        {
            Children =
            {
                new Image
                {
                    Source = new BitmapImage(new Uri(image)),
                },
                new TextBlock
                {
                    Text = head,
                    FontFamily = new FontFamily("Impact"),
                    FontSize = 64,
                    Margin = new Thickness(-3, 4, 0, 0),
                    Foreground = lineColor,
                },
                new TextBlock
                {
                    Text = String.Format("{0}th Edition", edition),
                    FontSize = 14,
                    Margin = new Thickness(0, -7, 0, 0),
                },
                new TextBlock
                {
                    Text = String.Format("\n{0}\n", description),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 9,
                    TextAlignment = TextAlignment.Justify,
                },
                new System.Windows.Shapes.Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 1,
                    Fill = lineColor,
                },
                TooltipFooter(released, authors, lineColor),
            }
        };

        private static Grid TooltipFooter(int released, string authors, Brush lineColor)
        {
            Grid footer = new Grid { Margin = new Thickness(0, 3, 0, 0) };

            footer.ColumnDefinitions.Add(new ColumnDefinition());
            footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(370) });
            footer.ColumnDefinitions.Add(new ColumnDefinition());

            footer.RowDefinitions.Add(new RowDefinition());

            footer.Children.Add(FooterElement(String.Format("Released: {0}", released), HorizontalAlignment.Left, 0, lineColor));
            footer.Children.Add(FooterElement(String.Format("Written by {0}", authors), HorizontalAlignment.Center, 1, lineColor));
            footer.Children.Add(FooterElement("© Games Workshop", HorizontalAlignment.Right, 2, lineColor));

            return footer;
        }

        private static TextBlock FooterElement(string text, HorizontalAlignment aligment, int column, Brush lineColor)
        {
            TextBlock element = new TextBlock();

            element.Text = text;
            element.HorizontalAlignment = aligment;
            element.FontSize = 11;
            element.Foreground = lineColor;

            Grid.SetColumn(element, column);
            Grid.SetRow(element, 0);

            return element;
        }

        public static void SetContentDescription(object obj)
        {
            if (obj is Unit)
            {
                Unit unit = obj as Unit;

                main.armyArtefactName.Content = unit.Name.ToUpper();
                main.armyArtefactDescription.Text = unit.Description;
                main.armyArtefactSpecific.Text = unit.SelfDescription();
            }
            else
            {
                Option option = obj as Option;

                main.armyArtefactName.Content = option.Name.ToUpper();
                main.armyArtefactDescription.Text = option.Description;
                main.armyArtefactSpecific.Text = option.SelfDescription();
            }

            main.armyArtefactName.Foreground = Brushes.White;
            main.armyArtefactName.Background = ArmyBook.Data.FrontColor;
            main.armyArtefactName.FontWeight = FontWeights.Bold;

            main.armybookArtefactDetailScroll.Visibility = Visibility.Visible;
            main.armybookDetailScroll.Visibility = Visibility.Hidden;
        }

        public static string TryHomologueImage(Unit unit)
        {
            List<string> homologuesLine = ArmyBook.XmlBook.GetHomologue(Army.Data.Internal,
                unit.Name, unit.Homologue, unit.IsHero());

            foreach (string homologueImage in homologuesLine)
            {
                if (String.IsNullOrEmpty(homologueImage))
                    continue;

                else if (homologueImage.EndsWith(".jpg"))
                    return homologueImage;

                else
                    return String.Format("{0}.jpg", homologueImage);
            }

            return String.Empty;
        }

        public static void SetContentDescriptionWithImage(Unit unit)
        {
            main.armyUnitName.Content = unit.Name.ToUpper();
            main.armyUnitDescription.Text = unit.Description;
            main.armyUnitSpecific.Text = unit.SelfDescription();
            main.detailUnitGrid.DataContext = unit.GetOptionRules(out bool hasMods);

            main.UpdateLayout();

            bool portrait = main.armyUnitImage.ActualWidth < main.armyUnitImage.ActualHeight;
            main.armyUnitImage.MaxWidth = unit.IsHeroOrHisMount() && portrait ? 450 : double.PositiveInfinity;

            main.armyUnitName.Foreground = Brushes.White;
            main.armyUnitName.Background = ArmyBook.Data.FrontColor;
            main.armyUnitName.FontWeight = FontWeights.Bold;

            main.armybookDetailScroll.Visibility = Visibility.Visible;
            main.armybookArtefactDetailScroll.Visibility = Visibility.Hidden;

            if (hasMods)
            {
                main.profileMods.Text = "↑ modified by artefacts or special rules";
                main.profileMods.Visibility = Visibility.Visible;
            }
            else
            {
                main.profileMods.Text = String.Empty;
                main.profileMods.Visibility = Visibility.Hidden;
            }

            if (unit.ImageFromAnotherEdition)
            {
                main.profileMods.Visibility = Visibility.Visible;

                if (!String.IsNullOrEmpty(main.profileMods.Text))
                    main.profileMods.Text += "\n";

                main.profileMods.Text += "← picture from another edition";
            }
        }

        public static void armyUnitTest_Resize()
        {
            main.UpdateLayout();

            double marginTop = ZeroFuse(main.unitGrid.ActualHeight - 66);

            main.specialRulesTest.Margin = Thick(main.specialRulesTest, top: marginTop);

            marginTop += main.specialRulesTest.ActualHeight;

            List<FrameworkElement> elements = new List<FrameworkElement>
            {
                main.enemyForTestText,
                main.enemyForTest,
                main.enemyTestUnit,
                main.enemyGridContainer,
                main.enemyGroupText,
                main.enemyGroup,
                main.startBattleRoyale
            };

            foreach (FrameworkElement element in elements)
                element.Margin = Thick(main.enemyForTestText, top: marginTop);

            marginTop += ZeroFuse(main.enemyGrid.ActualHeight - 66);

            main.specialRulesEnemyTest.Margin = Thick(main.enemyForTestText, top: marginTop);

            elements = new List<FrameworkElement>
            {
                main.startFullTest,
                main.startStatisticTest,
                main.testConsole,
            };

            foreach (FrameworkElement element in elements)
                element.Margin = Thick(main.enemyForTestText, top: marginTop + main.specialRulesEnemyTest.ActualHeight - 20);

            double unitTestHeight = (double)main.enemyForTest.GetValue(Canvas.TopProperty) +
                main.enemyForTest.ActualHeight + 50;

            double royalConsoleSize = 0;

            if (main.enemyGridContainer.Visibility == Visibility.Visible)
            {
                double startButtonPosition = (double)main.startFullTest.GetValue(Canvas.TopProperty);
                unitTestHeight = main.startFullTest.Margin.Top + main.startFullTest.ActualHeight + startButtonPosition + 20;

                main.startBattleRoyale.Visibility = Visibility.Hidden;
            }
            else
            {
                main.testConsole.Margin = Thick(main.enemyForTestText, top: marginTop - 155);
                royalConsoleSize = -70;

                main.startBattleRoyale.Visibility = Visibility.Visible;
            }

            if (unitTestHeight + 140 < main.armyUnitTestScroll.ActualHeight)
            {
                main.testConsole.Height = ZeroFuse(
                    main.armyUnitTestScroll.ActualHeight - unitTestHeight - 20) + royalConsoleSize;
            }
            else
            {
                unitTestHeight += 140;
                main.testConsole.Height = 120 + royalConsoleSize;
            }

            main.waitingSpinner.Margin = Thick(main.testConsole,
                top: main.testConsole.Margin.Top - Services.SPINNER_TOP_MARGIN,
                left: main.testConsole.Margin.Left - Services.SPINNER_LEFT_MARGIN);

            main.armyUnitTest.Height = unitTestHeight;
        }

        public static void armyUnitTest_SizeChanged(SizeChangedEventArgs e)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>
            {
                main.unitGrid,
                main.specialRulesTest,
                main.enemyForTest,
                main.enemyGrid,
                main.specialRulesEnemyTest,
                main.testConsole,
                main.enemyGroup
            };

            foreach (FrameworkElement element in elements)
                element.Width = ZeroFuse(e.NewSize.Width - 120);

            armyUnitTest_Resize();
        }
    }
}
