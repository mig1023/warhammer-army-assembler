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
                WeaponSkill = UnitParam.SetValue(3),
                BallisticSkill = UnitParam.SetValue(3),
                Strength = UnitParam.SetValue(3),
                Toughness = UnitParam.SetValue(3),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(3),
                Attacks = UnitParam.SetValue(1),
                Leadership = UnitParam.SetValue(7),
                Armour = 6
            },

            new Enemy
            {
                Name = "Saurus warrior",
                TestListName = "Saurus warrior <-- soldier, Lizardmen",
                Size = 1,
                Movement = UnitParam.SetValue(4),
                WeaponSkill = UnitParam.SetValue(3),
                BallisticSkill = UnitParam.SetValue(0),
                Strength = UnitParam.SetValue(4),
                Toughness = UnitParam.SetValue(4),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(1),
                Attacks = UnitParam.SetValue(2),
                Leadership = UnitParam.SetValue(8),
                Armour = 4,
                ColdBlooded = true
            },

            new Enemy
            {
                Name = "Chosen Knights of Chaos",
                TestListName = "Chosen Knights <-- mount soldier, Chaos",
                Size = 1,
                Movement = UnitParam.SetValue(4),
                WeaponSkill = UnitParam.SetValue(6),
                BallisticSkill = UnitParam.SetValue(3),
                Strength = UnitParam.SetValue(4),
                Toughness = UnitParam.SetValue(4),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(5),
                Attacks = UnitParam.SetValue(2),
                Leadership = UnitParam.SetValue(8),
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
                WeaponSkill = UnitParam.SetValue(3),
                BallisticSkill = UnitParam.SetValue(1),
                Strength = UnitParam.SetValue(5),
                Toughness = UnitParam.SetValue(4),
                Wounds = UnitParam.SetValue(3),
                Initiative = UnitParam.SetValue(1),
                Attacks = UnitParam.SetValue(3),
                Leadership = UnitParam.SetValue(4),
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
                WeaponSkill = UnitParam.SetValue(7),
                BallisticSkill = UnitParam.SetValue(0),
                Strength = UnitParam.SetValue(7),
                Toughness = UnitParam.SetValue(6),
                Wounds = UnitParam.SetValue(7),
                Initiative = UnitParam.SetValue(2),
                Attacks = UnitParam.SetValue(6),
                Leadership = UnitParam.SetValue(9),
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
                WeaponSkill = UnitParam.SetValue(3),
                BallisticSkill = UnitParam.SetValue(3),
                Strength = UnitParam.SetValue(3),
                Toughness = UnitParam.SetValue(3),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(3),
                Attacks = UnitParam.SetValue(1),
                Leadership = UnitParam.SetValue(7),
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
                WeaponSkill = UnitParam.SetValue(5),
                BallisticSkill = UnitParam.SetValue(0),
                Strength = UnitParam.SetValue(5),
                Toughness = UnitParam.SetValue(3),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(4),
                Attacks = UnitParam.SetValue(1),
                Leadership = UnitParam.SetValue(8),
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
                WeaponSkill = UnitParam.SetValue(6),
                BallisticSkill = UnitParam.SetValue(4),
                Strength = UnitParam.SetValue(5),
                Toughness = UnitParam.SetValue(3),
                Wounds = UnitParam.SetValue(1),
                Initiative = UnitParam.SetValue(5),
                Attacks = UnitParam.SetValue(2),
                Leadership = UnitParam.SetValue(8),
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
                WeaponSkill = UnitParam.SetValue(5),
                BallisticSkill = UnitParam.SetValue(4),
                Strength = UnitParam.SetValue(4),
                Toughness = UnitParam.SetValue(4),
                Wounds = UnitParam.SetValue(2),
                Initiative = UnitParam.SetValue(6),
                Attacks = UnitParam.SetValue(4),
                Leadership = UnitParam.SetValue(6),
                Armour = 5,
                Ward = 4,
            },

            new Enemy
            {
                Name = "Bloodthister",
                TestListName = "Greater Daemon Bloodthister <-- lord, Chaos",
                Size = 1,
                Movement = UnitParam.SetValue(6),
                WeaponSkill = UnitParam.SetValue(10),
                BallisticSkill = UnitParam.SetValue(0),
                Strength = UnitParam.SetValue(7),
                Toughness = UnitParam.SetValue(6),
                Wounds = UnitParam.SetValue(7),
                Initiative = UnitParam.SetValue(10),
                Attacks = UnitParam.SetValue(7),
                Leadership = UnitParam.SetValue(9),
                Armour = 4,
                Ward = 5,
                Terror = true,
                KillingBlow = true
            },
        };
    }
}
