﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace WarhammerArmyAssembler
{
    class Interface
    {
        public static MainWindow main = null;

        public static ObservableCollection<Unit> ArmyInInterface = new ObservableCollection<Unit>();

        public static object DragSender = null;

        public static string CurrentSelectedArmy = null;

        public static int CurrentSelectedUnit = -1;

        public static bool startArmybookMenu = true;

        public enum MovingType { ToMain, ToRight, ToLeft, ToTop, ToMainMenu }

        public static List<Label> PointsButtons = new List<Label>();

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
            Move(MovingType.ToMainMenu, toShow: InterfaceMod.ShowMainMenu, menu: true);
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

        public static void Move(MovingType moveTo, InterfaceMod.ShowSomething toShow = null, EventHandler secondAnimation = null,
            bool err = false, bool menu = false)
        {
            Thickness newPosition = new Thickness(0, 0, 0, 0);

            if (err)
                toShow = InterfaceMod.ShowError;

            if (!(menu && (moveTo == MovingType.ToMain)))
                InterfaceMod.HideAllAndShow(toShow);

            if (moveTo == MovingType.ToLeft)
                newPosition = new Thickness(main.armybookDetailScrollHead.Width, 0, 0, 0);
                
            if (moveTo == MovingType.ToRight)
                newPosition = new Thickness(-320, 0, 0, 0);

            if (moveTo == MovingType.ToTop)
                newPosition = new Thickness(0, main.errorDetail.Height, 0, 0);

            if (moveTo == MovingType.ToMainMenu)
                newPosition = new Thickness(0, main.mainMenu.Height, 0, 0);

            ThicknessAnimation move = new ThicknessAnimation();
            move.Duration = TimeSpan.FromSeconds(0.2);
            move.From = (err || menu ? main.mainPlaceCanvas.Margin : main.mainGrid.Margin);
            move.To = newPosition;

            if (secondAnimation != null)
                move.Completed += secondAnimation;

            if (err || menu)
                main.mainPlaceCanvas.BeginAnimation(FrameworkElement.MarginProperty, move);
            else
                main.mainGrid.BeginAnimation(FrameworkElement.MarginProperty, move);
        }

        public static void MoveToChangeArmybook(object Sender, EventArgs e)
        {
            PreviewArmyList();
            Move(MovingType.ToLeft, toShow: InterfaceMod.ShowArmybookMenu, secondAnimation: new EventHandler(InterfaceMod.ShowStartHelpInfo));
        }

        private static void PreviewLoadCurrentSelectedArmy(string armyName)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(armyName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookImage");
            main.imageArmybook.Source = new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + armyFile.InnerText));

            main.listArmybookVer.Content = "edition " + xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion").InnerText;

            main.UpdateLayout();

            Brush mainColor = InterfaceOther.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));

            foreach (Label label in PointsButtons)
            {
                label.BorderBrush = mainColor;
                label.Foreground = mainColor;
            }

            main.listArmybookPoints.Foreground = mainColor;

            foreach (Label label in new List<Label>() { main.listArmybookVer, main.buttonArmybook, main.closeArmybookDetail })
                label.Background = mainColor;

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
            int[] points = { 200, 500, 600, 750, 1000, 1250, 1500, 1750, 1850, 2000, 2400, 2500, 2700, 3000, 3500 };
            double[] xButtons = { 20, 116, 213 };
            double[] yButtons = { 367, 406, 445, 484, 523 };

            int xIndex = 0;
            int yIndex = 0;

            PointsButtons.Clear();

            foreach (int button in points)
            {
                Label newPointsBotton = new Label();

                newPointsBotton.Name = String.Format("button{0}pts", button);
                newPointsBotton.Content = String.Format("{0} points", button);

                newPointsBotton.Margin = Thick(newPointsBotton, xButtons[xIndex], yButtons[yIndex]);

                newPointsBotton.Height = 34;
                newPointsBotton.Width = 78;
                newPointsBotton.FontSize = 12;
                newPointsBotton.Background = Brushes.White;
                newPointsBotton.BorderThickness = new Thickness(1);

                newPointsBotton.HorizontalContentAlignment = HorizontalAlignment.Center;
                newPointsBotton.VerticalContentAlignment = VerticalAlignment.Center;

                newPointsBotton.MouseDown += main.buttonPoints_Click;

                main.menuArmybookPlace.Children.Add(newPointsBotton);
                PointsButtons.Add(newPointsBotton);

                xIndex += 1;

                if (xIndex >= xButtons.Length)
                {
                    xIndex = 0;
                    yIndex += 1;
                }
            }
        }
    }
}
