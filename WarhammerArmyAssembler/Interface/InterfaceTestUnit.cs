using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class InterfaceTestUnit
    {
        private static Unit unitForTest = new Unit(); 

        public static void TestCanvasPrepare(Unit unit)
        {
            unitForTest = unit.Clone().GetOptionRules();

            Interface.main.armyTestUnit.Content = unitForTest.Name;

            Interface.main.TestSize.Content = unitForTest.Size;
            Interface.main.TestM.Content = unitForTest.MovementView;
            Interface.main.TestWS.Content = unitForTest.WeaponSkillView;
            Interface.main.TestBS.Content = unitForTest.BallisticSkillView;
            Interface.main.TestS.Content = unitForTest.StrengthView;
            Interface.main.TestT.Content = unitForTest.ToughnessView;
            Interface.main.TestW.Content = unitForTest.WoundsView;
            Interface.main.TestI.Content = unitForTest.InitiativeView;
            Interface.main.TestA.Content = unitForTest.AttacksView;
            Interface.main.TestLD.Content = unitForTest.LeadershipView;
            Interface.main.TestAS.Content = unitForTest.ArmourView;
            Interface.main.TestWard.Content = unitForTest.WardView;
        }
    }
}
