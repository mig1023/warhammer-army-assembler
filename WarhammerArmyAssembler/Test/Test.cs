using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class Test
    {
        public enum TestTypes { fullTest, statisticTest, battleRoyale }; 

        public static Unit unit = null;
        public static Unit unitMount = null;
        public static Unit enemy = null;
        public static Unit enemyMount = null;

        public static List<string> testConsole = new List<string>();

        public static Brush text = Brushes.Black;
        public static Brush supplText = Brushes.Gray;
        public static Brush goodText = Brushes.Green;
        public static Brush badText = Brushes.Red;

        public static Random rand = new Random();

        public static void PrepareUnit(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (unit.MountOn > 0)
                Test.unitMount = Army.Data.Units[unit.MountOn].Clone().GetOptionRules(directModification: true).GetUnitMultiplier(Test.unit.Size);
            else
                Test.unitMount = null;
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (enemy.Mount != null)
                Test.enemyMount = enemy.Mount.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
            else
                Test.enemyMount = null;
        }

        public static void TestByName(TestTypes testType)
        {
            if (testType == TestTypes.battleRoyale)
                TestFight.BattleRoyaleTest(unit, unitMount);
            else if (testType == TestTypes.statisticTest)
                TestFight.StatisticTest(unit, unitMount, enemy, enemyMount);
            else
                TestFight.FullTest(unit, unitMount, enemy, enemyMount);
        }

        public static void Console(Brush color, string line)
        {
            Interface.TestUnit.LineToConsole(line, color);
        }

        public static void Console(Brush color, string line, params object[] p)
        {
            Console(color, String.Format(line, p));
        }
    }
}
