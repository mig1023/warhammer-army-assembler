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
            Test.unit = unit.Clone().GetOptionRules(directModification: true);
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = TestEnemies.GetByName(enemyName).Clone().GetOptionRules(directModification: true);
        }

        public static string TestFull()
        {
            return TestFight.Test(unit.Clone(), enemy.Clone());
        }

        public static string TestStatistic()
        {
            return "test statistic";
        }
    }
}
