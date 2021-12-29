using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public static int MaxIDindex = -10;

        public Enemy(string enemyName)
        {
            string[] profile = enemyName.Split('/');

            this.Name = profile[0];

            this.Movement = new Profile { Value = int.Parse(profile[1]) };
            this.WeaponSkill = new Profile { Value = int.Parse(profile[2]) };
            this.BallisticSkill = new Profile { Value = int.Parse(profile[3]) };
            this.Strength = new Profile { Value = int.Parse(profile[4]) };
            this.Toughness = new Profile { Value = int.Parse(profile[5]) };
            this.Wounds = new Profile { Value = int.Parse(profile[6]) };
            this.Initiative = new Profile { Value = int.Parse(profile[7]) };
            this.Attacks = new Profile { Value = int.Parse(profile[8]) };
            this.Leadership = new Profile { Value = int.Parse(profile[9]) };

            bool isArmour = int.TryParse(profile[10], out int armour);

            if (isArmour)
                this.Armour = new Profile { Value = armour };

            bool isWard = int.TryParse(profile[11], out int ward);

            if (isWard)
                this.Ward = new Profile { Value = ward };
        }

        public string Fullname() => String.Format("{0} ({1})", this.Name, this.Armybook);

        private static Dictionary<string, List<Enemy>> GetEnemiesDictionary() => new Dictionary<string, List<Enemy>>
        {
            ["Lords"] = new List<Enemy>(EnemiesLords),
            ["Heroes"] = new List<Enemy>(EnemiesHeroes),
            ["Core Units"] = new List<Enemy>(EnemiesCoreUnits),
            ["Special Units"] = new List<Enemy>(EnemiesSpecialUnits),
            ["Rare Units"] = new List<Enemy>(EnemiesRareUnits),
            ["Monsters"] = new List<Enemy>(EnemiesMonsters),
        };

        public static Enemy GetByName(string enemyName)
        {
            List<List<Enemy>> enemies = new List<List<Enemy>> {
                EnemiesMonsters,
                EnemiesHeroes,
                EnemiesLords,
                EnemiesCoreUnits,
                EnemiesSpecialUnits,
                EnemiesRareUnits
            };

            foreach (List<Enemy> enemyList in enemies)
                foreach (Enemy enemy in enemyList.Where(x => x.Fullname() == enemyName))
                    return enemy.SetID();

            return null;
        }

        private Enemy SetID()
        {
            MaxIDindex -= 1;

            this.ID = MaxIDindex;

            return this;
        }

        public static List<string> GetEnemiesGroups() => GetEnemiesDictionary().Keys.ToList<string>();

        public static List<Enemy> GetEnemiesByGroup(string groupName) => GetEnemiesDictionary()[groupName];

        public static int GetEnemiesCount()
        {
            int count = 0;

            foreach (string enemyGroupName in Enemy.GetEnemiesGroups())
                count += Enemy.GetEnemiesByGroup(enemyGroupName).Count();

            return count;
        }

        private static List<Enemy> EnemiesMonsters = new List<Enemy>
        {
            new Enemy("Troll/6/3/1/5/4/3/1/3/4//")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Rare,
                Size = 1,
                Fear = true,
                Regeneration = true,
                Stupidity = true,
                LargeBase = true,
            },

            new Enemy("Gyrobomber/1/4/3/4/5/3/2/2/9/4/")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Rare,
                Size = 1,
                LargeBase = true,
            },

            new Enemy("Ancient Kroxigor/6/3/1/7/4/3/1/4/7//")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Rare,
                Size = 1,
                Fear = true,
                ColdBlooded = true,
                LargeBase = true,
            },

            new Enemy("Tomb Scorpion/7/4/0/5/5/3/3/4/8//")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Rare,
                Size = 1,
                Undead = true,
                KillingBlow = true,
                PoisonAttack = true,
                LargeBase = true,
            },

            new Enemy("Hippogryph/8/4/0/5/5/4/4/4/8//")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,
            },

            new Enemy("Griffon/6/5/0/5/5/4/5/4/7//")
            {
                Armybook = "The Empire",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,
            },

            new Enemy("Manticore/6/5/0/5/5/4/5/4/5//")
            {
                Armybook = "Chaos",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                KillingBlow = true,
                LargeBase = true,
            },

            new Enemy("Varghulf/8/5/0/5/5/4/2/5/4//")
            {
                Armybook = "Vampire",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                Undead = true,
                Regeneration = true,
                Hate = true,
                LargeBase = true,
            },

            new Enemy("War Hydra/6/4/0/5/5/6/2/5/6/4/")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,

                Mount = new Enemy("Apparentice/5/4/4/3/3/2/3/2/8//")
                {
                    Type = UnitType.Mount,
                    Size = 2,
                    NoKillingBlow = true,
                }
            },

            new Enemy("Dragon Ogre Shaggoth/7/6/3/5/5/6/4/5/9/4/")
            {
                Armybook = "Beastmen",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                ImmuneToPsychology = true,
                LargeBase = true,
            },

            new Enemy("Stegadon/6/3/0/5/6/5/2/4/5/4/")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Rare,
                Size = 1,
                LargeBase = true,
                ColdBlooded = true,
                Terror = true,
                Stubborn = true,
                ImmuneToPsychology = true,

                Mount = new Enemy("Skink Crew/6/2/3/3/2/4/4/4/5/4/")
                {
                    Type = UnitType.Mount,
                    Size = 5,
                    ColdBlooded = true,
                    PoisonAttack = true,
                    NoKillingBlow = true,
                }
            },

            new Enemy("Treeman/5/5/0/6/6/6/2/5/8/3/")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,
            },

            new Enemy("Giant/6/3/3/6/5/6/3/0/10//")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                Stubborn = true,
                LargeBase = true,
                Giant = true,
            },

            new Enemy("Hell Pit Abomination/6/3/3/6/5/6/3/0/10//")
            {
                Armybook = "Skaven",
                Type = UnitType.Rare,
                Size = 1,
                Regeneration = true,
                Terror = true,
                Stubborn = true,
                LargeBase = true,
                HellPitAbomination = true,
            },

            new Enemy("Necrosphinx/6/4/0/5/8/5/1/5/8/3/")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,
                HeroicKillingBlow = true,
                Undead = true,
            },

            new Enemy("Star Dragon/6/7/0/7/6/7/2/6/9/3/")
            {
                Armybook = "High Elves",
                Type = UnitType.Rare,
                Size = 1,
                Terror = true,
                LargeBase = true,
            },

            new Enemy("Steam Tank/0/0/4/6/6/10/0/0/10/1/")
            {
                Armybook = "The Empire",
                Type = UnitType.Rare,
                Size = 1,
                Unbreakable = true,
                Terror = true,
                LargeBase = true,
                SteamTank = true,
            },
        };

        private static List<Enemy> EnemiesCoreUnits = new List<Enemy>
        {
            new Enemy("Clanrat Slaves/5/2/2/3/3/1/4/1/2//")
            {
                Armybook = "Skaven",
                Type = UnitType.Core,
                Size = 20,
                StrengthInNumbers = true,
            },

            //new Enemy
            //{
            //    Name = "Men-at-arms",
            //    TestListName = "20 Men-at-arms (Bretonnia)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 2,
            //    BallisticSkill = 2,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 5,
            //    Armour = 5
            //},

            //new Enemy
            //{
            //    Name = "Empire swordmens",
            //    TestListName = "20 Empire swordmens (Empire)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 7,
            //    Armour = 6,
            //},

            //new Enemy
            //{
            //    Name = "Orc boys",
            //    TestListName = "20 Orc Boys (Orcs&Goblins)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 7,
            //    Armour = 5,
            //},

            //new Enemy
            //{
            //    Name = "Skeleton Warriors",
            //    TestListName = "20 Skeleton Warriors (Tomb Kings)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 2,
            //    BallisticSkill = 2,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 5,
            //    Armour = 5,
            //    Undead = true,
            //},

            //new Enemy
            //{
            //    Name = "Lothern Sea Guard",
            //    TestListName = "20 Lothern Sea Guard (High Elves)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 4,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 6,
            //    HitFirst = true,
            //},

            //new Enemy
            //{
            //    Name = "Crypt Ghouls",
            //    TestListName = "20 Crypt Ghouls (Vampire Counts)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 0,
            //    Strength = 3,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 2,
            //    Leadership = 5,
            //    Undead = true,
            //    PoisonAttack = true,
            //},

            //new Enemy
            //{
            //    Name = "Back Ark Corsairs",
            //    TestListName = "20 Back Ark Corsairs (Dark Elves)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 4,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 5,
            //    Hate = true,
            //},

            //new Enemy
            //{
            //    Name = "Dryads",
            //    TestListName = "20 Dryads (Wood Elves)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 0,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 6,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Fear = true,
            //},

            //new Enemy
            //{
            //    Name = "Bestigor",
            //    TestListName = "20 Bestigor (Beastmen)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 6,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 7,
            //    Armour = 5,
            //    HitLast = true,
            //},

            //new Enemy
            //{
            //    Name = "Knights of the Realms",
            //    TestListName = "8 Knights of the Realms (Bretonnia)",
            //    Type = UnitType.Core,
            //    Size = 8,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 2,
            //    Lance = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Warhorse",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 5,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Longbeards",
            //    TestListName = "20 Longbeards (Dwarfs)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 3,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 9,
            //    Armour = 4,
            //    ImmuneToPsychology = true
            //},

            //new Enemy
            //{
            //    Name = "Temple Guard",
            //    TestListName = "20 Temple Guard (Lizardmen)",
            //    Type = UnitType.Core,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 0,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 4,
            //    ColdBlooded = true
            //},

            //new Enemy
            //{
            //    Name = "Chosen Knights of Chaos",
            //    TestListName = "8 Chosen Knights (Chaos)",
            //    Type = UnitType.Core,
            //    Size = 8,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 1,

            //    Mount = new Enemy
            //    {
            //        Name = "Chaos Steed",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 5,
            //    }
            //},
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            //new Enemy
            //{
            //    Name = "Tree Kin",
            //    TestListName = "5 Tree Kin (Wood Elves)",
            //    Type = UnitType.Special,
            //    Size = 5,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 4,
            //    Strength = 4,
            //    Toughness = 5,
            //    Wounds = 3,
            //    Initiative = 3,
            //    Attacks = 3,
            //    Leadership = 8,
            //    Armour = 4,
            //    Fear = true,
            //},

            //new Enemy
            //{
            //    Name = "Pegasus Knights",
            //    TestListName = "8 Pegasus Knights (Bretonnia)",
            //    Type = UnitType.Special,
            //    Size = 8,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 4,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 3,
            //    Lance = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Boar",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 4,
            //        Attacks = 2,
            //        Leadership = 7,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Chaos Ogres",
            //    TestListName = "5 Chaos Ogres (Beastmen)",
            //    Type = UnitType.Special,
            //    Size = 5,
            //    Movement = 6,
            //    WeaponSkill = 3,
            //    BallisticSkill = 2,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 2,
            //    Attacks = 3,
            //    Leadership = 7,
            //    Armour = 6,
            //    Fear = true,
            //},

            // new Enemy
            //{
            //    Name = "Plague Monks",
            //    TestListName = "20 Plague Monks (Skaven)",
            //    Type = UnitType.Special,
            //    Size = 20,
            //    Movement = 5,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 3,
            //    Leadership = 5,
            //    Frenzy = true,
            //    StrengthInNumbers = true,
            //},

            //new Enemy
            //{
            //    Name = "Tomb Guard",
            //    TestListName = "16 Tomb Guard (Tomb Kings)",
            //    Type = UnitType.Special,
            //    Size = 16,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 5,
            //    KillingBlow = true,
            //    Undead = true,
            //},

            //new Enemy
            //{
            //    Name = "Grave Guard",
            //    TestListName = "16 Grave Guard (Vampire Counts)",
            //    Type = UnitType.Special,
            //    Size = 16,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 4,
            //    KillingBlow = true,
            //    Undead = true,
            //},

            //new Enemy
            //{
            //    Name = "Greatswords",
            //    TestListName = "20 Greatswords (Empire)",
            //    Type = UnitType.Special,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 5,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 5,
            //    HitLast = true,
            //    Stubborn = true
            //},

            //new Enemy
            //{
            //    Name = "Black Orcs",
            //    TestListName = "16 Black Orcs (Orcs&Goblins)",
            //    Type = UnitType.Special,
            //    Size = 16,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 5,
            //    HitLast = true
            //},

            //new Enemy
            //{
            //    Name = "Orc Boar Boys",
            //    TestListName = "8 Orc Boar Boys (Orcs&Goblins)",
            //    Type = UnitType.Special,
            //    Size = 8,
            //    Movement = 4,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 3,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 7,
            //    Armour = 3,

            //    Mount = new Enemy
            //    {
            //        Name = "Boar",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 3,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Cold One Knights",
            //    TestListName = "8 Cold One Knights (Dark Elves)",
            //    Type = UnitType.Special,
            //    Size = 8,
            //    Movement = 5,
            //    WeaponSkill = 5,
            //    BallisticSkill = 4,
            //    Strength = 4,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 6,
            //    Attacks = 1,
            //    Leadership = 9,
            //    Armour = 2,
            //    Hate = true,
            //    Fear = true,
            //    Lance = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Cold One",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 2,
            //        Attacks = 1,
            //        Leadership = 3,
            //    }
            //},


            //new Enemy
            //{
            //    Name = "Cold One Cavalry",
            //    TestListName = "8 Cold One Cavalry (Lizardmen)",
            //    Type = UnitType.Special,
            //    Size = 8,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 0,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 2,
            //    ColdBlooded = true,
            //    Fear = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Cold One",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 2,
            //        Attacks = 1,
            //        Leadership = 3,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Bloodletters",
            //    TestListName = "20 Bloodletters (Chaos)",
            //    Type = UnitType.Special,
            //    Size = 20,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 0,
            //    Strength = 5,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 4,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 6,
            //    Frenzy = true
            //},

            //new Enemy
            //{
            //    Name = "Sword Masters",
            //    TestListName = "16 Sword Masters of Hoeth (High Elves)",
            //    Type = UnitType.Special,
            //    Size = 16,
            //    Movement = 5,
            //    WeaponSkill = 6,
            //    BallisticSkill = 4,
            //    Strength = 5,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 5,
            //    HitFirst = true
            //},

            //new Enemy
            //{
            //    Name = "Hammerers",
            //    TestListName = "16 Hammerers (Dwarfs)",
            //    Type = UnitType.Special,
            //    Size = 16,
            //    Movement = 3,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 6,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 9,
            //    Armour = 5,
            //    Stubborn = true,
            //},
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            //new Enemy
            //{
            //    Name = "Snotling Pump Wagon",
            //    TestListName = "Snotling Pump Wagon (Orcs&Goblins)",
            //    Type = UnitType.Rare,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 2,
            //    BallisticSkill = 0,
            //    Strength = 2,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 3,
            //    Attacks = 5,
            //    Leadership = 4,
            //    Unbreakable = true,
            //    Armour = 6,
            //},

            //new Enemy
            //{
            //    Name = "Flagellants",
            //    TestListName = "24 Flagellant Warband (The Empire)",
            //    Type = UnitType.Rare,
            //    Size = 24,
            //    Movement = 4,
            //    WeaponSkill = 2,
            //    BallisticSkill = 2,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 3,
            //    Attacks = 1,
            //    Leadership = 10,
            //    Unbreakable = true,
            //    Frenzy = true,
            //    Flail = true,
            //},

            //new Enemy
            //{
            //    Name = "Waywathers",
            //    TestListName = "16 Waywathers (Wood Elves)",
            //    Type = UnitType.Rare,
            //    Size = 16,
            //    Movement = 5,
            //    WeaponSkill = 4,
            //    BallisticSkill = 5,
            //    Strength = 3,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    HitFirst = true
            //},

            //new Enemy
            //{
            //    Name = "White Lions",
            //    TestListName = "16 White Lions (High Elves)",
            //    Type = UnitType.Rare,
            //    Size = 16,
            //    Movement = 5,
            //    WeaponSkill = 5,
            //    BallisticSkill = 4,
            //    Strength = 6,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 1,
            //    Leadership = 8,
            //    Armour = 6,
            //    HitFirst = true
            //},

            //new Enemy
            //{
            //    Name = "Black Guard",
            //    TestListName = "16 Black Guard (Dark Elves)",
            //    Type = UnitType.Rare,
            //    Size = 16,
            //    Movement = 5,
            //    WeaponSkill = 5,
            //    BallisticSkill = 4,
            //    Strength = 4,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 6,
            //    Attacks = 1,
            //    Leadership = 9,
            //    Armour = 5,
            //    Hate = true,
            //    Stubborn = true,
            //},

            //new Enemy
            //{
            //    Name = "Troll Slayers",
            //    TestListName = "16 Troll Slayers (Dwarfs)",
            //    Type = UnitType.Rare,
            //    Size = 16,
            //    Movement = 3,
            //    WeaponSkill = 4,
            //    BallisticSkill = 3,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 2,
            //    Attacks = 1,
            //    Leadership = 10,
            //    Unbreakable = true,
            //},

            //new Enemy
            //{
            //    Name = "Grail Knights",
            //    TestListName = "12 Grail Knights (Bretonnia)",
            //    Type = UnitType.Rare,
            //    Size = 12,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 3,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 2,
            //    Ward = 5,
            //    Lance = true,

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Warhorse",
            //        Size = 12,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 5,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Blood Knights",
            //    TestListName = "6 Blood Knights (Vampire Counts)",
            //    Type = UnitType.Rare,
            //    Size = 6,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 4,
            //    Attacks = 2,
            //    Leadership = 7,
            //    Armour = 2,
            //    Ward = 5,
            //    Frenzy = true,
            //    Undead = true,
            //    Lance = true,

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Nightmare",
            //        Size = 6,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 2,
            //        Attacks = 1,
            //        Leadership = 3,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Skullcrushers",
            //    TestListName = "6 Skullcrushers of Khorne (Chaos)",
            //    Type = UnitType.Rare,
            //    Size = 6,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 1,
            //    Initiative = 5,
            //    Attacks = 2,
            //    Leadership = 8,
            //    Armour = 1,
            //    Fear = true,

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Juggernaut",
            //        Size = 6,
            //        Movement = 7,
            //        WeaponSkill = 5,
            //        BallisticSkill = 0,
            //        Strength = 5,
            //        Toughness = 4,
            //        Wounds = 3,
            //        Initiative = 2,
            //        Attacks = 3,
            //        Leadership = 7,
            //    }
            //},
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            //new Enemy
            //{
            //    Name = "Tretch Craventail",
            //    TestListName = "Tretch Craventail (Skaven)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 5,
            //    BallisticSkill = 4,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 6,
            //    Attacks = 4,
            //    Leadership = 6,
            //    Armour = 5,
            //    Ward = 4,
            //},

            //new Enemy
            //{
            //    Name = "The Herald Nekaph",
            //    TestListName = "The Herald Nekaph (Tomb Kings)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 3,
            //    Attacks = 3,
            //    Leadership = 8,
            //    Ward = 5,
            //    KillingBlow = true,
            //    Undead = true,
            //    Flail = true,
            //    MultiWounds = "2",
            //},

            //new Enemy
            //{
            //    Name = "Gitilla",
            //    TestListName = "Gitilla da Hunter (Orcs&Goblins)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 4,
            //    BallisticSkill = 4,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 4,
            //    Attacks = 3,
            //    Leadership = 7,
            //    Armour = 3,

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Ulda the Great Wolf",
            //        Size = 1,
            //        Movement = 9,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 2,
            //        Leadership = 3,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Moonclaw",
            //    TestListName = "Moonclaw, Son of Murrslieb (Beastmen)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 3,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 3,
            //    Attacks = 3,
            //    Leadership = 7,
            //    Ward = 5,

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Umbralok",
            //        Size = 1,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 2,
            //        Attacks = 3,
            //        Leadership = 6,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Ludwig Schwarzhelm",
            //    TestListName = "Ludwig Schwarzhelm (The Empire)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 6,
            //    BallisticSkill = 5,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 5,
            //    Attacks = 3,
            //    Leadership = 8,
            //    Armour = 2,
            //    KillingBlow = true,
            //    Reroll = "ToWound",

            //    Mount = new Enemy
            //    {
            //        Type = UnitType.Mount,
            //        Name = "Warhorse",
            //        Size = 1,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 5,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Gor-Rok",
            //    TestListName = "Gor-Rok, The Great White Lizard (Lizardmen)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 0,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 2,
            //    Initiative = 3,
            //    Attacks = 4,
            //    Leadership = 8,
            //    ColdBlooded = true,
            //    Armour = 3,
            //    Stubborn = true,
            //    NoKillingBlow = true,
            //    NoMultiWounds = true,
            //    Reroll = "ToHit;OpponentToWound",
            //},

            //new Enemy
            //{
            //    Name = "Josef Bugman",
            //    TestListName = "Josef Bugman Master Brewer (Dwarfs)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 3,
            //    WeaponSkill = 6,
            //    BallisticSkill = 5,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 2,
            //    Initiative = 4,
            //    Attacks = 4,
            //    Leadership = 10,
            //    Armour = 3,
            //    Ward = 4,
            //    ImmuneToPsychology = true,
            //},

            //new Enemy
            //{
            //    Name = "Drycha",
            //    TestListName = "Drycha (Wood Elves)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 7,
            //    BallisticSkill = 4,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 8,
            //    Attacks = 5,
            //    Leadership = 8,
            //    Terror = true,
            //    Reroll = "ToHit",
            //},

            //new Enemy
            //{
            //    Name = "Caradryan",
            //    TestListName = "Caradryan, Capitain ot The Phoenix Guard (High Elves)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 6,
            //    BallisticSkill = 6,
            //    Strength = 4,
            //    Toughness = 3,
            //    Wounds = 2,
            //    Initiative = 7,
            //    Attacks = 3,
            //    Leadership = 9,
            //    Armour = 5,
            //    Ward = 4,
            //    Fear = true,
            //    HitFirst = true,
            //    MultiWounds = "D3",
            //},

            //new Enemy
            //{
            //    Name = "Konrad",
            //    TestListName = "Konrad Von Carstein (Vampire Counts)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 7,
            //    BallisticSkill = 4,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 6,
            //    Attacks = 4,
            //    Leadership = 6,
            //    Armour = 5,
            //    Fear = true,
            //    HitFirst = true,
            //    Reroll = "ToHit",
            //    MultiWounds = "2",
            //    Undead = true,
            //},

            //new Enemy
            //{
            //    Name = "Throgg",
            //    TestListName = "Throgg, King of Trolls (Chaos)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 5,
            //    BallisticSkill = 2,
            //    Strength = 6,
            //    Toughness = 5,
            //    Wounds = 4,
            //    Initiative = 2,
            //    Attacks = 5,
            //    Leadership = 8,
            //    Fear = true,
            //    Regeneration = true,
            //    LargeBase = true,
            //},

            //new Enemy
            //{
            //    Name = "Malus Darkblade (Tz'arkan)",
            //    TestListName = "Malus Darkblade in Tz'arkan state (Dark Elves)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 7,
            //    BallisticSkill = 5,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 2,
            //    Initiative = 9,
            //    Attacks = 3,
            //    Leadership = 10,
            //    Armour = 3,
            //    Reroll = "ToWound",
            //    NoArmour = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Spite",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 2,
            //        Attacks = 2,
            //        Leadership = 4,
            //        Armour = 5,
            //        Fear = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Deathmaster Snikch",
            //    TestListName = "Deathmaster Snikch (Skaven)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 8,
            //    BallisticSkill = 6,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 2,
            //    Initiative = 10,
            //    Attacks = 6,
            //    Leadership = 8,
            //    Ward = 4,
            //    HitFirst = true,
            //    ArmourPiercing = 2,
            //    MultiWounds = "D3",
            //},

            //new Enemy
            //{
            //    Name = "Chakax",
            //    TestListName = "Chakax, The Eternity Warden (Lizardmen)",
            //    Type = UnitType.Hero,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 5,
            //    BallisticSkill = 0,
            //    Strength = 7,
            //    Toughness = 5,
            //    Wounds = 2,
            //    Initiative = 3,
            //    Attacks = 4,
            //    Leadership = 8,
            //    ColdBlooded = true,
            //    Armour = 4,
            //    Ward = 5,
            //    Unbreakable = true,
            //    HitFirst = true,
            //    Reroll = "ToHit",
            //},
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            //new Enemy
            //{
            //    Name = "Green Knight",
            //    TestListName = "Green Knight (Bretonnia)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 7,
            //    BallisticSkill = 3,
            //    Strength = 6,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 6,
            //    Attacks = 4,
            //    Leadership = 9,
            //    Ward = 5,
            //    ImmuneToPsychology = true,
            //    Terror = true,
            //    Undead = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Shadow Steed",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 8,
            //        WeaponSkill = 4,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 4,
            //        Attacks = 1,
            //        Leadership = 5,
            //        Armour = 5,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Khuzrak",
            //    TestListName = "Khuzrak, The One-eye (Beastmen)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 7,
            //    BallisticSkill = 1,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 3,
            //    Initiative = 5,
            //    Attacks = 4,
            //    Leadership = 9,
            //    Armour = 2,
            //},


            //new Enemy
            //{
            //    Name = "Khalida",
            //    TestListName = "High Queen Khalida, Beloved of Asaph (Tomb Kings)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 6,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 5,
            //    Wounds = 3,
            //    Initiative = 9,
            //    Attacks = 5,
            //    Leadership = 10,
            //    HitFirst = true,
            //    Undead = true,
            //    PoisonAttack = true,
            //},

            //new Enemy
            //{
            //    Name = "Louen Leoncoeur",
            //    TestListName = "Louen Leoncoeur, The Lionhearted (Bretonnia)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 7,
            //    BallisticSkill = 5,
            //    Strength = 5,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 7,
            //    Attacks = 5,
            //    Leadership = 9,
            //    Armour = 3,
            //    Ward = 5,
            //    Lance = true,
            //    ImmuneToPsychology = true,
            //    Regeneration = true,
            //    Reroll = "ToHit;ToWound",

            //    Mount = new Enemy
            //    {
            //        Name = "Beaquis",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 8,
            //        WeaponSkill = 5,
            //        BallisticSkill = 0,
            //        Strength = 5,
            //        Toughness = 5,
            //        Wounds = 4,
            //        Initiative = 6,
            //        Attacks = 4,
            //        Leadership = 9,
            //        Armour = 5,
            //        Terror = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Kurt Helborg",
            //    TestListName = "Kurt Helborg (The Empire)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 7,
            //    BallisticSkill = 3,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 6,
            //    Attacks = 4,
            //    Leadership = 9,
            //    Armour = 2,
            //    Stubborn = true,
            //    ImmuneToPsychology = true,
            //    AutoWound = true,
            //    NoArmour = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Warhorse",
            //        Type = UnitType.Mount,
            //        Size = 8,
            //        Movement = 8,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 3,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 1,
            //        Leadership = 5,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Zacharias",
            //    TestListName = "Zacharias The Everliving (Vampire Counts)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 6,
            //    BallisticSkill = 6,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 4,
            //    Initiative = 8,
            //    Attacks = 5,
            //    Leadership = 10,
            //    Ward = 4,
            //    Undead = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Zombie Dragon",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 6,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 6,
            //        Toughness = 6,
            //        Wounds = 6,
            //        Initiative = 1,
            //        Attacks = 4,
            //        Leadership = 4,
            //        Armour = 5,
            //        Terror = true,
            //        Undead = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Karl Franz",
            //    TestListName = "The Emperor Karl Franz (The Empire)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 6,
            //    BallisticSkill = 5,
            //    Strength = 4,
            //    Toughness = 4,
            //    Wounds = 3,
            //    Initiative = 6,
            //    Attacks = 4,
            //    Leadership = 10,
            //    Armour = 4,
            //    Ward = 4,
            //    AutoWound = true,
            //    NoArmour = true,
            //    MultiWounds = "D3",

            //    Mount = new Enemy
            //    {
            //        Name = "Deathclaw",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 6,
            //        WeaponSkill = 6,
            //        BallisticSkill = 0,
            //        Strength = 5,
            //        Toughness = 5,
            //        Wounds = 4,
            //        Initiative = 5,
            //        Attacks = 4,
            //        Leadership = 8,
            //        Terror = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Tyrion",
            //    TestListName = "Tyrion, The Defender of Ulthuan (High Elves)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 9,
            //    BallisticSkill = 7,
            //    Strength = 7,
            //    Toughness = 3,
            //    Wounds = 4,
            //    Initiative = 10,
            //    Attacks = 4,
            //    Leadership = 10,
            //    HitFirst = true,
            //    Armour = 1,
            //    Ward = 4,
            //    Regeneration = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Malhandir",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 10,
            //        WeaponSkill = 4,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 3,
            //        Wounds = 1,
            //        Initiative = 5,
            //        Attacks = 2,
            //        Leadership = 7,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Torgrim Grudgebearer",
            //    TestListName = "Torgrim Grudgebearer (Dwarfs)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 3,
            //    WeaponSkill = 7,
            //    BallisticSkill = 6,
            //    Strength = 4,
            //    Toughness = 5,
            //    Wounds = 7,
            //    Initiative = 4,
            //    Attacks = 4,
            //    Leadership = 10,
            //    Armour = 2,
            //    Ward = 4,
            //    HitFirst = true,
            //    ImmuneToPsychology = true,
            //    Stubborn = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Thronebearers",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 3,
            //        WeaponSkill = 5,
            //        BallisticSkill = 3,
            //        Strength = 4,
            //        Toughness = 0,
            //        Wounds = 1,
            //        Initiative = 3,
            //        Attacks = 4,
            //        Leadership = 0,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Orion",
            //    TestListName = "Orion, The King in the Wood (Wood Elves)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 9,
            //    WeaponSkill = 8,
            //    BallisticSkill = 8,
            //    Strength = 6,
            //    Toughness = 5,
            //    Wounds = 5,
            //    Initiative = 9,
            //    Attacks = 5,
            //    Leadership = 10,
            //    HitFirst = true,
            //    Frenzy = true,
            //    Terror = true,
            //    Unbreakable = true,
            //    Ward = 5,

            //    Mount = new Enemy
            //    {
            //        Name = "Hound of Orion",
            //        Type = UnitType.Hero,
            //        Size = 2,
            //        Movement = 9,
            //        WeaponSkill = 4,
            //        BallisticSkill = 0,
            //        Strength = 4,
            //        Toughness = 4,
            //        Wounds = 1,
            //        Initiative = 4,
            //        Attacks = 1,
            //        Leadership = 6,
            //        Frenzy = true,
            //        Unbreakable = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Durthu",
            //    TestListName = "Durthu, Eldest of Ancients (Wood Elves)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 5,
            //    WeaponSkill = 7,
            //    BallisticSkill = 7,
            //    Strength = 6,
            //    Toughness = 6,
            //    Wounds = 6,
            //    Initiative = 2,
            //    Attacks = 6,
            //    Leadership = 10,
            //    Armour = 3,
            //    Ward = 6,
            //    LargeBase = true,
            //    Frenzy = true,
            //    Terror = true,
            //    Hate = true,
            //    Stubborn = true,
            //},

            //new Enemy
            //{
            //    Name = "Vermin Lord",
            //    TestListName = "Vermin Lord (Skaven)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 8,
            //    WeaponSkill = 8,
            //    BallisticSkill = 4,
            //    Strength = 6,
            //    Toughness = 5,
            //    Wounds = 5,
            //    Initiative = 10,
            //    Attacks = 5,
            //    Leadership = 8,
            //    Ward = 5,
            //    ImmuneToPsychology = true,
            //    Terror = true,
            //    MultiWounds = "D3",
            //},


            //new Enemy
            //{
            //    Name = "Malekith",
            //    TestListName = "Malekith, Witch King of Naggaroth (Dark Elves)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 8,
            //    WeaponSkill = 5,
            //    BallisticSkill = 4,
            //    Strength = 6,
            //    Toughness = 3,
            //    Wounds = 3,
            //    Initiative = 8,
            //    Attacks = 4,
            //    Leadership = 10,
            //    NoArmour = true,
            //    Armour = 4,
            //    Ward = 2,

            //    Mount = new Enemy
            //    {
            //        Name = "Seraphon",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 6,
            //        WeaponSkill = 6,
            //        BallisticSkill = 0,
            //        Strength = 6,
            //        Toughness = 6,
            //        Wounds = 6,
            //        Initiative = 4,
            //        Attacks = 5,
            //        Leadership = 8,
            //        Armour = 3,
            //        Terror = true,
            //        LargeBase = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Kroq-Gar",
            //    TestListName = "Kroq-Gar Ancient (Lizardmen)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 6,
            //    BallisticSkill = 3,
            //    Strength = 6,
            //    Toughness = 5,
            //    Wounds = 3,
            //    Initiative = 4,
            //    Attacks = 5,
            //    Leadership = 8,
            //    ColdBlooded = true,
            //    Armour = 3,
            //    Ward = 5,
            //    MultiWounds = "2",

            //    Mount = new Enemy
            //    {
            //        Name = "Grymloq",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 7,
            //        WeaponSkill = 3,
            //        BallisticSkill = 0,
            //        Strength = 7,
            //        Toughness = 5,
            //        Wounds = 5,
            //        Initiative = 2,
            //        Attacks = 5,
            //        Leadership = 5,
            //        Armour = 4,
            //        Terror = true,
            //        ColdBlooded = true,
            //        MultiWounds = "D3",
            //        LargeBase = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Archaon",
            //    TestListName = "Archaon, Lord of the End Times (Chaos)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 9,
            //    BallisticSkill = 5,
            //    Strength = 5,
            //    Toughness = 5,
            //    Wounds = 4,
            //    Initiative = 7,
            //    Attacks = 10,
            //    Leadership = 10,
            //    Armour = 1,
            //    Ward = 3,
            //    ImmuneToPsychology = true,
            //    NoArmour = true,
            //    Terror = true,

            //    Mount = new Enemy
            //    {
            //        Name = "Dorghar",
            //        Type = UnitType.Mount,
            //        Size = 1,
            //        Movement = 8,
            //        WeaponSkill = 4,
            //        BallisticSkill = 0,
            //        Strength = 5,
            //        Toughness = 5,
            //        Wounds = 3,
            //        Initiative = 3,
            //        Attacks = 3,
            //        Leadership = 9,
            //        Armour = 4,
            //        LargeBase = true,
            //    }
            //},

            //new Enemy
            //{
            //    Name = "Grimgor Ironhide",
            //    TestListName = "Grimgor Ironhide (Orcs&Goblins)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 4,
            //    WeaponSkill = 8,
            //    BallisticSkill = 1,
            //    Strength = 7,
            //    Toughness = 5,
            //    Wounds = 3,
            //    Initiative = 5,
            //    Attacks = 7,
            //    Leadership = 9,
            //    Armour = 1,
            //    Ward = 5,
            //    Hate = true,
            //    HitFirst = true,
            //    ImmuneToPsychology = true,
            //},

            //new Enemy
            //{
            //    Name = "Bloodthister",
            //    TestListName = "Greater Daemon Bloodthister (Chaos)",
            //    Type = UnitType.Lord,
            //    Size = 1,
            //    Movement = 6,
            //    WeaponSkill = 10,
            //    BallisticSkill = 0,
            //    Strength = 7,
            //    Toughness = 6,
            //    Wounds = 7,
            //    Initiative = 10,
            //    Attacks = 7,
            //    Leadership = 9,
            //    Armour = 4,
            //    Ward = 5,
            //    Terror = true,
            //    KillingBlow = true,
            //    LargeBase = true,
            //},
        };
    }
}
