﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public static int MaxIDindex = -10;

        public string TestListName { get; set; }

        public static Enemy GetByName(string enemyName)
        {
            foreach (List<Enemy> enemyList in new List<List<Enemy>>
            {
                EnemiesSoldiers, EnemiesMonsters, EnemiesHeroes,
                EnemiesCoreUnits, EnemiesSpecialUnits, EnemiesRareUnits,
            })
                foreach (Enemy enemy in enemyList)
                    if (enemy.TestListName == enemyName)
                        return enemy.SetID();

            return null;
        }

        private Enemy SetID()
        {
            MaxIDindex -= 1;

            this.ID = MaxIDindex;

            return this;
        }

        public static List<string> GetEnemiesGroups()
        {
            return new List<string>
            {
                "Single soldiers",
                "Core Units",
                "Special Units",
                "Rare Units",
                "Monsters",
                "Lords and heroes",
            };
        }

        public static List<Enemy> GetEnemiesByGroup(string groupName)
        {
            if (groupName == "Single soldiers")
                return new List<Enemy>(EnemiesSoldiers);
            else if (groupName == "Core Units")
                return new List<Enemy>(EnemiesCoreUnits);
            else if (groupName == "Special Units")
                return new List<Enemy>(EnemiesSpecialUnits);
            else if (groupName == "Rare Units")
                return new List<Enemy>(EnemiesRareUnits);
            else if (groupName == "Monsters")
                return new List<Enemy>(EnemiesMonsters);
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
                TestListName = "Empire soldier (Empire)",
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

            new Enemy
            {
                Name = "Saurus warrior",
                TestListName = "Saurus warrior (Lizardmen)",
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

            new Enemy
            {
                Name = "Chosen Knights of Chaos",
                TestListName = "Chosen Knights (Chaos)",
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
        };

        private static List<Enemy> EnemiesMonsters = new List<Enemy>
        {
            new Enemy
            {
                Name = "Troll",
                TestListName = "Troll (Orcs&Goblin)",
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
                Stupidity = true,
                UnitStrength = 3,
            },

            new Enemy
            {
                Name = "Star Dragon",
                TestListName = "Star Dragon (High Elves)",
                Size = 1,
                Movement = 6,
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
                UnitStrength = 7,
            },
        };

        private static List<Enemy> EnemiesCoreUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "Clanrat Slaves",
                TestListName = "20 Clanrat Slaves (Skaven)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 5,
                WeaponSkill = 2,
                BallisticSkill = 2,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 4,
                Attacks = 1,
                Leadership = 2,
            },

            new Enemy
            {
                Name = "Empire swordmens",
                TestListName = "20 Empire swordmens (Empire)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 7,
                Armour = 6,
            },

            new Enemy
            {
                Name = "Bloodletters",
                TestListName = "20 Bloodletters (Chaos)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 4,
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
                Name = "Longbeards",
                TestListName = "20 Longbeards (Dwarfs)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 3,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 2,
                Attacks = 1,
                Leadership = 9,
                Armour = 4,
                ImmuneToPsychology = true
            },
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "Grave Guard",
                TestListName = "16 Grave Guard (Vampire Counts)",
                Type = UnitType.Special,
                Size = 16,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 3,
                Attacks = 2,
                Leadership = 8,
                Armour = 4,
                KillingBlow = true
            },

            new Enemy
            {
                Name = "Sword Masters of Hoeth",
                TestListName = "16 Sword Master (High Elves)",
                Type = UnitType.Special,
                Size = 16,
                Movement = 5,
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

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "White Lions",
                TestListName = "16 White Lions (High Elves)",
                Type = UnitType.Rare,
                Size = 16,
                Movement = 5,
                WeaponSkill = 5,
                BallisticSkill = 4,
                Strength = 6,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 1,
                Leadership = 8,
                Armour = 6,
                HitFirst = true
            },

            new Enemy
            {
                Name = "Black Guard",
                TestListName = "16 Black Guard (Dark Elves)",
                Type = UnitType.Rare,
                Size = 16,
                Movement = 5,
                WeaponSkill = 5,
                BallisticSkill = 4,
                Strength = 4,
                Toughness = 3,
                Wounds = 1,
                Initiative = 6,
                Attacks = 1,
                Leadership = 9,
                Armour = 5,
                Hate = true,
                Stubborn = true,
            },
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            new Enemy
            {
                Name = "Tretch Craventail",
                TestListName = "Tretch Craventail (Skaven)",
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
            },

            new Enemy
            {
                Name = "Zacharias The Everliving",
                TestListName = "Zacharias The Everliving (Vampire Counts)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 6,
                BallisticSkill = 6,
                Strength = 5,
                Toughness = 5,
                Wounds = 4,
                Initiative = 8,
                Attacks = 5,
                Leadership = 10,
                Ward = 4,

                EnemyMount = new Enemy
                {
                    Name = "Zombie Dragon",
                    Size = 1,
                    Movement = 6,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 6,
                    Toughness = 6,
                    Wounds = 6,
                    Initiative = 1,
                    Attacks = 4,
                    Leadership = 4,
                    Armour = 5,
                    Terror = true,
                }
            },

            new Enemy
            {
                Name = "Bloodthister",
                TestListName = "Greater Daemon Bloodthister (Chaos)",
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
                KillingBlow = true,
                UnitStrength = 7,
            },
        };
    }
}
