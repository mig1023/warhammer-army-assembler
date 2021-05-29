using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WarhammerArmyAssembler.Interface
{
    class UnitDetails
    {
        private static double GetDetailHeight()
        {
            double detailHeight = Changes.main.unitDetail.ActualHeight;
            return (detailHeight > 0 ? detailHeight : 250);
        }

        private static double[] CheckColumn(double[] margins, ref double lastColumnMaxWidth,
            bool header = false, bool newColumn = false, bool sizeCollapse = false)
        {
            if (newColumn || sizeCollapse || (margins[1] + (header ? 90 : 60) > GetDetailHeight()))
            {
                margins[0] += (lastColumnMaxWidth > 175 ? lastColumnMaxWidth + 10 : 175);
                margins[1] = (header ? 50 : 40) + (newColumn ? 0 : 45);
                lastColumnMaxWidth = 0;
            }

            return margins;
        }

        private static bool NotEnoughColumnForThis(string caption, double height, double[] margins)
        {
            string[] captionLines = Other.WordSplit(caption);

            if (captionLines.Length < 2)
                return false;

            return margins[1] + 25 + (height * captionLines.Length) > GetDetailHeight();
        }

        private static string GetMagicPointsString(int unitID, string head)
        {
            Unit unit = Army.Data.Units[unitID];

            if (head == "MAGIC ITEMS")
                return String.Format("{0} / {1}", unit.MagicPointsAlreadyUsed(), unit.GetUnitMagicPoints());

            if ((head == Army.Params.MagicPowersName()) && (unit.MagicPowersCount > 0))
                return String.Format("{0} / {1}", unit.MagicPowersCountAlreadyUsed(), unit.GetMagicPowersCount());

            else if (head == Army.Params.MagicPowersName())
                return String.Format("{0} / {1}", unit.MagicPowersPointsAlreadyUsed(), unit.GetUnitMagicPowersPoints());

            return String.Empty;
        }

        private static double[] CreateColumn(string head, double[] margins, int unitID, Unit unit,
            ref bool notFirstColumn, ref double lastColumnMaxWidth)
        {
            if (notFirstColumn)
                margins[1] += 10;

            margins = CheckColumn(margins, ref lastColumnMaxWidth, header: true, newColumn: notFirstColumn);
            margins[1] += AddLabel(head, margins, 25, ref lastColumnMaxWidth, bold: true, addLine: GetMagicPointsString(unitID, head), fixPadding: 10);

            int mountAlreadyOn = (unit.MountOn > 0 ? unit.GetMountOption() : 0);

            Option.OnlyForType mountTypeAlreadyFixed = unit.GetMountTypeAlreadyFixed();

            if (head == "SPECIAL RULES")
            {
                foreach (string rule in unit.GetSpecialRules())
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth, sizeCollapse: NotEnoughColumnForThis(rule, 15, margins));
                    margins[1] += AddLabel((rule == "FC" ? "Full command" : rule), margins, 15, ref lastColumnMaxWidth, fixPadding: 5);
                }

                notFirstColumn = true;
            }
            else
            {
                if ((!unit.ExistsMagicItems() && (head == "MAGIC ITEMS")) || (!unit.ExistsMagicPowers() && (head == Army.Params.MagicPowersName())))
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth);
                    margins[1] += AddLabel("empty yet", margins, 15, ref lastColumnMaxWidth, fixPadding: 5);

                    notFirstColumn = true;
                }
                else
                    foreach (Option option in unit.Options)
                    {
                        if (head == "OPTION" || head == "COMMAND" || head == "MAGIC ITEMS" || head == Army.Params.MagicPowersName())
                        {
                            if (head == "OPTION" && (!option.IsOption() || option.FullCommand))
                                continue;

                            if (head == "COMMAND" && !option.FullCommand)
                                continue;

                            if (head == "MAGIC ITEMS" && (!option.IsMagicItem() || ((option.Points <= 0) && !option.Honours)))
                                continue;

                            if (head == Army.Params.MagicPowersName() && !option.IsPowers())
                                continue;

                            margins = CheckColumn(margins, ref lastColumnMaxWidth);

                            bool canBeUsed = true;

                            if (option.OnlyOneInArmy || option.OnlyOneForSuchUnits)
                                canBeUsed = (Army.Checks.IsOptionAlreadyUsed(option.Name, unitID, unit.Name, option.OnlyOneForSuchUnits) == 0);

                            margins[1] += AddButton(option.FullName(), margins, 25, ref lastColumnMaxWidth,
                                String.Format("{0}|{1}", unitID, option.ID), option, mountAlreadyOn: mountAlreadyOn,
                                mountTypeAlreadyFixed: mountTypeAlreadyFixed, unit: unit, mustBeEnabled: canBeUsed);

                            margins[1] += 20;
                        }
                        else
                        {
                            bool thisIsStandartEquipment = !option.IsMagicItem() || (option.Points != 0) ||
                                option.Honours || String.IsNullOrEmpty(option.Name);
                            bool thisIsSpecialRuleOrMount = option.Realised && !option.Mount &&
                                !option.FullCommand && option.SpecialRuleDescription.Length == 0;

                            if (head == "WEAPONS & ARMOUR" && thisIsStandartEquipment && !thisIsSpecialRuleOrMount)
                                continue;

                            if (option.NativeArmour && unit.IsArmourOptionAdded())
                                continue;

                            margins = CheckColumn(margins, ref lastColumnMaxWidth);
                            margins[1] += AddLabel(option.Name, margins, 15, ref lastColumnMaxWidth, fixPadding: 5);
                        }

                        notFirstColumn = true;
                    }
            }

            return margins;
        }

        public static void AddOptionsList(int unitID, Unit unit)
        {
            double[] margins = new double[] { Changes.main.unitName.Margin.Left, Changes.main.unitName.Margin.Top + 35 };

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in Changes.main.unitDetail.Children)
                if (element.Name != "closeUnitDetail" && element.Name != "unitName")
                    elementsForRemoving.Add(element);

            foreach (FrameworkElement element in elementsForRemoving)
                Changes.main.unitDetail.Children.Remove(element);

            bool notFirstColumn = false;

            double lastColumnMaxWidth = 0;

            int wizard = unit.GetUnitWizard();

            if (wizard > 0)
            {
                double[] wizardMargins = new double[]
                {
                    Changes.main.unitName.Margin.Left + Changes.main.unitName.ActualWidth + 5,
                    Changes.main.unitName.Margin.Top
                };

                AddLabel(caption: String.Format("Wizard Level {0}", wizard), margins: wizardMargins, height: 25,
                    lastColumnMaxWidth: ref lastColumnMaxWidth);
            }
                
            if (unit.ExistsOptions())
                margins = CreateColumn("OPTION", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsCommand())
                margins = CreateColumn("COMMAND", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsMagicItems() || (Army.Data.Units[unitID].GetUnitMagicPoints() > 0))
                margins = CreateColumn("MAGIC ITEMS", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if ((unit.GetUnitMagicPowersPoints() > 0) || (unit.GetMagicPowersCount() > 0))
                margins = CreateColumn(Army.Params.MagicPowersName(), margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsOrdinaryItems())
                margins = CreateColumn("WEAPONS & ARMOUR", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.GetSpecialRules().Count > 0)
                margins = CreateColumn("SPECIAL RULES", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            Changes.main.unitDetail.Width = margins[0] + lastColumnMaxWidth + 25;

            if (Changes.main.unitDetail.Width > Changes.main.unitDetailScroll.Width)
                Changes.main.unitDetailScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Label).Tag.ToString();

            string[] id = id_tag.Split('|');

            int optionID = Other.IntParse(id[1]);
            int unitID = Other.IntParse(id[0]);

            Army.Data.Units[unitID].AddOption(optionID);
            Army.Data.Units[unitID].ThrowAwayIncompatibleOption();

            Reload.ReloadArmyData();
            Mod.SetArtefactAlreadyUsed(Other.IntParse(id[1]), false);
            UpdateUnitDescription(unitID, Army.Data.Units[unitID]);
        }

        private static void CountableOption_Click(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;

            string[] id = label.Tag.ToString().Split('|');

            int optionID = Other.IntParse(id[1]);
            int unitID = Other.IntParse(id[0]);

            Army.Data.Units[unitID].ChangeCountableOption(optionID, direction: label.Content.ToString());

            Reload.ReloadArmyData();
            UpdateUnitDescription(unitID, Army.Data.Units[unitID]);
        }

        public static void UpdateUnitDescription(int unitID, Unit unit)
        {
            Changes.main.unitName.Content = unit.Name.ToUpper();

            Changes.main.unitName.Foreground = Brushes.White;
            Changes.main.unitName.Background = ArmyBook.Data.MainColor;
            Changes.main.unitName.FontWeight = FontWeights.Bold;

            Changes.main.UpdateLayout();

            AddOptionsList(unitID, unit);
        }

        private static double AddLabel(string caption, double[] margins, double height, ref double lastColumnMaxWidth,
            bool selected = false, double points = 0, bool perModel = false, bool bold = false, string addLine = "",
            int fixPadding = 0, bool enabled = true)
        {
            Label newOption = new Label();

            string[] captionLines = Other.WordSplit(caption);

            newOption.Content = String.Empty;

            foreach (string line in captionLines)
                newOption.Content += (String.IsNullOrEmpty(newOption.Content.ToString()) ? String.Empty : Environment.NewLine + "   ") + line;

            newOption.Margin = Changes.Thick(newOption, margins[0], margins[1]);

            if (!enabled)
                newOption.Foreground = Brushes.Gray;
            else if (selected)
                newOption.Foreground = ArmyBook.Data.AdditionalColor;

            if (selected || bold)
                newOption.FontWeight = FontWeights.Bold;

            if (bold)
            {
                newOption.Foreground = Brushes.White;
                newOption.Background = ArmyBook.Data.MainColor;
            }

            Changes.main.unitDetail.Children.Add(newOption);
            Changes.main.UpdateLayout();

            double actualWidth = newOption.ActualWidth;

            if (points > 0 || points < 0 || !String.IsNullOrEmpty(addLine))
            {
                double leftPadding = (points > 0 ? -5 : 5);

                bool pointsNeed = (points > 0) || (points < 0);

                Label optionPoints = new Label
                {
                    Content = (pointsNeed ? points.ToString() + " pts" + (perModel ? "/m" : String.Empty) : addLine)
                };
                optionPoints.Margin = Changes.Thick(optionPoints, margins[0] + newOption.ActualWidth + leftPadding, margins[1]);

                if (!enabled)
                    optionPoints.Foreground = Brushes.Gray;
                else
                    optionPoints.Foreground = ArmyBook.Data.MainColor;

                Changes.main.unitDetail.Children.Add(optionPoints);
                Changes.main.UpdateLayout();

                actualWidth += optionPoints.ActualWidth - 5;
            }

            if (captionLines.Length > 1)
            {
                Line longOptionLine = new Line
                {
                    X1 = newOption.Margin.Left + 8,
                    Y1 = newOption.Margin.Top + height + 8
                };
                longOptionLine.X2 = longOptionLine.X1;
                longOptionLine.Y2 = longOptionLine.Y1 + height * (captionLines.Length - 1);

                longOptionLine.StrokeThickness = 2;
                longOptionLine.Stroke = ArmyBook.Data.MainColor;

                Changes.main.unitDetail.Children.Add(longOptionLine);
            }

            if (actualWidth > lastColumnMaxWidth)
                lastColumnMaxWidth = actualWidth;

            double bottomPadding = (captionLines.Length > 1 ? 5 : 0);

            return (height * captionLines.Length) + bottomPadding + fixPadding;
        }

        private static double AddButtonPart(string caption, double[] margins, double actualPrevPartWidth,
            string id, Brush background, double? partWidth = null, bool enabled = true, bool countable = false)
        {
            Label newPart = new Label
            {
                Tag = id,
                Content = caption,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                Background = background,
            };

            newPart.Margin = Changes.Thick(newPart, margins[0] + 2 + actualPrevPartWidth, margins[1] + 20);
            newPart.Width = partWidth ?? (countable ? 51 : 77);

            if (countable && enabled && (caption == "+" || caption == "-"))
                newPart.MouseDown += CountableOption_Click;
            else if (!countable && enabled)
                newPart.MouseDown += AddOption_Click;

            Changes.main.unitDetail.Children.Add(newPart);
            Changes.main.UpdateLayout();

            return newPart.ActualWidth;
        }

        private static void AddButtonAllParts(string captionFirst, string captionSecond, Brush backgroundFirst, 
            Brush backgroundSecond, double[] margins, string id,  double? partWidth = null, bool enabled = true)
        {
            double actualWidth = AddButtonPart(captionFirst, margins, 0, id, backgroundFirst, enabled: enabled);
            AddButtonPart(captionSecond, margins, actualWidth, id, backgroundSecond, enabled: enabled);
        }

        private static void AddButtonsCountable(string caption, Brush backFirst, Brush backSecond,
            Option option, Unit unit, double[] margins, string id, bool enabled = true)
        {
            int maxByDependency = 0;

            if (!String.IsNullOrEmpty(option.Countable.Dependency))
            {
                PropertyInfo unitParam = typeof(Unit).GetProperty(option.Countable.Dependency);
                maxByDependency = (int)((int)unitParam.GetValue(unit) / option.Countable.Ratio);
            }

            bool canBeReduced = ((option.Countable.Value > 0) && (option.Countable.Value > option.Countable.Min)) && enabled;

            double left = AddButtonPart("-", margins, 0, id, (canBeReduced ? backFirst : Brushes.Gainsboro), enabled: canBeReduced, countable: true);

            left += AddButtonPart(option.Countable.Value.ToString(), margins, left, id, backSecond, enabled: enabled, countable: true);

            bool canByIncreasedByDependency = ((maxByDependency == 0) || (option.Countable.Value < maxByDependency));
            bool canBeIncreasedByMaxParam = ((option.Countable.Max == 0) || (option.Countable.Value < option.Countable.Max));
            bool canBeIncreased = canByIncreasedByDependency && canBeIncreasedByMaxParam && enabled;

            AddButtonPart("+", margins, left, id, (canBeIncreased ? backFirst : Brushes.Gainsboro), enabled: canBeIncreased, countable: true);
        }

        private static double AddButton(string caption, double[] margins, double height, ref double lastColumnMaxWidth,
            string id, Option option, int mountAlreadyOn = 0, Option.OnlyForType mountTypeAlreadyFixed = Option.OnlyForType.All,
            Unit unit = null, bool mustBeEnabled = true)
        {
            bool optionIsEnabled = unit.IsOptionEnabled(option, mountAlreadyOn, mountTypeAlreadyFixed);

            if (!mustBeEnabled)
                optionIsEnabled = false;

            if ((unit != null) && unit.IsAnotherOptionIsIncompatible(option))
                optionIsEnabled = false;

            AddLabel(caption, margins, height, ref lastColumnMaxWidth, option.Realised, option.Points,
                option.PerModel, enabled: optionIsEnabled);

            if (option.IsMagicItem() || option.IsPowers())
            {
                AddButtonPart("drop " + (option.IsPowers() ? "power" : "artefact"), margins, 0, id, ArmyBook.Data.MainColor, 154);
                return height;
            }

            if (option.Countable != null)
                AddButtonsCountable(caption: option.Countable.Value.ToString(), backFirst: ArmyBook.Data.AdditionalColor,
                    backSecond: ArmyBook.Data.MainColor, option: option, unit: unit, margins: margins, id: id, enabled: optionIsEnabled);

            else if (!optionIsEnabled)
                AddButtonAllParts(captionFirst: String.Empty, captionSecond: String.Empty, backgroundFirst: Brushes.WhiteSmoke,
                    backgroundSecond: Brushes.Gainsboro, margins: margins, id: id, enabled: false);

            else if (option.Realised)
                AddButtonAllParts(captionFirst: "drop", captionSecond: String.Empty, backgroundFirst: ArmyBook.Data.AdditionalColor,
                    backgroundSecond: ArmyBook.Data.MainColor, margins: margins, id: id);

            else
                AddButtonAllParts(captionFirst: String.Empty, captionSecond: "add", backgroundFirst: Brushes.LightGray,
                    backgroundSecond: Brushes.Silver, margins: margins, id: id);

            return height;
        }

    }
}
