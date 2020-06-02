using System;
using System.Collections.Generic;
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
            double detailHeight = Interface.Changes.main.unitDetail.ActualHeight;
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
            string[] captionLines = Interface.Other.WordSplit(caption);

            if (captionLines.Length < 2)
                return false;

            return (margins[1] + 25 + (height * captionLines.Length) > GetDetailHeight() ? true : false);
        }

        private static string GetMagicPointsString(int unitID, string head)
        {
            if (head != "MAGIC ITEMS")
                return String.Empty;

            return String.Format("{0} / {1}", Army.Data.Units[unitID].MagicPointsAlreadyUsed(), Army.Data.Units[unitID].GetUnitMagicPoints());
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
                if (!unit.ExistsMagicItems() && (head == "MAGIC ITEMS"))
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth);
                    margins[1] += AddLabel("empty yet", margins, 15, ref lastColumnMaxWidth, fixPadding: 5);

                    notFirstColumn = true;
                }
                else
                    foreach (Option option in unit.Options)
                    {
                        if (head == "OPTION" || head == "COMMAND" || head == "MAGIC ITEMS")
                        {
                            if (head == "OPTION" && (!option.IsOption() || option.FullCommand))
                                continue;

                            if (head == "COMMAND" && !option.FullCommand)
                                continue;

                            if (head == "MAGIC ITEMS" && (!option.IsMagicItem() || (option.Points <= 0)))
                                continue;

                            margins = CheckColumn(margins, ref lastColumnMaxWidth);

                            bool canBeUsed = true;

                            if (option.OnlyOneInArmy || option.OnlyOneForSuchUnits)
                                canBeUsed = (Army.Checks.IsOptionAlreadyUsed(option.Name, unitID, unit.Name, option.OnlyOneForSuchUnits) == 0);

                            margins[1] += AddButton(option.Name, margins, 25, ref lastColumnMaxWidth, String.Format("{0}|{1}", unitID, option.ID),
                                option, mountAlreadyOn: mountAlreadyOn, mountTypeAlreadyFixed: mountTypeAlreadyFixed, unit: unit,
                                mustBeEnabled: canBeUsed);

                            margins[1] += 20;
                        }
                        else
                        {
                            bool thisIsStandartEquipment = !option.IsMagicItem() || (option.Points != 0) || String.IsNullOrEmpty(option.Name);
                            bool thisIsSpecialRuleOrMount = option.Realised && !option.Mount &&
                                !option.FullCommand && option.SpecialRuleDescription.Length == 0;

                            if (head == "WEAPONS & ARMOUR" && thisIsStandartEquipment && !thisIsSpecialRuleOrMount)
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
            double[] margins = new double[] { Interface.Changes.main.unitName.Margin.Left, Interface.Changes.main.unitName.Margin.Top + 35 };

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in Interface.Changes.main.unitDetail.Children)
                if (element.Name != "closeUnitDetail" && element.Name != "unitName")
                    elementsForRemoving.Add(element);

            foreach (FrameworkElement element in elementsForRemoving)
                Interface.Changes.main.unitDetail.Children.Remove(element);

            bool notFirstColumn = false;

            double lastColumnMaxWidth = 0;

            if (unit.GetUnitWizard() > 0)
                AddLabel(
                    caption: String.Format("Wizard Level {0}", unit.GetUnitWizard()),
                    margins: new double[] {
                        Interface.Changes.main.unitName.Margin.Left + Interface.Changes.main.unitName.ActualWidth + 5,
                        Interface.Changes.main.unitName.Margin.Top
                    },
                    height: 25,
                    lastColumnMaxWidth: ref lastColumnMaxWidth
                );

            if (unit.ExistsOptions())
                margins = CreateColumn("OPTION", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsCommand())
                margins = CreateColumn("COMMAND", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsMagicItems() || (Army.Data.Units[unitID].GetUnitMagicPoints() > 0))
                margins = CreateColumn("MAGIC ITEMS", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsOrdinaryItems())
                margins = CreateColumn("WEAPONS & ARMOUR", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.GetSpecialRules().Count > 0)
                margins = CreateColumn("SPECIAL RULES", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            Interface.Changes.main.unitDetail.Width = margins[0] + lastColumnMaxWidth + 25;

            if (Interface.Changes.main.unitDetail.Width > Interface.Changes.main.unitDetailScroll.Width)
                Interface.Changes.main.unitDetailScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Label).Tag.ToString();

            string[] id = id_tag.Split('|');

            int optionID = Interface.Other.IntParse(id[1]);
            int unitID = Interface.Other.IntParse(id[0]);

            Army.Data.Units[unitID].AddOption(optionID, Army.Data.Units[unitID], unitID);
            Army.Data.Units[unitID].ThrowAwayIncompatibleOption();

            Interface.Reload.ReloadArmyData();
            Interface.Mod.SetArtefactAlreadyUsed(Interface.Other.IntParse(id[1]), false);
            UpdateUnitDescription(unitID, Army.Data.Units[unitID]);
        }

        public static void UpdateUnitDescription(int unitID, Unit unit)
        {
            Interface.Changes.main.unitName.Content = unit.Name.ToUpper();

            Interface.Changes.main.unitName.Foreground = Brushes.White;
            Interface.Changes.main.unitName.Background = ArmyBook.Data.MainColor;
            Interface.Changes.main.unitName.FontWeight = FontWeights.Bold;

            Interface.Changes.main.UpdateLayout();

            AddOptionsList(unitID, unit);
        }

        private static double AddLabel(string caption, double[] margins, double height, ref double lastColumnMaxWidth,
            bool selected = false, double points = 0, bool perModel = false, bool bold = false, string addLine = "",
            int fixPadding = 0)
        {
            Label newOption = new Label();

            string[] captionLines = Interface.Other.WordSplit(caption);

            newOption.Content = String.Empty;

            foreach (string line in captionLines)
                newOption.Content += (String.IsNullOrEmpty(newOption.Content.ToString()) ? String.Empty : Environment.NewLine + "   ") + line;

            newOption.Margin = Interface.Changes.Thick(newOption, margins[0], margins[1]);

            if (selected)
                newOption.Foreground = ArmyBook.Data.AdditionalColor;

            if (selected || bold)
                newOption.FontWeight = FontWeights.Bold;

            if (bold)
            {
                newOption.Foreground = Brushes.White;
                newOption.Background = ArmyBook.Data.MainColor;
            }

            Interface.Changes.main.unitDetail.Children.Add(newOption);

            Interface.Changes.main.UpdateLayout();

            double actualWidth = newOption.ActualWidth;

            if (points > 0 || !String.IsNullOrEmpty(addLine))
            {
                double leftPadding = (points > 0 ? -5 : 5);

                Label optionPoints = new Label
                {
                    Content = (points > 0 ? points.ToString() + " pts" + (perModel ? "/m" : String.Empty) : addLine)
                };
                optionPoints.Margin = Interface.Changes.Thick(optionPoints, margins[0] + newOption.ActualWidth + leftPadding, margins[1]);
                optionPoints.Foreground = ArmyBook.Data.MainColor;
                Interface.Changes.main.unitDetail.Children.Add(optionPoints);

                Interface.Changes.main.UpdateLayout();

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

                Interface.Changes.main.unitDetail.Children.Add(longOptionLine);
            }

            if (actualWidth > lastColumnMaxWidth)
                lastColumnMaxWidth = actualWidth;

            double bottomPadding = (captionLines.Length > 1 ? 5 : 0);

            return (height * captionLines.Length) + bottomPadding + fixPadding;
        }

        private static double AddButtonPart(string caption, double[] margins, double actualPrevPartWidth,
            string id, Brush background, double? partWidth = null, bool enabled = true)
        {
            Label newPart = new Label
            {
                Tag = id,
                Content = caption,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                Background = background,
            };

            newPart.Margin = Interface.Changes.Thick(newPart, margins[0] + 2 + actualPrevPartWidth, margins[1] + 20);
            newPart.Width = partWidth ?? 77;

            if (enabled)
                newPart.MouseDown += AddOption_Click;

            Interface.Changes.main.unitDetail.Children.Add(newPart);
            Interface.Changes.main.UpdateLayout();

            return newPart.ActualWidth;
        }

        private static void AddButtonAllParts(string captionFirst, string captionSecond, Brush backgroundFirst, 
            Brush backgroundSecond, double[] margins, string id,  double? partWidth = null, bool enabled = true)
        {
            double actualWidth = AddButtonPart(captionFirst, margins, 0, id, backgroundFirst, enabled: enabled);
            AddButtonPart(captionSecond, margins, actualWidth, id, backgroundSecond, enabled: enabled);
        }

        private static double AddButton(string caption, double[] margins, double height, ref double lastColumnMaxWidth, string id,
            Option option, int mountAlreadyOn = 0, Option.OnlyForType mountTypeAlreadyFixed = Option.OnlyForType.All, Unit unit = null,
            bool mustBeEnabled = true)
        {
            AddLabel(caption, margins, height, ref lastColumnMaxWidth, (option.Realised ? true : false),
                option.Points, option.PerModel);

            if (option.IsMagicItem())
            {
                AddButtonPart("drop artefact", margins, 0, id, ArmyBook.Data.MainColor, 154);
                return height;
            }

            bool optionIsEnabled = unit.IsOptionEnabled(option, mountAlreadyOn, mountTypeAlreadyFixed);

            if (!mustBeEnabled)
                optionIsEnabled = false;

            if (
                    (unit != null)
                    && (
                        !unit.IsAnotherOptionRealised(option.OnlyIfAnotherService, defaultResult: true)
                        ||
                        unit.IsAnotherOptionRealised(option.OnlyIfNotAnotherService, defaultResult: false)
                    )
                )
                optionIsEnabled = false;

            if (!optionIsEnabled)
                AddButtonAllParts(
                    captionFirst: String.Empty,
                    captionSecond: String.Empty,
                    backgroundFirst: Brushes.WhiteSmoke,
                    backgroundSecond: Brushes.Gainsboro,
                    margins: margins,
                    id: id,
                    enabled: false
                );

            else if (option.Realised)
                AddButtonAllParts(
                    captionFirst: "drop",
                    captionSecond: String.Empty,
                    backgroundFirst: ArmyBook.Data.AdditionalColor,
                    backgroundSecond: ArmyBook.Data.MainColor,
                    margins: margins,
                    id: id
                );

            else
                AddButtonAllParts(
                    captionFirst: String.Empty,
                    captionSecond: "add",
                    backgroundFirst: Brushes.LightGray,
                    backgroundSecond: Brushes.Silver,
                    margins: margins,
                    id: id
                );

            return height;
        }

    }
}
