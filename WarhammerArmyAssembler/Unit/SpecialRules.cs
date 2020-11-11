using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class SpecialRules
    {
        public static Dictionary<string, string> All = new Dictionary<string, string>()
        {
            ["ImmuneToPsychology"] = "Immune to Psychology",
            ["Stubborn"] = "Stubborn",
            ["Hate"] = "Hate",
            ["Fear"] = "Fear",
            ["Terror"] = "Terror",
            ["Frenzy"] = "Frenzy",
            ["BloodFrenzy"] = "Blood Frenzy",
            ["Unbreakable"] = "Unbreakable",
            ["ColdBlooded"] = "ColdBlooded",
            ["AutoHit"] = "Hit automatically",
            ["AutoWound"] = "Wound automatically",
            ["AutoDeath"] = "Slain automatically",
            ["HitFirst"] = "Hit First",
            ["HitLast"] = "Hit Last",
            ["Regeneration"] = "Regeneration",
            ["KillingBlow"] = "Killing Blow",
            ["ExtendedKillingBlow"] = "Killing Blow ([X]+)",
            ["HeroicKillingBlow"] = "Heroic Killing Blow",
            ["PoisonAttack"] = "Poison Attack",
            ["MultiWounds"] = "Multiple wounds ([X])",
            ["NoArmour"] = "No Armour",
            ["NoWard"] = "No Ward",
            ["ArmourPiercing"] = "Armour piercing ([X])",
            ["Reroll"] = "Reroll ([X])",
            ["ImpactHit"] = "Impact Hit ([X])",
            ["SteamTank"] = "Steam Tank",
            ["Stupidity"] = "Stupidity",
            ["Undead"] = "Undead",
            ["StrengthInNumbers"] = "Strength in numbers!",
            ["Lance"] = "Lance",
            ["Flail"] = "Flail",
            ["Resolute"] = "+1 Strength during a turn in which they charge",
            ["PredatoryFighter"] = "Predatory Fighter",
            ["MurderousProwess"] = "Murderous Prowess",
            ["AddToCloseCombat"] = "Add to Close Combat Result ([X])",
            ["Bloodroar"] = "Bloodroar",
            ["AddToHit"] = "+[X] To Hit",
            ["SubOpponentToHit"] = "-[X] To Hit opponent penalty",
            ["AddToWound"] = "+[X] To Wound",
            ["SubOpponentToWound"] = "-[X] To Wound opponent penalty",
            ["HitOn"] = "Hit on [X]+",
            ["WoundOn"] = "Wound on [X]+",
            ["NoMultiWounds"] = "No Multiple wounds",
            ["NoKillingBlow"] = "No Killing Blow",
            ["WardForFirstWound"] = "Ward save [X]+ for first wound",
            ["WardForLastWound"] = "Ward save [X]+ for last wound",
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
    }
}
