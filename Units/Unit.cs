using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Units
{
    class Unit : IUnit
    {
        public string Name { get; }

        public int Movement { get; }
        public int WeaponSkill { get; }
        public int BallisticSkill { get; }
        public int Strength { get; }
        public int Toughness { get; }
        public int Wounds { get; }
        public int Initiative { get; }
        public int Attacks { get; }
        public int Leadership { get; }
        public int Armour { get; }
        public int Ward { get; }

        public bool HitFirst { get; }
        public bool ImmuneToPsychology { get; }
        public bool Stubborn { get; }
        public bool KillingBlow { get; }
        public bool Hate { get; }
        public bool Regeneration { get; }
        public bool Fear { get; }
        public bool Terror { get; }
        public bool Frenzy { get; }
        public bool PoisonAttack { get; }
        public bool Unbreakable { get; }
        public bool ColdBlooded { get; }
    }
}
