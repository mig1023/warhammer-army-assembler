using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Test
    {
        public enum TestTypes { fullTest, statisticTest, battleRoyale }; 

        public static Unit unit = null;
        public static Unit unitMount = null;
        public static Unit enemy = null;
        public static Unit enemyMount = null;

        public static void PrepareUnit(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (unit.MountOn > 0)
                Test.unitMount = Army.Units[unit.MountOn].Clone().GetOptionRules(directModification: true).GetUnitMultiplier(Test.unit.Size);
            else
                Test.unitMount = null;
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (enemy.EnemyMount != null)
                Test.enemyMount = enemy.EnemyMount.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
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
    }
}
