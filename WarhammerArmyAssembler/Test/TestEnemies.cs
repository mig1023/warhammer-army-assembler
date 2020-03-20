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
            foreach (Unit enemy in Enemies)
                if (enemy.Name == enemyName)
                    return enemy;

            return null;
        }

        public static List<Unit> GetAllEnemies()
        {
            return new List<Unit>(Enemies);
        }

        private static List<Unit> Enemies = new List<Unit>
        {
            new Unit
            {
                Name = "Empire soldier",
                Size = 1,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 7,
                Armour = 6
            },

            new Unit
            {
                Name = "Saurus warrior",
                Size = 1,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 0,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 1,
                Attacks = 2,
                Leadership = 8,
                Armour = 4,
                ColdBlooded = true
            },

            new Unit
            {
                Name = "Chosen Knights of Chaos",
                Size = 1,
                Movement = 4,
                WeaponSkill = 6,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 2
            },

            new Unit
            {
                Name = "Troll",
                Size = 1,
                Movement = 6,
                WeaponSkill = 3,
                BallisticSkill = 1,
                Strength = 5,
                Toughness = 4,
                Wounds = 3,
                Initiative = 1,
                Attacks = 3,
                Leadership = 4,
                Fear = true,
                Regeneration = true,
                Stupidity = true
            },

            new Unit
            {
                Name = "Skaven Tretch Craventail",
                Size = 1,
                Movement = 5,
                WeaponSkill = 5,
                BallisticSkill = 4,
                Strength = 4,
                Toughness = 4,
                Wounds = 2,
                Initiative = 6,
                Attacks = 4,
                Leadership = 6,
                Armour = 5,
                Ward = 4,
                Fear = true,
                Regeneration = true,
                Stupidity = true
            },

            new Unit
            {
                Name = "Chaos Greater Daemon Bloodthister",
                Size = 1,
                Movement = 6,
                WeaponSkill = 10,
                BallisticSkill = 0,
                Strength = 7,
                Toughness = 6,
                Wounds = 7,
                Initiative = 10,
                Attacks = 7,
                Leadership = 9,
                Armour = 4,
                Ward = 5,
                Terror = true,
                KillingBlow = true
            },
        };
    }
}
