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

        public static string ArmyName { get; set; }

        public static int MaxPoints = 2000;
        public static int MaxIDindex = 0;
    }
}
