using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarhammerArmyAssembler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            Interface.main = this;

            ArmyBook.LoadArmy("Orcs&Goblins.xml");

            Interface.LoadArmyList();
            Interface.ReloadArmyData();
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;

            if (t.Tag == null || String.IsNullOrEmpty(t.Tag.ToString()))
                return;

            FrameworkElement f = sender as FrameworkElement;
            int id = Interface.IntParse(f.Tag.ToString());

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (ArmyBook.Units.ContainsKey(id))
                {
                    armyUnitName.Content = ArmyBook.Units[id].Name.ToUpper();
                    armyUnitDescription.Text = ArmyBook.Units[id].Description;

                }

                if (ArmyBook.Artefact.ContainsKey(id))
                {
                    armyUnitName.Content = ArmyBook.Artefact[id].Name.ToUpper();
                    armyUnitDescription.Text = ArmyBook.Artefact[id].Description;
                }

                armyUnitName.Foreground = Brushes.White;
                armyUnitName.Background = ArmyBook.MainColor;
                armyUnitName.FontWeight = FontWeights.Bold;

                UpdateLayout();

                armybookDetail.Height = armyUnitDescription.Margin.Top +
                    (armyUnitDescription.ActualHeight > 0 ? armyUnitDescription.ActualHeight : 20) + 20;

                Interface.Move(Interface.MovingType.ToLeft);
            }

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
            int id = Interface.IntParse((string)e.Data.GetData(DataFormats.Text));

            if ((Interface.DragSender as FrameworkElement).Name == "ArmyGrid")
                return;

            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            Interface.ArmyGridDrop(id, container);
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

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

            int pointsDiff = u.GetUnitPoints() - Army.Units[u.ID].GetUnitPoints();

            if (!Interface.EnoughPointsForEditUnit(u.ID, u.Size))
                u.Size = ErrorAndReturnSizeBack("Количество очков недостаточно для изменения", u.ID);
            else if (u.IsHero() && u.Size > 1)
                u.Size = ErrorAndReturnSizeBack("Герои всегда одиноки", u.ID);
            else if ((u.MaxSize != 0) && (u.Size > u.MaxSize))
                u.Size = ErrorAndReturnSizeBack("Размер отряда превышает максимально допустимый", u.ID);
            else if (u.Size < u.MinSize)
                u.Size = ErrorAndReturnSizeBack("Размер отряда меньше минимально допустимого", u.ID);
            else if ((u.Size > Army.Units[u.ID].Size) && (!Army.IsArmyUnitsPointsPercentOk(u.Type, pointsDiff)))
                u.Size = ErrorAndReturnSizeBack(Interface.UnitPercentError(u.Type), u.ID);
            else
                Army.Units[u.ID].Size = u.Size;

            Interface.ReloadArmyData();
        }

        private void ArmyGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.DragOver += new DragEventHandler(ArmyGridRow_DragOver);
        }

        private void ArmyGridRow_DragOver(object sender, DragEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Unit unit = row.DataContext as Unit;

            ArmyGrid.SelectedItem = row.Item;
            ArmyGrid.ScrollIntoView(row.Item);

            int id = Interface.IntParse((string)e.Data.GetData(DataFormats.Text));

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                bool usable = ArmyBook.Artefact[id].IsUsableByUnit(unit.MagicItemsType);
                e.Effects = ((unit.MagicItems > 0) && usable ? DragDropEffects.Copy : DragDropEffects.None);
            }
        }

        private void ArmyGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (container == null)
                return;

            Unit unit = container.DataContext as Unit;

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                Interface.UpdateUnitDescription(unit.ID, unit);
                Interface.Move(Interface.MovingType.ToRight);
            }

            Interface.DragSender = sender;

            DragDrop.DoDragDrop(container, unit.ID.ToString(), DragDropEffects.Copy);
        }

        private void unitDelete_Drop(object sender, DragEventArgs e)
        {
            int id = Interface.IntParse((string)e.Data.GetData(DataFormats.Text));

            Interface.UnitDeleteDrop(id);
        }

        private void mainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainGrid.Height = e.NewSize.Height;
            mainGrid.Width = e.NewSize.Width;

            unitDetailScroll.Height = e.NewSize.Height - 70;
            unitDetailScroll.Margin = new Thickness(e.NewSize.Width - unitDetailScroll.Width, 70, 0, 0);
            unitDetailScrollHead.Margin = Interface.Thick(unitDetailScroll, top: 0);

            armybookDetailScroll.Height = e.NewSize.Height - 70;

            errorDetail.Width = e.NewSize.Width;
            closeErrorDetail.Margin = new Thickness(e.NewSize.Width - closeErrorDetail.Width - 10, 10, 0, 0);
        }

        private void closeDetail_Click(object sender, RoutedEventArgs e)
        {
            Interface.Move(Interface.MovingType.ToMain);
        }
    }
}
