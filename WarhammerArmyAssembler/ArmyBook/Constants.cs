using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Constants
    {
        public enum PropertyDiff
        {
            OnlyUnit,
            OnlyOption,
        }

        public static double Quarter = 0.25; 
        public static double Hulf = 0.5;
        public static int BaseArmySize = 2000;
        public static int LargeArmySize = 3000;
        public static int DogsOfWarCategory = 5;
        public static int SingleRunicItem = 1;

        public static string CommonXmlOptionPath { get; set; }
        public static string EnemiesOptionPath { get; set; }
        public static string DogOfWarPath { get; set; }

        public static Dictionary<string, string> CommonXmlOption { get; set; }
        public static Dictionary<string, string> CommonXmlSpecialRules { get; set; }

        public static SortedDictionary<string, string> ProfilesNames = new SortedDictionary<string, string>
        {
            ["Movement"] = "M",
            ["WeaponSkill"] = "WS",
            ["BallisticSkill"] = "BS",
            ["Strength"] = "S",
            ["Toughness"] = "T",
            ["Wounds"] = "W",
            ["Initiative"] = "I",
            ["Attacks"] = "A",
            ["Leadership"] = "Ld",
            ["Armour"] = "AS",
        };

        public static Dictionary<string, string> EnemyPathTypes = new Dictionary<string, string>
        {
            ["Lord"] = "Heroes",
            ["Hero"] = "Heroes",
            ["Mount"] = "Mounts",
            ["Special"] = "Units",
            ["Core"] = "Units",
            ["Rare"] = "Units",
        };

        public static bool IsPropertiesOnlyFor(string name, PropertyDiff diff)
        {
            if (!PropertiesDiff.ContainsKey(name))
                return false;

            else if (PropertiesDiff[name] != diff)
                return false;

            else
                return true;
        }

        public static List<string> GetPropertiesDiff(PropertyDiff diff) =>
            PropertiesDiff.Where(x => x.Value == diff).Select(x => x.Key).ToList();

        private static Dictionary<string, PropertyDiff> PropertiesDiff = new Dictionary<string, PropertyDiff>
        {
            ["Giant"] = PropertyDiff.OnlyUnit,
            ["HellPitAbomination"] = PropertyDiff.OnlyUnit,
            ["ImpactHitByFront"] = PropertyDiff.OnlyUnit,
            ["LargeBase"] = PropertyDiff.OnlyUnit,
            ["MagicPowers"] = PropertyDiff.OnlyUnit,
            ["MagicPowersCount"] = PropertyDiff.OnlyUnit,
            ["MagicResistance"] = PropertyDiff.OnlyUnit,
            ["OpponentHitOn"] = PropertyDiff.OnlyUnit,

            ["AddToCast"] = PropertyDiff.OnlyOption,
            ["AddToDispell"] = PropertyDiff.OnlyOption,
            ["AddToModelsInPack"] = PropertyDiff.OnlyOption,
            ["AddToWard"] = PropertyDiff.OnlyOption,
            ["AddToWizard"] = PropertyDiff.OnlyOption,
            ["Command"] = PropertyDiff.OnlyOption,
            ["Group"] = PropertyDiff.OnlyOption,
            ["MagicItems"] = PropertyDiff.OnlyOption,
            ["MagicItemsPoints"] = PropertyDiff.OnlyOption,
            ["MasterRunic"] = PropertyDiff.OnlyOption,
            ["Mount"] = PropertyDiff.OnlyOption,
            ["Multiple"] = PropertyDiff.OnlyOption,
            ["Name"] = PropertyDiff.OnlyOption,
            ["NativeArmour"] = PropertyDiff.OnlyOption,
            ["OnlyOneInArmy"] = PropertyDiff.OnlyOption,
            ["OnlyOneSuchUnits"] = PropertyDiff.OnlyOption,
            ["OnlyRuleOption"] = PropertyDiff.OnlyOption,
            ["PersonifiedCommander"] = PropertyDiff.OnlyOption,
            ["RandomGroup"] = PropertyDiff.OnlyOption,
            ["TypeUnitIncrese"] = PropertyDiff.OnlyOption,
            ["Virtue"] = PropertyDiff.OnlyOption,
            ["WizardTo"] = PropertyDiff.OnlyOption,
        };

        //public static List<string> OptionProperties = new List<string> {
        //    "Name",
        //    "OnlyOneInArmy",
        //    "OnlyOneSuchUnits",
        //    "Multiple",
        //    "Virtue",
        //    "Honours",
        //    "NativeArmour",
        //    "Group",
        //    "AutoHit",
        //    "AutoWound",
        //    "AutoDeath",
        //    "HitFirst",
        //    "HitLast",
        //    "KillingBlow",
        //    "ExtendedKillingBlow",
        //    "HeroicKillingBlow",
        //    "PoisonAttack",
        //    "MultiWounds",
        //    "NoArmour",
        //    "NoWard",
        //    "NoMultiWounds",
        //    "NoKillingBlow",
        //    "ArmourPiercing",
        //    "Regeneration",
        //    "ExtendedRegeneration",
        //    "ImmuneToPsychology",
        //    "ImmuneToPoison",
        //    "Stubborn",
        //    "Hate",
        //    "Fear",
        //    "Terror",
        //    "Frenzy",
        //    "BloodFrenzy",
        //    "Unbreakable",
        //    "ColdBlooded",
        //    "Reroll",
        //    "Stupidity",
        //    "Undead",
        //    "StrengthInNumbers",
        //    "ImpactHit",
        //    "SteamTank",
        //    "Lance",
        //    "Flail",
        //    "ChargeStrengthBonus",
        //    "Resolute",
        //    "PredatoryFighter",
        //    "MurderousProwess",
        //    "AddToCloseCombat",
        //    "Bloodroar",
        //    "AddToHit",
        //    "SubOpponentToHit",
        //    "AddToWound",
        //    "SubOpponentToWound",
        //    "HitOn",
        //    "OpponentHitOn",
        //    "WoundOn",
        //    "MasterRunic",
        //    "RandomGroup",
        //    "TypeUnitIncrese",
        //    "WardForFirstWound",
        //    "WardForLastWound",
        //    "FirstWoundDiscount",
        //    "NotALeader",
        //    "General",
        //    "MagicItemsPoints",
        //    "AddToWard",
        //    "AddToCast",
        //    "AddToDispell",
        //    "AddToWizard",
        //    "AddToModelsInPack",
        //    "WizardTo",
        //    "MagicItems",
        //    "Command",
        //    "PersonifiedCommander",
        //    "Mount",
        //    "OnlyRuleOption",
        //    "MagicResistance",
        //    "Scout",
        //    "Scouts",
        //};

        public static Dictionary<int, int> ArmySizeAngles = new Dictionary<int, int>
        {
            [-170] = 200,
            [-160] = 225,
            [-150] = 250,
            [-140] = 275,
            [-130] = 300,
            [-120] = 350,
            [-110] = 375,
            [-100] = 400,
            [-90] = 500,
            [-80] = 600,
            [-70] = 700,
            [-60] = 800,
            [-50] = 900,
            [-40] = 1000,
            [-30] = 1250,
            [-20] = 1500,
            [-10] = 1750,
            [0] = 2000,
            [10] = 2100,
            [20] = 2200,
            [30] = 2200,
            [40] = 2400,
            [50] = 2500,
            [60] = 2600,
            [70] = 2700,
            [80] = 2800,
            [90] = 2900,
            [100] = 3000,
            [110] = 3125,
            [120] = 3300,
            [130] = 3400,
            [140] = 3500,
            [150] = 3750,
            [160] = 4000,
        };
    }
}
