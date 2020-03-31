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
        public static Unit mount;
        public static Unit enemy;

        public static void PrepareUnit(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules(directModification: true).GetUnitMultiplier();

            if (unit.MountOn > 0)
                Test.mount = Army.Units[unit.MountOn].Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
            else
                Test.mount = null;
        }

        public static void PrepareEnemy(string enemyName)
        {
            Test.enemy = Enemy.GetByName(enemyName).Clone().GetOptionRules(directModification: true).GetUnitMultiplier();
        }

        public static void TestFull()
        {
            TestFight.FullTest(unit, mount, enemy);
        }

        public static void TestStatistic()
        {
            TestFight.StatisticTest(unit, mount, enemy);
        }
    }
}
