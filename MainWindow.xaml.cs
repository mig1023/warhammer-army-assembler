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
using WarhammerArmyAssembler.ArmyBook;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler
{
    public partial class MainWindow : Window
    {
        public static int? CurrentEditedUnit = null;

        public MainWindow()
        {
            InitializeComponent();

            LoadArmyFromXml.LoadArmy("Orcs&Goblins.xml");

            Interface.Interface.main = this;

            Interface.Interface.LoadArmyList();
        }

        private void UnitInArmyList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;
            DragDrop.DoDragDrop(t, t.Tag, DragDropEffects.Copy);
        }

        private void ArmyGrid_Drop(object sender, DragEventArgs e)
        {
            Interface.Interface.ArmyGridDrop(sender, e);
        }

        private void ArmyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if ((grid == null) || (grid.Items.Count <= 0))
                return;

            Unit unitRow = grid.SelectedItems[0] as Unit;

            if (unitRow == null)
                return;

            CurrentEditedUnit = Interface.Interface.IntParse(unitRow.ID);

            unitName.Content = unitRow.Name;
            unitSize.Text = unitRow.Size.ToString();
        }

        private void unitSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentEditedUnit != null)
            {
                int currentEditedUnit = CurrentEditedUnit ?? 0;

                Army.Army.Units[currentEditedUnit].Size = Interface.Interface.IntParse(unitSize.Text);
                Interface.Interface.ReloadArmyData();
            }
        }
    }
}
