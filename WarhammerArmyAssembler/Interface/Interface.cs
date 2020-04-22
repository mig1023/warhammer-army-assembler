using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace WarhammerArmyAssembler
{
    class Interface
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
            if (ArmyBook.Artefact.ContainsKey(id))
                ArmyGridDropArtefact(id, container);
            else if (ArmyBook.Mounts.ContainsKey(id))
                ArmyGridDropMount(id, points, unit);
            else
                ArmyGridDropUnit(id);

            ArmyMod.ChangeGeneralIfNeed();
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
            if (!InterfaceChecks.EnoughPointsForAddArtefact(id))
                Error("Not enough points add an item");
            else if (!InterfaceChecks.EnoughUnitPointsForAddArtefact(id, Army.Units[unitID]))
                Error("Not enough magic item points to add an item");
            else if (!ArmyChecks.IsArmyUnitsPointsPercentOk(Army.Units[unitID].Type, ArmyBook.Artefact[id].Points))
                Error("For this type, a point cost limit has been reached");
            else
            {
                Army.Units[unitID].AddAmmunition(id);
                InterfaceReload.ReloadArmyData();
                InterfaceUnitDetails.UpdateUnitDescription(unitID, Army.Units[unitID]);

                if (!ArmyBook.Artefact[id].Multiple)
                    InterfaceMod.SetArtefactAlreadyUsed(id, true);
            }
        }

        public static void ArmyGridDropUnit(int id)
        {
            bool slotExists = (ArmyParams.GetArmyUnitsNumber(ArmyBook.Units[id].Type) < ArmyParams.GetArmyMaxUnitsNumber(ArmyBook.Units[id].Type));
            bool coreUnit = (ArmyBook.Units[id].Type == Unit.UnitType.Core);

            int allHeroes = ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Lord) + ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (ArmyBook.Units[id].Type == Unit.UnitType.Hero) && (allHeroes >= ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if (ArmyBook.Units[id].PersonifiedHero && ArmyChecks.IsUnitExistInArmyByArmyBookID(id))
                Error("Personalities cannot be repeated");
            else if ((!slotExists && !coreUnit) || lordInHeroSlot)
                Error(String.Format("The number of {0} of this type has been exhausted.", (ArmyBook.Units[id].IsHero() ? "heroes" : "units")));
            else if (!InterfaceChecks.EnoughPointsForAddUnit(id))
                Error(String.Format("Not enough points to add a {0}", (ArmyBook.Units[id].IsHero() ? "hero" : "unit")));
            else if (!ArmyChecks.IsArmyUnitsPointsPercentOk(ArmyBook.Units[id].Type, ArmyBook.Units[id].Points))
                Error(String.Format("The {0} has reached a point cost limit", ArmyBook.Units[id].UnitTypeName()));
            else if(!ArmyChecks.IsArmyDublicationOk(ArmyBook.Units[id]))
                Error(String.Format("Army can't include as many duplicates of {0}", ArmyBook.Units[id].UnitTypeName()));
            else
            {
                CurrentSelectedUnit = ArmyMod.AddUnitByID(id);
                InterfaceReload.ReloadArmyData();
            }
        }

        public static void ArmyGridDropMount(int id, double points, int unit)
        {
            if (!InterfaceChecks.EnoughUnitPointsForAddOption(points))
                Error("Not enough points to add a mount");
            else if (Army.Units[unit].MountOn > 0)
                Error("The hero already has a mount");
            else if (!ArmyChecks.IsArmyUnitsPointsPercentOk(Army.Units[unit].Type, points))
                Error(String.Format("The {0} has reached a point cost limit", ArmyBook.Units[id].UnitTypeName()));
            else
            {
                ArmyMod.AddMountByID(id, unit);
                InterfaceReload.ReloadArmyData();
            }
        }

        public static void UnitDeleteDrop(int id)
        {
            if (CurrentSelectedUnit == id)
                DetailResize(open: false);

            ArmyMod.DeleteUnitByID(id);
            InterfaceReload.ReloadArmyData();
        }

        public static void AllUnitDelete()
        {
            DetailResize(open: false);
            ArmyMod.DeleteAllUnits();
            InterfaceReload.ReloadArmyData();
        }

        public static void Error(string text)
        {
            main.errorText.Content = text;

            Move(MovingType.ToTop, err: true);
        }

        public static void MainMenu()
        {
            List<string> buttonName = new List<string>
            {
                "Change Armybook", "Export Army to PDF", "Export Army to TXT", "Close"
            };

            List<MouseButtonEventHandler> buttonAction = new List<MouseButtonEventHandler>
            {
                main.toNewArmy_MouseDown, main.saveArmyToPDF_MouseDown,
                main.saveArmyToTXT_MouseDown, main.closeMainMenu_MouseDown
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
                newButton.Background = (name == "Close" ? Brushes.DarkGray : ArmyBook.MainColor);
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
                InterfaceMod.View(canvasToShow: main.mainMenu);

            if (err)
                InterfaceMod.View(canvasToShow: main.errorDetail);

            if (moveTo == MovingType.ToLeft)
            {
                InterfaceMod.View(canvasToShow: null, left: true);
                newPosition = new Thickness(main.armybookDetailScroll.Width, 0, 0, 0);
            }
                
            if (moveTo == MovingType.ToRight)
            {
                InterfaceMod.View(canvasToShow: null, right: true);
                newPosition = new Thickness((main.ActualWidth * -1), 0, 0, 0);
            }

            if (moveTo == MovingType.ToTop)
                newPosition = new Thickness(0, main.errorDetail.Height, 0, 0);

            if (moveTo == MovingType.ToMainMenu)
                newPosition = new Thickness(0, height, 0, 0);

            if ((moveTo == MovingType.ToMain) && unitTestIsOpen)
                InterfaceMod.UnitTestClose();

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

        private static void PreviewLoadCurrentSelectedArmy(string armyName)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(armyName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookImage");
            changeArmybook.imageArmybook.Source = new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + armyFile.InnerText));
            changeArmybook.imageArmybookBack.Source = changeArmybook.imageArmybook.Source;

            changeArmybook.listArmybookVer.Content = "edition " + xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion").InnerText;

            changeArmybook.UpdateLayout();

            Brush mainColor = InterfaceOther.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));

            foreach (Label label in PointsButtons)
            {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            foreach (Label label in new List<Label> { changeArmybook.next, changeArmybook.prev })
            {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            changeArmybook.listArmybookPoints.Foreground = mainColor;

            foreach (Label label in new List<Label>() {
                changeArmybook.listArmybookVer,
                changeArmybook.buttonArmybook,
                changeArmybook.closeArmybook,
                main.closeArmybookDetail,
            })
                label.Background = mainColor;

            main.mainWindowHeader.Background = mainColor;
            changeArmybook.gridCloseArmybook.Background = mainColor;

            InterfaceReload.LoadArmySize(2000, onlyReload: true);
        }

        public static void PreviewArmyList(bool next = false, bool prev = false)
        {
            string currentFile = ArmyBookInInterface.GetXmlArmyBooks(next, prev);
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
