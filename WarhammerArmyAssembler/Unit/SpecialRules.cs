using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler
{
    public class SpecialRules
    {
        public string Group { get; set; }

        public bool AutoHit { get; set; }
        public bool AutoWound { get; set; }
        public bool AutoDeath { get; set; }
        public bool HitFirst { get; set; }
        public bool HitLast { get; set; }
        public bool KillingBlow { get; set; }
        public int ExtendedKillingBlow { get; set; }
        public bool HeroicKillingBlow { get; set; }
        public bool PoisonAttack { get; set; }
        public string MultiWounds { get; set; }
        public bool NoArmour { get; set; }
        public bool NoWard { get; set; }
        public bool NoKillingBlow { get; set; }
        public bool NoMultiWounds { get; set; }
        public int ArmourPiercing { get; set; }
        public string Reroll { get; set; }
        public bool Regeneration { get; set; }
        public int ExtendedRegeneration { get; set; }
        public bool ImmuneToPsychology { get; set; }
        public bool ImmuneToPoison { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool BloodFrenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }
        public bool Stupidity { get; set; }
        public bool Undead { get; set; }
        public bool StrengthInNumbers { get; set; }
        public string ImpactHit { get; set; }
        public int ImpactHitByFront { get; set; }
        public bool SteamTank { get; set; }
        public bool Lance { get; set; }
        public bool Flail { get; set; }
        public int ChargeStrengthBonus { get; set; }
        public bool Resolute { get; set; }
        public bool PredatoryFighter { get; set; }
        public bool MurderousProwess { get; set; }
        public bool Bloodroar { get; set; }
        public int AddToHit { get; set; }
        public int SubOpponentToHit { get; set; }
        public int AddToWound { get; set; }
        public int SubOpponentToWound { get; set; }
        public int HitOn { get; set; }
        public int OpponentHitOn { get; set; }
        public int WoundOn { get; set; }
        public int WardForFirstWound { get; set; }
        public int WardForLastWound { get; set; }
        public bool FirstWoundDiscount { get; set; }
        public bool DogsOfWar { get; set; }
        public bool CloseTreeView { get; set; }
        public bool Scout { get; set; }
        public bool Scouts { get; set; }
        public bool FastCavalry { get; set; }
        public bool NotALeader { get; set; }
        public bool General { get; set; }
        public int MagicResistance { get; set; }


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
            ["MagicResistance"] = "Magic resistance ([X])",
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
