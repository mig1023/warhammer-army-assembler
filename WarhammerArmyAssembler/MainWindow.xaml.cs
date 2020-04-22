using System;
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

        private void ArmyList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView armyList = sender as TreeView;

            if (armyList.SelectedItem is Unit)
                ChangeArmyListDetail((armyList.SelectedItem as Unit).ID, (armyList.SelectedItem as Unit).GroopBold);

            if (armyList.SelectedItem is Option)
                ChangeArmyListDetail((armyList.SelectedItem as Option).ID, (armyList.SelectedItem as Option).GroopBold);
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;

            if (t.Tag == null || String.IsNullOrEmpty(t.Tag.ToString()))
                return;

            FrameworkElement f = sender as FrameworkElement;
            int id = InterfaceOther.IntParse(f.Tag.ToString());

            ChangeArmyListDetail(id);

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (Interface.armybookDetailIsOpen)
                {
                    Interface.armybookDetailIsOpen = false;
                    Interface.Move(Interface.MovingType.ToMain);
                    return;
                }
                else
                {
                    Interface.armybookDetailIsOpen = true;
                    Interface.Move(Interface.MovingType.ToLeft, detail: true);
                }
            }
                
            if (ArmyBook.Artefact.ContainsKey(id) && ArmyBook.Artefact[id].ArtefactAlreadyUsed)
                return;
            else
            {
                Interface.DragSender = sender;
                DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
            }

            HideStartArmyHelpText();
        }

        private void ChangeArmyListDetail(int id, bool group = false)
        {
            if (group)
            {
                armyUnitName.Content = String.Empty;
                armyUnitDescription.Text = String.Empty;
                armyUnitSpecific.Text = String.Empty;
            }
            else if (ArmyBook.Units.ContainsKey(id))
            {
                armyUnitName.Content = ArmyBook.Units[id].Name.ToUpper();
                armyUnitDescription.Text = ArmyBook.Units[id].Description;
                armyUnitSpecific.Text = ArmyBook.Units[id].SelfDescription();
            }
            else if (ArmyBook.Artefact.ContainsKey(id))
            {
                armyUnitName.Content = ArmyBook.Artefact[id].Name.ToUpper();
                armyUnitDescription.Text = ArmyBook.Artefact[id].Description;
                armyUnitSpecific.Text = ArmyBook.Artefact[id].SelfDescription();
            }

            armyUnitName.Foreground = Brushes.White;
            armyUnitName.Background = ( group ? Brushes.White : ArmyBook.MainColor );
            armyUnitName.FontWeight = FontWeights.Bold;

            UpdateLayout();

            armybookDetail.Height = armyUnitDescription.Margin.Top +
                (armyUnitDescription.ActualHeight > 0 ? armyUnitDescription.ActualHeight : 20) +
                (armyUnitSpecific.ActualHeight > 0 ? armyUnitSpecific.ActualHeight : 20) + 20;

            armyUnitSpecific.Margin = Interface.Thick(armybookDetail, left: 20, top: armybookDetail.Margin.Top + armyUnitDescription.ActualHeight + 30);
            armyUnitSpecific.Foreground = ArmyBook.MainColor;
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            int id = InterfaceOther.IntParse((string)e.Data.GetData(DataFormats.Text));

            if ((Interface.DragSender as FrameworkElement).Name == "ArmyGrid")
                return;

            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (!(sender is ScrollViewer))
                Interface.ArmyGridDrop(id, container);

            if (!ArmyChecks.IsUnitExistInArmy(Interface.CurrentSelectedUnit))
                return;

            if (sender is ScrollViewer)
                Interface.ArmyGridDropArtefact(id, Interface.CurrentSelectedUnit);
            else
                InterfaceUnitDetails.UpdateUnitDescription(Interface.CurrentSelectedUnit, Army.Units[Interface.CurrentSelectedUnit]);

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
            {
                int clickedColumnNum = (clickedColumn as DataGridCell).Column.DisplayIndex;

                if (clickedColumnNum == 1 || clickedColumnNum == 3)
                    return;
            }
                
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
            mainPlaceCanvas.Height = e.NewSize.Height;

            closeArmybookDetail.Width = e.NewSize.Height;

            if (!Interface.unitTestIsOpen)
                mainGrid.Width = e.NewSize.Width;
            
            foreach (ScrollViewer scroll in new List<ScrollViewer> { armybookDetailScroll, armyUnitTestScroll })
                scroll.Height = e.NewSize.Height;

            armyUnitTestScroll.Width = Interface.ZeroFuse(e.NewSize.Width - 25);

            foreach (Canvas canvas in new List<Canvas> { errorDetail, mainMenu, mainPlaceCanvas, armyUnitTest })
                canvas.Width = e.NewSize.Width;

            closeErrorDetail.Margin = new Thickness(Interface.ZeroFuse(e.NewSize.Width - closeErrorDetail.Width - 10), 10, 0, 0);

            if (Interface.mainMenuIsOpen)
                Interface.MainMenu();
        }

        private void closeArmybookDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.armybookDetailIsOpen = false;
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

        public void closeMainMenu_MouseDown(object sender, MouseButtonEventArgs e)
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
            if (Interface.mainMenuIsOpen)
                closeMainMenu_MouseDown(null, null);
            else
                Interface.MainMenu();
        }

        public void toNewArmy_MouseDown(object sender, MouseButtonEventArgs e)
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
            double availablePoints = (ArmyParams.GetArmyMaxPoints() - armyCurrentPoint);

            string pointsMsg = String.Format(
                "All points:\t\t{0} pts\n\nAlready used:\t{1} pts / {2}%\n\nAvailable:\t\t{3} pts / {4}%\n\n\n\n",
                ArmyParams.GetArmyMaxPoints(),
                armyCurrentPoint, InterfaceOther.CalcPercent(armyCurrentPoint, ArmyParams.GetArmyMaxPoints()),
                availablePoints, InterfaceOther.CalcPercent(availablePoints, ArmyParams.GetArmyMaxPoints())
            );

            foreach(KeyValuePair<Unit.UnitType, double> entry in unitPercents)
                pointsMsg += String.Format("{0}:\t{1,10} pts / {2}%\t( {3} {4} pts / {5}% )\n\n",
                    entry.Key,
                    units[entry.Key],
                    InterfaceOther.CalcPercent(units[entry.Key], Army.MaxPoints),
                    (entry.Key == Unit.UnitType.Core ? "min" : "max"),
                    (int)(Army.MaxPoints * entry.Value),
                    entry.Value * 100
                );

            MessageBox.Show(pointsMsg);
        }


        private void armyModels_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string baseMsg = String.Format(
                "Normal base:\t{0}\n\nCavalry base:\t{1}\n\nLarge base:\t{2}",
                ArmyParams.GetUnitsNumberByBase(ArmyParams.BasesTypes.normal),
                ArmyParams.GetUnitsNumberByBase(ArmyParams.BasesTypes.cavalry),
                ArmyParams.GetUnitsNumberByBase(ArmyParams.BasesTypes.large)
            );

            MessageBox.Show(baseMsg);
        }

        public void saveArmyToPDF_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ArmyChecks.IsArmyValid())
                ExportPDF.SaveArmy();
            else 
                MessageBox.Show(ArmyChecks.ArmyProblems());

            Interface.Move(Interface.MovingType.ToMain, menu: true);
        }

        public void saveArmyToTXT_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ArmyChecks.IsArmyValid())
                ExportTXT.SaveArmy();
            else
                MessageBox.Show(ArmyChecks.ArmyProblems());

            Interface.Move(Interface.MovingType.ToMain, menu: true);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ArmyList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (startArmyHelpText.Visibility == Visibility.Visible)
                startArmyHelpText.Margin = new Thickness(ArmyList.ActualWidth + 60, 0, 0, 0);
        }

        private void ArmyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (startArmyHelpText.Visibility == Visibility.Visible)
            {
                startArmyHelpText.Width = Interface.ZeroFuse(ArmyGrid.ActualWidth - 45);
                startArmyHelpText.Height = Interface.ZeroFuse(ArmyGrid.ActualHeight - 50);
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (container == null)
                return;

            InterfaceTestUnit.TestCanvasPrepare(container.DataContext as Unit);

            Interface.Move(Interface.MovingType.ToRight);
        }

        public void armyUnitTest_Resize()
        {
            UpdateLayout();

            double marginTop = Interface.ZeroFuse(unitGrid.ActualHeight - 66);

            specialRulesTest.Margin = Interface.Thick(specialRulesTest, top: marginTop);

            marginTop += specialRulesTest.ActualHeight;

            foreach (FrameworkElement element in new List<FrameworkElement> {
                enemyForTestText, enemyForTest, enemyTestUnit, enemyGridContainer,
                enemyGroupText, enemyGroup
            })
                element.Margin = Interface.Thick(enemyForTestText, top: marginTop);

            marginTop += Interface.ZeroFuse(enemyGrid.ActualHeight - 66);

            foreach (FrameworkElement element in new List<FrameworkElement> {
                specialRulesEnemyTest, startFullTest, startStatisticTest, testConsole,
            })
                element.Margin = Interface.Thick(enemyForTestText, top: marginTop);

            double unitTestHeight = (double)enemyForTest.GetValue(Canvas.TopProperty) + enemyForTest.ActualHeight + 50;

            if (enemyGridContainer.Visibility == Visibility.Visible)
            {
                double startButtonPosition = (double)startFullTest.GetValue(Canvas.TopProperty);
                unitTestHeight = startFullTest.Margin.Top + startFullTest.ActualHeight + startButtonPosition + 20;
            }

            if (unitTestHeight + 140 < armyUnitTestScroll.ActualHeight)
                testConsole.Height = Interface.ZeroFuse(armyUnitTestScroll.ActualHeight - unitTestHeight - 20);
            else
            {
                unitTestHeight += 140;
                testConsole.Height = 120;
            }

            armyUnitTest.Height = unitTestHeight;
        }

        private void armyUnitTest_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach(FrameworkElement element in new List<FrameworkElement> {
                unitGrid, specialRulesTest, enemyForTest, enemyGrid, specialRulesEnemyTest, testConsole, enemyGroup
            })
                element.Width = Interface.ZeroFuse(e.NewSize.Width - 120);

            armyUnitTest_Resize();
        }

        private void enemyForTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InterfaceTestUnit.TestEnemyPrepare();
            InterfaceTestUnit.TestCanvasShow();
        }

        private void enemyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InterfaceTestUnit.LoadEnemyGroups();
        }

        private void startFullTest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            armyUnitTest_Resize();
            InterfaceTestUnit.CleanConsole();
            Test.TestFull();
            testConsole.Visibility = Visibility.Visible;
        }

        private void startStatisticTest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            armyUnitTest_Resize();
            InterfaceTestUnit.CleanConsole();
            Test.TestStatistic();
            testConsole.Visibility = Visibility.Visible;
        }

        private void dragWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void closeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void maximizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        }

        private void minimizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
