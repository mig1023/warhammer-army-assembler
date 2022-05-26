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
            ["AdditionalWeapon"] = "Additional weapon|AddToAttacks: 1",
            ["Choppa"] = "Choppa|",
            ["GreatWeapon"] = "Great weapon|AddToStrength: 2, HitLast",
            ["Halberd"] = "Halberd|AddToStrength: 1",
            ["Flail"] = "Flail|Flail",
            ["TwoHandsWeapon"] = "Two hands weapons|AddToAttacks: 1",
            ["Spear"] = "Spear|",
            ["Lance"] = "Lance|Lance",
            ["LightArmour"] = "Light armour|AddToArmour: 6",
            ["HeavyArmour"] = "Heavy armour|AddToArmour: 5",
            ["FullPlateArmour"] = "Full plate armour|AddToArmour: 4",
            ["Shield"] = "Shield|AddToArmour: 6",
            ["LongBow"] = "Longbow|",
            ["Bow"] = "Bow|",
            ["ShortBow"] = "Short bow|",
            ["CommandGroup"] = "Command|CommandGroup",
        };
    }
}
