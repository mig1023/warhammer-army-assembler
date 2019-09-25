using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;

namespace WarhammerArmyAssembler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ArmyBook.LoadArmy("Orcs&Goblins.xml");

            Interface.main = this;

            Interface.LoadArmyList();
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;

            if (t.Tag == null || String.IsNullOrEmpty(t.Tag.ToString()))
                return;

            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                FrameworkElement f = sender as FrameworkElement;
                string id = f.Tag as string;

                unitName.Content = ArmyBook.Units[id].Name;

                ThicknessAnimation move = new ThicknessAnimation();
                move.Duration = TimeSpan.FromSeconds(0.2);
                move.From = mainGrid.Margin;
                move.To = new Thickness(250, 0, 0, 0);
                mainGrid.BeginAnimation(MarginProperty, move);
            }

            Interface.DragSender = sender;

            DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            if ((Interface.DragSender as FrameworkElement).Name == "ArmyGrid")
                return;

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

                if (container != null)
                {
                    Unit unit = container.DataContext as Unit;

                    if (!Interface.EnoughPointsForAddArtefact(id))
                        MessageBox.Show("Количество очков недостаточно добавления предмета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (!Interface.EnoughUnitPointsForAddArtefact(id, Interface.IntParse(unit.ID)))
                        MessageBox.Show("Недостаточно очков магических предметов для добавления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        Army.Units[Interface.IntParse(unit.ID)].AddAmmunition(id);
                        Interface.ReloadArmyData();
                    }
                }
            }
            else
                Interface.ArmyGridDrop(id);
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
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

        private void ArmyGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Unit u = e.Row.Item as Unit;

            if (!Interface.EnoughPointsForEditUnit(Interface.IntParse(u.ID), u.Size))
            {
                u.Size = Army.Units[Interface.IntParse(u.ID)].Size;
                MessageBox.Show("Количество очков недостаточно для изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (u.IsHero() && u.Size > 1)
            {
                MessageBox.Show("Герои всегда одиноки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                u.Size = Army.Units[Interface.IntParse(u.ID)].Size;
            }
            else
                Army.Units[Interface.IntParse(u.ID)].Size = u.Size;

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

            string id = (string)e.Data.GetData(DataFormats.Text);

            if (ArmyBook.Artefact.ContainsKey(id))
                e.Effects = (unit.MagicItems > 0 ? DragDropEffects.Copy : DragDropEffects.None);
        }

        private void ArmyGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

            if (container == null)
                return;

            Unit unit = container.DataContext as Unit;

            Interface.DragSender = sender;

            DragDrop.DoDragDrop(container, unit.ID, DragDropEffects.Copy);
        }

        private void unitDelete_Drop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            Interface.UnitDeleteDrop(id);
        }

        private void mainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainGrid.Height = e.NewSize.Height;
            armybookDetail.Height = e.NewSize.Height;
            mainGrid.Width = e.NewSize.Width;
        }

        private void closeArmybookDetail_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation move = new ThicknessAnimation();
            move.Duration = TimeSpan.FromSeconds(0.2);
            move.From = mainGrid.Margin;
            move.To = new Thickness(0, 0, 0, 0);
            mainGrid.BeginAnimation(MarginProperty, move);
        }
    }
}
