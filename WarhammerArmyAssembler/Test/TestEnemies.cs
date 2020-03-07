using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class TestEnemies
    {
        public static Unit GetByName(string enemyName)
        {
            return new Unit{
                Name = enemyName,
                Size = 1,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 3,
                Initiative = 3,
                Attacks = 3,
                Armour = 5,
            };
        }
    }
}
