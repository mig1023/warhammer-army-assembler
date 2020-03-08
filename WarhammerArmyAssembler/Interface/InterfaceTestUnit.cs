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
                "Size",
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

        private static void LoadUnitParamInInterface(Unit unitForLoad, string elemetnsPostfix)
        {
            foreach (string name in unitParam)
            {
                PropertyInfo param = typeof(Unit).GetProperty(name == "Size" ? name : String.Format("{0}View", name));
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}{1}", name, elemetnsPostfix));
                testUnitElement.Content = param.GetValue(unitForLoad);
            }
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules();

            Interface.main.armyTestUnit.Content = Test.unit.Name;
            LoadUnitParamInInterface(unitForLoad: Test.unit, elemetnsPostfix: "Test");
            Interface.main.specialRulesTest.Text = String.Format("Special: {0}", Test.unit.GetSpecialRulesLine());
        }

        public static void TestEnemyPrepare(string enemyName)
        {
            Interface.main.enemyTestUnit.Content = enemyName;
            Test.PrepareEnemy(enemyName);
            LoadUnitParamInInterface(unitForLoad: Test.enemy, elemetnsPostfix: "Enemy");
        }
    }
}
