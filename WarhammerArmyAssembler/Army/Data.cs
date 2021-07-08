using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Army
{
    class Data
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        public static string Name { get; set; }
        public static string AdditionalName { get; set; }
        public static int ArmyVersion { get; set; }
        public static string MagicPowers { get; set; }

        public static int MaxPoints = 2000;
        public static int MaxIDindex = 0;
        public static bool NoMagicItemsColumn = false;
    }
}
