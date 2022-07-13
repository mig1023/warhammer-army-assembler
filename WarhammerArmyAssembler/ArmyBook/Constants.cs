using System.Collections.Generic;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Constants
    {
        public static double Quarter = 0.25; 
        public static double Hulf = 0.5;
        public static int BaseArmySize = 2000;
        public static int LargeArmySize = 3000;

        public static string CommonXmlOptionPath { get; set; }
        public static string EnemiesOptionPath { get; set; }
        public static Dictionary<string, string> CommonXmlOption { get; set; }
    }
}
