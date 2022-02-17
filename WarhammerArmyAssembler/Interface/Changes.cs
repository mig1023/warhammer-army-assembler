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

        public static List<Label> PointsButtons = new List<Label>();
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

                do id = group[Test.Data.rand.Next(0, group.Count)].ID;
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
                Error("Not enough points add an item");

            else if (!Checks.EnoughUnitPointsForAddArtefact(id, unit, pointsPenalty: prevRunicPointsPenalty))
                Error(String.Format("Not enough magic item {0} to add an item", (unit.MagicItemCount > 0 ? "slots" : "points")));

            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(unit.Type, artefact.Points))
                Error("For this type, a point cost limit has been reached");

            else if (artefact.TypeUnitIncrese && Army.Checks.IsArmyFullForTypeIcrease(unit))
                Error("Cant upgrade unit type: the army already has many such units");

            else
            {
                if (prevRunicItem != null)
                    unit.Options.Remove(prevRunicItem);

                if ((ArmyBook.Data.Artefact[id].TypeUnitIncrese) && (unit.Type == Unit.UnitType.Core))
                    unit.Type = Unit.UnitType.Special;

                else if ((ArmyBook.Data.Artefact[id].TypeUnitIncrese) && (unit.Type == Unit.UnitType.Special))
                    unit.Type = Unit.UnitType.Rare;

                unit.AddAmmunition(id);
                Reload.ReloadArmyData();
                UnitDetails.UpdateUnitDescription(unitID, unit);

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

            bool slotExists = (Army.Params.GetArmyUnitsNumber(unit.Type) < Army.Params.GetArmyMaxUnitsNumber(unit.Type));
            bool coreUnit = (unit.Type == Unit.UnitType.Core);

            int allHeroes = Army.Params.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.Params.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (unit.Type == Unit.UnitType.Hero) && (allHeroes >= Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if (unit.Personified && Army.Checks.IsUnitExistInArmyByArmyBookID(id))
                Error("Personalities cannot be repeated");

            else if ((!slotExists && !coreUnit) || lordInHeroSlot)
                Error(String.Format("The number of {0} of this type has been exhausted.", (unit.IsHero() ? "heroes" : "units")));

            else if (!Checks.EnoughPointsForAddUnit(id))
                Error(String.Format("Not enough points to add a {0}", (unit.IsHero() ? "hero" : "unit")));

            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(unit.Type, unit.Points))
                Error(String.Format("The {0} has reached a point cost limit", unit.UnitTypeName()));

            else if (!Army.Checks.IsArmyDublicationOk(unit))
                Error(String.Format("Army can't include as many duplicates of {0}", unit.UnitTypeName()));

            else if (!Army.Checks.IsArmyUnitMaxLimitOk(unit))
                Error("Only one unit of this type can be in the army");

            else
            {
                CurrentSelectedUnit = Army.Mod.AddUnitByID(id);
                Reload.ReloadArmyData();
            }
        }

        public static void ArmyGridDropMount(int id, double points, int unit)
        {
            if (!Checks.EnoughUnitPointsForAddOption(points))
                Error("Not enough points to add a mount");

            else if (Army.Data.Units[unit].MountOn > 0)
                Error("The hero already has a mount");

            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(Army.Data.Units[unit].Type, points))
                Error(String.Format("The {0} has reached a point cost limit", ArmyBook.Data.Units[id].UnitTypeName()));

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
                main.unitDetailScrollSlitter.IsEnabled = true;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(5);
                main.unitDetailScrollSlitter.Height = 5;
            }
            else
            {
                main.unitDetailScroll.Visibility = Visibility.Hidden;
                main.mainGrid.RowDefinitions[2].MinHeight = 0;
                main.mainGrid.RowDefinitions[2].Height = new GridLength(0);
                main.unitDetailScrollSlitter.IsEnabled = false;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(0);
                main.unitDetailScrollSlitter.Height = 0;
            }
        }

        public static void Move(MovingType moveTo, EventHandler secondAnimation = null,
            bool err = false, bool menu = false, bool detail = false, double height = -1)
        {
            Thickness newPosition = new Thickness(0, 0, 0, 0);

            main.ResizeMode = (detail ? ResizeMode.NoResize : ResizeMode.CanResizeWithGrip);

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

        private static BitmapImage GetImage(XmlNode imageName, string armyName) =>
            new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + imageName.InnerText));

        public static void LoadArmyImage(XmlNode imageName, string armyName)
        {
            if (imageName != null)
                main.armySymbol.Source = GetImage(imageName, armyName);
            else
                main.armySymbol.Source = null;
        }

        public static bool ConfirmedDataCleaning()
        {
            if (Army.Data.Units.Count <= 0)
                return true;

            return MessageBox.Show("All army data will be lost.\nContinue?", String.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
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

            XmlNode armyFile = ArmyBook.Services.Intro(xmlFile, "Images/Cover");
            changeArmybook.imageArmybook.Source = GetImage(armyFile, armyName);
            changeArmybook.listArmybookVer.Content = String.Format("{0}th edition", ArmyBook.Services.Intro(xmlFile, "Edition").InnerText);
            changeArmybook.UpdateLayout();

            Brush mainColor = Services.BrushFromXml(ArmyBook.Services.Intro(xmlFile, "Colors/Front"));

            CurrentSelectedArmyBackColor = Services.BrushFromXml(ArmyBook.Services.Intro(xmlFile, "Colors/Grid"));
            ArmyChangesColors(changeArmybook.prev);
            ArmyChangesColors(changeArmybook.next);

            foreach (Label label in PointsButtons)
            {
                label.BorderBrush = mainColor;
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
            string randomArmy = allArmies.ElementAt(Test.Data.rand.Next(0, allArmies.Count)).Key;

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

                XmlNode armyFile = ArmyBook.Services.Intro(xmlFile, "Images/Cover");
                string source = String.Format("{0}\\{1}", Path.GetDirectoryName(armyName), armyFile.InnerText);

                Image newImage = new Image()
                {
                    Source = new BitmapImage(new Uri(source)),
                    Margin = new Thickness(2),
                    Stretch = Stretch.UniformToFill,
                    Tag = String.Format("{0}|{1}", armyName, source),
                };

                string head = ArmyBook.Parsers.StringParse(ArmyBook.Services.Intro(xmlFile, "Name")).ToUpper();
                string edition = ArmyBook.Parsers.StringParse(ArmyBook.Services.Intro(xmlFile, "Edition"));
                string description = ArmyBook.Parsers.StringParse(ArmyBook.Services.Intro(xmlFile, "Annotation"));
                string authors = ArmyBook.Parsers.StringParse(ArmyBook.Services.Intro(xmlFile, "Authors"));
                int released = ArmyBook.Parsers.IntParse(ArmyBook.Services.Intro(xmlFile, "Released"));
                string illustration = ArmyBook.Parsers.StringParse(ArmyBook.Services.Intro(xmlFile, "Images/Illustration"));

                Brush backColor = Services.BrushFromXml(ArmyBook.Services.Intro(xmlFile, "Colors/Tooltip"));
                Brush lineColor = Services.BrushFromXml(ArmyBook.Services.Intro(xmlFile, "Colors/Front"));

                if (String.IsNullOrEmpty(illustration))
                    illustration = source;
                else
                    illustration = String.Format("{0}\\{1}", Path.GetDirectoryName(armyName), illustration);

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

                newImage.MouseDown += Armybook_Click;

                changeArmybook.armybookList.Children.Add(newImage);
                allImages.Add(newImage, source);
                allArmies.Add(armyName, source);

                lastImage = newImage;
            }

            changeArmybook.UpdateLayout();

            Grid.SetRow(changeArmybook.armybookList, (int)lastImage.ActualHeight);           
        }

        private static void Armybook_Click(object sender, RoutedEventArgs e)
        {
            Image image = (sender as Image);
            string[] armyData = image.Tag.ToString().Split('|');

            PreviewArmy(armyData[0]);
            SetArmySelected(armyData[1]);
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

        public static void CreatePointsButtons()
        {
            int[] points = { 200, 500, 600, 750, 1000, 1250, 1500, 1750, 1850, 2000, 2250, 2400, 2500, 2700, 3000 };
            double[] xButtons = { 20, 116, 213 };
            double[] yButtons = { 377, 416, 455, 494, 533 };

            int xIndex = 0;
            int yIndex = 0;

            foreach (Label button in PointsButtons)
                changeArmybook.menuArmybookPlace.Children.Remove(button);

            PointsButtons.Clear();

            foreach (int button in points)
            {
                Label newPointsBotton = new Label
                {
                    Name = String.Format("button{0}pts", button),
                    Content = String.Format("{0} points", button),
                    Height = 34,
                    Width = 78,
                    FontSize = 12,
                    Background = Brushes.White,
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };

                newPointsBotton.Margin = Thick(newPointsBotton, xButtons[xIndex], yButtons[yIndex]);
                newPointsBotton.MouseDown += changeArmybook.buttonPoints_Click;
                newPointsBotton.MouseEnter += (sender, e) => newPointsBotton.Background = CurrentSelectedArmyBackColor;
                newPointsBotton.MouseLeave += (sender, e) => newPointsBotton.Background = Brushes.White;

                changeArmybook.menuArmybookPlace.Children.Add(newPointsBotton);
                PointsButtons.Add(newPointsBotton);

                xIndex += 1;

                if (xIndex >= xButtons.Length)
                {
                    xIndex = 0;
                    yIndex += 1;
                }
            }
        }

        public static double ZeroFuse(double currentParam) => (currentParam < 0 ? 0 : currentParam);

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
                    Text = String.Format("\n{0}", head),
                    FontWeight = FontWeights.Bold,
                },
                new TextBlock
                {
                    Text = String.Format("{0}th Edition", edition),
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
                TooltipFooter(released, authors),
            }
        };

        private static Grid TooltipFooter(int released, string authors)
        {
            Grid footer = new Grid { Margin = new Thickness(0, 3, 0, 0) };

            footer.ColumnDefinitions.Add(new ColumnDefinition());
            footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(370) });
            footer.ColumnDefinitions.Add(new ColumnDefinition());

            footer.RowDefinitions.Add(new RowDefinition());

            footer.Children.Add(FooterElement(String.Format("Released: {0}", released), HorizontalAlignment.Left, 0));
            footer.Children.Add(FooterElement(String.Format("Written by {0}", authors), HorizontalAlignment.Center, 1));
            footer.Children.Add(FooterElement("© Games Workshop", HorizontalAlignment.Right, 2));

            return footer;
        }

        private static TextBlock FooterElement(string text, HorizontalAlignment aligment, int column)
        {
            TextBlock element = new TextBlock();

            element.Text = text;
            element.HorizontalAlignment = aligment;
            element.FontSize = 11;

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

        private static BitmapImage GetUnitImage(string path)
        {
            BitmapImage image = null;

            try
            {
                image = new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }

            return image;
        }

        private static BitmapImage TryHomologueImage(Unit unit)
        {
            foreach (string homologueImage in ArmyBook.XmlBook.GetHomologue(Army.Data.Name, unit.Name, unit.IsHero()))
            {
                BitmapImage image = GetUnitImage(homologueImage);

                if (image != null)
                    return image;
            }

            return null;
        }

        public static void SetContentDescriptionWithImage(Unit unit)
        {
            main.armyUnitName.Content = unit.Name.ToUpper();
            main.armyUnitDescription.Text = unit.Description;
            main.armyUnitSpecific.Text = unit.SelfDescription();
            main.armyUnitImage.Source = GetUnitImage(Army.Data.UnitsImagesDirectory + unit.Image);

            if (main.armyUnitImage.Source == null)
                main.armyUnitImage.Source = TryHomologueImage(unit);

            main.UpdateLayout();

            bool portrait = main.armyUnitImage.ActualWidth < main.armyUnitImage.ActualHeight;
            main.armyUnitImage.MaxWidth = (unit.IsHeroOrHisMount() && portrait ? 450 : double.PositiveInfinity);

            main.armyUnitName.Foreground = Brushes.White;
            main.armyUnitName.Background = ArmyBook.Data.FrontColor;
            main.armyUnitName.FontWeight = FontWeights.Bold;

            main.armybookDetailScroll.Visibility = Visibility.Visible;
            main.armybookArtefactDetailScroll.Visibility = Visibility.Hidden;
        }

        public static void armyUnitTest_Resize()
        {
            main.UpdateLayout();

            double marginTop = Changes.ZeroFuse(main.unitGrid.ActualHeight - 66);

            main.specialRulesTest.Margin = Changes.Thick(main.specialRulesTest, top: marginTop);

            marginTop += main.specialRulesTest.ActualHeight;

            List<FrameworkElement> elements = new List<FrameworkElement>
            {
                main.enemyForTestText,
                main.enemyForTest,
                main.enemyTestUnit,
                main.enemyGridContainer,
                main.enemyGroupText,
                main.enemyGroup
            };

            foreach (FrameworkElement element in elements)
                element.Margin = Changes.Thick(main.enemyForTestText, top: marginTop);

            marginTop += Changes.ZeroFuse(main.enemyGrid.ActualHeight - 66);

            elements = new List<FrameworkElement>
            {
                main.specialRulesEnemyTest,
                main.startFullTest,
                main.startStatisticTest,
                main.startBattleRoyale,
                main.testConsole,
                main.currentTest,
            };

            foreach (FrameworkElement element in elements)
                element.Margin = Changes.Thick(main.enemyForTestText, top: marginTop);

            double unitTestHeight = (double)main.enemyForTest.GetValue(Canvas.TopProperty) +
                main.enemyForTest.ActualHeight + 50;

            double royalConsoleSize = 0;

            if (main.enemyGridContainer.Visibility == Visibility.Visible)
            {
                double startButtonPosition = (double)main.startFullTest.GetValue(Canvas.TopProperty);
                unitTestHeight = main.startFullTest.Margin.Top + main.startFullTest.ActualHeight + startButtonPosition + 20;

                main.startBattleRoyale.Margin = Changes.Thick(main.enemyForTestText, top: marginTop + 154,
                    left: main.startBattleRoyale.Margin.Left + 163);

                main.currentTest.Margin = Changes.Thick(main.startBattleRoyale);
            }
            else
            {
                main.testConsole.Margin = Changes.Thick(main.enemyForTestText, top: marginTop - 155);
                royalConsoleSize = -70;
            }

            if (unitTestHeight + 140 < main.armyUnitTestScroll.ActualHeight)
            {
                main.testConsole.Height = Changes.ZeroFuse(
                    main.armyUnitTestScroll.ActualHeight - unitTestHeight - 20) + royalConsoleSize;
            }
            else
            {
                unitTestHeight += 140;
                main.testConsole.Height = 120 + royalConsoleSize;
            }

            main.waitingSpinner.Margin = Changes.Thick(main.testConsole,
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
                element.Width = Changes.ZeroFuse(e.NewSize.Width - 120);

            armyUnitTest_Resize();
        }
    }
}
