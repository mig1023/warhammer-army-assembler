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

            new Enemy("Men-at-arms/4/2/2/3/3/1/3/1/5/5/")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Core,
                Size = 20,
            },

            new Enemy("Empire swordmens/4/3/3/3/3/1/3/1/7/6/")
            {
                Armybook = "The Empire",
                Type = UnitType.Core,
                Size = 20,
            },

            new Enemy("Orc boys/4/3/3/3/4/1/2/1/7/5/")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Core,
                Size = 20,
            },

            new Enemy("Skeleton Warriors/4/2/2/3/3/1/2/1/5/5/")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Core,
                Size = 20,
                Undead = true,
            },

            new Enemy("Lothern Sea Guard/5/4/4/3/3/1/5/1/8/6/")
            {
                Armybook = "High Elves",
                Type = UnitType.Core,
                Size = 20,
                HitFirst = true,
            },

            new Enemy("Crypt Ghouls/4/3/0/3/4/1/3/2/5//")
            {
                Armybook = "Vampire Counts",
                Type = UnitType.Core,
                Size = 20,
                Undead = true,
                PoisonAttack = true,
            },

            new Enemy("Back Ark Corsairs/5/4/4/3/3/1/5/2/8/5/")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Core,
                Size = 20,
                Hate = true,
            },

            new Enemy("Dryads/5/4/0/4/4/1/6/2/8//")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Core,
                Size = 20,
                Fear = true,
            },

            new Enemy("Bestigor/5/4/3/6/4/1/3/1/7/5/")
            {
                Armybook = "Beastmen",
                Type = UnitType.Core,
                Size = 20,
                HitLast = true,
            },

            new Enemy("Knights of the Realms/4/4/3/3/3/1/3/1/8/2/")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Core,
                Size = 8,
                Lance = true,

                Mount = new Enemy("Warhorse/8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Longbeards/3/5/3/4/4/1/2/1/9/4/")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Core,
                Size = 20,
                ImmuneToPsychology = true
            },

            new Enemy("Temple Guard/4/4/0/5/4/1/2/2/8/4/")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Core,
                Size = 20,
                ColdBlooded = true
            },

            new Enemy("Chosen Knights/4/5/3/5/4/1/5/2/8/1/")
            {
                Armybook = "Chaos",
                Type = UnitType.Core,
                Size = 8,

                Mount = new Enemy("Chaos Steed/8/3/0/4/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            new Enemy("Tree Kin/5/4/4/4/5/3/3/3/8/4/")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Special,
                Size = 5,
                Fear = true,
            },

            new Enemy("Pegasus Knights/4/4/3/3/4/2/4/1/8/3/")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Special,
                Size = 8,
                Lance = true,

                Mount = new Enemy("Pegasus/8/3/0/4/4/1/4/2/7//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Chaos Ogre/6/3/2/4/4/3/2/3/7/6/")
            {
                Armybook = "Beastmen",
                Type = UnitType.Special,
                Size = 5,
                Fear = true,
            },

             new Enemy("Plague Monks/5/3/3/3/4/1/3/3/5//")
            {
                Armybook = "Skaven",
                Type = UnitType.Special,
                Size = 20,
                Frenzy = true,
                StrengthInNumbers = true,
            },

            new Enemy("Tomb Guard/4/3/3/4/4/1/3/1/8/5/")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Special,
                Size = 16,
                KillingBlow = true,
                Undead = true,
            },

            new Enemy("Grave Guard/4/3/3/4/4/1/3/2/8/4/")
            {
                Armybook = "Vampire Counts",
                Type = UnitType.Special,
                Size = 16,
                KillingBlow = true,
                Undead = true,
            },

            new Enemy("Greatswords/4/4/3/5/3/1/3/2/8/5/")
            {
                Armybook = "The Empire",
                Type = UnitType.Special,
                Size = 20,
                HitLast = true,
                Stubborn = true
            },

            new Enemy("Black Orcs/4/4/3/5/4/1/2/1/8/5/")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Special,
                Size = 16,
                HitLast = true
            },

            new Enemy("Orc Boar Boys/4/3/3/3/4/1/2/1/7/3/")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Special,
                Size = 8,

                Mount = new Enemy("Boar/7/3/0/3/4/1/3/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Cold One Knights/5/5/4/4/3/1/6/1/9/2/")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Special,
                Size = 8,
                Hate = true,
                Fear = true,
                Lance = true,

                Mount = new Enemy("Cold One/7/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Cold One Cavalry/4/4/0/4/4/1/2/2/8/2/")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Special,
                Size = 8,
                ColdBlooded = true,
                Fear = true,

                Mount = new Enemy("Cold One/7/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Bloodletters/4/5/0/5/3/1/4/2/8/6/")
            {
                Armybook = "Chaos",
                Type = UnitType.Special,
                Size = 20,
                Frenzy = true
            },

            new Enemy("Sword Masters/5/6/4/5/3/1/5/2/8/5/")
            {
                Armybook = "High Elves",
                Type = UnitType.Special,
                Size = 16,
                HitFirst = true
            },

            new Enemy("Hammerers/3/5/3/6/4/1/2/1/9/5/")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Special,
                Size = 16,
                Stubborn = true,
            },
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            new Enemy("Snotling Pump Wagon/6/2/0/2/4/3/3/5/4/6/")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Rare,
                Size = 1,
                Unbreakable = true,
            },

            new Enemy("Flagellants/4/2/2/3/3/1/3/1/10//")
            {
                Armybook = "The Empire",
                Type = UnitType.Rare,
                Size = 24,
                Unbreakable = true,
                Frenzy = true,
                Flail = true,
            },

            new Enemy("Waywathers/5/4/5/3/3/1/5/2/8//")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Rare,
                Size = 16,
                HitFirst = true,
            },

            new Enemy("White Lions/5/5/4/6/3/1/5/1/8/6/")
            {
                Armybook = "High Elves",
                Type = UnitType.Rare,
                Size = 16,
                HitFirst = true
            },

            new Enemy("Black Guard/5/5/4/4/3/1/6/1/9/5/")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Rare,
                Size = 16,
                Hate = true,
                Stubborn = true,
            },

            new Enemy("Troll Slayers/3/4/3/5/4/1/2/1/10//")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Rare,
                Size = 16,
                Unbreakable = true,
            },

            new Enemy("Grail Knights/4/5/3/4/3/1/5/2/8/2/5")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Rare,
                Size = 12,
                Lance = true,

                Mount = new Enemy("Warhorse/8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 12,
                }
            },

            new Enemy("Blood Knights/4/5/3/5/4/1/4/2/7/2/5")
            {
                Armybook = "Vampire Counts",
                Type = UnitType.Rare,
                Size = 6,
                Frenzy = true,
                Undead = true,
                Lance = true,

                Mount = new Enemy("Nightmare/8/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 6,
                }
            },

            new Enemy("Skullcrushers/4/5/3/4/4/1/5/2/8/1/")
            {
                Armybook = "Chaos",
                Type = UnitType.Rare,
                Size = 6,
                Fear = true,

                Mount = new Enemy("Juggernaut/7/5/0/5/4/3/2/3/7//")
                {
                    Type = UnitType.Mount,
                    Size = 6,
                }
            },
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            new Enemy("Tretch Craventail/5/5/4/4/4/2/6/4/6/5/4")
            {
                Armybook = "Skaven",
                Type = UnitType.Hero,
                Size = 1,
            },

            new Enemy("The Herald Nekaph/4/5/3/4/4/2/3/3/8//5")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Hero,
                Size = 1,
                KillingBlow = true,
                Undead = true,
                Flail = true,
                MultiWounds = "2",
            },

            new Enemy("Gitilla/4/4/4/4/4/2/4/3/7/3/")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Hero,
                Size = 1,

                Mount = new Enemy("Ulda the Great Wolf/9/3/0/3/3/1/3/2/3//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Moonclaw/5/3/3/4/4/2/3/3/7//5")
            {
                Armybook = "Beastmen",
                Type = UnitType.Hero,
                Size = 1,

                Mount = new Enemy("Umbralok/7/3/0/4/4/1/2/3/6//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Ludwig Schwarzhelm/4/6/5/4/4/2/5/3/8/2/")
            {
                Armybook = "The Empire",
                Type = UnitType.Hero,
                Size = 1,
                KillingBlow = true,
                Reroll = "ToWound",

                Mount = new Enemy("Warhorse/8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Gor-Rok/4/5/0/5/5/2/3/4/8/3/")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Hero,
                Size = 1,
                ColdBlooded = true,
                Stubborn = true,
                NoKillingBlow = true,
                NoMultiWounds = true,
                Reroll = "ToHit;OpponentToWound",
            },

            new Enemy("Josef Bugman/3/6/5/5/5/2/4/4/10/3/4")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Hero,
                Size = 1,
                ImmuneToPsychology = true,
            },

            new Enemy("Drycha/5/7/4/5/4/3/8/5/8//")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Hero,
                Size = 1,
                Terror = true,
                Reroll = "ToHit",
            },

            new Enemy("Caradryan/5/6/6/4/3/4/7/3/9/5/4")
            {
                Armybook = "High Elves",
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                HitFirst = true,
                MultiWounds = "D3",
            },

            new Enemy("Konrad/6/7/4/5/4/2/6/4/6/5/5")
            {
                Armybook = "Vampire Counts",
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                HitFirst = true,
                Reroll = "ToHit",
                MultiWounds = "2",
                Undead = true,
            },

            new Enemy("Throgg/6/5/2/6/5/4/2/5/8//")
            {
                Armybook = "Chaos",
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                Regeneration = true,
                LargeBase = true,
            },

            new Enemy("Malus (Tz'arkan)/6/7/5/5/5/2/9/3/10/3/")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Hero,
                Size = 1,
                Reroll = "ToWound",
                NoArmour = true,

                Mount = new Enemy("Spite/7/3/0/4/4/1/2/2/4/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Fear = true,
                }
            },

            new Enemy("Deathmaster Snikch/6/8/6/4/4/2/10/6/8//4")
            {
                Armybook = "Skaven",
                Type = UnitType.Hero,
                Size = 1,
                HitFirst = true,
                ArmourPiercing = 2,
                MultiWounds = "D3",
            },

            new Enemy("Chakax/4/5/0/7/5/2/3/4/8/4/5")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Hero,
                Size = 1,
                Unbreakable = true,
                HitFirst = true,
                Reroll = "ToHit",
            },
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            new Enemy("Green Knight/6/7/3/6/4/3/6/4/9//4")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                Terror = true,
                Undead = true,

                Mount = new Enemy("Shadow Steed/8/4/0/4/3/1/4/1/5/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Khuzrak/5/7/1/5/5/3/5/4/9/2/")
            {
                Armybook = "Beastmen",
                Type = UnitType.Lord,
                Size = 1,
            },

            new Enemy("Khalida/6/6/3/4/5/3/9/5/10//")
            {
                Armybook = "Tomb Kings",
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Undead = true,
                PoisonAttack = true,
            },

            new Enemy("Louen Leoncoeur/4/7/5/5/4/3/7/5/9/3/5")
            {
                Armybook = "Bretonnia",
                Type = UnitType.Lord,
                Size = 1,
                Lance = true,
                ImmuneToPsychology = true,
                Regeneration = true,
                Reroll = "ToHit;ToWound",

                Mount = new Enemy("Beaquis/8/5/0/5/5/4/6/4/9/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                }
            },

            new Enemy("Kurt Helborg/6/7/3/4/4/3/6/4/9/2/")
            {
                Armybook = "The Empire",
                Type = UnitType.Lord,
                Size = 1,
                Stubborn = true,
                ImmuneToPsychology = true,
                AutoWound = true,
                NoArmour = true,

                Mount = new Enemy("Warhorse/8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Zacharias/6/6/6/5/5/4/8/5/10//4")
            {
                Armybook = "Vampire Counts",
                Type = UnitType.Lord,
                Size = 1,
                Undead = true,

                Mount = new Enemy("Zombie Dragon/6/3/0/6/6/6/1/4/4/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    Undead = true,
                }
            },

            new Enemy("Karl Franz/4/6/5/4/4/3/6/4/10/4/4")
            {
                Armybook = "The Empire",
                Type = UnitType.Lord,
                Size = 1,
                AutoWound = true,
                NoArmour = true,
                MultiWounds = "D3",

                Mount = new Enemy("Deathclaw/6/6/0/5/5/4/5/4/8//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                }
            },

            new Enemy("Tyrion/5/9/7/7/3/4/10/4/10/1/4")
            {
                Armybook = "High Elves",
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Regeneration = true,

                Mount = new Enemy("Malhandir/10/4/0/4/3/1/5/2/7//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Torgrim Grudgebearer/3/7/6/4/5/7/4/4/10/2/4")
            {
                Armybook = "Dwarfs",
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                ImmuneToPsychology = true,
                Stubborn = true,

                Mount = new Enemy("Thronebearers/3/5/3/4/0/1/3/4/0//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Orion/9/8/8/6/5/5/9/5/10//5")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Frenzy = true,
                Terror = true,
                Unbreakable = true,

                Mount = new Enemy("Hound of Orion/9/4/0/4/4/1/4/1/6//")
                {
                    Type = UnitType.Hero,
                    Size = 2,
                    Frenzy = true,
                    Unbreakable = true,
                }
            },

            new Enemy("Durthu/5/7/7/6/6/6/2/6/10/3/6")
            {
                Armybook = "Wood Elves",
                Type = UnitType.Lord,
                Size = 1,
                LargeBase = true,
                Frenzy = true,
                Terror = true,
                Hate = true,
                Stubborn = true,
            },

            new Enemy("Vermin Lord/8/8/4/6/5/5/10/5/8//5")
            {
                Armybook = "Skaven",
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                Terror = true,
                MultiWounds = "D3",
            },

            new Enemy("Malekith/8/5/4/6/3/3/8/4/10/4/2")
            {
                Armybook = "Dark Elves",
                Type = UnitType.Lord,
                Size = 1,
                NoArmour = true,

                Mount = new Enemy("Seraphon/6/6/0/6/6/6/4/5/8/3/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    LargeBase = true,
                }
            },

            new Enemy("Kroq-Gar/4/6/3/6/5/3/4/5/8/3/5")
            {
                Armybook = "Lizardmen",
                Type = UnitType.Lord,
                Size = 1,
                ColdBlooded = true,
                MultiWounds = "2",

                Mount = new Enemy("Grymloq/7/3/0/7/5/5/2/5/5/4/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    ColdBlooded = true,
                    MultiWounds = "D3",
                    LargeBase = true,
                }
            },

            new Enemy("Archaon/4/9/5/5/5/4/7/10/10/1/3")
            {
                Armybook = "Chaos",
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                NoArmour = true,
                Terror = true,

                Mount = new Enemy("Dorghar/8/4/0/5/5/3/3/3/9/4/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    LargeBase = true,
                }
            },

            new Enemy("Grimgor Ironhide/4/8/1/7/5/3/5/7/9/1/5")
            {
                Armybook = "Orcs&Goblins",
                Type = UnitType.Lord,
                Size = 1,
                Hate = true,
                HitFirst = true,
                ImmuneToPsychology = true,
            },

            new Enemy("Bloodthister/6/10/0/7/6/7/10/7/9/4/5")
            {
                Armybook = "Chaos",
                Type = UnitType.Lord,
                Size = 1,
                Terror = true,
                KillingBlow = true,
                LargeBase = true,
            },
        };
    }
}
