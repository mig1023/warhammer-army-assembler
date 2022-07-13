using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            Enemy thisEnemy = this;

            foreach (string specialRule in profile[14].Split(','))
                if (!SpecialProperty(specialRule.Trim(), ref thisEnemy))
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

        private bool SpecialProperty(string specialRule, ref Enemy enemy)
        {
            if (!specialRule.Contains(':'))
                return false;

            List<string> rule = specialRule.Split(':').Select(x => x.Trim()).ToList();
            PropertyInfo property = enemy.GetType().GetProperty(rule[0]);

            if (property.PropertyType == typeof(int))
                property.SetValue(enemy, int.Parse(rule[1]));
            else
                property.SetValue(enemy, rule[1]);

            return true;
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

        private static List<Enemy> GetEnemiesListType(string type)
        {
            switch (type)
            {
                case "EnemiesMonsters": return EnemiesMonsters;
                case "EnemiesCoreUnits": return EnemiesCoreUnits;
                case "EnemiesSpecialUnits": return EnemiesSpecialUnits;
                case "EnemiesRareUnits": return EnemiesRareUnits;
                case "EnemiesHeroes": return EnemiesHeroes;
                case "EnemiesLords": return EnemiesLords;
            }

            return null;
        }

        public static void CleanEnemies()
        {
            EnemiesMonsters = new List<Enemy>();
            EnemiesCoreUnits = new List<Enemy>();
            EnemiesSpecialUnits = new List<Enemy>();
            EnemiesRareUnits = new List<Enemy>();
            EnemiesHeroes = new List<Enemy>();
            EnemiesLords = new List<Enemy>();
        }

        public static void AddEnemies(string type, string enemy) =>
            GetEnemiesListType(type).Add(new Enemy(enemy));

        private static List<Enemy> EnemiesMonsters { get; set; }
        private static List<Enemy> EnemiesCoreUnits { get; set; }
        private static List<Enemy> EnemiesSpecialUnits { get; set; }
        private static List<Enemy> EnemiesRareUnits { get; set; }
        private static List<Enemy> EnemiesHeroes { get; set; }

        private static List<Enemy> EnemiesLords = new List<Enemy>
        {
            new Enemy("Green Knight/Bretonnia/6/7/3/6/4/3/6/4/9//4/ImmuneToPsychology, Terror, Undead " +
                "+ Shadow Steed//8/4/0/4/3/1/4/1/5/5"),
            new Enemy("Khuzrak/Beastmen/5/7/1/5/5/3/5/4/9/2"),
            new Enemy("Khalida/Tomb Kings/6/6/3/4/5/3/9/5/10///HitFirst, Undead, PoisonAttack"),
            new Enemy("Louen Leoncoeur/Bretonnia/4/7/5/5/4/3/7/5/9/3/5/Lance, ImmuneToPsychology, Regeneration, Reroll:ToHit;ToWound " +
                "+ Beaquis//8/5/0/5/5/4/6/4/9/5//Terror"),
            new Enemy("Kurt Helborg/The Empire/6/7/3/4/4/3/6/4/9/2//Stubborn, ImmuneToPsychology, AutoWound, NoArmour " +
                "+ Warhorse//8/3/0/3/3/1/3/1/5"),
            new Enemy("Greasus Goldtooth/Ogre Kingdoms/4/6/3/10/6/6/1/3/9//4/Fear, ImmuneToPsychology, MultiWounds:D3"),
            new Enemy("Zacharias/Vampire Counts/6/6/6/5/5/4/8/5/10//4/Undead " +
                "+ Zombie Dragon//6/3/0/6/6/6/1/4/4/5//Terror, Undead"),
            new Enemy("Karl Franz/The Empire/4/6/5/4/4/3/6/4/10/4/4/AutoWound, NoArmour " +
                "+ Deathclaw//6/6/0/5/5/4/5/4/8///Terror, MultiWounds:D3"),
            new Enemy("Tyrion/High Elves/5/9/7/7/3/4/10/4/10/1/4/HitFirst, Regeneration " +
                "+ Malhandir//10/4/0/4/3/1/5/2/7"),
            new Enemy("Torgrim Grudgebearer/Dwarfs/3/7/6/4/5/7/4/4/10/2/4/HitFirst, ImmuneToPsychology, Stubborn " +
                "+ Thronebearers//3/5/3/4/0/1/3/4/0"),
            new Enemy("Orion/Wood Elves/9/8/8/6/5/5/9/5/10//5/HitFirst, Frenzy, Terror, Unbreakable " +
                "+ 2/Hound of Orion//9/4/0/4/4/1/4/1/6///Frenzy, Unbreakable"),
            new Enemy("Durthu/Wood Elves/5/7/7/6/6/6/2/6/10/3/6/LargeBase, Frenzy, Terror, Hate, Stubborn"),
            new Enemy("Vermin Lord/Skaven/8/8/4/6/5/5/10/5/8//5/ImmuneToPsychology, Terror, MultiWounds:D3"),
            new Enemy("Malekith/Dark Elves/8/5/4/6/3/3/8/4/10/4/2/NoArmour " +
                "+ Seraphon//6/6/0/6/6/6/4/5/8/3//Terror, LargeBase"),
            new Enemy("Kroq-Gar/Lizardmen/4/6/3/6/5/3/4/5/8/3/5/ColdBlooded, MultiWounds:2 " +
                "+ Grymloq//7/3/0/7/5/5/2/5/5/4//Terror, ColdBlooded, LargeBase, MultiWounds:D3"),
            new Enemy("Archaon/Chaos/4/9/5/5/5/4/7/10/10/1/3/ImmuneToPsychology, NoArmour, Terror " +
                "+ Dorghar//8/4/0/5/5/3/3/3/9/4//LargeBase"),
            new Enemy("Grimgor Ironhide/Orcs&Goblins/4/8/1/7/5/3/5/7/9/1/5/Hate, HitFirst, ImmuneToPsychology"),
            new Enemy("Ku'gath Plaguefather/Daemons/6/6/3/6/7/7/4/6/9///Terror, PoisonAttack, Hate, LargeBase"),
            new Enemy("Bloodthister/Chaos/6/10/0/7/6/7/10/7/9/4/5/Terror, KillingBlow, LargeBase"),
        };
    }
}
