using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.Army
{
    class Army
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        public static int GetNextIndex()
        {
            return Units.Count;
        }
    }
}
