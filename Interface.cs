﻿using System;
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

        private static List<Unit> GetArmyCategories()
        {
            return new List<Unit>
            {
                new Unit() { Name = "Лорды" },
                new Unit() { Name = "Герои" },
                new Unit() { Name = "Основные" },
                new Unit() { Name = "Специальные" },
                new Unit() { Name = "Редкие" },
            };
        }

        public static void LoadArmyList()
        {
            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.Units)
            {
                Unit unit = entry.Value.Clone();
                unit.PointsModifecated = String.Format(" {0} pts", unit.Points);
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
                main.ArmyList.Items.Add(unitType);
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

            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                Unit unit = entry.Value.Clone();

                unit = unit.GetWeaponsRules();

                unit.InterfaceRules = unit.GetSpecialRules();
                unit.InterfacePoints = unit.GetUnitPoints();
                unit.ID = entry.Key.ToString();

                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                if (unitType.Items.Count <= 0)
                    continue;

                foreach (Unit unit in unitType.Items)
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
