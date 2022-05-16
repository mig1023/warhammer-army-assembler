using System.Collections.Generic;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Constants
    {
        public static double Quarter = 0.25; 
        public static double Hulf = 0.5;
        public static int BaseArmySize = 2000;
        public static int LargeArmySize = 3000;

        public static Dictionary<string, string> BanalXmlOption = new Dictionary<string, string>
        {
            ["HandWeapon"] = "Hand weapon|",
            ["Spear"] = "Spear|",
            ["LightArmour"] = "Light armour|AddToArmour: 6",
            ["HeavyArmour"] = "Heavy armour|AddToArmour: 5",
            ["Shield"] = "Shield|AddToArmour: 6",
            ["LongBow"] = "Longbow|",
            ["Bow"] = "Bow|",
            ["ShortBow"] = "Short bow|",
            ["GreatWeapon"] = "Great weapon|AddToStrength: 2, HitLast",
        };
    }
}
