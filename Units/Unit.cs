using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WarhammerArmyAssembler.Units
{
    public class Unit
    {
        public enum UnitType
        {
            Lord,
            Hero,
            Core,
            Special,
            Rare,
        }

        public string Name { get; set; }
        public string ID { get; set; }

        public UnitType Type { get; set; }

        public int Size { get; set; }

        public int Points { get; set; }

        public int Movement { get; set; }
        public int WeaponSkill { get; set; }
        public int BallisticSkill { get; set; }
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Wounds { get; set; }
        public int Initiative { get; set; }
        public int Attacks { get; set; }
        public int Leadership { get; set; }
        public int Armour { get; set; }
        public int Ward { get; set; }

        public bool ImmuneToPsychology { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }

        public bool HitFirst { get; set; }
        public bool Regeneration { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }

        public List<Ammunition> Weapons = new List<Ammunition>();

        public string InterfaceRules { get; set; }

        public int InterfacePoints { get; set; }

        public int GetUnitPoints()
        {
            return Size * Points;
        }

        public string GetSpecialRules()
        {
            string rules = String.Empty;

            if (ImmuneToPsychology)
                rules += "иммунен к психологии; ";

            if (Stubborn)
                rules += "упорность; ";

            if (Hate)
                rules += "ненависть; ";

            if (Fear)
                rules += "страх; ";

            if (Terror)
                rules += "ужас; ";

            if (Frenzy)
                rules += "бешенство; ";

            if (Unbreakable)
                rules += "несломимость; ";

            if (ColdBlooded)
                rules += "хладнокровие; ";

            if (HitFirst)
                rules += "всегда бьёт первым; ";

            if (Regeneration)
                rules += "регенерация; ";

            if (KillingBlow)
                rules += "смертельный удар; ";

            if (PoisonAttack)
                rules += "ядовитые атаки; ";

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }
    }
}
