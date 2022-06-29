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

            // 6th/7th edition common magic items
            ["SwordOfStriking"] = "Sword of Striking|Type: Weapon, AddToHit: 1, Description: +1 to Hit",
            ["SwordOfBattle"] = "Sword of Battle|Type: Weapon, AddToAttacks: 1, Description: +1 Attack",
            ["SwordOfMight"] = "Sword of Might|Type: Weapon, AddToStrength: 1, Description: +1 Strenght",
            ["BitingBlade"] = "Biting Blade|Type: Weapon, ArmourPiercing: 1, Description: -1 Armour save",
            ["EnchantedShield"] = "Enchanted shield|Type: Shield, AddToArmour: 5, Description: 5+ Armour save",
            ["TalismanOfProtection"] = "Talisman of Protection|Type: AdditionalArmour, AddToWard: 5, Description: 6+ Ward save",
            ["StaffOfSorcery"] = "Staff of Sorcery|Type: Arcane, AddToWard: 5, Rule: +1 to dispell, Description: +1 to dispell",
            ["DispellScroll"] = "Dispell Scroll|Type: Arcane, Multiple, Description: Automatically dispel an enemy spell; one use only",
            ["PowerStone"] = "Power Stone|Type: Arcane, Multiple, Description: +2 dice to cast a spell",
            ["WarBanner"] = "War Banner|Type: Banner, AddToCloseCombat: 1, Description: +1 Combat Resolution",

            // 8th edition common magic items
            ["GiantBlade"] = "Giant Blade|Type: Weapon, AddToStrength: 3, Description: " +
                "Close combat attacks made with this sword are resolved at +3 Strength",
            ["SwordOfBloodshed"] = "Sword of Bloodshed|Type: Weapon, AddToAttacks: 3, " +
                "Description: The wielder has +3 Attacks",
            ["ObsidianBlade"] = "Obsidian Blade|Type: Weapon, NoArmour, Description: " +
                "Armour saves cannot be taken against wounds caused by the Obsidian Blade",
            ["OgreBlade"] = "Ogre Blade|Type: Weapon, AddToStrength: 2, Description: " +
                "Close combat attacks made with this sword are resolved at +2 Strength",
        };
    }
}
