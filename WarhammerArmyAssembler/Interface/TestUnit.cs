using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class TestUnit
    {
        private static bool showLinesToConsole = true;

        private static Dictionary<string, List<FrameworkElement>> mountRow = new Dictionary<string, List<FrameworkElement>>();

        private static List<string> unitParam = new List<string> {
            "Name", "Size", "Movement", "WeaponSkill", "BallisticSkill", "Strength", "Toughness",
            "Wounds", "Initiative", "Attacks", "Leadership", "Armour", "Ward"
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
                Label testUnitElement = (Label)Interface.Changes.main.FindName(String.Format("{0}{1}", name, elemetnsPostfix));
                testUnitElement.Content = param.GetValue(unitForLoad);

                if (mountForLoad != null)
                {
                    PropertyInfo mountParam = typeof(Unit).GetProperty(paramName);
                    var value = mountParam.GetValue(mountForLoad) ?? String.Empty;
                    AddMountUnitParam(value.ToString(), mountIndex, unitGrid);

                    mountIndex += 1;
                }
            }

            Interface.Changes.main.armyUnitTest_Resize();
        }

        public static void startTest(Test.Data.TestTypes testType)
        {
            Interface.Changes.main.armyUnitTest_Resize();
            CleanConsole();
            Test.Data.TestByName(testType);
        }

        public static string GetFullConsoleText() => new TextRange(Interface.Changes.main.testConsole.Document.ContentStart,
            Interface.Changes.main.testConsole.Document.ContentEnd).Text;

        private static void AddMountUnitParam(string param, int gridIndex, Grid unitGrid)
        {
            StackPanel panel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

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

            MainWindow main = Interface.Changes.main;

            List<FrameworkElement> elements = new List<FrameworkElement>
            {
                main.enemyTestUnit,
                main.enemyGridContainer,
                main.specialRulesEnemyTest,
                main.startFullTest,
                main.startStatisticTest,
                main.waitingSpinner
            };

            foreach (FrameworkElement element in elements)
                element.Visibility = System.Windows.Visibility.Visible;

            Interface.Changes.main.startBattleRoyale.Margin = Interface.Changes.Thick(main.startBattleRoyale,
                top: main.startStatisticTest.Margin.Top + 154, left: main.startBattleRoyale.Margin.Left + 163);

            Interface.Changes.main.waitingSpinner.Margin = Interface.Changes.main.testConsole.Margin;

            Interface.Changes.main.waitingSpinner.Margin = Interface.Changes.Thick(Changes.main.testConsole,
                top: Changes.main.testConsole.Margin.Top - Other.SPINNER_TOP_MARGIN,
                left: Changes.main.testConsole.Margin.Left - Other.SPINNER_LEFT_MARGIN);
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            Test.Data.PrepareUnit(unit);

            MainWindow main = Interface.Changes.main;

            List<FrameworkElement> elements = new List<FrameworkElement>
            {
                main.enemyTestUnit,
                main.enemyGridContainer,
                main.specialRulesEnemyTest,
                main.startFullTest,
                main.startStatisticTest,
                main.testConsole
            };

            foreach (FrameworkElement element in elements)
                element.Visibility = Visibility.Hidden;

            main.startBattleRoyale.Visibility = Visibility.Visible;

            main.armyTestUnit.Content = Test.Data.unit.Name;
            LoadUnitParamInInterface(unitForLoad: Test.Data.unit, mountForLoad: Test.Data.unitMount, elemetnsPostfix: "Test", unitGrid: Interface.Changes.main.unitGrid);
            LoadSpecialRules(unitForLoad: Test.Data.unit, target: main.specialRulesTest, onlyUnitRules: true);

            foreach (Label label in new List<Label> { main.startFullTest, main.startStatisticTest, main.startBattleRoyale })
            {
                label.Foreground = ArmyBook.Data.MainColor;
                label.BorderBrush = ArmyBook.Data.MainColor;
            }

            main.enemyGroup.Items.Clear();
            main.enemyForTest.Items.Clear();

            foreach (string enemy in Enemy.GetEnemiesGroups())
                main.enemyGroup.Items.Add(enemy);

            main.armyUnitTest_Resize();
        }

        private static string SelectedEnemy() => (string)Interface.Changes.main.enemyForTest.SelectedItem;

        private static string SelectedGroup() => (string)Interface.Changes.main.enemyGroup.SelectedItem;

        public static void TestEnemyPrepare()
        {
            if (String.IsNullOrEmpty(SelectedEnemy()))
                return;

            Test.Data.PrepareEnemy(SelectedEnemy());

            Interface.Changes.main.enemyTestUnit.Content = Enemy.GetByName(SelectedEnemy()).Name;
            LoadUnitParamInInterface(unitForLoad: Test.Data.enemy, mountForLoad: Test.Data.enemyMount,
                elemetnsPostfix: "Enemy", unitGrid: Interface.Changes.main.enemyGrid);
            LoadSpecialRules(unitForLoad: Test.Data.enemy, target: Interface.Changes.main.specialRulesEnemyTest, onlyUnitRules: true);

            Interface.Changes.main.armyUnitTest_Resize();
        }

        public static void LoadEnemyGroups()
        {
            if (String.IsNullOrEmpty(SelectedGroup()))
                return;

            Interface.Changes.main.enemyForTest.Items.Clear();

            foreach (Enemy enemy in Enemy.GetEnemiesByGroup(SelectedGroup()))
                Interface.Changes.main.enemyForTest.Items.Add(enemy.TestListName);
        }

        public static void CleanConsole() => Interface.Changes.main.testConsole.Document.Blocks.Clear();

        public static void LineToConsole(string line, Brush color = null)
        {
            if (!showLinesToConsole)
                return;

            Test.Data.testConsole.Add(new Text { Content = line, Color = color });
        }

        public static void FromConsoleToOutput(string line, Brush color = null)
        {
            RichTextBox box = Interface.Changes.main.testConsole;

            TextRange text = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            text.Text = line;
            text.ApplyPropertyValue(TextElement.ForegroundProperty, color ?? Brushes.Black);
        }

        public static void PreventConsoleOutput(bool prevent = true) => showLinesToConsole = !prevent;

        public static bool PreventConsoleOutputStatus() => showLinesToConsole;
    }
}
