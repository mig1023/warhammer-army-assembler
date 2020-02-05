﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();

            this.Hide();

            this.DataContext = this;

            Interface.main = this;

            Interface.CreatePointsButtons();

            Interface.changeArmybook.Show();

            armyMainLabelPlace.SizeChanged += armyMainLabelPlace_SizeChanged;

            List<string> allXmlFiles = ArmyBookInInterface.FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory);
            Interface.CurrentSelectedArmy = allXmlFiles[InterfaceOther.Rand.Next(allXmlFiles.Count)];

            Interface.PreviewArmyList();
        }

        private void armyMainLabelPlace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            armyMainMenu.Margin = Interface.Thick(armyMainMenu, left: (e.NewSize.Width - armyMainMenu.ActualWidth));
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;

            if (t.Tag == null || String.IsNullOrEmpty(t.Tag.ToString()))
                return;

            FrameworkElement f = sender as FrameworkElement;
            int id = InterfaceOther.IntParse(f.Tag.ToString());

            if (ArmyBook.Units.ContainsKey(id))
            {
                armyUnitName.Content = ArmyBook.Units[id].Name.ToUpper();
                armyUnitDescription.Text = ArmyBook.Units[id].Description;
                armyUnitSpecific.Text = ArmyBook.Units[id].SelfDescription();
            }

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                armyUnitName.Content = ArmyBook.Artefact[id].Name.ToUpper();
                armyUnitDescription.Text = ArmyBook.Artefact[id].Description; 
                armyUnitSpecific.Text = ArmyBook.Artefact[id].SelfDescription();
            }

            armyUnitName.Foreground = Brushes.White;
            armyUnitName.Background = ArmyBook.MainColor;
            armyUnitName.FontWeight = FontWeights.Bold;

            UpdateLayout();

            armybookDetail.Height = armyUnitDescription.Margin.Top +
                (armyUnitDescription.ActualHeight > 0 ? armyUnitDescription.ActualHeight : 20) +
                (armyUnitSpecific.ActualHeight > 0 ? armyUnitSpecific.ActualHeight : 20) + 20;

            armyUnitSpecific.Margin = Interface.Thick(armybookDetail, left: 20, top: armybookDetail.Margin.Top + armyUnitDescription.ActualHeight + 40);
            armyUnitSpecific.Foreground = ArmyBook.MainColor;

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                Interface.Move(Interface.MovingType.ToLeft, detail: true);

            if (ArmyBook.Artefact.ContainsKey(id) && ArmyBook.Artefact[id].ArtefactAlreadyUsed)
                return;
            else
            {
                Interface.DragSender = sender;

                DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
            }
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            int id = InterfaceOther.IntParse((string)e.Data.GetData(DataFormats.Text));

            if ((Interface.DragSender as FrameworkElement).Name == "ArmyGrid")
                return;

            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (sender is ScrollViewer)
            {
                if (ArmyChecks.IsUnitExistInArmy(Interface.CurrentSelectedUnit))
                    Interface.ArmyGridDropArtefact(id, Interface.CurrentSelectedUnit);
            }
            else
            {
                Interface.ArmyGridDrop(id, container);

                if (ArmyChecks.IsUnitExistInArmy(Interface.CurrentSelectedUnit))
                    InterfaceUnitDetails.UpdateUnitDescription(Interface.CurrentSelectedUnit, Army.Units[Interface.CurrentSelectedUnit]);
            }
                
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                    return correctlyTyped;

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private int ErrorAndReturnSizeBack(string error, int id)
        {
            Interface.Error(error);
            return Army.Units[id].Size;
        }

        private void ArmyGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Unit u = e.Row.Item as Unit;

            double pointsDiff = u.GetUnitPoints() - Army.Units[u.ID].GetUnitPoints();

            if (!InterfaceChecks.EnoughPointsForEditUnit(u.ID, u.Size))
                u.Size = ErrorAndReturnSizeBack("Not enough points to change", u.ID);
            else if ((u.MaxSize != 0) && (u.Size > u.MaxSize))
                u.Size = ErrorAndReturnSizeBack("Unit size exceeds the maximum allowed", u.ID);
            else if (u.Size < u.MinSize)
                u.Size = ErrorAndReturnSizeBack("Unit size is less than the minimum allowed", u.ID);
            else if ((u.Size > Army.Units[u.ID].Size) && (!ArmyChecks.IsArmyUnitsPointsPercentOk(u.Type, pointsDiff)))
                u.Size = ErrorAndReturnSizeBack(String.Format("The {0} has reached a point cost limit", u.UnitTypeName()), u.ID);
            else
                Army.Units[u.ID].Size = u.Size;

            InterfaceReload.ReloadArmyData();
        }

        private void ArmyGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.DragOver += new DragEventHandler(ArmyGridRow_DragOver);
        }

        private void ArmyGridRow_DragOver(object sender, DragEventArgs e)
        {
            Unit unit = null;

            if (sender is ScrollViewer)
            {
                if (ArmyChecks.IsUnitExistInArmy(Interface.CurrentSelectedUnit))
                    unit = Army.Units[Interface.CurrentSelectedUnit];
            }
            else
            {
                DataGridRow row = sender as DataGridRow;
                unit = row.DataContext as Unit;

                ArmyGrid.SelectedItem = row.Item;
                ArmyGrid.ScrollIntoView(row.Item);
            }

            int id = InterfaceOther.IntParse((string)e.Data.GetData(DataFormats.Text));

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                bool enabled = unit.IsOptionEnabled(ArmyBook.Artefact[id], unit.GetMountOn(), unit.GetMountTypeAlreadyFixed());
                bool usable = ArmyBook.Artefact[id].IsUsableByUnit(unit);
                e.Effects = (usable && enabled ? DragDropEffects.Copy : DragDropEffects.None);
            }
            else
                e.Effects = DragDropEffects.None;
        }

        private void ArmyGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (container == null)
                return;

            DependencyObject clickedColumn = (DependencyObject)e.OriginalSource;

            while ((clickedColumn != null) && !(clickedColumn is DataGridCell) && !(clickedColumn is DataGridColumnHeader))
                clickedColumn = VisualTreeHelper.GetParent(clickedColumn);

            if (clickedColumn != null && clickedColumn is DataGridCell)
                if ((clickedColumn as DataGridCell).Column.DisplayIndex == 1)
                    return;

            Unit unit = container.DataContext as Unit;

            InterfaceUnitDetails.UpdateUnitDescription(unit.ID, unit);

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                Interface.DetailResize(open: true);

            Interface.DragSender = sender;

            DragDrop.DoDragDrop(container, unit.ID.ToString(), DragDropEffects.Copy);

            Interface.CurrentSelectedUnit = unit.ArmyID;
        }

        private void unitDelete_Drop(object sender, DragEventArgs e)
        {
            int id = InterfaceOther.IntParse((string)e.Data.GetData(DataFormats.Text));

            Interface.UnitDeleteDrop(id);
        }

        private void mainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainGrid.Height = e.NewSize.Height;
            mainGrid.Width = e.NewSize.Width;

            mainPlaceCanvas.Height = e.NewSize.Height;
            mainPlaceCanvas.Width = e.NewSize.Width;

            armybookDetailScroll.Height = e.NewSize.Height - 70;

            errorDetail.Width = e.NewSize.Width;
            closeErrorDetail.Margin = new Thickness(e.NewSize.Width - closeErrorDetail.Width - 10, 10, 0, 0);

            mainMenu.Width = e.NewSize.Width;
            closeMainMenu.Margin = new Thickness(
                e.NewSize.Width - closeMainMenu.Width - 10,
                mainMenu.Height - closeMainMenu.Height - 10,
                0, 0
            );
        }

        private void closeArmybookDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.Move(Interface.MovingType.ToMain);
        }

        private void closeDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.DetailResize(open: false);
        }

        private void closeErrorDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.Move(Interface.MovingType.ToMain, err: true);
        }

        private void closeMainMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Interface.Move(Interface.MovingType.ToMain, menu: true);
        }

        private void unitDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                if (MessageBox.Show("Clear entire army list? ", String.Empty, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Interface.DetailResize(open: false);
                    Interface.AllUnitDelete();
                }
        }

        private void buttonArmybook_Click(object sender, RoutedEventArgs e)
        {
            ArmyBookLoad.LoadArmy(Interface.CurrentSelectedArmy);

            InterfaceReload.LoadArmyList();
            InterfaceReload.ReloadArmyData();

            Interface.Move(Interface.MovingType.ToMain);
        }

        private void armyMainLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Interface.MainMenu();
        }

        private void toNewArmy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Interface.Move(Interface.MovingType.ToMain, menu: true);
            Interface.AllUnitDelete();
            Interface.DetailResize(open: false);

            this.Hide();
            Interface.changeArmybook.Show();
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(prev: true);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(next: true);
        }

        public void buttonPoints_Click(object sender, RoutedEventArgs e)
        {
            InterfaceReload.LoadArmySize(InterfaceOther.IntParse((sender as Label).Content.ToString().Split()[0]));
        }

        private void unitDetailScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ArmyChecks.IsUnitExistInArmy(Interface.CurrentSelectedUnit))
                InterfaceUnitDetails.UpdateUnitDescription(Interface.CurrentSelectedUnit, Army.Units[Interface.CurrentSelectedUnit]);
        }

        private void armyPoints_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Dictionary<Unit.UnitType, double> units = ArmyChecks.UnitsPointsPercent();
            Dictionary<Unit.UnitType, double> unitPercents = ArmyChecks.UnitsMaxPointsPercent();

            double armyCurrentPoint = ArmyParams.GetArmyPoints();

            string pointsMsg = String.Format("All points:\t\t{0}\nAlready used:\t{1}\nAvailable:\t\t{2}\n\n\n\n",
                ArmyParams.GetArmyMaxPoints(), armyCurrentPoint, (ArmyParams.GetArmyMaxPoints() - armyCurrentPoint));

            foreach(KeyValuePair<Unit.UnitType, double> entry in unitPercents)
                pointsMsg += String.Format("{0}:\t{1,10} pts / {2}%\t({3} {4} pts / {5}%)\n\n",
                    entry.Key,
                    units[entry.Key],
                    InterfaceOther.CalcPercent(units[entry.Key], Army.MaxPoints),
                    (entry.Key == Unit.UnitType.Core ? "min" : "max"),
                    (int)(Army.MaxPoints * entry.Value),
                    entry.Value * 100
                );

            MessageBox.Show(pointsMsg);
        }

        private void saveArmyToPDF_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExportPDF.SaveArmyToPDF();

            Interface.Move(Interface.MovingType.ToMain, menu: true);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
