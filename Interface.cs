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

            Ammunition artefacts = new Ammunition() { Name = "Артефакты" };

            foreach (KeyValuePair<string, Ammunition> entry in ArmyBook.Artefact)
            {
                Ammunition artefact = entry.Value.Clone();
                artefact.PointsModifecated = String.Format(" {0} pts", artefact.Points);
                artefacts.Items.Add(artefact);
            }

            main.ArmyList.Items.Add(artefacts);
        }

        public static bool EnoughPointsForAddUnit(string id)
        {
            return (ArmyBook.Units[id].Size * ArmyBook.Units[id].Points) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughPointsForAddArtefact(string id)
        {
            return (ArmyBook.Artefact[id].Points) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            int newPrice = (newSize * Army.Units[id].Points);
            int currentPrice = (Army.Units[id].Size * Army.Units[id].Points);

            return (newPrice - currentPrice) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static void ArmyGridDrop(string id)
        {
            bool slotExists = (Army.GetArmyUnitsNumber(ArmyBook.Units[id].Type) < Army.GetArmyMaxUnitsNumber(ArmyBook.Units[id].Type));
            bool coreUnit = (ArmyBook.Units[id].Type == Unit.UnitType.Core);

            int allHeroes = Army.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (ArmyBook.Units[id].Type == Unit.UnitType.Hero) && (allHeroes >= Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if ((!slotExists && !coreUnit) || lordInHeroSlot)
            {
                string unitType = (ArmyBook.Units[id].Type == Unit.UnitType.Lord || ArmyBook.Units[id].Type == Unit.UnitType.Hero ? "героев" : "отрядов");
                MessageBox.Show(String.Format("Количество {0} данного типа исчерпано", unitType), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!EnoughPointsForAddUnit(id))
            {
                string unitType = (ArmyBook.Units[id].Type == Unit.UnitType.Lord || ArmyBook.Units[id].Type == Unit.UnitType.Hero ? "героя" : "отряда");
                MessageBox.Show(String.Format("Недостаточно очков для добавления {0}", unitType), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Army.AddUnitByID(id);
                ReloadArmyData();
            }
           
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
            main.armyHeroes.Content = String.Format("Героев: {0}/{1} из {2}/{3}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyUnitsNumber(Unit.UnitType.Hero),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            main.armyUnits.Content = String.Format("Отрядов: {0}/{1}/{2} из {3}+/{4}/{5}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyUnitsNumber(Unit.UnitType.Rare),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            main.armyPoints.Content = String.Format("Очков: {0} из {1}", Army.GetArmyPoints(), Army.GetArmyMaxPoints());
            main.armySize.Content = String.Format("Моделей: {0}", Army.GetArmySize());
            main.armyCasting.Content = String.Format("Каст: {0}", 4);
            main.armyDispell.Content = String.Format("Диспелл: {0}", 2);
        }
    }
}
