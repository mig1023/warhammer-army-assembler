using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Test
{
    class Data
    {
        public enum TestTypes { fullTest, statisticTest, battleRoyale }; 

        public static Unit unit = null;
        public static Unit unitMount = null;
        public static Unit enemy = null;
        public static Unit enemyMount = null;

        public static List<Interface.Text> testConsole = new List<Interface.Text>();

        public static Brush text = Brushes.Black;
        public static Brush supplText = Brushes.Gray;
        public static Brush goodText = Brushes.Green;
        public static Brush badText = Brushes.Red;

        public static Random rand = new Random();


        public static void PrepareUnit(Unit unit)
        {
            Data.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (unit.MountOn > 0)
            {
                int size = (Army.Data.Units[unit.ArmyID].Chariot > 0 ? Army.Data.Units[unit.ArmyID].Chariot : Data.unit.Size);

                Test.Data.unitMount = Army.Data.Units[unit.MountOn]
                    .Clone()
                    .GetOptionRules(directModification: true)
                    .GetUnitMultiplier(size);
            }
            else
                Test.Data.unitMount = null;
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.Data.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (enemy.Mount != null)
                Test.Data.enemyMount = enemy.Mount.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
            else
                Test.Data.enemyMount = null;
        }

        public static void Console(Brush color, string line) => Interface.TestUnit.LineToConsole(line, color);

        public static void Console(Brush color, string line, params object[] p) => Console(color, String.Format(line, p));
    }
}
