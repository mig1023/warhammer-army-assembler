using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public string TestListName { get; set; }

        public static Enemy GetByName(string enemyName)
        {
            foreach (List<Enemy> enemyList in new List<List<Enemy>>
            {
                EnemiesSoldiers, EnemiesMonsters, EnemiesUnits, EnemiesHeroes
            })
                foreach (Enemy enemy in enemyList)
                    if (enemy.TestListName == enemyName)
                        return enemy;

            return null;
        }

        public static List<string> GetEnemiesGroups()
        {
            return new List<string>
            {
                "Single soldiers",
                "Monsters",
                "Units",
                "Lords and heroes",
            };
        }

        public static List<Enemy> GetEnemiesByGroup(string groupName)
        {
            if (groupName == "Single soldiers")
                return new List<Enemy>(EnemiesSoldiers);
            else if (groupName == "Monsters")
                return new List<Enemy>(EnemiesMonsters);
            else if (groupName == "Units")
                return new List<Enemy>(EnemiesUnits);
            else if (groupName == "Lords and heroes")
                return new List<Enemy>(EnemiesHeroes);
            else
                return new List<Enemy> { };
        }

        private static List<Enemy> EnemiesSoldiers = new List<Enemy>
        {
            new Enemy
            {
                Name = "Empire soldier",
                TestListName = "Empire soldier <-- soldier, Empire",
                Size = 1,
                Movement = UnitParam.SetValue(4),
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

            new Enemy
            {
                Name = "Saurus warrior",
                TestListName = "Saurus warrior <-- soldier, Lizardmen",
                Size = 1,
                Movement = UnitParam.SetValue(4),
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

            new Enemy
            {
                Name = "Chosen Knights of Chaos",
                TestListName = "Chosen Knights <-- mount soldier, Chaos",
                Size = 1,
                Movement = UnitParam.SetValue(4),
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
        };

        private static List<Enemy> EnemiesMonsters = new List<Enemy>
        {
            new Enemy
            {
                Name = "Troll",
                TestListName = "Troll <-- monster, Orcs&Goblin",
                Size = 1,
                Movement = UnitParam.SetValue(6),
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

            new Enemy
            {
                Name = "Star Dragon",
                TestListName = "Star Dragon <-- monster, High Elves",
                Size = 1,
                Movement = UnitParam.SetValue(6),
                WeaponSkill = 7,
                BallisticSkill = 0,
                Strength = 7,
                Toughness = 6,
                Wounds = 7,
                Initiative = 2,
                Attacks = 6,
                Leadership = 9,
                Armour = 3,
                Terror = true,
            },
        };

        private static List<Enemy> EnemiesUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "Empire swordmens",
                TestListName = "20 Empire swordmens <-- unit, Empire",
                Size = 20,
                Movement = UnitParam.SetValue(4),
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 7,
                Armour = 6,
                Type = UnitType.Core
            },

            new Enemy
            {
                Name = "Bloodletters",
                TestListName = "20 Bloodletters <-- unit, Chaos",
                Type = UnitType.Core,
                Size = 20,
                Movement = UnitParam.SetValue(4),
                WeaponSkill = 5,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 3,
                Wounds = 1,
                Initiative = 4,
                Attacks = 1,
                Leadership = 8,
                Armour = 6,
                Frenzy = true
            },

            new Enemy
            {
                Name = "Sword Masters of Hoeth",
                TestListName = "16 Sword Master <-- unit, High Elves",
                Type = UnitType.Rare,
                Size = 16,
                Movement = UnitParam.SetValue(5),
                WeaponSkill = 6,
                BallisticSkill = 4,
                Strength = 5,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 5,
                HitFirst = true
            },

        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            new Enemy
            {
                Name = "Tretch Craventail",
                TestListName = "Tretch Craventail <-- hero, Skaven",
                Size = 1,
                Movement = UnitParam.SetValue(5),
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
            },

            new Enemy
            {
                Name = "Bloodthister",
                TestListName = "Greater Daemon Bloodthister <-- lord, Chaos",
                Size = 1,
                Movement = UnitParam.SetValue(6),
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
