using System;
using System.Collections.Generic;
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
            string id = (string)e.Data.GetData(DataFormats.Text);

            Unit unit = ArmyBook.ArmyBook.Units[id];

            DataGrid l = sender as DataGrid;
            l.Items.Add(new ArmyListViewItem {
                Name = unit.Name,
                Movement = unit.Movement,
                WeaponSkill = unit.WeaponSkill,
                BallisticSkill = unit.BallisticSkill,
                Strength = unit.Strength,
                Toughness = unit.Toughness,
                Wounds = unit.Wounds,
                Initiative = unit.Initiative,
                Attacks = unit.Attacks,
                Leadership = unit.Leadership,
                Points = unit.Points,
            });
        }
    }
}
