using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WarhammerArmyAssembler
{
    class Interface
    {
        public static MainWindow main = null;

        public static ObservableCollection<Unit> ArmyInInterface = new ObservableCollection<Unit>();

        public static void LoadArmyList()
        {
            Unit UnitType = new Unit() { Name = "Основные" };

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.Units)
            {
                Unit unit = entry.Value.Clone();
                unit.PointsModifecated = String.Format(" {0} pts", unit.Points);
                UnitType.Items.Add(unit);
            }

            main.ArmyList.Items.Add(UnitType);
        }

        public static void ArmyGridDrop(string id)
        {
            Army.AddUnitByID(id);

            ReloadArmyData();
        }

        public static int IntParse(string line)
        {
            int value = 0;

            bool success = Int32.TryParse(line, out value);

            return (success ? value : 0);
        }

        public static void ReloadArmyData()
        {
            ArmyInInterface.Clear();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                Unit unit = entry.Value.Clone();

                unit = unit.GetWeaponsRules();

                unit.InterfaceRules = unit.GetSpecialRules();
                unit.InterfacePoints = unit.GetUnitPoints();
                unit.ID = entry.Key.ToString();

                ArmyInInterface.Add(unit);
            }

            main.ArmyGrid.ItemsSource = ArmyInInterface;
            main.armyPoints.Content = String.Format("Очков: {0}", Army.GetArmyPoints());
            main.armySize.Content = String.Format("Моделей: {0}", Army.GetArmySize());
            main.armyCasting.Content = String.Format("Каст: {0}", 4);
            main.armyDispell.Content = String.Format("Диспелл: {0}", 2);
        }
    }
}
