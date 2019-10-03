using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WarhammerArmyAssembler
{
    class Interface
    {
        public static MainWindow main = null;

        public static ObservableCollection<Unit> ArmyInInterface = new ObservableCollection<Unit>();

        public static object DragSender = null;

        public enum MovingType
        {
            ToMain,
            ToRight,
            ToLeft,
            ToTop,
        }

        private static List<Unit> GetArmyCategories()
        {
            return new List<Unit>
            {
                new Unit() { Name = "Лорды" },
                new Unit() { Name = "Герои" },
                new Unit() { Name = "Основные" },
                new Unit() { Name = "Специальные" },
                new Unit() { Name = "Редкие" },
            };
        }

        public static void LoadArmyList()
        {
            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.Units)
            {
                Unit unit = entry.Value.Clone();
                unit.PointsView = String.Format(" {0} pts", unit.Points);
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
                main.ArmyList.Items.Add(unitType);

            Option artefacts = new Option() { Name = "Артефакты" };

            foreach (KeyValuePair<string, Option> entry in ArmyBook.Artefact)
            {
                Option artefact = entry.Value.Clone();
                artefact.PointsView = String.Format(" {0} pts", artefact.Points);
                artefacts.Items.Add(artefact);
            }

            main.ArmyList.Items.Add(artefacts);
        }

        public static bool EnoughPointsForAddUnit(string id)
        {
            return (ArmyBook.Units[id].Size * ArmyBook.Units[id].Points) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughPointsForAddArtefact(string id)
        {
            return (ArmyBook.Artefact[id].Points) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddArtefact(string artefactID, int unitID)
        {
            int pointsAlreayUsed = 0;

            foreach (Option option in Army.Units[unitID].Option)
                if (option.IsMagicItem())
                    pointsAlreayUsed += option.Points;

            return ((ArmyBook.Artefact[artefactID].Points + pointsAlreayUsed) <= Army.Units[unitID].MagicItems);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            int newPrice = (newSize * Army.Units[id].Points);
            int currentPrice = (Army.Units[id].Size * Army.Units[id].Points);

            return (newPrice - currentPrice) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static double AddOptionsList(Unit unit)
        {
            double topMargin = main.unitName.Margin.Top + main.unitName.Height;

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in main.unitDetail.Children)
                if (element.Name != "closeUnitDetail" && element.Name != "unitName" && element.Name != "unitDescription")
                    elementsForRemoving.Add(element);

            foreach (FrameworkElement element in elementsForRemoving)
                main.unitDetail.Children.Remove(element);

            if (unit.ExistsOptions())
            {
                topMargin += AddLabel("ОПЦИИ", main.unitName.Margin.Left, topMargin, 20);

                foreach (Option option in unit.Option)
                {
                    bool canBeUsed = (
                        !option.OnlyOneInArmy ||
                        (Army.OptionAlreadyUsed(option.ID) == 0) ||
                        (Army.OptionAlreadyUsed(option.ID).ToString() == unit.ID)
                    );

                    if (option.IsOption() && canBeUsed)
                        topMargin += AddButton(option.Name, main.unitName.Margin.Left, topMargin, 40, String.Format("{0}|{1}", unit.ID, option.ID), option);
                }

                topMargin += 25;
            }

            if (unit.ExistsMagicItems())
            {
                topMargin += AddLabel("МАГИЧЕСКИЕ ПРЕДМЕТЫ", main.unitName.Margin.Left, topMargin, 20);

                foreach (Option option in unit.Option)
                    if (option.IsMagicItem() && (option.Points > 0))
                        topMargin += AddButton(option.Name, main.unitName.Margin.Left, topMargin, 40, String.Format("{0}|{1}", unit.ID, option.ID), option);

                topMargin += 25;
            }

            if (unit.ExistsOrdinaryItems())
            {
                topMargin += AddLabel("ОБЫЧНЫЕ ПРЕДМЕТЫ", main.unitName.Margin.Left, topMargin, 20);

                foreach (Option option in unit.Option)
                    if (option.IsMagicItem() && (option.Points == 0))
                        topMargin += AddLabel(option.Name, main.unitName.Margin.Left, topMargin, 20);

                topMargin += 25;
            }

            return topMargin;
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Button).Tag.ToString();

            string[] id = id_tag.Split('|');

            Army.Units[Interface.IntParse(id[0])].AddOption(id[1], Army.Units[Interface.IntParse(id[0])]);
            Interface.ReloadArmyData();

            Interface.Move(Interface.MovingType.ToMain);
        }

        private static double AddLabel(string caption, double left, double top, double height)
        {
            Label newOption = new Label();
            newOption.Content = caption;
            newOption.Margin = Thick(newOption, left, top);
            main.unitDetail.Children.Add(newOption);

            return height;
        }

        private static double AddButton(string caption, double left, double top, double height, string id, Option option)
        {
            AddLabel(caption, left, top, height);

            Button newButton = new Button();

            if (option.IsMagicItem())
                newButton.Content = "отказаться";
            else
                newButton.Content = (option.Realised ? "отменить" : "добавить");

            newButton.Margin = Thick(newButton, left + 2, top + 20);
            newButton.Tag = id;
            newButton.Click += AddOption_Click;
            newButton.Width = 200;
            main.unitDetail.Children.Add(newButton);

            return height;
        }

        public static Thickness Thick(object element, double? left = null, double? top = null, double? right = null, double? bottom = null)
        {
            FrameworkElement control = element as FrameworkElement;

            double newLeft = left ?? control.Margin.Left;
            double newTop = top ?? control.Margin.Top;
            double newRight = right ?? control.Margin.Right;
            double newBottom = bottom ?? control.Margin.Bottom;

            return new Thickness(newLeft, newTop, newRight, newBottom);
        }

        public static void ArmyGridDrop(string id)
        {
            bool slotExists = (Army.GetArmyUnitsNumber(ArmyBook.Units[id].Type) < Army.GetArmyMaxUnitsNumber(ArmyBook.Units[id].Type));
            bool coreUnit = (ArmyBook.Units[id].Type == Unit.UnitType.Core);

            int allHeroes = Army.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (ArmyBook.Units[id].Type == Unit.UnitType.Hero) && (allHeroes >= Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if ((!slotExists && !coreUnit) || lordInHeroSlot)
            {
                string unitType = (ArmyBook.Units[id].IsHero() ? "героев" : "отрядов");
                Error(String.Format("Количество {0} данного типа исчерпано", unitType));
            }
            else if (!EnoughPointsForAddUnit(id))
            {
                string unitType = (ArmyBook.Units[id].IsHero() ? "героя" : "отряда");
                Error(String.Format("Недостаточно очков для добавления {0}", unitType));
            }
            else
            {
                Army.AddUnitByID(id);
                ReloadArmyData();
            }
        }
        
        public static void UnitDeleteDrop(string id)
        {
            
            Army.DeleteUnitByID(id);
            ReloadArmyData();
        }

        public static int IntParse(string line)
        {
            int value = 0;

            bool success = Int32.TryParse(line, out value);

            return (success ? value : 0);
        }

        public static void ReloadArmyData()
        {
            ArmyInInterface.Clear();

            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                Unit unit = entry.Value.Clone();

                unit = unit.GetOptionRules();

                unit.InterfaceRules = unit.GetSpecialRulesLine();
                unit.InterfacePoints = unit.GetUnitPoints();
                unit.ID = entry.Key.ToString();

                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                if (unitType.Items.Count <= 0)
                    continue;

                foreach (Unit unit in unitType.Items)
                    ArmyInInterface.Add(unit);
            }

            main.ArmyGrid.ItemsSource = ArmyInInterface;
            main.armyHeroes.Content = String.Format("Героев: {0}/{1} из {2}/{3}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyUnitsNumber(Unit.UnitType.Hero),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            main.armyUnits.Content = String.Format("Отрядов: {0}/{1}/{2} из {3}+/{4}/{5}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyUnitsNumber(Unit.UnitType.Rare),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            main.armyPoints.Content = String.Format("Очков: {0} из {1}", Army.GetArmyPoints(), Army.GetArmyMaxPoints());
            main.armySize.Content = String.Format("Моделей: {0}", Army.GetArmySize());
            main.armyCasting.Content = String.Format("Каст: {0}", Army.GetArmyCast());
            main.armyDispell.Content = String.Format("Диспелл: {0}", Army.GetArmyDispell());
        }

        public static void Error(string text)
        {
            main.errorText.Content = text;

            Move(MovingType.ToTop);
        }

        private static void HideAllDetails()
        {
            main.errorDetail.Visibility = Visibility.Hidden;
            main.unitDetail.Visibility = Visibility.Hidden;
            main.armybookDetail.Visibility = Visibility.Hidden;
        }

        public static void Move(MovingType moveTo)
        {
            Thickness newPosition = new Thickness(0, 0, 0, 0);

            if (moveTo == MovingType.ToLeft)
            {
                newPosition = new Thickness(250, 0, 0, 0);
                HideAllDetails();
                main.armybookDetail.Visibility = Visibility.Visible;
            }
                
            if (moveTo == MovingType.ToRight)
            {
                newPosition = new Thickness(-250, 0, 0, 0);
                HideAllDetails();
                main.unitDetail.Visibility = Visibility.Visible;
            }

            if (moveTo == MovingType.ToTop)
            {
                newPosition = new Thickness(0, 50, 0, 0);
                HideAllDetails();
                main.errorDetail.Visibility = Visibility.Visible;
            }

            ThicknessAnimation move = new ThicknessAnimation();
            move.Duration = TimeSpan.FromSeconds(0.2);
            move.From = main.mainGrid.Margin;
            move.To = newPosition;
            main.mainGrid.BeginAnimation(FrameworkElement.MarginProperty, move);
        }
    }
}
