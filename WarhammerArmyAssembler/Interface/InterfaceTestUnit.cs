﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private static void LoadSpecialRules(Unit unitForLoad, TextBlock target)
        {
            string specialRules = unitForLoad.GetSpecialRulesLine();

            if (!String.IsNullOrEmpty(specialRules))
                target.Text = String.Format("Special: {0}", specialRules);
            else
                target.Text = String.Empty;
        }

        public static void TestCanvasShow()
        {
            foreach (FrameworkElement element in new List<FrameworkElement> {
                Interface.main.enemyTestUnit,
                Interface.main.enemyGridContainer,
                Interface.main.specialRulesEnemyTest
            })
                element.Visibility = System.Windows.Visibility.Visible;
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.unit = unit.Clone().GetOptionRules();

            Interface.main.armyTestUnit.Content = Test.unit.Name;
            LoadUnitParamInInterface(unitForLoad: Test.unit, elemetnsPostfix: "Test");
            LoadSpecialRules(unitForLoad: Test.unit, target: Interface.main.specialRulesTest);
        }

        public static void TestEnemyPrepare(string enemyName)
        {
            Test.PrepareEnemy(enemyName);

            Interface.main.enemyTestUnit.Content = enemyName;
            LoadUnitParamInInterface(unitForLoad: Test.enemy, elemetnsPostfix: "Enemy");
            LoadSpecialRules(unitForLoad: Test.enemy, target: Interface.main.specialRulesEnemyTest);
        }
    }
}
