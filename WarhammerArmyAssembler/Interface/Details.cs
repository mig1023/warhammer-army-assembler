using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WarhammerArmyAssembler.Interface
{
    class Details
    {
        private static double GetDetailHeight() =>
            (Changes.main.unitDetail.ActualHeight > 0 ? Changes.main.unitDetail.ActualHeight : 250);

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
            string[] captionLines = Services.WordSplit(caption);

            if (captionLines.Length < 2)
                return false;

            return margins[1] + 25 + (height * captionLines.Length) > GetDetailHeight();
        }

        private static string GetMagicPointsString(int unitID, string head)
        {
            Unit unit = Army.Data.Units[unitID];

            if (head == ArmyBook.Data.MagicItemsStyle)
                return $"{unit.MagicPointsAlreadyUsed()} / {unit.GetUnitMagicPoints()}";

            if ((head == ArmyBook.Data.MagicPowersStyle) && (unit.MagicPowersCount > 0))
                return $"{unit.MagicPowersCountAlreadyUsed()} / {unit.GetMagicPowersCount()}";

            if (head == ArmyBook.Data.MagicPowersStyle)
                return $"{unit.MagicPowersPointsAlreadyUsed()} / {unit.GetUnitMagicPowersPoints()}";

            return String.Empty;
        }

        private static double[] CreateColumn(string head, double[] margins, int unitID, Unit unit,
            ref bool notFirstColumn, ref double lastColumnMaxWidth)
        {
            if (notFirstColumn)
                margins[1] += 10;

            margins = CheckColumn(margins, ref lastColumnMaxWidth, header: true, newColumn: notFirstColumn);
            margins[1] += AddLabel(head, margins, 25, ref lastColumnMaxWidth, bold: true, addLine:
                GetMagicPointsString(unitID, head), fixPadding: 10);

            int mountAlreadyOn = (unit.MountOn > 0 ? unit.GetMountOption() : 0);

            string mountTypeAlreadyFixed = unit.GetMountTypeAlreadyFixed();

            if (head == "SPECIAL RULES")
            {
                foreach (string rule in unit.GetSpecialRules())
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth,
                        sizeCollapse: NotEnoughColumnForThis(rule, 15, margins));

                    string caption = rule == "FC" ? "Full command" : rule;

                    margins[1] += AddLabel(caption, margins, 15, ref lastColumnMaxWidth, fixPadding: 5);
                }

                notFirstColumn = true;
            }
            else
            {
                bool magicItemsPointsExists = unit.Options.Where(x => x.MagicItemsPoints).Count() > 0;
                bool headIsPowers = head == ArmyBook.Data.MagicPowersStyle;
                bool headIsMagic = head == ArmyBook.Data.MagicItemsStyle;
                bool magicPowersDontExists = !unit.ExistsMagicPowers() && headIsPowers;

                if ((!unit.ExistsMagicItems() && headIsMagic && !magicItemsPointsExists) || magicPowersDontExists)
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth);
                    margins[1] += AddLabel("empty yet", margins, 15, ref lastColumnMaxWidth, fixPadding: 5);

                    notFirstColumn = true;
                }
                else
                    foreach (Option option in unit.Options)
                    {
                        if (head == "OPTION" || head == "COMMAND" || headIsMagic || headIsPowers)
                        {
                            if (head == "OPTION" && (!option.IsOption() || option.Command || option.MagicItemsPoints))
                                continue;

                            if (head == "COMMAND" && !option.Command)
                                continue;

                            bool noMagicOrHonours = !option.IsMagicItem() || ((option.Points <= 0) && !option.Honours);

                            if (headIsMagic && !option.MagicItemsPoints && noMagicOrHonours)
                                continue;

                            if (headIsPowers && !option.IsPowers())
                                continue;

                            margins = CheckColumn(margins, ref lastColumnMaxWidth);

                            bool canBeUsed = true;

                            if (option.OnlyOneInArmy || option.OnlyOneSuchUnits)
                            {
                                int alreadyUsed = Army.Checks.OptionsAlreadyUsed(option.Name,
                                    unitID, unit.Name, option.OnlyOneSuchUnits);

                                canBeUsed = alreadyUsed == 0;
                            }

                            margins[1] += AddButton(option.FullName(), margins, 25, ref lastColumnMaxWidth,
                                $"{unitID}|{option.ID}", option, mountAlreadyOn: mountAlreadyOn,
                                mountTypeAlreadyFixed: mountTypeAlreadyFixed, unit: unit, mustBeEnabled: canBeUsed);

                            margins[1] += 20;
                        }
                        else
                        {
                            bool thisIsStandartEquip = !option.IsMagicItem() || (option.Points != 0) ||
                                option.Honours || String.IsNullOrEmpty(option.Name);

                            bool thisIsRuleOrMount = option.Realised && !option.Mount &&
                                !option.Command && option.SpecialRuleDescription.Length == 0;

                            bool thisIsSpecRule = option.IsSpecaialRule();
                            bool thisIsEqOrSpec = thisIsStandartEquip || thisIsSpecRule;

                            if (head == "WEAPONS & ARMOUR" && thisIsEqOrSpec && !thisIsRuleOrMount)
                                continue;

                            if (option.NativeArmour && unit.IsArmourOptionAdded())
                                continue;

                            if (option.OnlyRuleOption)
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
            double[] margins = new double[]
            {
                Changes.main.unitName.Margin.Left,
                Changes.main.unitName.Margin.Top + 35
            };

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in Changes.main.unitDetail.Children)
            {
                if (element.Name != "closeUnitDetail" && element.Name != "unitName")
                    elementsForRemoving.Add(element);
            }

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

                AddLabel(caption: $"Wizard Level {wizard}", margins: wizardMargins, height: 25,
                    lastColumnMaxWidth: ref lastColumnMaxWidth);
            }

            if (!unit.Character)
            {
                CreatePersonificationField(lastColumnMaxWidth, unitID);
            }

            if (unit.ExistsOptions())
            {
                margins = CreateColumn("OPTION", margins, unitID, unit,
                    ref notFirstColumn, ref lastColumnMaxWidth);
            }

            if (unit.ExistsCommand())
            {
                margins = CreateColumn("COMMAND", margins, unitID, unit,
                    ref notFirstColumn, ref lastColumnMaxWidth);
            }

            if (unit.ExistsMagicItems() || (Army.Data.Units[unitID].GetUnitMagicPoints() > 0))
            {
                margins = CreateColumn(ArmyBook.Data.MagicItemsStyle,
                    margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);
            }

            if ((unit.GetUnitMagicPowersPoints() > 0) || (unit.GetMagicPowersCount() > 0))
            {
                margins = CreateColumn(ArmyBook.Data.MagicPowersStyle,
                    margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);
            }

            if (unit.ExistsEquipmentsItems())
            {
                margins = CreateColumn("WEAPONS & ARMOUR", margins, unitID, unit,
                    ref notFirstColumn, ref lastColumnMaxWidth);
            }

            if (unit.GetSpecialRules().Count > 0)
            {
                margins = CreateColumn("SPECIAL RULES", margins, unitID, unit,
                    ref notFirstColumn, ref lastColumnMaxWidth);
            }

            Changes.main.unitDetail.Width = margins[0] + lastColumnMaxWidth + 25;

            if (Changes.main.unitDetail.Width > Changes.main.unitDetailScroll.Width)
            {
                Changes.main.unitDetailScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        }

        private static void CreatePersonificationField(double lastColumnMaxWidth, int unitID)
        {
            double personificationWidth = Changes.main.unitName.Margin.Left +
                Changes.main.unitName.ActualWidth + 5;

            if (lastColumnMaxWidth > 0)
                personificationWidth += lastColumnMaxWidth;

            Label newOption = new Label
            {
                Content = "Personification",
                Foreground = ArmyBook.Data.FrontColor,
            };

            newOption.Margin = Changes.Thick(newOption,
                personificationWidth, Changes.main.unitName.Margin.Top);

            Changes.main.unitDetail.Children.Add(newOption);
            Changes.main.UpdateLayout();

            TextBox personificationName = new TextBox
            {
                Width = 150,
                Height = 26,
                Padding = new Thickness(4),
                BorderBrush = ArmyBook.Data.FrontColor,
            };

            if (String.IsNullOrEmpty(Army.Data.Units[unitID].Personification))
            {
                personificationName.Visibility = Visibility.Hidden;
            }
            else
            {
                personificationName.Text = Army.Data.Units[unitID].Personification;
            }

            personificationName.Margin = Changes.Thick(personificationName,
                personificationWidth, Changes.main.unitName.Margin.Top);

            personificationName.KeyUp += (sender, e) =>
            {
                Army.Data.Units[unitID].Personification = (sender as TextBox).Text;

                Reload.ReloadArmyData();
                Changes.main.UpdateLayout();
            };
            Changes.main.unitDetail.Children.Add(personificationName);

            newOption.MouseDown += (sender, e) =>
            {
                personificationName.Visibility = Visibility.Visible;

                Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => { personificationName.Focus(); }),
                    System.Windows.Threading.DispatcherPriority.Render);
            };
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Label).Tag.ToString();

            string[] id = id_tag.Split('|');

            int optionID = Services.IntParse(id[1]);
            int unitID = Services.IntParse(id[0]);

            Army.Data.Units[unitID].AddOption(optionID);
            Army.Data.Units[unitID].ThrowAwayIncompatibleOption();

            Reload.ReloadArmyData();
            Mod.SetArtefactAlreadyUsed(Services.IntParse(id[1]), false);
            UpdateUnitDescription(unitID, Army.Data.Units[unitID]);
        }

        private static void CountableOption_Click(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;

            string[] id = label.Tag.ToString().Split('|');

            int optionID = Services.IntParse(id[1]);
            int unitID = Services.IntParse(id[0]);

            Army.Data.Units[unitID].ChangeCountableOption(optionID, direction: label.Content.ToString());

            Reload.ReloadArmyData();
            UpdateUnitDescription(unitID, Army.Data.Units[unitID]);
        }

        public static void UpdateUnitDescription(int unitID, Unit unit)
        {
            Changes.main.unitName.Content = unit.Name.ToUpper();

            Changes.main.unitName.Foreground = Brushes.White;
            Changes.main.unitName.Background = ArmyBook.Data.FrontColor;
            Changes.main.unitName.FontWeight = FontWeights.Bold;

            Changes.main.UpdateLayout();

            AddOptionsList(unitID, unit);
        }

        private static double AddLabel(string caption, double[] margins,
            double height, ref double lastColumnMaxWidth, bool selected = false,
            double points = 0, bool perModel = false, bool bold = false,
            string addLine = "", int fixPadding = 0, bool enabled = true)
        {
            Label newOption = new Label();

            string[] captionLines = Services.WordSplit(caption);

            newOption.Content = String.Empty;

            foreach (string line in captionLines)
            {
                string newLine = Environment.NewLine + "   ";
                bool emptyContent = String.IsNullOrEmpty(newOption.Content.ToString());

                newOption.Content += (emptyContent ? String.Empty : newLine) + line;
            }

            newOption.Margin = Changes.Thick(newOption, margins[0], margins[1]);

            if (!enabled)
            {
                newOption.Foreground = Brushes.Gray;
            }
            else if (selected)
            {
                newOption.Foreground = ArmyBook.Data.BackColor;
            }

            if (selected || bold)
            {
                newOption.FontWeight = FontWeights.Bold;
            }

            if (bold)
            {
                newOption.Foreground = Brushes.White;
                newOption.Background = ArmyBook.Data.FrontColor;
            }

            Changes.main.unitDetail.Children.Add(newOption);
            Changes.main.UpdateLayout();

            double actualWidth = newOption.ActualWidth;

            if (points > 0 || points < 0 || !String.IsNullOrEmpty(addLine))
            {
                double leftPadding = (points > 0 ? -5 : 5);

                bool pointsNeed = (points > 0) || (points < 0);
                string content = points.ToString() + " pts" + (perModel ? "/m" : String.Empty);
                Brush color = !enabled || selected ? Brushes.Gray : ArmyBook.Data.FrontColor;

                Label optionPoints = new Label
                {
                    Content = (pointsNeed ? content : addLine),
                    Foreground = color,
                };

                double marginLeft = margins[0] + newOption.ActualWidth + leftPadding;
                optionPoints.Margin = Changes.Thick(optionPoints, marginLeft, margins[1]);

                Changes.main.unitDetail.Children.Add(optionPoints);
                Changes.main.UpdateLayout();

                actualWidth += optionPoints.ActualWidth + 5;
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
                longOptionLine.Stroke = ArmyBook.Data.FrontColor;

                Changes.main.unitDetail.Children.Add(longOptionLine);
            }

            if (actualWidth > lastColumnMaxWidth)
            {
                lastColumnMaxWidth = actualWidth;
            }

            double bottomPadding = (captionLines.Length > 1 ? 5 : 0);

            return (height * captionLines.Length) + bottomPadding + fixPadding;
        }

        private static double AddButtonPart(string caption, double[] margins, double actualPrevPartWidth,
            string id, Brush background, double? partWidth = null, bool enabled = true, bool countable = false,
            bool withoutBorder = false, bool disabledBorder = false)
        {
            Label newPart = new Label
            {
                Tag = id,
                Content = caption,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                Background = background,
            };

            newPart.Width = partWidth ?? (countable ? 51 : 77);

            if (countable && enabled && (caption == "+" || caption == "-"))
                newPart.MouseDown += CountableOption_Click;

            else if (!countable && enabled)
                newPart.MouseDown += AddOption_Click;

            if ((!String.IsNullOrEmpty(caption) || disabledBorder) && !withoutBorder)
            {
                Border border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = (disabledBorder ? Brushes.LightGray : Brushes.Black),
                    Background = background,
                    Child = newPart,
                };
                border.Margin = Changes.Thick(newPart, Math.Ceiling(margins[0] + 2 + actualPrevPartWidth), margins[1] + 20);
                border.Width = partWidth ?? (countable ? 51 : 77);

                Changes.main.unitDetail.Children.Add(border);
            }
            else
            {
                newPart.Margin = Changes.Thick(newPart, Math.Ceiling(margins[0] + 2 + actualPrevPartWidth), margins[1] + 21);
                Changes.main.unitDetail.Children.Add(newPart);
            }
            
            Changes.main.UpdateLayout();

            return newPart.ActualWidth;
        }

        private static void AddButtonAllParts(string captionFirst, string captionSecond, Brush backgroundFirst, 
            Brush backgroundSecond, double[] margins, string id, bool enabled = true)
        {
            double actualWidth = AddButtonPart(captionFirst, margins, 0, id, backgroundFirst, enabled: enabled);
            AddButtonPart(captionSecond, margins, actualWidth, id, backgroundSecond, enabled: enabled, disabledBorder: !enabled);
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

            bool canBeReducedByMin = (option.Countable.Value > option.Countable.Min);
            bool canBeReducedByNullable = (option.Countable.Min > 0) && option.Countable.Nullable;
            bool canBeReduced = ((option.Countable.Value > 0) && (canBeReducedByMin || canBeReducedByNullable)) && enabled;

            double left = AddButtonPart("-", margins, 0, id,
                (canBeReduced ? backFirst : Brushes.Gainsboro), enabled: canBeReduced, countable: true);

            left += AddButtonPart(option.Countable.Value.ToString(),
                margins, left, id, backSecond, enabled: enabled, countable: true, withoutBorder: true);

            bool canByIncreasedByDependency = ((maxByDependency == 0) || (option.Countable.Value < maxByDependency));
            bool canBeIncreasedByMaxParam = ((option.Countable.Max == 0) || (option.Countable.Value < option.Countable.Max));
            bool canBeIncreased = canByIncreasedByDependency && canBeIncreasedByMaxParam && enabled;

            AddButtonPart("+", margins, left, id, (canBeIncreased ? backFirst : Brushes.Gainsboro),
                enabled: canBeIncreased, countable: true);
        }

        private static double AddButton(string caption, double[] margins, double height, ref double lastColumnMaxWidth,
            string id, Option option, int mountAlreadyOn = 0, string mountTypeAlreadyFixed = "",
            Unit unit = null, bool mustBeEnabled = true)
        {
            bool optionIsEnabled = unit.IsOptionEnabled(option, mountAlreadyOn, mountTypeAlreadyFixed);

            if (!mustBeEnabled)
                optionIsEnabled = false;

            if ((unit != null) && unit.IsAnotherOptionIsIncompatible(option, postCheck: true))
                optionIsEnabled = false;

            AddLabel(caption, margins, height, ref lastColumnMaxWidth, option.Realised, option.Points,
                option.PerModel, enabled: optionIsEnabled);

            if (option.IsMagicItem() || option.IsPowers())
            {
                string powers = option.IsPowers() ? "power" : "artefact";
                AddButtonPart($"drop {powers}", margins, 0, id, ArmyBook.Data.FrontColor, 154);

                return height;
            }

            if (option.Countable != null)
                AddButtonsCountable(
                    caption: option.Countable.Value.ToString(),
                    backFirst: ArmyBook.Data.BackColor,
                    backSecond: ArmyBook.Data.FrontColor,
                    option: option,
                    unit: unit,
                    margins: margins,
                    id: id,
                    enabled: optionIsEnabled);

            else if (!optionIsEnabled)
                AddButtonAllParts(
                    captionFirst: String.Empty,
                    captionSecond: String.Empty,
                    backgroundFirst: Brushes.WhiteSmoke,
                    backgroundSecond: Brushes.Gainsboro,
                    margins: margins,
                    id: id,
                    enabled: false);

            else if (option.Realised)
                AddButtonAllParts(
                    captionFirst: ArmyBook.Data.DropStyle,
                    captionSecond: String.Empty,
                    backgroundFirst: ArmyBook.Data.BackColor,
                    backgroundSecond: ArmyBook.Data.FrontColor,
                    margins: margins,
                    id: id);

            else
                AddButtonAllParts(
                    captionFirst: String.Empty,
                    captionSecond: ArmyBook.Data.AddStyle,
                    backgroundFirst: (Brush)new BrushConverter().ConvertFrom("#E1E1E1"),
                    backgroundSecond: Brushes.Silver,
                    margins: margins,
                    id: id);

            return height;
        }
    }
}
