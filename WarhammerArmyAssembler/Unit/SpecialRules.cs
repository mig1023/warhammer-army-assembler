using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler
{
    class SpecialRules
    {
        public static Dictionary<string, string> All = new Dictionary<string, string>()
        {
            ["AddToCloseCombat"] = "Add to Close Combat Result ([X])",
            ["AddToHit"] = "+[X] To Hit",
            ["AddToWound"] = "+[X] To Wound",
            ["ArmourPiercing"] = "Armour piercing ([X])",
            ["AutoDeath"] = "Slain automatically",
            ["AutoHit"] = "Hit automatically",
            ["AutoWound"] = "Wound automatically",
            ["BloodFrenzy"] = "Blood Frenzy",
            ["Bloodroar"] = "Bloodroar",
            ["ChargeStrengthBonus"] = "+[X] Strength on charge",
            ["ColdBlooded"] = "ColdBlooded",
            ["DogsOfWar"] = "Dogs of War",
            ["ExtendedKillingBlow"] = "Killing Blow ([X]+)",
            ["ExtendedRegeneration"] = "Regeneration ([X]+)",
            ["FastCavalry"] = "Fast Cavalry",
            ["Fear"] = "Fear",
            ["FirstWoundDiscount"] = "Discounts the first wound",
            ["Flail"] = "Flail",
            ["Frenzy"] = "Frenzy",
            ["General"] = String.Empty,
            ["Giant"] = String.Empty,
            ["Hate"] = "Hate",
            ["HeroicKillingBlow"] = "Heroic Killing Blow",
            ["HitFirst"] = "Hit First",
            ["HitLast"] = "Hit Last",
            ["HitOn"] = "Hit on [X]+",
            ["ImmuneToPoison"] = "Immune to poison",
            ["ImmuneToPsychology"] = "Immune to Psychology",
            ["ImpactHit"] = "Impact Hit ([X])",
            ["ImpactHitByFront"] = "Impact Hit ([X])",
            ["KillingBlow"] = "Killing Blow",
            ["Lance"] = "Lance",
            ["LargeBase"] = String.Empty,
            ["MagicPowers"] = String.Empty,
            ["MagicPowersCount"] = String.Empty,
            ["MagicResistance"] = "Magic resistance ([X])",
            ["MagicResistance"] = String.Empty,
            ["MultiWounds"] = "Multiple wounds ([X])",
            ["MurderousProwess"] = "Murderous Prowess",
            ["NoArmour"] = "No Armour",
            ["NoKillingBlow"] = "No Killing Blow",
            ["NoMultiWounds"] = "No Multiple wounds",
            ["NotALeader"] = "Not a leader: can't be General, other models can never use this character's Leadership",
            ["NoWard"] = "No Ward",
            ["OpponentHitOn"] = "Opponent Hit on [X]+",
            ["PoisonAttack"] = "Poison Attack",
            ["PredatoryFighter"] = "Predatory Fighter",
            ["Regeneration"] = "Regeneration",
            ["Reroll"] = "Reroll ([X])",
            ["Resolute"] = "+1 Strength during a turn in which they charge",
            ["Scout"] = "Scout",
            ["Scouts"] = "Scouts",
            ["SteamTank"] = "Steam Tank",
            ["StrengthInNumbers"] = "Strength in numbers!",
            ["Stubborn"] = "Stubborn",
            ["Stupidity"] = "Stupidity",
            ["SubOpponentToHit"] = "-[X] To Hit opponent penalty",
            ["SubOpponentToWound"] = "-[X] To Wound opponent penalty",
            ["Terror"] = "Terror",
            ["Unbreakable"] = "Unbreakable",
            ["Undead"] = "Undead",
            ["WardForFirstWound"] = "Ward save [X]+ for first wound",
            ["WardForLastWound"] = "Ward save [X]+ for last wound",
            ["WoundOn"] = "Wound on [X]+",

        };

        public static List<string> UnitParam = new List<string> {
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
            "Ward"
        };

        public static Dictionary<string, string> RerollsLines = new Dictionary<string, string>
        {
            ["OpponentToHit"] = "opponent re-roll all succeful rolls to Hit",
            ["OpponentToWound"] = "opponent re-roll all succeful rolls to Wound",
            ["OpponentToArmour"] = "opponent re-roll all succeful rolls to Armour Save",
            ["OpponentToWard"] = "opponent re-roll all succeful rolls to Ward",
            ["ToHit"] = "all failed rolls To Hit",
            ["ToShoot"] = "all failed rolls To Shoot",
            ["ToWound"] = "all failed rolls To Wound",
            ["ToLeadership"] = "all failed rolls To Leadership",
            ["ToArmour"] = "all failed rolls To Armour Save",
            ["ToWard"] = "all failed rolls To Ward",
            ["All"] = "all failed rolls",
        };

        private static Dictionary<string, string> IncompatibleRulesList = new Dictionary<string, string>
        {
            ["HitLast"] = "HitFirst",
            ["Frenzy"] = "ImmuneToPsychology",
            ["Stubborn"] = "ImmuneToPsychology",
        };

        public static bool IncompatibleRules(string name, Unit unit)
        {
            if (!IncompatibleRulesList.ContainsKey(name))
                return false;

            List<string> rules = IncompatibleRulesList[name]
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => unit.RuleFromAnyOption(x, out string _, out int _))
                .ToList();

            return rules.Count() > 0;
        }
    }
}
