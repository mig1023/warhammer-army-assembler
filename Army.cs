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
    }
}
