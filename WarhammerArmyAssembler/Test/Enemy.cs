using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public static int MaxIDindex = -10;

        private static Dictionary<string, List<Enemy>> EnemiesDirectories = new Dictionary<string, List<Enemy>>();

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

        public static Enemy GetByName(string enemyName)
        {
            foreach (List<Enemy> enemyList in EnemiesDirectories.Values)
                foreach (Enemy enemy in enemyList.Where(x => x.Fullname() == enemyName))
                    return enemy.SetID();

            return null;
        }

        private Enemy SetID()
        {
            this.ID = --MaxIDindex;

            return this;
        }

        public static List<string> GetEnemiesGroups() => EnemiesDirectories.Keys.ToList<string>();
            
        public static List<Enemy> GetEnemiesByGroup(string groupName) => EnemiesDirectories[groupName];

        public static int GetEnemiesCount() => GetEnemiesGroups().Sum(x => GetEnemiesByGroup(x).Count());

        public static void CleanEnemies()
        {
            EnemiesDirectories["Lords"] = new List<Enemy>();
            EnemiesDirectories["Heroes"] = new List<Enemy>();
            EnemiesDirectories["Core Units"] = new List<Enemy>();
            EnemiesDirectories["Special Units"] = new List<Enemy>();
            EnemiesDirectories["Rare Units"] = new List<Enemy>();
            EnemiesDirectories["Monsters"] = new List<Enemy>();
        }

        public static void AddEnemies(string type, string enemy) =>
            EnemiesDirectories[type].Add(new Enemy(enemy));
    }
}
