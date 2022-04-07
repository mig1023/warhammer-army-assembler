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
            List<string> multiplesProfile = enemyName.Split(new string[] { " + " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> profile = multiplesProfile[0].Trim().Split('/').ToList();

            bool isSized = int.TryParse(profile[0], out int size);

            if (!isSized)
            {
                this.Size = 1;
                profile.Insert(0, String.Empty);
            }
            else
                this.Size = size;
                
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

            this.Armour = SetProfile(profile, 12);
            this.Ward = SetProfile(profile, 13);

            if ((profile.Count < 15) || String.IsNullOrEmpty(profile[14]))
                return;

            foreach (string specialRule in profile[14].Split(','))
                this.GetType().GetProperty(specialRule.Trim()).SetValue(this, true);

            if (multiplesProfile.Count > 1)
                this.Mount = new Enemy(multiplesProfile[1].Trim());
        }

        private Profile NewProfile(string line) => ArmyBook.Parsers.ProfileParse(line);

        private Profile SetProfile(List<string> profile, int index)
        {
            if ((profile.Count > index) && int.TryParse(profile[index], out int value))
                return new Profile { Value = value };
            else
                return new Profile { Null = true };
        }

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
                EnemiesMonsters, EnemiesHeroes, EnemiesLords,
                EnemiesCoreUnits, EnemiesSpecialUnits, EnemiesRareUnits
            };

            foreach (List<Enemy> enemyList in enemies)
                foreach (Enemy enemy in enemyList.Where(x => x.Fullname() == enemyName))
                    return enemy.SetID();

            return null;
        }

        private Enemy SetID()
        {
            this.ID = --MaxIDindex;

            return this;
        }

        public static List<string> GetEnemiesGroups() => GetEnemiesDictionary().Keys.ToList<string>();

        public static List<Enemy> GetEnemiesByGroup(string groupName) => GetEnemiesDictionary()[groupName];

        public static int GetEnemiesCount() => GetEnemiesGroups().Sum(x => GetEnemiesByGroup(x).Count());

        private static List<Enemy> EnemiesMonsters = new List<Enemy>
        {
            new Enemy("Troll/Orcs&Goblins/6/3/1/5/4/3/1/3/4///Fear, Regeneration, Stupidity, LargeBase"),
            new Enemy("Gyrobomber/Dwarfs/1/4/3/4/5/3/2/2/9/4//LargeBase"),
            new Enemy("Ancient Kroxigor/Lizardmen/6/3/1/7/4/3/1/4/7///Fear, ColdBlooded, LargeBase"),
            new Enemy("Tomb Scorpion/Tomb Kings/7/4/0/5/5/3/3/4/8///Undead, KillingBlow, PoisonAttack, LargeBase"),
            new Enemy("Hippogryph/Bretonnia/8/4/0/5/5/4/4/4/8///Terror, LargeBase"),
            new Enemy("Griffon/The Empire/6/5/0/5/5/4/5/4/7///Terror, LargeBase"),
            new Enemy("Manticore/Chaos/6/5/0/5/5/4/5/4/5///Terror, KillingBlow, LargeBase"),
            new Enemy("Varghulf/Vampire/8/5/0/5/5/4/2/5/4///Terror, Undead, Regeneration, Hate, LargeBase"),
            new Enemy("War Hydra/Dark Elves/6/4/0/5/5/6/2/5/6/4//Terror, LargeBase + 2/Apparentice//5/4/4/3/3/2/3/2/8///NoKillingBlow"),
            new Enemy("Stonehorn/Ogre Kingdoms/7/3/0/6/6/6/2/5/5/4//Frenzy, Terror, LargeBase"),
            new Enemy("Dragon Ogre Shaggoth/Beastmen/7/6/3/5/5/6/4/5/9/4//Terror, ImmuneToPsychology, LargeBase"),
            new Enemy("Stegadon/Lizardmen/6/3/0/5/6/5/2/4/5/4//LargeBase, ColdBlooded, Terror, Stubborn, ImmuneToPsychology " +
                "+ 5/Skink Crew//6/2/3/3/2/4/4/4/5/4//ColdBlooded, PoisonAttack, NoKillingBlow"),
            new Enemy("Treeman/Wood Elves/5/5/0/6/6/6/2/5/8/3//Terror, LargeBase"),
            new Enemy("Giant/Orcs&Goblins/6/3/3/6/5/6/3/0/10///Terror, Stubborn, LargeBase, Giant"),
            new Enemy("Soul Grinder/Daemons/8/3/3/6/7/6/3/4/7/4//Terror, LargeBase"),
            new Enemy("Hell Pit Abomination/Skaven/6/3/3/6/5/6/3/0/10///Regeneration, Terror, Stubborn, LargeBase, HellPitAbomination"),
            new Enemy("Necrosphinx/Tomb Kings/6/4/0/5/8/5/1/5/8/3//Terror, LargeBase, HeroicKillingBlow, Undead"),
            new Enemy("Star Dragon/High Elves/6/7/0/7/6/7/2/6/9/3//Terror, LargeBase"),
            new Enemy("Steam Tank/The Empire/0/0/4/6/6/10/0/0/10/1//Unbreakable, Terror, LargeBase, SteamTank"),
        };

        private static List<Enemy> EnemiesCoreUnits = new List<Enemy>
        {
            new Enemy("20/Clanrat Slaves/Skaven/5/2/2/3/3/1/4/1/2///StrengthInNumbers"),
            new Enemy("20/Men-at-arms/Bretonnia/4/2/2/3/3/1/3/1/5/5"),
            new Enemy("20/Empire swordmens/The Empire/4/3/3/3/3/1/3/1/7/6"),
            new Enemy("20/Demonettes/Daemons/6/5/4/3/3/1/5/2/7"),
            new Enemy("5/Ogres/Ogre Kingdoms/6/3/2/4/4/3/2/3/7/6//Fear"),
            new Enemy("20/Orc boys/Orcs&Goblins/4/3/3/3/4/1/2/1/7/5"),
            new Enemy("20/Skeleton Warriors/Tomb Kings/4/2/2/3/3/1/2/1/5/5//Undead"),
            new Enemy("20/Lothern Sea Guard/High Elves/5/4/4/3/3/1/5/1/8/6//HitFirst"),
            new Enemy("20/Crypt Ghouls/Vampire Counts/4/3/0/3/4/1/3/2/5///Undead, PoisonAttack"),
            new Enemy("20/Back Ark Corsairs/Dark Elves/5/4/4/3/3/1/5/2/8/5//Hate"),
            new Enemy("20/Dryads/Wood Elves/5/4/0/4/4/1/6/2/8///Fear"),
            new Enemy("20/Bestigor/Beastmen/5/4/3/6/4/1/3/1/7/5//HitLast"),
            new Enemy("8/Knights of the Realms/Bretonnia/4/4/3/3/3/1/3/1/8/2//Lance + 8/Warhorse//8/3/0/3/3/1/3/1/5"),
            new Enemy("20/Longbeards/Dwarfs/3/5/3/4/4/1/2/1/9/4//ImmuneToPsychology"),
            new Enemy("20/Temple Guard/Lizardmen/4/4/0/5/4/1/2/2/8/4//ColdBlooded"),
            new Enemy("8/Chosen Knights/Chaos/4/5/3/5/4/1/5/2/8/1 + 8/Chaos Steed//8/3/0/4/3/1/3/1/5"),
        };

        private static List<Enemy> EnemiesSpecialUnits = new List<Enemy>
        {
            new Enemy("5/Tree Kin/Wood Elves/5/4/4/4/5/3/3/3/8/4//Fear"),
            new Enemy("8/Pegasus Knights/Bretonnia/4/4/3/3/4/2/4/1/8/3//Lance + 8/Pegasus//8/3/0/4/4/1/4/2/7"),
            new Enemy("5/Chaos Ogre/Beastmen/6/3/2/4/4/3/2/3/7/6//Fear"),
            new Enemy("4/Mournfang Cavalry/Ogre Kingdoms/6/3/2/4/4/3/2/3/7/6//Fear, LargeBase + 4/Mournfang//8/3/0/5/4/3/2/4/5/5"),
            new Enemy("20/Plague Monks/Skaven/5/3/3/3/4/1/3/3/5///Frenzy, StrengthInNumbers"),
            new Enemy("16/Tomb Guard/Tomb Kings/4/3/3/4/4/1/3/1/8/5//KillingBlow, Undead"),
            new Enemy("16/Grave Guard/Vampire Counts/4/3/3/4/4/1/3/2/8/4//KillingBlow, Undead"),
            new Enemy("20/Greatswords/The Empire/4/4/3/5/3/1/3/2/8/5//HitLast, Stubborn"),
            new Enemy("3/Beasts of Nurgle/Daemons/6/3/0/4/5/4/2/D6+1/7///PoisonAttack, Regeneration"),
            new Enemy("16/Black Orcs/Orcs&Goblins/4/4/3/5/4/1/2/1/8/5//HitLast"),
            new Enemy("8/Orc Boar Boys/Orcs&Goblins/4/3/3/3/4/1/2/1/7/3 + 8/Boar//7/3/0/3/4/1/3/1/3"),
            new Enemy("8/Cold One Knights/Dark Elves/5/5/4/4/3/1/6/1/9/2//Hate, Fear, Lance + 8/Cold One//7/3/0/4/4/1/2/1/3"),
            new Enemy("8/Cold One Cavalry/Lizardmen/4/4/0/4/4/1/2/2/8/2//ColdBlooded, Fear")
            {
                Mount = new Enemy("8/Cold One//7/3/0/4/4/1/2/1/3"),
            },
            new Enemy("20/Bloodletters/Chaos/4/5/0/5/3/1/4/2/8/6//Frenzy"),
            new Enemy("16/Sword Masters/High Elves/5/6/4/5/3/1/5/2/8/5//HitFirst"),
            new Enemy("16/Hammerers/Dwarfs/3/5/3/6/4/1/2/1/9/5//Stubborn"),
        };

        private static List<Enemy> EnemiesRareUnits = new List<Enemy>
        {
            new Enemy("Snotling Pump Wagon/Orcs&Goblins/6/2/0/2/4/3/3/5/4/6//Unbreakable"),
            new Enemy("3/Plague Drones/Daemons/4/3/3/4/4/1/2/1/7///PoisonAttack")
            {
                Mount = new Enemy("3/Rot Fly//1/3/3/5/5/3/2/3/7"),
            },
            new Enemy("24/Flagellants/The Empire/4/2/2/3/3/1/3/1/10///Unbreakable, Frenzy, Flail"),
            new Enemy("16/Waywathers/Wood Elves/5/4/5/3/3/1/5/2/8///HitFirst"),
            new Enemy("Doomwheel/Skaven/2D6/3/3/6/6/5/4/2D6/7/4//ImmuneToPsychology, LargeBase, Terror"),
            new Enemy("16/White Lions/High Elves/5/5/4/6/3/1/5/1/8/6//HitFirst"),
            new Enemy("16/Black Guard/Dark Elves/5/5/4/4/3/1/6/1/9/5//Hate, Stubborn"),
            new Enemy("16/Troll Slayers/Dwarfs/3/4/3/5/4/1/2/1/10///Unbreakable"),
            new Enemy("12/Grail Knights/Bretonnia/4/5/3/4/3/1/5/2/8/2/5/Lance")
            {
                Mount = new Enemy("12/Warhorse//8/3/0/3/3/1/3/1/5"),
            },
            new Enemy("6/Blood Knights/Vampire Counts/4/5/3/5/4/1/4/2/7/2/5/Frenzy, Undead, Lance")
            {
                Mount = new Enemy("6/Nightmare//8/3/0/4/4/1/2/1/3"),
            },
            new Enemy("6/Skullcrushers/Chaos/4/5/3/4/4/1/5/2/8/1//Fear")
            {
                Mount = new Enemy("6/Juggernaut//7/5/0/5/4/3/2/3/7"),
            },
        };

        private static List<Enemy> EnemiesHeroes = new List<Enemy>
        {
            new Enemy("Tretch Craventail/Skaven/5/5/4/4/4/2/6/4/6/5/4"),
            new Enemy("The Herald Nekaph/Tomb Kings/4/5/3/4/4/2/3/3/8//5/KillingBlow, Undead, Flail")
            {
                MultiWounds = "2",
            },
            new Enemy("Gitilla/Orcs&Goblins/4/4/4/4/4/2/4/3/7/3")
            {
                Mount = new Enemy("Ulda the Great Wolf//9/3/0/3/3/1/3/2/3"),
            },
            new Enemy("Moonclaw/Beastmen/5/3/3/4/4/2/3/3/7//5")
            {
                Mount = new Enemy("Umbralok//7/3/0/4/4/1/2/3/6"),
            },
            new Enemy("Ludwig Schwarzhelm/The Empire/4/6/5/4/4/2/5/3/8/2//KillingBlow")
            {
                Reroll = "ToWound",
                Mount = new Enemy("Warhorse//8/3/0/3/3/1/3/1/5"),
            },
            new Enemy("Gor-Rok/Lizardmen/4/5/0/5/5/2/3/4/8/3//ColdBlooded, Stubborn, NoKillingBlow, NoMultiWounds")
            {
                Reroll = "ToHit;OpponentToWound",
            },
            new Enemy("Josef Bugman/Dwarfs/3/6/5/5/5/2/4/4/10/3/4/ImmuneToPsychology"),
            new Enemy("Drycha/Wood Elves/5/7/4/5/4/3/8/5/8///Terror")
            {
                Reroll = "ToHit",
            },
            new Enemy("Caradryan/High Elves/5/6/6/4/3/4/7/3/9/5/4/Fear, HitFirst")
            {
                MultiWounds = "D3",
            },
            new Enemy("Konrad/Vampire Counts/6/7/4/5/4/2/6/4/6/5/5/Fear, HitFirst, Undead")
            {
                Reroll = "ToHit",
                MultiWounds = "2",
            },
            new Enemy("Bragg The Gutsman/Ogre Kingdoms/6/5/3/6/5/4/3/4/8/6//Fear, HeroicKillingBlow, LargeBase"),
            new Enemy("Throgg/Chaos/6/5/2/6/5/4/2/5/8///Fear, Regeneration, LargeBase"),
            new Enemy("Karanak/Daemons/8/7/0/5/5/3/6/4/8/6//Hate"),
            new Enemy("Malus (Tz'arkan)/Dark Elves/6/7/5/5/5/2/9/3/10/3//NoArmour")
            {
                Reroll = "ToWound",
                Mount = new Enemy("Spite//7/3/0/4/4/1/2/2/4/5//Fear"),
            },
            new Enemy("Deathmaster Snikch/Skaven/6/8/6/4/4/2/10/6/8//4/HitFirst")
            {
                ArmourPiercing = 2,
                MultiWounds = "D3",
            },
            new Enemy("Chakax/Lizardmen/4/5/0/7/5/2/3/4/8/4/5/Unbreakable, HitFirst")
            {
                Reroll = "ToHit",
            },
        };

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            new Enemy("Green Knight/Bretonnia/6/7/3/6/4/3/6/4/9//4/ImmuneToPsychology, Terror, Undead")
            {
                Mount = new Enemy("Shadow Steed//8/4/0/4/3/1/4/1/5/5"),
            },
            new Enemy("Khuzrak/Beastmen/5/7/1/5/5/3/5/4/9/2"),
            new Enemy("Khalida/Tomb Kings/6/6/3/4/5/3/9/5/10///HitFirst, Undead, PoisonAttack"),
            new Enemy("Louen Leoncoeur/Bretonnia/4/7/5/5/4/3/7/5/9/3/5/Lance, ImmuneToPsychology, Regeneration")
            {
                Reroll = "ToHit;ToWound",
                Mount = new Enemy("Beaquis//8/5/0/5/5/4/6/4/9/5//Terror"),
            },
            new Enemy("Kurt Helborg/The Empire/6/7/3/4/4/3/6/4/9/2//Stubborn, ImmuneToPsychology, AutoWound, NoArmour")
            {
                Mount = new Enemy("8/Warhorse//8/3/0/3/3/1/3/1/5"),
            },
            new Enemy("Greasus Goldtooth/Ogre Kingdoms/4/6/3/10/6/6/1/3/9//4/Fear, ImmuneToPsychology")
            {
                MultiWounds = "D3",
            },
            new Enemy("Zacharias/Vampire Counts/6/6/6/5/5/4/8/5/10//4/Undead")
            {
                Undead = true,
                Mount = new Enemy("Zombie Dragon//6/3/0/6/6/6/1/4/4/5//Terror, Undead"),
            },
            new Enemy("Karl Franz/The Empire/4/6/5/4/4/3/6/4/10/4/4/AutoWound, NoArmour")
            {
                MultiWounds = "D3",
                Mount = new Enemy("Deathclaw//6/6/0/5/5/4/5/4/8///Terror"),
            },
            new Enemy("Tyrion/High Elves/5/9/7/7/3/4/10/4/10/1/4/HitFirst, Regeneration")
            {
                Mount = new Enemy("Malhandir//10/4/0/4/3/1/5/2/7"),
            },
            new Enemy("Torgrim Grudgebearer/Dwarfs/3/7/6/4/5/7/4/4/10/2/4/HitFirst, ImmuneToPsychology, Stubborn")
            {
                Mount = new Enemy("Thronebearers//3/5/3/4/0/1/3/4/0"),
            },
            new Enemy("Orion/Wood Elves/9/8/8/6/5/5/9/5/10//5/HitFirst, Frenzy, Terror, Unbreakable")
            {
                Mount = new Enemy("2/Hound of Orion//9/4/0/4/4/1/4/1/6///Frenzy, Unbreakable")
            },
            new Enemy("Durthu/Wood Elves/5/7/7/6/6/6/2/6/10/3/6/LargeBase, Frenzy, Terror, Hate, Stubborn"),
            new Enemy("Vermin Lord/Skaven/8/8/4/6/5/5/10/5/8//5/ImmuneToPsychology, Terror")
            {
                MultiWounds = "D3",
            },
            new Enemy("Malekith/Dark Elves/8/5/4/6/3/3/8/4/10/4/2/NoArmour")
            {
                Mount = new Enemy("Seraphon//6/6/0/6/6/6/4/5/8/3//Terror, LargeBase")
            },
            new Enemy("Kroq-Gar/Lizardmen/4/6/3/6/5/3/4/5/8/3/5/ColdBlooded")
            {
                MultiWounds = "2",
                Mount = new Enemy("Grymloq//7/3/0/7/5/5/2/5/5/4//Terror, ColdBlooded, LargeBase")
                {
                    MultiWounds = "D3",
                }
            },
            new Enemy("Archaon/Chaos/4/9/5/5/5/4/7/10/10/1/3/ImmuneToPsychology, NoArmour, Terror")
            {
                Mount = new Enemy("Dorghar//8/4/0/5/5/3/3/3/9/4//LargeBase")
            },
            new Enemy("Grimgor Ironhide/Orcs&Goblins/4/8/1/7/5/3/5/7/9/1/5/Hate, HitFirst, ImmuneToPsychology"),
            new Enemy("Ku'gath Plaguefather/Daemons/6/6/3/6/7/7/4/6/9///Terror, PoisonAttack, Hate, LargeBase"),
            new Enemy("Bloodthister/Chaos/6/10/0/7/6/7/10/7/9/4/5/Terror, KillingBlow, LargeBase"),
        };
    }
}
