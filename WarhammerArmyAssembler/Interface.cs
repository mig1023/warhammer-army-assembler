﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace WarhammerArmyAssembler
{
    class Interface
    {
        public static MainWindow main = null;

        public static ObservableCollection<Unit> ArmyInInterface = new ObservableCollection<Unit>();

        public static object DragSender = null;

        public static string CurrentSelectedArmy = null;
        public static int? CurrentSelectedUnit = null;

        public enum MovingType { ToMain, ToRight, ToLeft, ToTop }

        private static List<Unit> GetArmyCategories()
        {
            return new List<Unit>
            {
                new Unit() { Name = "Lords" },
                new Unit() { Name = "Heroes" },
                new Unit() { Name = "Core" },
                new Unit() { Name = "Special" },
                new Unit() { Name = "Rare" },
            };
        }

        public static void LoadArmyList()
        {
            main.ArmyList.Items.Clear();

            AllUnitDelete();

            main.armyMainLabel.Content = Army.ArmyName;
            main.armyMainLabel.Foreground = Brushes.White;
            main.armyMainLabel.Background = ArmyBook.MainColor;
            main.armyMainLabelPlace.Background = ArmyBook.MainColor;
            main.unitDetailHead.Background = ArmyBook.MainColor;

            main.armyMainMenu.Content = '\u2630';
            main.armyMainMenu.Foreground = Brushes.White;
            main.armyMainMenu.Background = ArmyBook.AdditionalColor;

            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in ArmyBook.Units)
            {
                Unit unit = entry.Value.Clone();
                unit.PointsView = String.Format(" {0} pts", unit.Points);
                unit.InterfaceColor = ArmyBook.MainColor;
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                unitType.GroopBold = true;
                main.ArmyList.Items.Add(unitType);
            }

            List<string> artefactsTypes = new List<string>();

            foreach (KeyValuePair<int, Option> entry in ArmyBook.Artefact)
                if (!artefactsTypes.Contains(entry.Value.ArtefactGroup))
                    artefactsTypes.Add(entry.Value.ArtefactGroup);

            foreach (string artefactType in artefactsTypes)
            {
                Option artefacts = new Option() { Name = artefactType };

                foreach (KeyValuePair<int, Option> entry in ArmyBook.Artefact)
                    if (entry.Value.ArtefactGroup == artefactType)
                    {
                        Option artefact = entry.Value.Clone();
                        artefact.PointsView = String.Format(" {0} pts", artefact.Points);
                        artefact.InterfaceColor = ArmyBook.MainColor;
                        artefacts.Items.Add(artefact);
                    }

                artefacts.GroopBold = true;
                artefacts.Artefacts = true;
                main.ArmyList.Items.Add(artefacts);
            }
        }

        public static void SetArmyGridAltColor(Brush color)
        {
            main.ArmyGrid.AlternatingRowBackground = color;
        }

        public static bool EnoughPointsForAddUnit(int id)
        {
            return (ArmyBook.Units[id].Size * ArmyBook.Units[id].Points) <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddOption(int points)
        {
            return points <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughPointsForAddArtefact(int id)
        {
            return ArmyBook.Artefact[id].Points <= (Army.GetArmyMaxPoints() - Army.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, int unitID)
        {
            int pointsAlreayUsed = 0;

            foreach (Option option in Army.Units[unitID].Options)
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

        private static double[] CheckColumn(double[] margins, bool header = false, bool newColumn = false)
        {
            double detailHeight = main.unitDetail.ActualHeight;
            detailHeight = (detailHeight > 0 ? detailHeight : 250);

            if (newColumn || (margins[1] + (header ? 90 : 60) > detailHeight))
            {
                margins[0] += 155;
                margins[1] = (header ? 50 : 40) + (newColumn ? 0 : 45);
            }

            return margins;
        }

        private static double[] CreateColumn(string head, double[] margins, int unitID, Unit unit, ref bool notFirstColumn)
        {
            if (notFirstColumn)
                margins[1] += 10;

            margins = CheckColumn(margins, header: true, newColumn: notFirstColumn);

            margins[1] += AddLabel(head, margins, 25, bold: true);

            margins[1] += 10;

            int mountAlreadyOn = 0;

            if (unit.MountOn > 0)
                mountAlreadyOn = Army.GetMountOption(unit);

            if (head == "SPECIAL RULES")
            {
                foreach (string rule in unit.GetSpecialRules())
                {
                    margins = CheckColumn(margins);

                    margins[1] += AddLabel((rule == "FC" ? "FULL COMMAND" : rule), margins, 15);

                    margins[1] += 5;
                }

                notFirstColumn = true;
            }
            else
            {
                foreach (Option option in unit.Options)
                {
                    if (head == "OPTION" || head == "COMMAND" || head == "MAGIC ITAMS")
                    {
                        if (head == "OPTION")
                        {
                            int alredyUsedBy = (option.OnlyOneInArmy ? Army.OptionAlreadyUsed(option.Name) : 0);
                            bool canBeUsed = (!option.OnlyOneInArmy || (alredyUsedBy == 0) || (alredyUsedBy == unitID));

                            if (!option.IsOption() || option.FullCommand && !canBeUsed)
                                continue;
                        }

                        if (head == "COMMAND" && !option.FullCommand)
                            continue;

                        if (head == "MAGIC ITAMS" && (!option.IsMagicItem() || (option.Points <= 0)))
                            continue;

                        margins = CheckColumn(margins);

                        margins[1] += AddButton(option.Name, margins, 25, String.Format("{0}|{1}", unitID, option.ID),
                            option, mountAlreadyOn: mountAlreadyOn, unit: unit);

                        margins[1] += 20;
                    }
                    else
                    {
                        if (head == "WEAPONS & ARMOUR" && (!option.IsMagicItem() || (option.Points != 0) || String.IsNullOrEmpty(option.Name)))
                            continue;

                        margins = CheckColumn(margins);

                        margins[1] += AddLabel(option.Name, margins, 15);

                        margins[1] += 5;
                    }

                    notFirstColumn = true;
                }
            }

            return margins;
        }

        public static void AddOptionsList(int unitID, Unit unit)
        {
            double[] margins = new double[] { main.unitName.Margin.Left, main.unitName.Margin.Top + 35 };

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in main.unitDetail.Children)
                if (element.Name != "closeUnitDetail" && element.Name != "unitName")
                    elementsForRemoving.Add(element);

            foreach (FrameworkElement element in elementsForRemoving)
                main.unitDetail.Children.Remove(element);

            bool notFirstColumn = false;

            if (unit.Mage > 0)
            {
                double left = main.unitName.Margin.Left + main.unitName.ActualWidth + 5;
                AddLabel(String.Format("Mage Level {0}", unit.GetUnitMage()), margins, 25);
            }
                
            if (unit.ExistsOptions())
                margins = CreateColumn("OPTION", margins, unitID, unit, ref notFirstColumn);

            if (unit.ExistsCommand())
                margins = CreateColumn("COMMAND", margins, unitID, unit, ref notFirstColumn);

            if (unit.ExistsMagicItems())
                margins = CreateColumn("MAGIC ITAMS", margins, unitID, unit, ref notFirstColumn);

            if (unit.ExistsOrdinaryItems())
                margins = CreateColumn("WEAPONS & ARMOUR", margins, unitID, unit, ref notFirstColumn);

            if (unit.GetSpecialRules().Count > 0)
                margins = CreateColumn("SPECIAL RULES", margins, unitID, unit, ref notFirstColumn);

            main.unitDetail.Width = margins[0] + 200;

            if (main.unitDetail.Width > main.unitDetailScroll.Width)
                main.unitDetailScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Button).Tag.ToString();

            string[] id = id_tag.Split('|');

            int optionID = IntParse(id[1]);
            int unitID = IntParse(id[0]);

            Army.Units[unitID].AddOption(optionID, Army.Units[unitID], unitID);
            
            ReloadArmyData();
            SetArtefactAlreadyUsed(IntParse(id[1]), false);
            UpdateUnitDescription(unitID, Army.Units[unitID]);
        }

        public static void UpdateUnitDescription(int unitID, Unit unit)
        {
            main.unitName.Content = unit.Name.ToUpper();

            main.unitName.Foreground = Brushes.White;
            main.unitName.Background = ArmyBook.MainColor;
            main.unitName.FontWeight = FontWeights.Bold;

            main.UpdateLayout();

            AddOptionsList(unitID, unit);
        }

        private static double AddLabel(string caption, double[] margins, double height,
            bool selected = false, int points = 0, bool perModel = false, bool bold = false)
        {
            Label newOption = new Label();
            newOption.Content = caption;
            newOption.Margin = Thick(newOption, margins[0], margins[1]);

            if (selected)
                newOption.Foreground = ArmyBook.AdditionalColor;

            if (selected || bold)
                newOption.FontWeight = FontWeights.Bold;

            if (bold)
            {
                newOption.Foreground = Brushes.White;
                newOption.Background = ArmyBook.MainColor;
            }

            main.unitDetail.Children.Add(newOption);

            main.UpdateLayout();

            if (points > 0)
            {
                Label optionPoints = new Label();
                optionPoints.Content = points.ToString() + " pts" + (perModel ? "/m" : String.Empty);
                optionPoints.Margin = Thick(optionPoints, margins[0] + newOption.ActualWidth - 5, margins[1]);
                optionPoints.Foreground = ArmyBook.MainColor;
                main.unitDetail.Children.Add(optionPoints);
            }

            return height;
        }

        private static double AddButton(string caption, double[] margins, double height, string id,
            Option option, int mountAlreadyOn = 0, Unit unit = null)
        {
            AddLabel(caption, margins, height, (option.Realised ? true : false), option.Points, option.PerModel);

            Button newButton = new Button();

            if (option.IsMagicItem())
                newButton.Content = "drop";
            else
                newButton.Content = (option.Realised ? "drop" : "add");

            if (option.Mount && (mountAlreadyOn > 0) && (option.ID != mountAlreadyOn))
                newButton.IsEnabled = false;

            if ((option.OnlyFor == Option.OnlyForType.Mount) && (mountAlreadyOn == 0))
                newButton.IsEnabled = false;

            if ((option.OnlyFor == Option.OnlyForType.Infantry) && (mountAlreadyOn > 0))
                newButton.IsEnabled = false;

            if (option.IsSlannOption() && !option.Realised && (unit != null) && unit.IsMaxSlannOption())
                newButton.IsEnabled = false;

            if (
                    (unit != null)
                    && (
                        !unit.IsAnotherOptionRealised(option.OnlyIfAnotherService, defaultResult: true)
                        ||
                        unit.IsAnotherOptionRealised(option.OnlyIfNotAnotherService, defaultResult: false)
                    )
                )
                newButton.IsEnabled = false;

            newButton.Margin = Thick(newButton, margins[0] + 2, margins[1] + 20);
            newButton.Tag = id;
            newButton.Click += AddOption_Click;
            newButton.Width = 135;
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

        public static void ArmyGridDrop(int id, DataGridRow container = null, int points = 0, int unit = 0)
        {
            if (ArmyBook.Artefact.ContainsKey(id))
                ArmyGridDropArtefact(id, container);
            else if (ArmyBook.Mounts.ContainsKey(id))
                ArmyGridDropMount(id, points, unit);
            else
                ArmyGridDropUnit(id);
        }

        public static void ArmyGridDropArtefact(int id, DataGridRow container)
        {
            Unit unit = container.DataContext as Unit;

            if (unit != null)
                ArmyGridDropArtefact(id, unit.ID);
        }

        public static void ArmyGridDropArtefact(int id, int unitID)
        {
            if (!EnoughPointsForAddArtefact(id))
                Error("Not enough points add an item");
            else if (!EnoughUnitPointsForAddArtefact(id, unitID))
                Error("Not enough magic item points to add an item");
            else if (!Army.IsArmyUnitsPointsPercentOk(Army.Units[unitID].Type, ArmyBook.Artefact[id].Points))
                Error("For this type, a point cost limit has been reached");
            else
            {
                Army.Units[unitID].AddAmmunition(id);
                ReloadArmyData();
                UpdateUnitDescription(unitID, Army.Units[unitID]);

                if (!ArmyBook.Artefact[id].Multiple)
                    SetArtefactAlreadyUsed(id, true);
            }
        }

        public static void ArmyGridDropUnit(int id)
        {
            bool slotExists = (Army.GetArmyUnitsNumber(ArmyBook.Units[id].Type) < Army.GetArmyMaxUnitsNumber(ArmyBook.Units[id].Type));
            bool coreUnit = (ArmyBook.Units[id].Type == Unit.UnitType.Core);

            int allHeroes = Army.GetArmyUnitsNumber(Unit.UnitType.Lord) + Army.GetArmyUnitsNumber(Unit.UnitType.Hero);
            bool lordInHeroSlot = (ArmyBook.Units[id].Type == Unit.UnitType.Hero) && (allHeroes >= Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero));

            if ((!slotExists && !coreUnit) || lordInHeroSlot)
            {
                string unitType = (ArmyBook.Units[id].IsHero() ? "heroes" : "units");
                Error(String.Format("The number of {0} of this type has been exhausted.", unitType));
            }
            else if (!EnoughPointsForAddUnit(id))
            {
                string unitType = (ArmyBook.Units[id].IsHero() ? "hero" : "unit");
                Error(String.Format("Not enough points to add a {0}", unitType));
            }
            else if (!Army.IsArmyUnitsPointsPercentOk(ArmyBook.Units[id].Type, ArmyBook.Units[id].Points))
                Error(String.Format("The {0} has reached a point cost limit", Army.UnitTypeName(ArmyBook.Units[id].Type)));
            else if(!Army.IsArmyDublicationOk(ArmyBook.Units[id]))
                Error(String.Format("Army can't include as many duplicates of {0}", Army.UnitTypeName(ArmyBook.Units[id].Type)));
            else
            {
                Army.AddUnitByID(id);
                ReloadArmyData();
            }
        }

        public static void ArmyGridDropMount(int id, int points, int unit)
        {
            if (!EnoughUnitPointsForAddOption(points))
                Error("Not enough points to add a mount");
            else if (Army.Units[unit].MountOn > 0)
                Error("The hero already has a mount");
            else if (!Army.IsArmyUnitsPointsPercentOk(Army.Units[unit].Type, points))
                Error(String.Format("The {0} has reached a point cost limit", Army.UnitTypeName(Army.Units[unit].Type)));
            else
            {
                Army.AddMountByID(id, unit);
                ReloadArmyData();
            }
        }

        public static void UnitDeleteDrop(int id)
        {
            if (CurrentSelectedUnit == id)
                DetailResize(open: false);

            Army.DeleteUnitByID(id);
            ReloadArmyData();
        }

        public static void AllUnitDelete()
        {
            Army.DeleteAllUnits();
            ReloadArmyData();
        }

        public static int IntParse(string line)
        {
            int value = 0;

            bool success = Int32.TryParse(line, out value);

            return (success ? value : 0);
        }

        public static Unit ReloadArmyUnit(int id, Unit unit)
        {
            Unit newUnit = unit.Clone();

            newUnit = newUnit.GetOptionRules();

            newUnit.RulesView = newUnit.GetSpecialRulesLine();
            newUnit.PointsView = newUnit.GetUnitPoints().ToString();
            newUnit.ID = id;

            return newUnit;
        }

        public static void ReloadArmyData()
        {
            ArmyInInterface.Clear();

            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                if (entry.Value.Type != Unit.UnitType.Mount)
                    categories[(int)entry.Value.Type].Items.Add(ReloadArmyUnit(entry.Key, entry.Value));

                if (
                    (entry.Value.MountOn > 0)
                    &&
                    ((Army.Units[entry.Value.MountOn].Wounds > 1) || (Army.Units[entry.Value.MountOn].Wounds == 0))
                    )
                    categories[(int)entry.Value.Type].Items.Add(
                        ReloadArmyUnit(entry.Value.MountOn, Army.Units[entry.Value.MountOn])
                    );
            }

            foreach (Unit unitType in categories)
            {
                if (unitType.Items.Count <= 0)
                    continue;

                foreach (Unit unit in unitType.Items)
                    ArmyInInterface.Add(unit);
            }

            main.ArmyGrid.ItemsSource = ArmyInInterface;
            main.armyHeroes.Content = String.Format("Heroes: {0}/{1} out of {2}/{3}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyUnitsNumber(Unit.UnitType.Hero),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            main.armyUnits.Content = String.Format("Units: {0}/{1}/{2} out of {3}+/{4}/{5}",
                Army.GetArmyUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyUnitsNumber(Unit.UnitType.Rare),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                Army.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            main.armyPoints.Content = String.Format("Points: {0} out of {1}", Army.GetArmyPoints(), Army.GetArmyMaxPoints());
            main.armySize.Content = String.Format("Models: {0}", Army.GetArmySize());
            main.armyCasting.Content = String.Format("Cast: {0}", Army.GetArmyCast());
            main.armyDispell.Content = String.Format("Dispell: {0}", Army.GetArmyDispell());
        }

        public static void Error(string text)
        {
            main.errorText.Content = text;

            Move(MovingType.ToTop, err: true);
        }

        private static void HideAllDetails()
        {
            main.armybookDetailScroll.Visibility = Visibility.Hidden;
            main.menuArmybookScroll.Visibility = Visibility.Hidden;
        }

        public static void DetailResize(bool open)
        {
            if (open)
            {
                main.unitDetailScroll.Visibility = Visibility.Visible;
                main.mainGrid.RowDefinitions[2].Height = new GridLength(250);
                main.unitDetailScrollSlitter.IsEnabled = true;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(5);
                main.unitDetailScrollSlitter.Height = 5;
            }
            else
            {
                main.unitDetailScroll.Visibility = Visibility.Hidden;
                main.mainGrid.RowDefinitions[2].Height = new GridLength(0);
                main.unitDetailScrollSlitter.IsEnabled = false;
                main.mainGrid.RowDefinitions[1].Height = new GridLength(0);
                main.unitDetailScrollSlitter.Height = 0;
            }
        }

        public static void Move(MovingType moveTo, ScrollViewer toOpen = null, bool err = false)
        {
            Thickness newPosition = new Thickness(0, 0, 0, 0);

            if (!err)
                HideAllDetails();

            if (moveTo == MovingType.ToLeft)
                newPosition = new Thickness(320, 0, 0, 0);
                
            if (moveTo == MovingType.ToRight)
                newPosition = new Thickness(-320, 0, 0, 0);

            if (moveTo == MovingType.ToTop)
                newPosition = new Thickness(0, 50, 0, 0);

            if (toOpen != null)
                toOpen.Visibility = Visibility.Visible;

            ThicknessAnimation move = new ThicknessAnimation();
            move.Duration = TimeSpan.FromSeconds(0.2);
            move.From = (err ? main.mainPlaceCanvas.Margin : main.mainGrid.Margin);
            move.To = newPosition;

            if (err)
                main.mainPlaceCanvas.BeginAnimation(FrameworkElement.MarginProperty, move);
            else
                main.mainGrid.BeginAnimation(FrameworkElement.MarginProperty, move);
        }

        public static void SetArtefactAlreadyUsed(int id, bool value)
        {
            foreach (Object group in main.ArmyList.Items)
            {
                if (group is Option)
                    foreach (Object item in (group as Option).Items)
                    {
                        Option artefact = item as Option;

                        if (artefact.ID == id)
                        {
                            artefact.ArtefactAlreadyUsed = value;
                            ArmyBook.Artefact[artefact.ID].ArtefactAlreadyUsed = value;
                        }
                    }
            }
        }

        public static Brush BrushFromXml(XmlNode path)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(path.InnerText);
        }

        private static void PreviewLoadCurrentSelectedArmy(string armyName)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(armyName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookImage");
            main.imageArmybook.Source = new BitmapImage(new Uri(Path.GetDirectoryName(armyName) + "\\" + armyFile.InnerText));

            main.listArmybookVer.Content = "edition " + xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion").InnerText;

            main.UpdateLayout();

            main.listArmybookVer.Background = BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));
        }

        public static void PreviewArmyList(bool next = false, bool prev = false)
        {
            string currentFile = ArmyBook.GetXmlArmyBooks(next, prev);

            PreviewLoadCurrentSelectedArmy(currentFile);
            CurrentSelectedArmy = currentFile;
        }

        public static void LoadArmySize(int points)
        {
            Army.MaxPoints = points;
            ArmyBook.LoadArmy(CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            DetailResize(open: false);
            Move(MovingType.ToMain);
        }
    }
}
