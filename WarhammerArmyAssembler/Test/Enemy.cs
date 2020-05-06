using System;
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

        private static Dictionary<string, List<Enemy>> GetEnemiesDictionary()
        {
            return  new Dictionary<string, List<Enemy>>
            {
                ["Lords"] = new List<Enemy>(EnemiesLords),
                ["Heroes"] = new List<Enemy>(EnemiesHeroes),
                ["Core Units"] = new List<Enemy>(EnemiesCoreUnits),
                ["Special Units"] = new List<Enemy>(EnemiesSpecialUnits),
                ["Rare Units"] = new List<Enemy>(EnemiesRareUnits),
                ["Monsters"] = new List<Enemy>(EnemiesMonsters),
            };
        }

        public static Enemy GetByName(string enemyName)
        {
            foreach (List<Enemy> enemyList in new List<List<Enemy>>
            {
                EnemiesMonsters, EnemiesHeroes, EnemiesLords,
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
            return GetEnemiesDictionary().Keys.ToList<string>();
        }

        public static List<Enemy> GetEnemiesByGroup(string groupName)
        {
            return GetEnemiesDictionary()[groupName];
        }

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
                Name = "Ancient Kroxigor",
                TestListName = "Ancient Kroxigor (Lizardmen)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 3,
                BallisticSkill = 1,
                Strength = 7,
                Toughness = 4,
                Wounds = 3,
                Initiative = 1,
                Attacks = 4,
                Leadership = 7,
                Fear = true,
                ColdBlooded = true,
                UnitStrength = 3,
            },

            new Enemy
            {
                Name = "Tomb Scorpion",
                TestListName = "Tomb Scorpion (Tomb Kings)",
                Size = 1,
                Movement = 7,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 5,
                Wounds = 3,
                Initiative = 3,
                Attacks = 4,
                Leadership = 8,
                Undead = true,
                KillingBlow = true,
                PoisonAttack = true,
            },

            new Enemy
            {
                Name = "Hippogryph",
                TestListName = "Hippogryph (Brettonia)",
                Size = 1,
                Movement = 8,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 5,
                Wounds = 4,
                Initiative = 4,
                Attacks = 4,
                Leadership = 8,
                Terror = true,
            },

            new Enemy
            {
                Name = "War Hydra",
                TestListName = "War Hydra (Dark Elves)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 5,
                Wounds = 6,
                Initiative = 2,
                Attacks = 5,
                Leadership = 6,
                Armour = 4,
                Terror = true,

                EnemyMount = new Enemy
                {
                    Name = "Apparentice",
                    Size = 2,
                    Movement = 5,
                    WeaponSkill = 4,
                    BallisticSkill = 4,
                    Strength = 3,
                    Toughness = 3,
                    Wounds = 2,
                    Initiative = 3,
                    Attacks = 2,
                    Leadership = 8,
                }
            },

            new Enemy
            {
                Name = "Dragon Ogre Shaggoth",
                TestListName = "Dragon Ogre Shaggoth (Beasts of Chaos)",
                Size = 1,
                Movement = 7,
                WeaponSkill = 6,
                BallisticSkill = 3,
                Strength = 5,
                Toughness = 5,
                Wounds = 6,
                Initiative = 4,
                Attacks = 5,
                Leadership = 9,
                Armour = 4,
                Terror = true,
                ImmuneToPsychology = true,
                UnitStrength = 6,
            },

            new Enemy
            {
                Name = "Stegadon",
                TestListName = "Stegadon (Lizardmen)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 3,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 6,
                Wounds = 5,
                Initiative = 2,
                Attacks = 4,
                Leadership = 5,
                Armour = 4,
                UnitStrength = 8,
                ColdBlooded = true,
                Terror = true,
                Stubborn = true,
                ImmuneToPsychology = true,

                EnemyMount = new Enemy
                {
                    Name = "Skink Crew",
                    Size = 5,
                    Movement = 6,
                    WeaponSkill = 2,
                    BallisticSkill = 3,
                    Strength = 3,
                    Toughness = 2,
                    Wounds = 4,
                    Initiative = 4,
                    Attacks = 4,
                    Leadership = 5,
                    Armour = 4,
                    ColdBlooded = true,
                    PoisonAttack = true,
                }
            },

            new Enemy
            {
                Name = "Treeman",
                TestListName = "Treeman (Wood Elves)",
                Size = 1,
                Movement = 5,
                WeaponSkill = 5,
                BallisticSkill = 0,
                Strength = 6,
                Toughness = 6,
                Wounds = 6,
                Initiative = 2,
                Attacks = 5,
                Leadership = 8,
                Armour = 3,
                Terror = true,
                UnitStrength = 6,
            },

            new Enemy
            {
                Name = "Necrosphinx",
                TestListName = "Necrosphinx (Tomb Kings)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 8,
                Wounds = 5,
                Initiative = 1,
                Attacks = 5,
                Leadership = 8,
                Armour = 3,
                Terror = true,
                UnitStrength = 5,
                HeroicKillingBlow = true,
                Undead = true,
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
                StrengthInNumbers = true,
            },

            new Enemy
            {
                Name = "Men-at-arms",
                TestListName = "20 Men-at-arms (Brettonia)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 4,
                WeaponSkill = 2,
                BallisticSkill = 2,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 5,
                Armour = 5
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
                Name = "Orc boys",
                TestListName = "20 Orc Boys (Orcs&Goblin)",
                Type = UnitType.Core,
                Size = 20,
                Movement = 4,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 4,
                Wounds = 1,
                Initiative = 2,
                Attacks = 1,
                Leadership = 7,
                Armour = 5,
            },

            new Enemy
            {
                Name = "Skeleton Warriors",
                TestListName = "20 Skeleton Warriors (Tomb Kings)",
                Size = 20,
                Movement = 4,
                WeaponSkill = 2,
                BallisticSkill = 2,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 2,
                Attacks = 1,
                Leadership = 5,
                Armour = 5,
                Undead = true,
            },

            new Enemy
            {
                Name = "Dryads",
                TestListName = "20 Dryads (Wood Elves)",
                Size = 20,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 6,
                Attacks = 2,
                Leadership = 8,
                Fear = true,
            },

            new Enemy
            {
                Name = "Lothern Sea Guard",
                TestListName = "20 Lothern Sea Guard (High Elves)",
                Size = 20,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 4,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 1,
                Leadership = 8,
                Armour = 6,
                HitFirst = true,
            },

            new Enemy
            {
                Name = "Back Ark Corsairs",
                TestListName = "20 Back Ark Corsairs (Dark Elves)",
                Size = 20,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 4,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 5,
                Hate = true,
            },

            new Enemy
            {
                Name = "Dryads",
                TestListName = "20 Dryads (Wood Elves)",
                Size = 20,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 6,
                Attacks = 2,
                Leadership = 8,
                Fear = true,
            },

            new Enemy
            {
                Name = "Bestigor",
                TestListName = "20 Bestigor (Beasts of Chaos)",
                Size = 20,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 3,
                Strength = 6,
                Toughness = 4,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 7,
                Armour = 5,
                HitLast = true,
            },

            new Enemy
            {
                Name = "Knights of the Realms",
                TestListName = "8 Knights of the Realms (Brettonia)",
                Size = 8,
                Movement = 4,
                WeaponSkill = 4,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 8,
                Armour = 2,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Warhorse",
                    Size = 8,
                    Movement = 8,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 3,
                    Toughness = 3,
                    Wounds = 1,
                    Initiative = 3,
                    Attacks = 1,
                    Leadership = 5,
                }
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

            new Enemy
            {
                Name = "Temple Guard",
                TestListName = "20 Temple Guard (Lizardmen)",
                Size = 20,
                Movement = 4,
                WeaponSkill = 4,
                BallisticSkill = 0,
                Strength = 5,
                Toughness = 4,
                Wounds = 1,
                Initiative = 2,
                Attacks = 2,
                Leadership = 8,
                Armour = 4,
                ColdBlooded = true
            },

            new Enemy
            {
                Name = "Chosen Knights of Chaos",
                TestListName = "8 Chosen Knights (Chaos)",
                Size = 8,
                Movement = 4,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 5,
                Toughness = 4,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 1,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Chaos Steed",
                    Size = 8,
                    Movement = 8,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 4,
                    Toughness = 3,
                    Wounds = 1,
                    Initiative = 3,
                    Attacks = 1,
                    Leadership = 5,
                }
            },
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "Tree Kin",
                TestListName = "5 Tree Kin (Wood Elves)",
                Type = UnitType.Special,
                Size = 5,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 4,
                Strength = 4,
                Toughness = 5,
                Wounds = 3,
                Initiative = 3,
                Attacks = 3,
                Leadership = 8,
                Armour = 4,
                Fear = true,
            },

            new Enemy
            {
                Name = "Chaos Ogres",
                TestListName = "5 Chaos Ogres (Beasts of Chaos)",
                Size = 5,
                Movement = 6,
                WeaponSkill = 3,
                BallisticSkill = 2,
                Strength = 4,
                Toughness = 4,
                Wounds = 3,
                Initiative = 2,
                Attacks = 3,
                Leadership = 7,
                Armour = 6,
                Fear = true,
            },

             new Enemy
            {
                Name = "Plague Monks",
                TestListName = "20 Plague Monks (Skaven)",
                Type = UnitType.Special,
                Size = 20,
                Movement = 5,
                WeaponSkill = 3,
                BallisticSkill = 3,
                Strength = 3,
                Toughness = 4,
                Wounds = 1,
                Initiative = 3,
                Attacks = 3,
                Leadership = 5,
                Frenzy = true,
                StrengthInNumbers = true,
            },

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
                KillingBlow = true,
                Undead = true,
            },

            new Enemy
            {
                Name = "Greatswords",
                TestListName = "20 Greatswords (Empire)",
                Type = UnitType.Special,
                Size = 20,
                Movement = 4,
                WeaponSkill = 4,
                BallisticSkill = 3,
                Strength = 5,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 2,
                Leadership = 8,
                Armour = 5,
                HitLast = true,
                Stubborn = true
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
                Attacks = 2,
                Leadership = 8,
                Armour = 6,
                Frenzy = true
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

            new Enemy
            {
                Name = "Hammerers",
                TestListName = "16 Hammerers (Dwarfs)",
                Type = UnitType.Special,
                Size = 16,
                Movement = 3,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 6,
                Toughness = 4,
                Wounds = 1,
                Initiative = 2,
                Attacks = 1,
                Leadership = 9,
                Armour = 5,
                Stubborn = true,
            },
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            new Enemy
            {
                Name = "Flagellant Warband",
                TestListName = "24 Flagellant Warband (The Empire)",
                Type = UnitType.Rare,
                Size = 24,
                Movement = 4,
                WeaponSkill = 2,
                BallisticSkill = 2,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 3,
                Attacks = 1,
                Leadership = 10,
                Unbreakable = true,
                Frenzy = true,
            },

            new Enemy
            {
                Name = "Waywathers",
                TestListName = "16 Waywathers (Wood Elves)",
                Type = UnitType.Rare,
                Size = 16,
                Movement = 5,
                WeaponSkill = 4,
                BallisticSkill = 5,
                Strength = 3,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                HitFirst = true
            },

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

            new Enemy
            {
                Name = "Troll Slayers",
                TestListName = "16 Troll Slayers (Dwarfs)",
                Type = UnitType.Special,
                Size = 16,
                Movement = 3,
                WeaponSkill = 4,
                BallisticSkill = 3,
                Strength = 5,
                Toughness = 4,
                Wounds = 1,
                Initiative = 2,
                Attacks = 1,
                Leadership = 10,
                Unbreakable = true,
            },

            new Enemy
            {
                Name = "Grail Knights",
                TestListName = "12 Grail Knights (Brettonia)",
                Size = 12,
                Movement = 4,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 3,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 2,
                Ward = 5,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Warhorse",
                    Size = 12,
                    Movement = 8,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 3,
                    Toughness = 3,
                    Wounds = 1,
                    Initiative = 3,
                    Attacks = 1,
                    Leadership = 5,
                }
            },

            new Enemy
            {
                Name = "Blood Knights",
                TestListName = "6 Blood Knights (Vampire Counts)",
                Size = 6,
                Movement = 4,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 5,
                Toughness = 4,
                Wounds = 1,
                Initiative = 4,
                Attacks = 2,
                Leadership = 7,
                Armour = 2,
                Ward = 5,
                Frenzy = true,
                Undead = true,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Nightmare",
                    Size = 6,
                    Movement = 8,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 4,
                    Toughness = 4,
                    Wounds = 1,
                    Initiative = 2,
                    Attacks = 1,
                    Leadership = 3,
                }
            },

            new Enemy
            {
                Name = "Skullcrushers of Khorne",
                TestListName = "6 Skullcrushers of Khorne (Chaos)",
                Size = 6,
                Movement = 4,
                WeaponSkill = 5,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 4,
                Wounds = 1,
                Initiative = 5,
                Attacks = 2,
                Leadership = 8,
                Armour = 1,
                Fear = true,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Juggernaut",
                    Size = 6,
                    Movement = 7,
                    WeaponSkill = 5,
                    BallisticSkill = 0,
                    Strength = 5,
                    Toughness = 4,
                    Wounds = 3,
                    Initiative = 2,
                    Attacks = 3,
                    Leadership = 7,
                }
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
                Name = "Josef Bugman",
                TestListName = "Josef Bugman Master Brewer (Dwarfs)",
                Size = 1,
                Movement = 3,
                WeaponSkill = 6,
                BallisticSkill = 5,
                Strength = 5,
                Toughness = 5,
                Wounds = 2,
                Initiative = 4,
                Attacks = 4,
                Leadership = 10,
                Armour = 3,
                Ward = 4,
                ImmuneToPsychology = true,
            },

            new Enemy
            {
                Name = "Drycha",
                TestListName = "Drycha (Wood Elves)",
                Size = 1,
                Movement = 5,
                WeaponSkill = 7,
                BallisticSkill = 4,
                Strength = 5,
                Toughness = 4,
                Wounds = 3,
                Initiative = 8,
                Attacks = 5,
                Leadership = 8,
                Terror = true,
                Reroll = "ToHit",
            },

            new Enemy
            {
                Name = "Caradryan",
                TestListName = "Caradryan, Capitain ot The Phoenix Guard (High Elves)",
                Size = 1,
                Movement = 5,
                WeaponSkill = 6,
                BallisticSkill = 6,
                Strength = 4,
                Toughness = 3,
                Wounds = 2,
                Initiative = 7,
                Attacks = 3,
                Leadership = 9,
                Armour = 5,
                Ward = 4,
                Fear = true,
                HitFirst = true,
                MultiWounds = "D3",
            },

            new Enemy
            {
                Name = "Konrad",
                TestListName = "Konrad Von Carstein (Vampire Counts)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 7,
                BallisticSkill = 4,
                Strength = 5,
                Toughness = 4,
                Wounds = 2,
                Initiative = 6,
                Attacks = 4,
                Leadership = 6,
                Armour = 5,
                Fear = true,
                HitFirst = true,
                Reroll = "ToHit",
                MultiWounds = "2",
                Undead = true,
            },

            new Enemy
            {
                Name = "Malus Darkblade (Tz'arkan)",
                TestListName = "Malus Darkblade in Tz'arkan state (Dark Elves)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 7,
                BallisticSkill = 5,
                Strength = 5,
                Toughness = 5,
                Wounds = 2,
                Initiative = 9,
                Attacks = 3,
                Leadership = 10,
                Armour = 3,
                Reroll = "ToWound",
                NoArmour = true,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Spite",
                    Size = 1,
                    Movement = 7,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 4,
                    Toughness = 4,
                    Wounds = 1,
                    Initiative = 2,
                    Attacks = 2,
                    Leadership = 4,
                    Armour = 5,
                    Fear = true,
                }
            },

            new Enemy
            {
                Name = "Deathmaster Snikch",
                TestListName = "Deathmaster Snikch (Skaven)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 8,
                BallisticSkill = 6,
                Strength = 4,
                Toughness = 4,
                Wounds = 2,
                Initiative = 10,
                Attacks = 6,
                Leadership = 8,
                Ward = 4,
                HitFirst = true,
                ArmourPiercing = 2,
                MultiWounds = "D3",
            },
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            new Enemy
            {
                Name = "Kurt Helborg",
                TestListName = "Kurt Helborg (The Empire)",
                Size = 1,
                Movement = 6,
                WeaponSkill = 7,
                BallisticSkill = 3,
                Strength = 4,
                Toughness = 4,
                Wounds = 3,
                Initiative = 6,
                Attacks = 4,
                Leadership = 9,
                Armour = 2,
                Stubborn = true,
                ImmuneToPsychology = true,
                AutoWound = true,
                NoArmour = true,

                EnemyMount = new Enemy
                {
                    Type = UnitType.Mount,
                    Name = "Warhorse",
                    Size = 8,
                    Movement = 8,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 3,
                    Toughness = 3,
                    Wounds = 1,
                    Initiative = 3,
                    Attacks = 1,
                    Leadership = 5,
                }
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
                Undead = true,

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
                    Undead = true,
                }
            },

            new Enemy
            {
                Name = "Malekith, Witch King of Naggaroth",
                TestListName = "Malekith, Witch King (Dark Elves)",
                Size = 1,
                Movement = 8,
                WeaponSkill = 5,
                BallisticSkill = 4,
                Strength = 6,
                Toughness = 3,
                Wounds = 3,
                Initiative = 8,
                Attacks = 4,
                Leadership = 10,
                NoArmour = true,
                Armour = 4,
                Ward = 2,

                EnemyMount = new Enemy
                {
                    Name = "Seraphon",
                    Size = 1,
                    Movement = 6,
                    WeaponSkill = 6,
                    BallisticSkill = 0,
                    Strength = 6,
                    Toughness = 6,
                    Wounds = 6,
                    Initiative = 4,
                    Attacks = 5,
                    Leadership = 8,
                    Armour = 3,
                    Terror = true,
                }
            },

            new Enemy
            {
                Name = "Kroq-Gar Ancient",
                TestListName = "Kroq-Gar Ancient (Lizardmen)",
                Size = 1,
                Movement = 4,
                WeaponSkill = 6,
                BallisticSkill = 3,
                Strength = 6,
                Toughness = 5,
                Wounds = 3,
                Initiative = 4,
                Attacks = 5,
                Leadership = 8,
                ColdBlooded = true,
                Armour = 3,
                Ward = 5,
                MultiWounds = "2",

                EnemyMount = new Enemy
                {
                    Name = "Grymloq",
                    Size = 1,
                    Movement = 7,
                    WeaponSkill = 3,
                    BallisticSkill = 0,
                    Strength = 7,
                    Toughness = 5,
                    Wounds = 5,
                    Initiative = 2,
                    Attacks = 5,
                    Leadership = 5,
                    Armour = 4,
                    Terror = true,
                    ColdBlooded = true,
                    MultiWounds = "D3",
                    UnitStrength = 5,
                }
            },

            new Enemy
            {
                Name = "Grimgor Ironhide",
                TestListName = "Grimgor Ironhide (Orcs&Goblin)",
                Size = 1,
                Movement = 4,
                WeaponSkill = 8,
                BallisticSkill = 1,
                Strength = 7,
                Toughness = 5,
                Wounds = 3,
                Initiative = 5,
                Attacks = 7,
                Leadership = 9,
                Armour = 1,
                Ward = 5,
                Hate = true,
                HitFirst = true,
                ImmuneToPsychology = true,
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
