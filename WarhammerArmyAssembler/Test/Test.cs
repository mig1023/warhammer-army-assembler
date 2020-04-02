using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Test
    {
        public static Unit unit;
        public static Unit unitMount;
        public static Unit enemy;
        public static Unit enemyMount;

        public static void PrepareUnit(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (unit.MountOn > 0)
                Test.unitMount = Army.Units[unit.MountOn].Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
            else
                Test.unitMount = null;
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (enemy.EnemyMount != null)
                Test.enemyMount = enemy.EnemyMount.Clone();
            else
                Test.enemyMount = null;
        }

        public static void TestFull()
        {
            TestFight.FullTest(unit, unitMount, enemy, enemyMount);
        }

        public static void TestStatistic()
        {
            TestFight.StatisticTest(unit, unitMount, enemy, enemyMount);
        }
    }
}
