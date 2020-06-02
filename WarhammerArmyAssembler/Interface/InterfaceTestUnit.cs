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

        private static Dictionary<string, List<FrameworkElement>> mountRow = new Dictionary<string, List<FrameworkElement>>();

        private static List<string> unitParam = new List<string> {
                "Name", "Size", "Movement", "WeaponSkill", "BallisticSkill", "Strength",
                "Toughness", "Wounds", "Initiative", "Attacks", "Leadership", "Armour", "Ward"
            };

        private static void LoadUnitParamInInterface(Unit unitForLoad, Unit mountForLoad, string elemetnsPostfix, Grid unitGrid)
        {
            if (unitGrid.RowDefinitions.Count > 2)
            {
                unitGrid.RowDefinitions.RemoveAt(2);

                if (mountRow.ContainsKey(unitGrid.Name))
                {
                    foreach (FrameworkElement f in mountRow[unitGrid.Name])
                    {
                        Panel p = (f as Panel);
                        p.Children.Clear();
                        unitGrid.Children.Remove(p);
                    }

                    mountRow[unitGrid.Name].Clear();
                }
            }

            if ((unitForLoad.MountOn > 0) || (unitForLoad.Mount != null))
            {
                if (unitGrid.RowDefinitions.Count < 3)
                    unitGrid.RowDefinitions.Add(new RowDefinition());

                unitGrid.Height = 99;
            }
            else
                unitGrid.Height = 66;

            int mountIndex = 0;

            foreach (string name in unitParam)
            {
                string paramName = (name == "Size" || name == "Name" ? name : String.Format("{0}View", name));

                PropertyInfo param = typeof(Unit).GetProperty(paramName);
                Label testUnitElement = (Label)Interface.main.FindName(String.Format("{0}{1}", name, elemetnsPostfix));
                testUnitElement.Content = param.GetValue(unitForLoad);

                if (mountForLoad != null)
                {
                    PropertyInfo mountParam = typeof(Unit).GetProperty(paramName);
                    var value = mountParam.GetValue(mountForLoad) ?? String.Empty;
                    AddMountUnitParam(value.ToString(), mountIndex, unitGrid);

                    mountIndex += 1;
                }
            }

            Interface.main.armyUnitTest_Resize();
        }

        public static void startTest(Test.TestTypes testType)
        {
            Interface.main.armyUnitTest_Resize();
            CleanConsole();
            Test.TestByName(testType);
            Interface.main.testConsole.Visibility = Visibility.Visible;
        }

        public static string GetFullConsoleText()
        {
            TextRange text = new TextRange(
                Interface.main.testConsole.Document.ContentStart,
                Interface.main.testConsole.Document.ContentEnd
            );

            return text.Text;
        }

        private static void AddMountUnitParam(string param, int gridIndex, Grid unitGrid)
        {
            StackPanel panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            unitGrid.Children.Add(panel);
            Grid.SetRow(panel, 2);
            Grid.SetColumn(panel, gridIndex);

            if (!mountRow.ContainsKey(unitGrid.Name))
                mountRow.Add(unitGrid.Name, new List<FrameworkElement>());

            mountRow[unitGrid.Name].Add(panel);

            Label label = new Label
            {
                FontSize = 16,
                Content = param
            };
            panel.Children.Add(label);
        }

        private static void LoadSpecialRules(Unit unitForLoad, TextBlock target, bool onlyUnitRules = false)
        {
            string specialRules = unitForLoad.GetSpecialRulesLine(onlyUnitParam: onlyUnitRules);

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
                Interface.main.startStatisticTest,
            })
                element.Visibility = System.Windows.Visibility.Visible;

            Interface.main.startBattleRoyale.Margin = Interface.Thick(
                Interface.main.startBattleRoyale,
                top: Interface.main.startStatisticTest.Margin.Top + 154,
                left: Interface.main.startBattleRoyale.Margin.Left + 163
            ); 
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.PrepareUnit(unit);

            foreach (FrameworkElement element in new List<FrameworkElement> {
                Interface.main.enemyTestUnit,
                Interface.main.enemyGridContainer,
                Interface.main.specialRulesEnemyTest,
                Interface.main.startFullTest,
                Interface.main.startStatisticTest,
                Interface.main.testConsole
            })
                element.Visibility = Visibility.Hidden;

            Interface.main.startBattleRoyale.Visibility = Visibility.Visible;

            Interface.main.armyTestUnit.Content = Test.unit.Name;
            LoadUnitParamInInterface(unitForLoad: Test.unit, mountForLoad: Test.unitMount, elemetnsPostfix: "Test", unitGrid: Interface.main.unitGrid);
            LoadSpecialRules(unitForLoad: Test.unit, target: Interface.main.specialRulesTest, onlyUnitRules: true);

            foreach (Label label in new List<Label> {
                Interface.main.startFullTest,
                Interface.main.startStatisticTest,
                Interface.main.startBattleRoyale,
            }) {
                label.Foreground = ArmyBook.Data.MainColor;
                label.BorderBrush = ArmyBook.Data.MainColor;
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
            LoadUnitParamInInterface(unitForLoad: Test.enemy, mountForLoad: Test.enemyMount, elemetnsPostfix: "Enemy", unitGrid: Interface.main.enemyGrid);
            LoadSpecialRules(unitForLoad: Test.enemy, target: Interface.main.specialRulesEnemyTest, onlyUnitRules: true);

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

        public static void LineToConsole(string line, Brush color = null)
        {
            if (!showLinesToConsole)
                return;

            TextRange tr = new TextRange(Interface.main.testConsole.Document.ContentEnd, Interface.main.testConsole.Document.ContentEnd)
            {
                Text = line
            };
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color ?? Brushes.Black);
        }

        public static void PreventConsoleOutput(bool prevent = true)
        {
            showLinesToConsole = !prevent;
        }

        public static bool PreventConsoleOutputStatus()
        {
            return showLinesToConsole;
        }
    }
}
