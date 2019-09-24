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

                Interface.ArmyGridDrop(id);
            }

            DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            Interface.ArmyGridDrop(id);
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
    }
}
