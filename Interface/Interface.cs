using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.Interface
{
    class Interface
    {
        public static MainWindow main = null;

        public static void LoadArmyList()
        {
            MenuItem UnitType = new MenuItem() { Title = "Основные", Points = String.Empty };

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.ArmyBook.Units)
                UnitType.Items.Add(new MenuItem() { ID = entry.Key, Title = entry.Value.Name, Points = string.Format(" ({0} pts)", entry.Value.Points) });

            main.ArmyList.Items.Add(UnitType);
        }

        public static void ArmyGridDrop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            Unit unit = ArmyBook.ArmyBook.Units[id];

            string armyIndex = String.Format("{0}_{1}", unit.ID, Army.Army.GetNextIndex());

            Army.Army.Units.Add(armyIndex, unit);

            unit.InterfaceRules = unit.GetSpecialRules();
            unit.InterfacePoints = unit.GetUnitPoints();
            unit.ID = armyIndex;

            DataGrid l = sender as DataGrid;
            l.Items.Add(unit);
        }
    }
}
