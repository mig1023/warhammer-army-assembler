using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Army
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        public static int GetNextIndex()
        {
            return Units.Count;
        }

        public static void AddUnitByID(string id)
        {
            Unit unit = ArmyBook.Units[id].Clone();

            Units.Add(GetNextIndex(), unit);
        }

        public static int GetArmyPoints()
        {
            int points = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                points += entry.Value.GetUnitPoints();

            return points;
        }

        public static int GetArmySize()
        {
            int size = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                size += entry.Value.Size;

            return size;
        }

        public static int GetArmyLords()
        {
            int lords = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.Type == Unit.UnitType.Lord)
                    lords += entry.Value.Size;

            return lords;
        }

        public static int GetArmyHero()
        {
            int heroes = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.Type == Unit.UnitType.Hero)
                    heroes += entry.Value.Size;

            return heroes;
        }
    }
}
