﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WarhammerArmyAssembler
{
    class Enemy : Unit
    {
        public static int MaxIDindex = -10;

        private static Dictionary<string, List<Enemy>> EnemiesDirectories { get; set; }

        public Enemy(string enemyName, string armybook, int size) =>
            ArmyBook.Load.LoadArmyUnitOnly(armybook, enemyName, this, ArmyBook.Load.LoadCommonXmlOption(null), size);

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

        public static List<string> GetEnemiesGroups() =>
            EnemiesDirectories.Keys.ToList<string>();
            
        public static List<Enemy> GetEnemiesByGroup(string groupName) =>
            EnemiesDirectories[groupName];

        public static int GetEnemiesCount() =>
            GetEnemiesGroups().Sum(x => GetEnemiesByGroup(x).Count());

        public static void CleanEnemies() =>
            EnemiesDirectories = new Dictionary<string, List<Enemy>>();

        public static void Add(XmlNode xmlAmybook, XmlNode xmlEnemy, XmlNode xmlSize, XmlNode xmlType)
        {
            string armybook = xmlAmybook.InnerText;
            string enemy = xmlEnemy.InnerText;
            string anotherType = xmlType?.InnerText ?? String.Empty;

            bool sizeExist = int.TryParse(xmlSize?.InnerText, out int size);

            List<string> type = enemy.Split('/').ToList();
            string enemyType = String.IsNullOrEmpty(anotherType) ? type[0] : anotherType;

            if (!EnemiesDirectories.ContainsKey(enemyType))
                EnemiesDirectories[enemyType] = new List<Enemy>();

            EnemiesDirectories[enemyType].Add(new Enemy(enemy, armybook, sizeExist ? size : 0));
        }
    }
}
