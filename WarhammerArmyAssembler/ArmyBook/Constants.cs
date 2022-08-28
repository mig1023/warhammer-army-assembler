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

        public static List<string> ProfilesNames = new List<string> {
            "Movement",
            "WeaponSkill",
            "BallisticSkill",
            "Strength",
            "Toughness",
            "Wounds",
            "Initiative",
            "Attacks",
            "Leadership",
            "Armour",
        };

        public static List<string> OptionProperties = new List<string> {
            "Name",
            "OnlyOneInArmy",
            "OnlyOneSuchUnits",
            "OnlyGroup",
            "Multiple",
            "Virtue",
            "Honours",
            "NativeArmour",
            "Group",
            "AutoHit",
            "AutoWound",
            "AutoDeath",
            "HitFirst",
            "HitLast",
            "KillingBlow",
            "ExtendedKillingBlow",
            "HeroicKillingBlow",
            "PoisonAttack",
            "MultiWounds",
            "NoArmour",
            "NoWard",
            "NoMultiWounds",
            "NoKillingBlow",
            "ArmourPiercing",
            "Regeneration",
            "ExtendedRegeneration",
            "ImmuneToPsychology",
            "ImmuneToPoison",
            "Stubborn",
            "Hate",
            "Fear",
            "Terror",
            "Frenzy",
            "BloodFrenzy",
            "Unbreakable",
            "ColdBlooded",
            "Reroll",
            "Stupidity",
            "Undead",
            "StrengthInNumbers",
            "ImpactHit",
            "SteamTank",
            "Lance",
            "Flail",
            "ChargeStrengthBonus",
            "Resolute",
            "PredatoryFighter",
            "MurderousProwess",
            "AddToCloseCombat",
            "Bloodroar",
            "AddToHit",
            "SubOpponentToHit",
            "AddToWound",
            "SubOpponentToWound",
            "HitOn",
            "OpponentHitOn",
            "WoundOn",
            "Runic",
            "MasterRunic",
            "RandomGroup",
            "TypeUnitIncrese",
            "WardForFirstWound",
            "WardForLastWound",
            "FirstWoundDiscount",
            "NotALeader",
            "PerModel",
            "MagicItemsPoints",
            "AddToWard",
            "AddToCast",
            "AddToDispell",
            "AddToWizard",
            "AddToModelsInPack",
            "WizardTo",
            "MagicItems",
            "CommandGroup",
            "PersonifiedCommander",
            "Mount",
            "OnlyRuleOption",
        };
    }
}
