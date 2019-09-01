using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.ArmyBook
{
    public class ArmyBook
    {
        public static Dictionary<string, IUnit> Units = new Dictionary<string, IUnit>();
    }
}
