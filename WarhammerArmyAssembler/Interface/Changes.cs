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

            if (ArmyBook.Data.Artefact[id].Runic > 0)
            {
                Dictionary<int, Option> versions = ArmyBook.Data.Artefact[id].AllRunicVersions();

                Option currentItem = Army.Data.Units[unitID].GetCurrentRunicItemByName(ArmyBook.Data.Artefact[id].Name);

                if ((currentItem != null) && (currentItem.Runic >= versions.Count))
                    return;
                else if (currentItem != null)
                {
                    prevRunicItem = currentItem;
                    prevRunicPointsPenalty = currentItem.Points;

                    id = versions[currentItem.Runic + 1].ID;
                }

                if (Army.Checks.IsRunicCombinationAlreadyExist(Army.Data.Units[unitID], ArmyBook.Data.Artefact[id]))
                {
                    Error("No more than one item may carry the same combination of runes");
                    return;
                }
            }

            if (ArmyBook.Data.Artefact[id].Virtue)
                ArmyBook.Data.Artefact[id].Points = Army.Params.GetVirtuePoints(id);
                
            if (!Interface.Checks.EnoughPointsForAddArtefact(id, prevRunicPointsPenalty))
                Error("Not enough points add an item");

            else if (!Interface.Checks.EnoughUnitPointsForAddArtefact(id, Army.Data.Units[unitID], pointsPenalty: prevRunicPointsPenalty))
                Error(String.Format("Not enough magic item {0} to add an item", (Army.Data.Units[unitID].MagicItemCount > 0 ? "slots" : "points")));

            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(Army.Data.Units[unitID].Type, ArmyBook.Data.Artefact[id].Points))
                Error("For this type, a point cost limit has been reached");

            else
            {
                if (prevRunicItem != null)
                    Army.Data.Units[unitID].Options.Remove(prevRunicItem);

                Army.Data.Units[unitID].AddAmmunition(id);
                Interface.Reload.ReloadArmyData();
                Interface.UnitDetails.UpdateUnitDescription(unitID, Army.Data.Units[unitID]);

                bool multiple = ArmyBook.Data.Artefact[id].Multiple || ArmyBook.Data.Artefact[id].Virtue
                    || (ArmyBook.Data.Artefact[id].Runic > 0);

                if (!multiple && (ArmyBook.Data.Artefact[id].Type != Option.OptionType.Powers))
                    Interface.Mod.SetArtefactAlreadyUsed(id, true);
            }
        }

        public static void ArmyGridDropUnit(int id)
        {
            bool slotExists = (Army.Params.GetArmyUnitsNumber(ArmyBook.Data.Units[id].Type) < Army.Params.GetArmyMaxUnitsNumber(ArmyBook.Data.Units[id].Type));
            bool coreUnit = (ArmyBook.Data.Units[id].Type == Unit.UnitType.Core);

            int allHeroes = Army.Params.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.Params.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (ArmyBook.Data.Units[id].Type == Unit.UnitType.Hero) && (allHeroes >= Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if (ArmyBook.Data.Units[id].PersonifiedHero && Army.Checks.IsUnitExistInArmyByArmyBookID(id))
                Error("Personalities cannot be repeated");

            else if ((!slotExists && !coreUnit) || lordInHeroSlot)
                Error(String.Format("The number of {0} of this type has been exhausted.", (ArmyBook.Data.Units[id].IsHero() ? "heroes" : "units")));

            else if (!Interface.Checks.EnoughPointsForAddUnit(id))
                Error(String.Format("Not enough points to add a {0}", (ArmyBook.Data.Units[id].IsHero() ? "hero" : "unit")));

            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(ArmyBook.Data.Units[id].Type, ArmyBook.Data.Units[id].Points))
                Error(String.Format("The {0} has reached a point cost limit", ArmyBook.Data.Units[id].UnitTypeName()));

            else if(!Army.Checks.IsArmyDublicationOk(ArmyBook.Data.Units[id]))
                Error(String.Format("Army can't include as many duplicates of {0}", ArmyBook.Data.Units[id].UnitTypeName()));

            else
            {
                CurrentSelectedUnit = Army.Mod.AddUnitByID(id);
                Interface.Reload.ReloadArmyData();
            }
        }

        public static void ArmyGridDropMount(int id, double points, int unit)
        {
            if (!Interface.Checks.EnoughUnitPointsForAddOption(points))
                Error("Not enough points to add a mount");
            else if (Army.Data.Units[unit].MountOn > 0)
                Error("The hero already has a mount");
            else if (!Army.Checks.IsArmyUnitsPointsPercentOk(Army.Data.Units[unit].Type, points))
                Error(String.Format("The {0} has reached a point cost limit", ArmyBook.Data.Units[id].UnitTypeName()));
            else
            {
                Army.Mod.AddMountByID(id, unit);
                Interface.Reload.ReloadArmyData();
            }
        }

        public static void UnitDeleteDrop(int id)
        {
            DetailResize(open: false);

            Army.Mod.DeleteUnitByID(id);
            Interface.Reload.ReloadArmyData();
        }

        public static void AllUnitDelete()
        {
            DetailResize(open: false);
            Army.Mod.DeleteAllUnits();
            Interface.Reload.ReloadArmyData();
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
            else
                if (toPDF)
                    Export.PDF.SaveArmy(fullRules);
                else
                    Export.Text.SaveArmy();

            Move(MovingType.ToMain, menu: true);
        }

        public static void MainMenu()
        {
            List<string> buttonName = new List<string>
            {
                "Change Armybook",
                "Export Army to PDF",
                "Export Army to PDF (light)",
                "Export Army to TXT",
                "Exit",
                "Close"
            };

            List<MouseButtonEventHandler> buttonAction = new List<MouseButtonEventHandler>
            {
                main.toNewArmy_MouseDown,
                main.saveArmyToPDF_MouseDown,
                main.saveArmyToPDF_light_MouseDown,
                main.saveArmyToTXT_MouseDown,
                main.closeWindow_MouseLeftButtonDown,
                main.closeMainMenu_MouseDown,
            };

            foreach (Label button in MainMenuButtons)
                main.mainMenu.Children.Remove(button);

            MainMenuButtons.Clear();

            int buttonIndex = 0;
            double buttonXPosition = 15;
            double buttonYPosition = 10;

            buttonIndex = 0;

            foreach (string name in buttonName)
            {
                Label newButton = new Label
                {
                    Content = name
                };
                newButton.Margin = Thick(newButton, buttonXPosition, buttonYPosition);
                newButton.Height = 30;
                newButton.Width = Double.NaN;
                newButton.MouseDown += buttonAction[buttonIndex];
                newButton.Foreground = Brushes.White;
                newButton.Background = (name == "Close" ? Brushes.DarkGray : ArmyBook.Data.MainColor);
                newButton.FontSize = 16;
                newButton.FontWeight = FontWeights.Bold;

                main.mainMenu.Children.Add(newButton);
                MainMenuButtons.Add(newButton);

                main.UpdateLayout();

                buttonIndex += 1;

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

            if (menu)
                Interface.Mod.View(canvasToShow: main.mainMenu, top: true);

            if (err)
                Interface.Mod.View(canvasToShow: main.errorDetail, top: true);

            if (moveTo == MovingType.ToLeft)
            {
                Interface.Mod.View(left: true);
                newPosition = new Thickness(main.ActualWidth, 0, 0, 0);
            }
                
            if (moveTo == MovingType.ToRight)
            {
                Interface.Mod.View(right: true);
                newPosition = new Thickness((main.ActualWidth * -1), 0, 0, 0);
            }

            if (moveTo == MovingType.ToTop)
                newPosition = new Thickness(0, main.errorDetail.Height, 0, 0);

            if (moveTo == MovingType.ToMainMenu)
                newPosition = new Thickness(0, height, 0, 0);

            if ((moveTo == MovingType.ToMain) && unitTestIsOpen)
                Interface.Mod.UnitTestClose();

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

        public static void LoadArmyImage(XmlNode imageName, string armyName)
        {
            if (imageName != null)
                main.armySymbol.Source = new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + imageName.InnerText));
            else
                main.armySymbol.Source = null;
        }

        public static bool ConfirmedDataCleaning()
        {
            if (Army.Data.Units.Count <= 0)
                return true;

            return MessageBox.Show("All army data will be lost.\nContinue?", String.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        private static void PreviewLoadCurrentSelectedArmy(string armyName)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(armyName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookImage");
            changeArmybook.imageArmybook.Source = new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + armyFile.InnerText));
            changeArmybook.imageArmybookBack.Source = changeArmybook.imageArmybook.Source;

            changeArmybook.listArmybookVer.Content = String.Format("{0}th edition", xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion").InnerText);
            changeArmybook.UpdateLayout();

            Brush mainColor = Interface.Other.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));

            foreach (Label label in PointsButtons)
            {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            foreach (Control label in new List<Control> {
                changeArmybook.next,
                changeArmybook.prev,
                changeArmybook.listArmybookPoints,
                changeArmybook.armyAdditionalName
            }) {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            foreach (Control label in new List<Control>() {
                changeArmybook.listArmybookVer,
                changeArmybook.buttonArmybook,
                changeArmybook.closeArmybook,
                main.closeArmybookDetail,
            })
                label.Background = mainColor;

            main.mainWindowHeader.Background = mainColor;
            changeArmybook.gridCloseArmybook.Background = mainColor;
            changeArmybook.showArmyAdditionalName.Foreground = mainColor;
        }

        public static void PreviewArmyList(bool next = false, bool prev = false)
        {
            string currentFile = ArmyBook.XmlBook.GetXmlArmyBooks(next, prev);
            CurrentSelectedArmy = currentFile;
            PreviewLoadCurrentSelectedArmy(currentFile);
        }

        public static void CreatePointsButtons()
        {
            int[] points =
            {
                200, 500, 600, 750, 1000, 1250, 1500, 1750,
                1850, 2000, 2250, 2400, 2500, 2700, 3000
            };
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
                    Content = String.Format("{0} points", button)
                };

                newPointsBotton.Margin = Thick(newPointsBotton, xButtons[xIndex], yButtons[yIndex]);

                newPointsBotton.Height = 34;
                newPointsBotton.Width = 78;
                newPointsBotton.FontSize = 12;
                newPointsBotton.Background = Brushes.White;
                newPointsBotton.BorderThickness = new Thickness(1);

                newPointsBotton.HorizontalContentAlignment = HorizontalAlignment.Center;
                newPointsBotton.VerticalContentAlignment = VerticalAlignment.Center;

                newPointsBotton.MouseDown += changeArmybook.buttonPoints_Click;

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

        public static double ZeroFuse(double currentParam)
        {
            return (currentParam < 0 ? 0 : currentParam);
        }
    }
}
