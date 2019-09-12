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

            Army.Army.Units.Add(Army.Army.GetNextIndex(), unit);

            ReloadArmyData();
        }

        public static void ReloadArmyData()
        {
            main.ArmyGrid.Items.Clear();

            foreach (KeyValuePair<int, Unit> entry in Army.Army.Units)
            {
                entry.Value.InterfaceRules = entry.Value.GetSpecialRules();
                entry.Value.InterfacePoints = entry.Value.GetUnitPoints();
                entry.Value.ID = entry.Key.ToString();

                main.ArmyGrid.Items.Add(entry.Value);
            }
        }
    }
}
