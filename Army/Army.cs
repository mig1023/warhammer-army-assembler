using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.Army
{
    class Army
    {
        public static Dictionary<string, Unit> Units = new Dictionary<string, Unit>();

        public static int GetUnitPoints(string unitID)
        {
            return Units[unitID].Size * Units[unitID].Points;
        }

        public static string GetSpecialRules(string unitID)
        {
            string rules = String.Empty;

            if (Units[unitID].ImmuneToPsychology)
                rules += "иммунен к психологии; ";

            if (Units[unitID].Stubborn)
                rules += "упорность; ";

            if (Units[unitID].Hate)
                rules += "ненависть; ";

            if (Units[unitID].Fear)
                rules += "страх; ";

            if (Units[unitID].Terror)
                rules += "ужас; ";

            if (Units[unitID].Frenzy)
                rules += "бешенство; ";

            if (Units[unitID].Unbreakable)
                rules += "несломимость; ";

            if (Units[unitID].ColdBlooded)
                rules += "хладнокровие; ";

            if (Units[unitID].HitFirst)
                rules += "всегда бьёт первым; ";

            if (Units[unitID].Regeneration)
                rules += "регенерация; ";

            if (Units[unitID].KillingBlow)
                rules += "смертельный удар; ";

            if (Units[unitID].PoisonAttack)
                rules += "ядовитые атаки; ";

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }
    }
}
