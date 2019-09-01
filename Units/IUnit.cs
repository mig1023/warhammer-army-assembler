using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public interface IUnit
    {
        string Name { get; }

        int Movement { get; }
        int WeaponSkill { get; }
        int BallisticSkill { get; }
        int Strength { get; }
        int Toughness { get; }
        int Wounds { get; }
        int Initiative { get; }
        int Attacks { get; }
        int Leadership { get; }
        int Armour { get; }
        int Ward { get; }

        bool HitFirst { get; }
        bool ImmuneToPsychology { get; }
        bool Stubborn { get; }
        bool KillingBlow { get; }
        bool Hate { get; }
        bool Regeneration { get; }
        bool Fear { get; }
        bool Terror { get; }
        bool Frenzy { get; }
        bool PoisonAttack { get; }
        bool Unbreakable { get; }
        bool ColdBlooded { get; } 
    }
}
