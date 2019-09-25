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

                if (!ArmyBook.Artefact.ContainsKey(id))
                    Interface.ArmyGridDrop(id);
            }

            DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                DataGridRow container = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);

                if (container != null)
                {
                    Unit unit = container.DataContext as Unit;

                    if (!Interface.EnoughPointsForAddArtefact(id))
                        MessageBox.Show("Количество очков недостаточно добавления предмета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (Interface.EnoughPointsForEditUnit(Interface.IntParse(u.ID), u.Size))
            {
                Army.Units[Interface.IntParse(u.ID)].Size = u.Size;
                Interface.ReloadArmyData();
            }
            else
            {
                u.Size = Army.Units[Interface.IntParse(u.ID)].Size;
                MessageBox.Show("Количество очков недостаточно для изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ArmyGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.DragOver += new DragEventHandler(ArmyGridRow_DragOver);
        }

        private void ArmyGridRow_DragOver(object sender, DragEventArgs e)
        {
            DataGridRow row = (DataGridRow)sender;
            ArmyGrid.SelectedItem = row.Item;
            ArmyGrid.ScrollIntoView(row.Item);

            Unit unit = row.DataContext as Unit;

            string id = (string)e.Data.GetData(DataFormats.Text);

            if (ArmyBook.Artefact.ContainsKey(id))
            {
                if (unit.Type == Unit.UnitType.Hero || unit.Type == Unit.UnitType.Lord)
                    e.Effects = DragDropEffects.Copy;
                else
                    e.Effects = DragDropEffects.None;
            }
        }
    }
}
