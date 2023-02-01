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

            Interface.Changes.main = this;
            Interface.Changes.changeArmybook.Show();

            armyMainLabelPlace.SizeChanged += armyMainLabelPlace_SizeChanged;

            List<string> allXmlFiles = ArmyBook.XmlBook.FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory);

            if (allXmlFiles.Count > 0)
                Interface.Changes.CurrentSelectedArmy = allXmlFiles[Interface.Services.Rand.Next(allXmlFiles.Count)];

            Interface.Changes.LoadAllArmy(allXmlFiles);
            Interface.Changes.PreviewArmyList();

            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        public void armyEditionLabel_PositionCorrect()
        {
            UpdateLayout();

            double left = armyMainLabel.Margin.Left + armyMainLabel.ActualWidth - 5;
            armyEditionLabel.Margin = Interface.Changes.Thick(armyEditionLabel, left: left);
        }

        private void armyMainLabelPlace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double left = e.NewSize.Width - armyMainMenu.ActualWidth;
            armyMainMenu.Margin = Interface.Changes.Thick(armyMainMenu, left: left);
            armyEditionLabel_PositionCorrect();
        }

        private void ArmyList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView armyList = sender as TreeView;

            if (armyList.SelectedItem is Unit)
                ChangeArmyListDetail((armyList.SelectedItem as Unit).ID, (armyList.SelectedItem as Unit).GroupBold);

            if (armyList.SelectedItem is Option)
                ChangeArmyListDetail((armyList.SelectedItem as Option).ID, (armyList.SelectedItem as Option).GroupBold);
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;

            if (t.Tag == null || String.IsNullOrEmpty(t.Tag.ToString()))
                return;

            FrameworkElement f = sender as FrameworkElement;
            int id = Interface.Services.IntParse(f.Tag.ToString());

            ChangeArmyListDetail(id);

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (Interface.Changes.armybookDetailIsOpen)
                {
                    Interface.Changes.armybookDetailIsOpen = false;
                    Interface.Changes.Move(Interface.Changes.MovingType.ToMain);
                    return;
                }
                else
                {
                    Interface.Changes.armybookDetailIsOpen = true;
                    Interface.Changes.Move(Interface.Changes.MovingType.ToLeft, detail: true);
                }
            }
                
            if (ArmyBook.Data.Artefact.ContainsKey(id) && ArmyBook.Data.Artefact[id].ArtefactAlreadyUsed)
                return;
            else
            {
                Interface.Changes.DragSender = sender;
                DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
            }

            HideStartArmyHelpText();
        }

        private void ChangeArmyListDetail(int id, bool group = false)
        {
            armyUnitImage.Source = null;

            bool thisIsUnit = ArmyBook.Data.Units.ContainsKey(id);
            bool thisIsArtefact = ArmyBook.Data.Artefact.ContainsKey(id);
            bool whithImage = false;

            if (thisIsUnit)
            {
                Unit unit = ArmyBook.Data.Units[id];

                armyUnitImage.Source = ArmyBook.Services.GetUnitImage(unit.Image);
                whithImage = armyUnitImage.Source != null;
            }

            if (group)
            {
                armyUnitName.Content = String.Empty;
                armyUnitDescription.Text = String.Empty;
                armyUnitSpecific.Text = String.Empty;
            }
            else if (thisIsUnit && whithImage)
            {
                Interface.Changes.SetContentDescriptionWithImage(ArmyBook.Data.Units[id]);
            }
            else if (thisIsUnit && !whithImage)
            {
                Interface.Changes.SetContentDescription(ArmyBook.Data.Units[id]);
            }
            else if (thisIsArtefact)
            {
                Interface.Changes.SetContentDescription(ArmyBook.Data.Artefact[id]);
            }

            UpdateLayout();

            double descrHeight = armyUnitDescription.ActualHeight > 0 ? armyUnitDescription.ActualHeight : 20;
            double specHeight = armyUnitSpecific.ActualHeight > 0 ? armyUnitSpecific.ActualHeight : 20;
            double paramsHeight = detailUnitGrid.ActualHeight > 0 ? detailUnitGrid.ActualHeight : 20;
            
            armybookArtefactDetail.Height = armyUnitDescription.Margin.Top + descrHeight + specHeight + 20;
            armybookDetail.Height = armybookArtefactDetail.Height + paramsHeight;

            double top = armyUnitDescription.Margin.Top + 35;
            detailUnitGridBorder.Margin = Interface.Changes.Thick(armybookDetail, left: 20, top: top);
            top += armyUnitDescription.ActualHeight + detailUnitGrid.ActualHeight - 5;
            armyUnitSpecific.Margin = Interface.Changes.Thick(armybookDetail, left: 20, top: top);

            double detailGridSubline = detailUnitGridBorder.Margin.Top + detailUnitGridBorder.ActualHeight;
            profileMods.Margin = Interface.Changes.Thick(detailUnitGridBorder, left: 170, top: detailGridSubline);

            armyUnitSpecific.Foreground = ArmyBook.Data.FrontColor;

            top = armyArtefactDescription.Margin.Top + armyArtefactDescription.ActualHeight + 15;
            armyArtefactSpecific.Margin = Interface.Changes.Thick(armyArtefactSpecific, left: 20, top: top);
            armyArtefactSpecific.Foreground = ArmyBook.Data.FrontColor;
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            int id = Interface.Services.IntParse((string)e.Data.GetData(DataFormats.Text));

            if ((Interface.Changes.DragSender as FrameworkElement).Name == "ArmyGrid")
                return;

            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (!(sender is ScrollViewer))
                Interface.Changes.ArmyGridDrop(id, container);

            if (!Army.Checks.IsUnitExistInArmy(Interface.Changes.CurrentSelectedUnit))
                return;

            if ((sender is ScrollViewer) && Army.Checks.ThisIsAftefact(id))
                Interface.Changes.ArmyGridDropArtefact(id, Interface.Changes.CurrentSelectedUnit);
            else
                Interface.Details.UpdateUnitDescription(
                    Interface.Changes.CurrentSelectedUnit, Army.Data.Units[Interface.Changes.CurrentSelectedUnit]);

            HideStartArmyHelpText();
        }

        private void HideStartArmyHelpText()
        {
            if (startArmyHelpText.Visibility == Visibility.Visible)
                startArmyHelpText.Visibility = Visibility.Hidden;
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;

            while (parent != null)
            {
                if (parent is T correctlyTyped)
                    return correctlyTyped;

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }

        private int ErrorAndReturnSizeBack(string error, int id)
        {
            Interface.Changes.Error(error);
            return Army.Data.Units[id].Size;
        }

        private void ArmyGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Unit u = e.Row.Item as Unit;

            double diff = u.GetUnitPoints() - Army.Data.Units[u.ID].GetUnitPoints();

            if (!Interface.Checks.EnoughPointsForEditUnit(u.ID, u.Size))
                u.Size = ErrorAndReturnSizeBack("Not enough points to change", u.ID);

            else if ((u.MaxSize != 0) && (u.Size > u.MaxSize))
                u.Size = ErrorAndReturnSizeBack("Unit size exceeds the maximum allowed", u.ID);

            else if (u.Size < u.MinSize)
                u.Size = ErrorAndReturnSizeBack("Unit size is less than the minimum allowed", u.ID);

            else if ((u.Size > Army.Data.Units[u.ID].Size) && (!Army.Checks.IsArmyUnitsPointsPercentOk(u.Type, diff, u.StaticPoints)))
                u.Size = ErrorAndReturnSizeBack(String.Format("The {0} has reached a point cost limit", u.UnitTypeName()), u.ID);

            else
                Army.Data.Units[u.ID].Size = u.Size;

            Interface.Reload.ReloadArmyData();
            Interface.Details.UpdateUnitDescription(u.ID, Army.Data.Units[u.ID]);
        }

        private void ArmyGrid_LoadingRow(object sender, DataGridRowEventArgs e) =>
            e.Row.DragOver += new DragEventHandler(ArmyGridRow_DragOver);

        private void ArmyGridRow_DragOver(object sender, DragEventArgs e)
        {
            Unit unit = null;

            if (sender is ScrollViewer)
            {
                if (Army.Checks.IsUnitExistInArmy(Interface.Changes.CurrentSelectedUnit))
                    unit = Army.Data.Units[Interface.Changes.CurrentSelectedUnit];
            }
            else
            {
                DataGridRow row = sender as DataGridRow;
                unit = row.DataContext as Unit;

                ArmyGrid.SelectedItem = row.Item;
                ArmyGrid.ScrollIntoView(row.Item);
            }

            int id = Interface.Services.IntParse((string)e.Data.GetData(DataFormats.Text));

            if (ArmyBook.Data.Artefact.ContainsKey(id))
            {
                string onlyForType = unit.GetMountTypeAlreadyFixed();
                bool enabled = unit.IsOptionEnabled(ArmyBook.Data.Artefact[id], unit.GetMountOn(), onlyForType);
                bool usable = ArmyBook.Data.Artefact[id].IsUsableByUnit(unit, dragOverCheck: true);
                e.Effects = (usable && enabled ? DragDropEffects.Copy : DragDropEffects.None);
            }
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
            {
                int clickedColumnNum = (clickedColumn as DataGridCell).Column.DisplayIndex;

                if (clickedColumnNum == 1 || clickedColumnNum == 3)
                    return;
            }
                
            Unit unit = container.DataContext as Unit;

            Interface.Details.UpdateUnitDescription(unit.ID, unit);

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                Interface.Changes.DetailResize(open: true);

            Interface.Changes.DragSender = sender;

            DragDrop.DoDragDrop(container, unit.ID.ToString(), DragDropEffects.Copy);

            Interface.Changes.CurrentSelectedUnit = unit.ArmyID;
        }

        private void unitDelete_Drop(object sender, DragEventArgs e)
        {
            int id = Interface.Services.IntParse((string)e.Data.GetData(DataFormats.Text));
            Interface.Changes.UnitDeleteDrop(id);
        }

        private void mainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainGrid.Height = e.NewSize.Height;
            mainPlaceCanvas.Height = e.NewSize.Height;

            closeArmybookDetail.Width = e.NewSize.Height;

            if (!Interface.Changes.unitTestIsOpen)
                mainGrid.Width = e.NewSize.Width;

            List<ScrollViewer> scrolls = new List<ScrollViewer>
            {
                armybookDetailScroll,
                armyUnitTestScroll,
                armybookArtefactDetailScroll
            };

            foreach (ScrollViewer scroll in scrolls)
            {
                scroll.Height = e.NewSize.Height;
                scroll.Width = Interface.Changes.ZeroFuse(e.NewSize.Width - 25);
            }

            foreach (Canvas canvas in new List<Canvas> { errorDetail, mainMenu, mainPlaceCanvas, armyUnitTest })
                canvas.Width = e.NewSize.Width;

            double margin = e.NewSize.Width - closeErrorDetail.Width - 10;
            closeErrorDetail.Margin = new Thickness(Interface.Changes.ZeroFuse(margin), 10, 0, 0);

            foreach (TextBlock text in new List<TextBlock> { armyUnitDescription, armyUnitSpecific })
                text.Width = Interface.Changes.ZeroFuse((e.NewSize.Width/2) - 75);

            foreach (TextBlock text in new List<TextBlock> { armyArtefactDescription, armyArtefactSpecific })
                text.Width = Interface.Changes.ZeroFuse(e.NewSize.Width - 75);

            if (Interface.Changes.mainMenuIsOpen)
                Interface.Changes.MainMenu();
        }

        private void closeArmybookDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.Changes.armybookDetailIsOpen = false;
            Interface.Changes.Move(Interface.Changes.MovingType.ToMain);
        }

        private void closeDetail_Click(object sender, RoutedEventArgs e) => Interface.Changes.DetailResize(open: false);

        private void closeErrorDetail_Click(object sender, RoutedEventArgs e) =>
            Interface.Changes.Move(Interface.Changes.MovingType.ToMain, err: true);

        public void closeMainMenu_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.Move(Interface.Changes.MovingType.ToMain, menu: true);

        private void unitDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.LeftButton != MouseButtonState.Pressed) || (e.ClickCount != 2) || !Interface.Changes.ConfirmedDataCleaning())
                return;

            Interface.Changes.DetailResize(open: false);
            Interface.Changes.AllUnitDelete();
        }

        private void armyMainLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Interface.Changes.mainMenuIsOpen)
                closeMainMenu_MouseDown(null, null);
            else
                Interface.Changes.MainMenu();
        }

        public void toNewArmy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Interface.Changes.ConfirmedDataCleaning())
                return;

            Interface.Changes.Move(Interface.Changes.MovingType.ToMain, menu: true);
            Interface.Changes.AllUnitDelete();
            Interface.Changes.DetailResize(open: false);

            this.Hide();
            Interface.Changes.changeArmybook.Show();
        }

        private void unitDetailScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Army.Checks.IsUnitExistInArmy(Interface.Changes.CurrentSelectedUnit))
                Interface.Details.UpdateUnitDescription(
                    Interface.Changes.CurrentSelectedUnit, Army.Data.Units[Interface.Changes.CurrentSelectedUnit]);
        }

        private void armyPoints_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyPoints());

        private void armyUnits_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyUnits());

        private void armyHeroes_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyHeroes());

        private void armyModels_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyModels());

        private void armyCast_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyCast());

        private void armyDispell_MouseDown(object sender, MouseButtonEventArgs e) =>
            MessageBox.Show(Interface.Info.ArmyDispell());

        public void saveArmyToPDF_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.CheckAndExportTo(toPDF: true, fullRules: true);

        public void saveArmyToPDF_light_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.CheckAndExportTo(toPDF: true);

        public void saveArmyToTXT_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.CheckAndExportTo(toPDF: false);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Interface.Changes.ConfirmedDataCleaning())
                e.Cancel = true;
        }

        private void Window_Closed(object sender, EventArgs e) => Environment.Exit(0);

        private void ArmyList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (startArmyHelpText.Visibility == Visibility.Visible)
                startArmyHelpText.Margin = new Thickness(ArmyList.ActualWidth + 60, 0, 0, 0);
        }

        private void ArmyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (startArmyHelpText.Visibility != Visibility.Visible)
                return;

            startArmyHelpText.Width = Interface.Changes.ZeroFuse(ArmyGrid.ActualWidth - 45);
            startArmyHelpText.Height = Interface.Changes.ZeroFuse(ArmyGrid.ActualHeight - 50);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (container == null)
                return;

            Interface.Test.TestCanvasPrepare(container.DataContext as Unit);
            Interface.Changes.Move(Interface.Changes.MovingType.ToRight);
        }

        private void armyUnitTest_SizeChanged(object sender, SizeChangedEventArgs e) =>
            Interface.Changes.armyUnitTest_SizeChanged(e);

        private void enemyForTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Interface.Test.TestEnemyPrepare();
            Interface.Test.TestCanvasShow();

            waitingSpinner.Visibility = Visibility.Hidden;
            testConsole.Visibility = Visibility.Hidden;

            Interface.Changes.armyUnitTest_Resize();
        }

        private void enemyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Interface.Test.LoadEnemyGroups();

        private void startFullTest_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Test.startTest(Test.Data.TestTypes.fullTest);

        private void startStatisticTest_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Test.startTest(Test.Data.TestTypes.statisticTest);

        private void startBattleRoyale_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Test.startTest(Test.Data.TestTypes.battleRoyale);


        private void dragWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void dragWindowBottom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                maximizeWindow_MouseLeftButtonDown(null, null);
        }

        public void closeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.Close();

        private void maximizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string newButtonCaption = null;

            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                BorderThickness = new Thickness(0);
                newButtonCaption = "Max";
            }
            else
            {
                WindowState = WindowState.Maximized;
                BorderThickness = new Thickness(5);
                newButtonCaption = "Normal";
            }

            maximizeWindow.Content = String.Format("  {0}  |", newButtonCaption);
        }

        private void minimizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            this.WindowState = WindowState.Minimized;

        private void ArmyGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                ArmyGrid.FontSize += (e.Delta > 0 ? 1 : -1);
        }

        private void waitingSpinner_MediaEnded(object sender, RoutedEventArgs e)
        {
            waitingSpinner.Position = new TimeSpan(0, 0, 1);
            waitingSpinner.Play();
        }
    }
}
