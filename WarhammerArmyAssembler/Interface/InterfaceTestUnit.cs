using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarhammerArmyAssembler
{
    class InterfaceTestUnit
    {
        private static List<string> unitParam = new List<string> {
                "Movement",
                "WeaponSkill",
                "BallisticSkill",
                "Strength",
                "Toughness",
                "Wounds",
                "Initiative",
                "Attacks",
                "Leadership",
                "Armour",
                "Ward"
            };

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules();

            Interface.main.armyTestUnit.Content = Test.unit.Name;

            foreach (string name in unitParam)
            {
                PropertyInfo param = typeof(Unit).GetProperty(String.Format("{0}View", name));
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}Test", name));
                testUnitElement.Content = param.GetValue(Test.unit);
            }

            Interface.main.specialRulesTest.Text = String.Format("Special: {0}", Test.unit.GetSpecialRulesLine());
        }

        public static void TestEnemyPrepare(string enemyName)
        {
            Interface.main.enemyTestUnit.Content = enemyName;

            Test.PrepareEnemy(enemyName);

            foreach (string name in unitParam)
            {
                PropertyInfo param = typeof(Unit).GetProperty(String.Format("{0}View", name));
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}Enemy", name));
                testUnitElement.Content = param.GetValue(Test.enemy);
            }
        }
    }
}
