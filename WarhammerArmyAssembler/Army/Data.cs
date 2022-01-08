using System.Collections.Generic;

namespace WarhammerArmyAssembler.Army
{
    class Data
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        public static string Name { get; set; }
        public static string AdditionalName { get; set; }
        public static int ArmyEdition { get; set; }
        public static string MagicPowers { get; set; }
        public static string UnitsImagesDirectory { get; set; }

        public static int MaxPoints = 2000;

        public static int MaxIDindex = 0;
    }
}
