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
            ArmyBook.Load.ArmyUnitOnly(armybook, enemyName, this, ArmyBook.Load.CommonXmlOption(null), size);

        public string Fullname()
        {
            if (this.Size > 1)
                return String.Format("{0} {1} ({2})", this.Size, this.Name, this.Armybook);
            else
                return String.Format("{0} ({1})", this.Name, this.Armybook);
        }

        public static Enemy ByName(string enemyName)
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

        public static List<string> Groups() =>
            EnemiesDirectories.Keys.ToList<string>();
            
        public static List<Enemy> ByGroup(string groupName) =>
            EnemiesDirectories[groupName];

        public static int Count() =>
            Groups().Sum(x => ByGroup(x).Count());

        public static void Clean() =>
            EnemiesDirectories = new Dictionary<string, List<Enemy>>();

        public static void Add(XmlNode xmlEnemy, XmlNode xmlSize, XmlNode xmlType)
        {
            List<string> enemyPath = xmlEnemy.InnerText
                .Split(new string[] { ".xml\\" }, StringSplitOptions.None)
                .ToList();

            string armybook = String.Format("{0}.xml", enemyPath[0]);
            string enemy = enemyPath[1];
            string anotherType = xmlType?.InnerText ?? String.Empty;

            bool sizeExist = int.TryParse(xmlSize?.InnerText, out int size);

            List<string> type = enemy.Split('\\').ToList();
            string enemyType = String.IsNullOrEmpty(anotherType) ? type[0] : anotherType;

            if (!EnemiesDirectories.ContainsKey(enemyType))
                EnemiesDirectories[enemyType] = new List<Enemy>();

            EnemiesDirectories[enemyType].Add(new Enemy(enemy, armybook, sizeExist ? size : 0));
        }
    }
}
