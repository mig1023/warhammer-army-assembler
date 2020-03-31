using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class InterfaceTestUnit
    {
        private static bool showLinesToConsole = true;

        private static List<FrameworkElement> mountRow = new List<FrameworkElement>();

        private static List<string> unitParam = new List<string> {
                "Name",
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

        private static void LoadUnitParamInInterface(Unit unitForLoad, string elemetnsPostfix, bool withMount = false)
        {
            if (Interface.main.unitGrid.RowDefinitions.Count > 2)
            {
                Interface.main.unitGrid.RowDefinitions.RemoveAt(2);

                foreach (FrameworkElement f in mountRow)
                {
                    Panel p = (f as Panel);
                    p.Children.Clear();
                    Interface.main.unitGrid.Children.Remove(p);
                }

                mountRow.Clear();
            }

            if (withMount && (unitForLoad.MountOn > 0))
            {
                if (Interface.main.unitGrid.RowDefinitions.Count < 3)
                    Interface.main.unitGrid.RowDefinitions.Add(new RowDefinition());

                Interface.main.unitGrid.Height = 99;
            }
            else if (withMount)
                Interface.main.unitGrid.Height = 66;

            int mountIndex = 0;

            foreach (string name in unitParam)
            {
                PropertyInfo param = typeof(Unit).GetProperty(name == "Size" || name == "Name" ? name : String.Format("{0}View", name));
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}{1}", name, elemetnsPostfix));
                testUnitElement.Content = param.GetValue(unitForLoad);

                if (withMount && (unitForLoad.MountOn > 0))
                {
                    PropertyInfo mountParam = typeof(Unit).GetProperty(name);
                    var value = mountParam.GetValue(Army.Units[unitForLoad.MountOn]) ?? String.Empty;
                    AddMountUnitParam(value.ToString(), mountIndex++);
                }
            }

            Interface.main.armyUnitTest_Resize();
        }

        private static void AddMountUnitParam(string param, int gridIndex)
        {
            StackPanel panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            Interface.main.unitGrid.Children.Add(panel);
            Grid.SetRow(panel, 2);
            Grid.SetColumn(panel, gridIndex);

            mountRow.Add(panel);

            Label label = new Label();
            label.FontSize = 16;
            label.Content = param;
            panel.Children.Add(label);
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
            if (String.IsNullOrEmpty(SelectedEnemy()))
                return;

            foreach (FrameworkElement element in new List<FrameworkElement> {
                Interface.main.enemyTestUnit,
                Interface.main.enemyGridContainer,
                Interface.main.specialRulesEnemyTest,
                Interface.main.startFullTest,
                Interface.main.startStatisticTest
            })
                element.Visibility = System.Windows.Visibility.Visible;
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.PrepareUnit(unit);

            foreach (FrameworkElement f in new List<FrameworkElement> {
                Interface.main.enemyTestUnit,
                Interface.main.enemyGridContainer,
                Interface.main.specialRulesEnemyTest,
                Interface.main.startFullTest,
                Interface.main.startStatisticTest,
                Interface.main.testConsole
            })
                f.Visibility = Visibility.Hidden;

            Interface.main.armyTestUnit.Content = Test.unit.Name;
            LoadUnitParamInInterface(unitForLoad: Test.unit, elemetnsPostfix: "Test", withMount: true);
            LoadSpecialRules(unitForLoad: Test.unit, target: Interface.main.specialRulesTest);

            foreach (Label label in new List<Label> { Interface.main.startFullTest, Interface.main.startStatisticTest })
            {
                label.Foreground = ArmyBook.MainColor;
                label.BorderBrush = ArmyBook.MainColor;
            }

            Interface.main.enemyGroup.Items.Clear();
            Interface.main.enemyForTest.Items.Clear();

            foreach (string enemy in Enemy.GetEnemiesGroups())
                Interface.main.enemyGroup.Items.Add(enemy);

            Interface.main.armyUnitTest_Resize();
        }

        private static string SelectedEnemy()
        {
            return (string)Interface.main.enemyForTest.SelectedItem;
        }

        private static string SelectedGroup()
        {
            return (string)Interface.main.enemyGroup.SelectedItem;
        }

        public static void TestEnemyPrepare()
        {
            if (String.IsNullOrEmpty(SelectedEnemy()))
                return;

            Test.PrepareEnemy(SelectedEnemy());

            Interface.main.enemyTestUnit.Content = Enemy.GetByName(SelectedEnemy()).Name;
            LoadUnitParamInInterface(unitForLoad: Test.enemy, elemetnsPostfix: "Enemy");
            LoadSpecialRules(unitForLoad: Test.enemy, target: Interface.main.specialRulesEnemyTest);

            Interface.main.armyUnitTest_Resize();
        }

        public static void LoadEnemyGroups()
        {
            if (String.IsNullOrEmpty(SelectedGroup()))
                return;

            Interface.main.enemyForTest.Items.Clear();

            foreach (Enemy enemy in Enemy.GetEnemiesByGroup(SelectedGroup()))
                Interface.main.enemyForTest.Items.Add(enemy.TestListName);
        }

        public static void CleanConsole()
        {
            Interface.main.testConsole.Document.Blocks.Clear();
        }

        public static void LineToConsole(string line, Brush color = null, bool bold = false)
        {
            if (!showLinesToConsole)
                return;

            TextRange tr = new TextRange(Interface.main.testConsole.Document.ContentEnd, Interface.main.testConsole.Document.ContentEnd);
            tr.Text = line;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color ?? Brushes.Black);
        }

        public static void PreventConsoleOutput(bool prevent = true)
        {
            showLinesToConsole = !prevent;
        }
    }
}
