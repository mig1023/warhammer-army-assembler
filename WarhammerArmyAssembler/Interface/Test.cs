using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class Test
    {
        private static bool showLinesToConsole = true;

        private static void LoadUnitParamInInterface(Unit unitForLoad, Unit mountForLoad, Grid unitGrid)
        {
            Unit load = unitForLoad.Clone();

            if ((unitForLoad.MountOn > 0) || (unitForLoad.Mount != null))
            {
                if (unitGrid.RowDefinitions.Count < 3)
                    unitGrid.RowDefinitions.Add(new RowDefinition());

                unitGrid.Height = 99;
            }
            else
            {
                if (unitGrid.RowDefinitions.Count > 2)
                    unitGrid.RowDefinitions.RemoveAt(2);

                unitGrid.Height = 66;
            }

            load.Mount = mountForLoad;
            unitGrid.DataContext = load;

            Changes.armyUnitTest_Resize();
        }

        public static void startTest(WarhammerArmyAssembler.Test.Data.TestTypes testType)
        {
            CleanConsole();
            Changes.armyUnitTest_Resize();

            WarhammerArmyAssembler.Test.Fight.TestByName(testType);
        }

        public static string GetFullConsoleText()
        {
            TextPointer startPos = Changes.main.testConsole.Document.ContentStart;
            TextPointer endPos = Changes.main.testConsole.Document.ContentEnd;
            return new TextRange(startPos, endPos).Text;
        }

        private static void LoadSpecialRules(Unit unitForLoad, TextBlock target, bool onlyUnitRules = false)
        {
            string specialRules = unitForLoad.GetSpecialRulesLine(onlyUnitParam: onlyUnitRules);

            if (!String.IsNullOrEmpty(specialRules))
                target.Text = $"Special: {specialRules}";
            else
                target.Text = String.Empty;
        }

        public static void TestCanvasShow()
        {
            if (String.IsNullOrEmpty(SelectedEnemy()))
                return;

            MainWindow main = Changes.main;

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
                element.Visibility = Visibility.Visible;
        }

        public static void TestCanvasPrepare(Unit unit)
        {
            WarhammerArmyAssembler.Test.Data.PrepareUnit(unit);

            MainWindow main = Changes.main;

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

            main.armyTestUnit.Content = WarhammerArmyAssembler.Test.Data.unit.Name;
            Unit unitLoad = WarhammerArmyAssembler.Test.Data.unit;
            Unit mountLoad = WarhammerArmyAssembler.Test.Data.unitMount;
            LoadUnitParamInInterface(unitLoad, mountLoad, Changes.main.unitGrid);

            LoadSpecialRules(unitForLoad: WarhammerArmyAssembler.Test.Data.unit,
                target: main.specialRulesTest, onlyUnitRules: true);

            List<Label> labels = new List<Label>
            {
                main.startFullTest,
                main.startStatisticTest,
                main.startBattleRoyale
            }; 

            foreach (Label label in labels)
            {
                label.Foreground = ArmyBook.Data.FrontColor;
                label.BorderBrush = ArmyBook.Data.FrontColor;
            }

            main.enemyGroup.Items.Clear();
            main.enemyForTest.Items.Clear();

            foreach (string enemy in Enemy.Groups())
                main.enemyGroup.Items.Add(enemy);

            Changes.armyUnitTest_Resize();
        }

        public static void VisibilityTest(bool before = false)
        {
            if (before)
            {
                Changes.main.waitingSpinner.Visibility = Visibility.Visible;
                Changes.main.currentTest.Visibility = Visibility.Visible;
                Changes.main.testConsole.Visibility = Visibility.Hidden;
            }
            else
            {
                Changes.main.waitingSpinner.Visibility = Visibility.Hidden;
                Changes.main.currentTest.Visibility = Visibility.Hidden;
                Changes.main.testConsole.Visibility = Visibility.Visible;
            }
        }

        private static string SelectedEnemy() =>
            (string)Changes.main.enemyForTest.SelectedItem;

        private static string SelectedGroup() =>
            (string)Changes.main.enemyGroup.SelectedItem;

        public static void TestEnemyPrepare()
        {
            if (String.IsNullOrEmpty(SelectedEnemy()))
                return;

            WarhammerArmyAssembler.Test.Data.PrepareEnemy(SelectedEnemy());

            Changes.main.enemyTestUnit.Content = Enemy.ByName(SelectedEnemy()).Name;

            Unit enemyLoad = WarhammerArmyAssembler.Test.Data.enemy;
            Unit mountLoad = WarhammerArmyAssembler.Test.Data.enemyMount;
            LoadUnitParamInInterface(enemyLoad, mountLoad, Changes.main.enemyGrid);
            LoadSpecialRules(unitForLoad: WarhammerArmyAssembler.Test.Data.enemy, target: Changes.main.specialRulesEnemyTest, onlyUnitRules: true);

            Changes.armyUnitTest_Resize();
        }

        public static void LoadEnemyGroups()
        {
            if (String.IsNullOrEmpty(SelectedGroup()))
                return;

            Changes.main.enemyForTest.Items.Clear();

            foreach (Enemy enemy in Enemy.ByGroup(SelectedGroup()))
                Changes.main.enemyForTest.Items.Add(enemy.Fullname());
        }

        public static void CleanConsole() =>
            Changes.main.testConsole.Document.Blocks.Clear();

        public static void LineToConsole(string line, Brush color = null)
        {
            if (!showLinesToConsole)
                return;

            Text newText = new Text
            {
                Content = line,
                Color = color
            };

            WarhammerArmyAssembler.Test.Data.testConsole.Add(newText);
        }

        public static void FromConsoleToOutput(string line, Brush color = null)
        {
            RichTextBox box = Changes.main.testConsole;

            TextRange text = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            text.Text = line;
            text.ApplyPropertyValue(TextElement.ForegroundProperty, color ?? Brushes.Black);
        }

        public static void PreventConsoleOutput(bool prevent = true) =>
            showLinesToConsole = !prevent;

        public static bool PreventConsoleOutputStatus() =>
            showLinesToConsole;
    }
}
