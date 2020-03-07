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
        private static Unit unitForTest = new Unit(); 

        public static void TestCanvasPrepare(Unit unit)
        {
            unitForTest = unit.Clone().GetOptionRules();

            Interface.main.armyTestUnit.Content = unitForTest.Name;

            List<string> unitParam = new List<string> {
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

            foreach (string name in unitParam)
            {
                PropertyInfo param = typeof(Unit).GetProperty(String.Format("{0}View", name));
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}Test", name));
                testUnitElement.Content = param.GetValue(unitForTest);
            }
        }
    }
}
