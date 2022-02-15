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

            this.Size = int.Parse(profile[0]);
            this.Name = profile[1];
            this.Armybook = profile[2];

            this.Movement = NewProfile(profile[3]);
            this.WeaponSkill = NewProfile(profile[4]);
            this.BallisticSkill = NewProfile(profile[5]);
            this.Strength = NewProfile(profile[6]);
            this.Toughness = NewProfile(profile[7]);
            this.Wounds = NewProfile(profile[8]);
            this.Initiative = NewProfile(profile[9]);
            this.Attacks = NewProfile(profile[10]);
            this.Leadership = NewProfile(profile[11]);

            bool isArmour = int.TryParse(profile[12], out int armour);

            if (isArmour)
                this.Armour = new Profile { Value = armour };
            else
                this.Armour = new Profile { Null = true };

            bool isWard = int.TryParse(profile[13], out int ward);

            if (isWard)
                this.Ward = new Profile { Value = ward };
            else
                this.Ward = new Profile { Null = true };
        }

        private Profile NewProfile(string line) => new Profile { Value = int.Parse(line) };

        public string Fullname()
        {
            if (this.Size > 1)
                return String.Format("{0} {1} ({2})", this.Size, this.Name, this.Armybook);
            else
                return String.Format("{0} ({1})", this.Name, this.Armybook);
        }

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
            new Enemy("1/Troll/Orcs&Goblins/6/3/1/5/4/3/1/3/4//")
            {
                Fear = true,
                Regeneration = true,
                Stupidity = true,
                LargeBase = true,
            },

            new Enemy("1/Gyrobomber/Dwarfs/1/4/3/4/5/3/2/2/9/4/")
            {
                LargeBase = true,
            },

            new Enemy("1/Ancient Kroxigor/Lizardmen/6/3/1/7/4/3/1/4/7//")
            {
                Fear = true,
                ColdBlooded = true,
                LargeBase = true,
            },

            new Enemy("1/Tomb Scorpion/Tomb Kings/7/4/0/5/5/3/3/4/8//")
            {
                Undead = true,
                KillingBlow = true,
                PoisonAttack = true,
                LargeBase = true,
            },

            new Enemy("1/Hippogryph/Bretonnia/8/4/0/5/5/4/4/4/8//")
            {
                Terror = true,
                LargeBase = true,
            },

            new Enemy("1/Griffon/The Empire/6/5/0/5/5/4/5/4/7//")
            {
                Terror = true,
                LargeBase = true,
            },

            new Enemy("1/Manticore/Chaos/6/5/0/5/5/4/5/4/5//")
            {
                Terror = true,
                KillingBlow = true,
                LargeBase = true,
            },

            new Enemy("1/Varghulf/Vampire/8/5/0/5/5/4/2/5/4//")
            {
                Terror = true,
                Undead = true,
                Regeneration = true,
                Hate = true,
                LargeBase = true,
            },

            new Enemy("1/War Hydra/Dark Elves/6/4/0/5/5/6/2/5/6/4/")
            {
                Terror = true,
                LargeBase = true,

                Mount = new Enemy("Apparentice//5/4/4/3/3/2/3/2/8//")
                {
                    Type = UnitType.Mount,
                    Size = 2,
                    NoKillingBlow = true,
                }
            },

            new Enemy("1/Dragon Ogre Shaggoth/Beastmen/7/6/3/5/5/6/4/5/9/4/")
            {
                Terror = true,
                ImmuneToPsychology = true,
                LargeBase = true,
            },

            new Enemy("1/Stegadon/Lizardmen/6/3/0/5/6/5/2/4/5/4/")
            {
                LargeBase = true,
                ColdBlooded = true,
                Terror = true,
                Stubborn = true,
                ImmuneToPsychology = true,

                Mount = new Enemy("Skink Crew//6/2/3/3/2/4/4/4/5/4/")
                {
                    Type = UnitType.Mount,
                    Size = 5,
                    ColdBlooded = true,
                    PoisonAttack = true,
                    NoKillingBlow = true,
                }
            },

            new Enemy("1/Treeman/Wood Elves/5/5/0/6/6/6/2/5/8/3/")
            {
                Terror = true,
                LargeBase = true,
            },

            new Enemy("1/Giant/Orcs&Goblins/6/3/3/6/5/6/3/0/10//")
            {
                Terror = true,
                Stubborn = true,
                LargeBase = true,
                Giant = true,
            },

            new Enemy("1/Hell Pit Abomination/Skaven/6/3/3/6/5/6/3/0/10//")
            {
                Regeneration = true,
                Terror = true,
                Stubborn = true,
                LargeBase = true,
                HellPitAbomination = true,
            },

            new Enemy("1/Necrosphinx/Tomb Kings/6/4/0/5/8/5/1/5/8/3/")
            {
                Terror = true,
                LargeBase = true,
                HeroicKillingBlow = true,
                Undead = true,
            },

            new Enemy("1/Star Dragon/High Elves/6/7/0/7/6/7/2/6/9/3/")
            {
                Terror = true,
                LargeBase = true,
            },

            new Enemy("1/Steam Tank/The Empire/0/0/4/6/6/10/0/0/10/1/")
            {
                Unbreakable = true,
                Terror = true,
                LargeBase = true,
                SteamTank = true,
            },
        };

        private static List<Enemy> EnemiesCoreUnits = new List<Enemy>
        {
            new Enemy("20/Clanrat Slaves/Skaven/5/2/2/3/3/1/4/1/2//")
            {
                StrengthInNumbers = true,
            },

            new Enemy("20/Men-at-arms/Bretonnia/4/2/2/3/3/1/3/1/5/5/"),

            new Enemy("20/Empire swordmens/The Empire/4/3/3/3/3/1/3/1/7/6/"),

            new Enemy("20/Orc boys/Orcs&Goblins/4/3/3/3/4/1/2/1/7/5/"),

            new Enemy("20/Skeleton Warriors/Tomb Kings/4/2/2/3/3/1/2/1/5/5/")
            {
                Undead = true,
            },

            new Enemy("20/Lothern Sea Guard/High Elves/5/4/4/3/3/1/5/1/8/6/")
            {
                HitFirst = true,
            },

            new Enemy("20/Crypt Ghouls/Vampire Counts/4/3/0/3/4/1/3/2/5//")
            {
                Undead = true,
                PoisonAttack = true,
            },

            new Enemy("20/Back Ark Corsairs/Dark Elves/5/4/4/3/3/1/5/2/8/5/")
            {
                Hate = true,
            },

            new Enemy("20/Dryads/Wood Elves/5/4/0/4/4/1/6/2/8//")
            {
                Fear = true,
            },

            new Enemy("20/Bestigor/Beastmen/5/4/3/6/4/1/3/1/7/5/")
            {
                HitLast = true,
            },

            new Enemy("8/Knights of the Realms/Bretonnia/4/4/3/3/3/1/3/1/8/2/")
            {
                Lance = true,

                Mount = new Enemy("Warhorse//8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("20/Longbeards/Dwarfs/3/5/3/4/4/1/2/1/9/4/")
            {
                ImmuneToPsychology = true
            },

            new Enemy("20/Temple Guard/Lizardmen/4/4/0/5/4/1/2/2/8/4/")
            {
                ColdBlooded = true
            },

            new Enemy("8/Chosen Knights/Chaos/4/5/3/5/4/1/5/2/8/1/")
            {
                Mount = new Enemy("Chaos Steed//8/3/0/4/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            new Enemy("5/Tree Kin/Wood Elves/5/4/4/4/5/3/3/3/8/4/")
            {
                Fear = true,
            },

            new Enemy("8/Pegasus Knights/Bretonnia/4/4/3/3/4/2/4/1/8/3/")
            {
                Lance = true,

                Mount = new Enemy("Pegasus//8/3/0/4/4/1/4/2/7//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("5/Chaos Ogre/Beastmen/6/3/2/4/4/3/2/3/7/6/")
            {
                Fear = true,
            },

            new Enemy("20/Plague Monks/Skaven/5/3/3/3/4/1/3/3/5//")
            {
                Frenzy = true,
                StrengthInNumbers = true,
            },

            new Enemy("16/Tomb Guard/Tomb Kings/4/3/3/4/4/1/3/1/8/5/")
            {
                KillingBlow = true,
                Undead = true,
            },

            new Enemy("16/Grave Guard/Vampire Counts/4/3/3/4/4/1/3/2/8/4/")
            {
                KillingBlow = true,
                Undead = true,
            },

            new Enemy("20/Greatswords/The Empire/4/4/3/5/3/1/3/2/8/5/")
            {
                HitLast = true,
                Stubborn = true
            },

            new Enemy("16/Black Orcs/Orcs&Goblins/4/4/3/5/4/1/2/1/8/5/")
            {
                HitLast = true
            },

            new Enemy("8/Orc Boar Boys/Orcs&Goblins/4/3/3/3/4/1/2/1/7/3/")
            {
                Mount = new Enemy("Boar//7/3/0/3/4/1/3/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("8/Cold One Knights/Dark Elves/5/5/4/4/3/1/6/1/9/2/")
            {
                Hate = true,
                Fear = true,
                Lance = true,

                Mount = new Enemy("Cold One//7/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Cold One Cavalry/Lizardmen/4/4/0/4/4/1/2/2/8/2/")
            {
                Type = UnitType.Special,
                Size = 8,
                ColdBlooded = true,
                Fear = true,

                Mount = new Enemy("Cold One//7/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Bloodletters/Chaos/4/5/0/5/3/1/4/2/8/6/")
            {
                Type = UnitType.Special,
                Size = 20,
                Frenzy = true
            },

            new Enemy("Sword Masters/High Elves/5/6/4/5/3/1/5/2/8/5/")
            {
                Type = UnitType.Special,
                Size = 16,
                HitFirst = true
            },

            new Enemy("Hammerers/Dwarfs/3/5/3/6/4/1/2/1/9/5/")
            {
                Type = UnitType.Special,
                Size = 16,
                Stubborn = true,
            },
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            new Enemy("Snotling Pump Wagon/Orcs&Goblins/6/2/0/2/4/3/3/5/4/6/")
            {
                Type = UnitType.Rare,
                Size = 1,
                Unbreakable = true,
            },

            new Enemy("Flagellants/The Empire/4/2/2/3/3/1/3/1/10//")
            {
                Type = UnitType.Rare,
                Size = 24,
                Unbreakable = true,
                Frenzy = true,
                Flail = true,
            },

            new Enemy("Waywathers/Wood Elves/5/4/5/3/3/1/5/2/8//")
            {
                Type = UnitType.Rare,
                Size = 16,
                HitFirst = true,
            },

            new Enemy("White Lions/High Elves/5/5/4/6/3/1/5/1/8/6/")
            {
                Type = UnitType.Rare,
                Size = 16,
                HitFirst = true
            },

            new Enemy("Black Guard/Dark Elves/5/5/4/4/3/1/6/1/9/5/")
            {
                Type = UnitType.Rare,
                Size = 16,
                Hate = true,
                Stubborn = true,
            },

            new Enemy("Troll Slayers/Dwarfs/3/4/3/5/4/1/2/1/10//")
            {
                Type = UnitType.Rare,
                Size = 16,
                Unbreakable = true,
            },

            new Enemy("Grail Knights/Bretonnia/4/5/3/4/3/1/5/2/8/2/5")
            {
                Type = UnitType.Rare,
                Size = 12,
                Lance = true,

                Mount = new Enemy("Warhorse//8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 12,
                }
            },

            new Enemy("Blood Knights/Vampire Counts/4/5/3/5/4/1/4/2/7/2/5")
            {
                Type = UnitType.Rare,
                Size = 6,
                Frenzy = true,
                Undead = true,
                Lance = true,

                Mount = new Enemy("Nightmare//8/3/0/4/4/1/2/1/3//")
                {
                    Type = UnitType.Mount,
                    Size = 6,
                }
            },

            new Enemy("Skullcrushers/Chaos/4/5/3/4/4/1/5/2/8/1/")
            {
                Armybook = "Chaos",
                Type = UnitType.Rare,
                Size = 6,
                Fear = true,

                Mount = new Enemy("Juggernaut//7/5/0/5/4/3/2/3/7//")
                {
                    Type = UnitType.Mount,
                    Size = 6,
                }
            },
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            new Enemy("Tretch Craventail/Skaven/5/5/4/4/4/2/6/4/6/5/4")
            {
                Type = UnitType.Hero,
                Size = 1,
            },

            new Enemy("The Herald Nekaph/Tomb Kings/4/5/3/4/4/2/3/3/8//5")
            {
                Type = UnitType.Hero,
                Size = 1,
                KillingBlow = true,
                Undead = true,
                Flail = true,
                MultiWounds = "2",
            },

            new Enemy("Gitilla/Orcs&Goblins/4/4/4/4/4/2/4/3/7/3/")
            {
                Type = UnitType.Hero,
                Size = 1,

                Mount = new Enemy("Ulda the Great Wolf//9/3/0/3/3/1/3/2/3//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Moonclaw/Beastmen/5/3/3/4/4/2/3/3/7//5")
            {
                Type = UnitType.Hero,
                Size = 1,

                Mount = new Enemy("Umbralok//7/3/0/4/4/1/2/3/6//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Ludwig Schwarzhelm/The Empire/4/6/5/4/4/2/5/3/8/2/")
            {
                Type = UnitType.Hero,
                Size = 1,
                KillingBlow = true,
                Reroll = "ToWound",

                Mount = new Enemy("Warhorse//8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Gor-Rok/Lizardmen/4/5/0/5/5/2/3/4/8/3/")
            {
                Type = UnitType.Hero,
                Size = 1,
                ColdBlooded = true,
                Stubborn = true,
                NoKillingBlow = true,
                NoMultiWounds = true,
                Reroll = "ToHit;OpponentToWound",
            },

            new Enemy("Josef Bugman/Dwarfs/3/6/5/5/5/2/4/4/10/3/4")
            {
                Type = UnitType.Hero,
                Size = 1,
                ImmuneToPsychology = true,
            },

            new Enemy("Drycha/Wood Elves/5/7/4/5/4/3/8/5/8//")
            {
                Type = UnitType.Hero,
                Size = 1,
                Terror = true,
                Reroll = "ToHit",
            },

            new Enemy("Caradryan/High Elves/5/6/6/4/3/4/7/3/9/5/4")
            {
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                HitFirst = true,
                MultiWounds = "D3",
            },

            new Enemy("Konrad/Vampire Counts/6/7/4/5/4/2/6/4/6/5/5")
            {
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                HitFirst = true,
                Reroll = "ToHit",
                MultiWounds = "2",
                Undead = true,
            },

            new Enemy("Throgg/Chaos/6/5/2/6/5/4/2/5/8//")
            {
                Type = UnitType.Hero,
                Size = 1,
                Fear = true,
                Regeneration = true,
                LargeBase = true,
            },

            new Enemy("Malus (Tz'arkan)/Dark Elves/6/7/5/5/5/2/9/3/10/3/")
            {
                Type = UnitType.Hero,
                Size = 1,
                Reroll = "ToWound",
                NoArmour = true,

                Mount = new Enemy("Spite//7/3/0/4/4/1/2/2/4/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Fear = true,
                }
            },

            new Enemy("Deathmaster Snikch/Skaven/6/8/6/4/4/2/10/6/8//4")
            {
                Type = UnitType.Hero,
                Size = 1,
                HitFirst = true,
                ArmourPiercing = 2,
                MultiWounds = "D3",
            },

            new Enemy("Chakax/Lizardmen/4/5/0/7/5/2/3/4/8/4/5")
            {
                Type = UnitType.Hero,
                Size = 1,
                Unbreakable = true,
                HitFirst = true,
                Reroll = "ToHit",
            },
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            new Enemy("Green Knight/Bretonnia/6/7/3/6/4/3/6/4/9//4")
            {
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                Terror = true,
                Undead = true,

                Mount = new Enemy("Shadow Steed//8/4/0/4/3/1/4/1/5/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Khuzrak/Beastmen/5/7/1/5/5/3/5/4/9/2/")
            {
                Type = UnitType.Lord,
                Size = 1,
            },

            new Enemy("Khalida/Tomb Kings/6/6/3/4/5/3/9/5/10//")
            {
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Undead = true,
                PoisonAttack = true,
            },

            new Enemy("Louen Leoncoeur/Bretonnia/4/7/5/5/4/3/7/5/9/3/5")
            {
                Type = UnitType.Lord,
                Size = 1,
                Lance = true,
                ImmuneToPsychology = true,
                Regeneration = true,
                Reroll = "ToHit;ToWound",

                Mount = new Enemy("Beaquis//8/5/0/5/5/4/6/4/9/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                }
            },

            new Enemy("Kurt Helborg/The Empire/6/7/3/4/4/3/6/4/9/2/")
            {
                Type = UnitType.Lord,
                Size = 1,
                Stubborn = true,
                ImmuneToPsychology = true,
                AutoWound = true,
                NoArmour = true,

                Mount = new Enemy("Warhorse//8/3/0/3/3/1/3/1/5//")
                {
                    Type = UnitType.Mount,
                    Size = 8,
                }
            },

            new Enemy("Zacharias/Vampire Counts/6/6/6/5/5/4/8/5/10//4")
            {
                Type = UnitType.Lord,
                Size = 1,
                Undead = true,

                Mount = new Enemy("Zombie Dragon//6/3/0/6/6/6/1/4/4/5/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    Undead = true,
                }
            },

            new Enemy("Karl Franz/The Empire/4/6/5/4/4/3/6/4/10/4/4")
            {
                Type = UnitType.Lord,
                Size = 1,
                AutoWound = true,
                NoArmour = true,
                MultiWounds = "D3",

                Mount = new Enemy("Deathclaw//6/6/0/5/5/4/5/4/8//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                }
            },

            new Enemy("Tyrion/High Elves/5/9/7/7/3/4/10/4/10/1/4")
            {
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Regeneration = true,

                Mount = new Enemy("Malhandir//10/4/0/4/3/1/5/2/7//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Torgrim Grudgebearer/Dwarfs/3/7/6/4/5/7/4/4/10/2/4")
            {
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                ImmuneToPsychology = true,
                Stubborn = true,

                Mount = new Enemy("Thronebearers//3/5/3/4/0/1/3/4/0//")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                }
            },

            new Enemy("Orion/Wood Elves/9/8/8/6/5/5/9/5/10//5")
            {
                Type = UnitType.Lord,
                Size = 1,
                HitFirst = true,
                Frenzy = true,
                Terror = true,
                Unbreakable = true,

                Mount = new Enemy("Hound of Orion//9/4/0/4/4/1/4/1/6//")
                {
                    Type = UnitType.Hero,
                    Size = 2,
                    Frenzy = true,
                    Unbreakable = true,
                }
            },

            new Enemy("Durthu/Wood Elves/5/7/7/6/6/6/2/6/10/3/6")
            {
                Type = UnitType.Lord,
                Size = 1,
                LargeBase = true,
                Frenzy = true,
                Terror = true,
                Hate = true,
                Stubborn = true,
            },

            new Enemy("Vermin Lord/Skaven/8/8/4/6/5/5/10/5/8//5")
            {
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                Terror = true,
                MultiWounds = "D3",
            },

            new Enemy("Malekith/Dark Elves/8/5/4/6/3/3/8/4/10/4/2")
            {
                Type = UnitType.Lord,
                Size = 1,
                NoArmour = true,

                Mount = new Enemy("Seraphon//6/6/0/6/6/6/4/5/8/3/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    LargeBase = true,
                }
            },

            new Enemy("Kroq-Gar/Lizardmen/4/6/3/6/5/3/4/5/8/3/5")
            {
                Type = UnitType.Lord,
                Size = 1,
                ColdBlooded = true,
                MultiWounds = "2",

                Mount = new Enemy("Grymloq//7/3/0/7/5/5/2/5/5/4/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    Terror = true,
                    ColdBlooded = true,
                    MultiWounds = "D3",
                    LargeBase = true,
                }
            },

            new Enemy("Archaon/Chaos/4/9/5/5/5/4/7/10/10/1/3")
            {
                Type = UnitType.Lord,
                Size = 1,
                ImmuneToPsychology = true,
                NoArmour = true,
                Terror = true,

                Mount = new Enemy("Dorghar//8/4/0/5/5/3/3/3/9/4/")
                {
                    Type = UnitType.Mount,
                    Size = 1,
                    LargeBase = true,
                }
            },

            new Enemy("Grimgor Ironhide/Orcs&Goblins/4/8/1/7/5/3/5/7/9/1/5")
            {
                Type = UnitType.Lord,
                Size = 1,
                Hate = true,
                HitFirst = true,
                ImmuneToPsychology = true,
            },

            new Enemy("Bloodthister/Chaos/6/10/0/7/6/7/10/7/9/4/5")
            {
                Type = UnitType.Lord,
                Size = 1,
                Terror = true,
                KillingBlow = true,
                LargeBase = true,
            },
        };
    }
}
