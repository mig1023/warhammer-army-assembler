using System;
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
            string tabs = Interface.Services.TabsBySize(this.Name.Length, this.Size);

            if (this.Size > 1)
                return $"{this.Size} {this.Name}{tabs}({this.Armybook}) " + this.Name.Length.ToString();
            else
                return $"{this.Name}{tabs}({this.Armybook}) " + this.Name.Length.ToString();
        }

        public static Enemy ByName(string enemyName)
        {
            foreach (List<Enemy> enemiesDirectory in EnemiesDirectories.Values)
            {
                List<Enemy> enemies = enemiesDirectory
                    .Where(x => x.Fullname() == enemyName)
                    .ToList();

                foreach (Enemy enemy in enemies)
                    return enemy.SetID();
            }

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

        public static bool AlreadyLoaded() =>
            EnemiesDirectories != null;

        public static bool CantBeLoaded() =>
         String.IsNullOrEmpty(ArmyBook.Constants.EnemiesOptionPath);

        public static void Add(XmlNode xmlEnemy, XmlNode xmlSize, XmlNode xmlType)
        {
            List<string> enemyPath = xmlEnemy.InnerText
                .Split(new string[] { ".xml\\" }, StringSplitOptions.None)
                .ToList();

            string armybook = $"{enemyPath[0]}.xml";
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
