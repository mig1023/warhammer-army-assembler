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
        public static Unit enemy;

        public static void PrepareUnit(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
        }

        public static void TestFull()
        {
            TestFight.FullTest(unit.Clone(), enemy.Clone());
        }

        public static void TestStatistic()
        {
            TestFight.StatisticTest(unit.Clone(), enemy.Clone());
        }
    }
}
