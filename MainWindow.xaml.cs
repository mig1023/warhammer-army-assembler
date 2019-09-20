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
        public static int? CurrentEditedUnit = null;

        public MainWindow()
        {
            InitializeComponent();

            LoadArmyFromXml.LoadArmy("Orcs&Goblins.xml");

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

        private void ArmyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid == null || grid.Items.Count <= 0 || grid.SelectedItems.Count <= 0)
                return;

            Unit unitRow = grid.SelectedItems[0] as Unit;

            if (unitRow == null)
                return;

            CurrentEditedUnit = Interface.IntParse(unitRow.ID);

            unitName.Content = unitRow.Name;
            unitSize.Text = unitRow.Size.ToString();
            spetialRules.Content = unitRow.GetSpecialRules();
            spetialAmmunition.Content = unitRow.GetAmmunition();
        }

        private void unitSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentEditedUnit != null)
            {
                int currentEditedUnit = CurrentEditedUnit ?? 0;

                Army.Units[currentEditedUnit].Size = Interface.IntParse(unitSize.Text);
                Interface.ReloadArmyData();
            }
        }
    }
}
