﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class InterfaceUnitDetails
    {
        private static double[] CheckColumn(double[] margins, ref double lastColumnMaxWidth,
            bool header = false, bool newColumn = false)
        {
            double detailHeight = Interface.main.unitDetail.ActualHeight;
            detailHeight = (detailHeight > 0 ? detailHeight : 250);

            if (newColumn || (margins[1] + (header ? 90 : 60) > detailHeight))
            {
                margins[0] += (lastColumnMaxWidth > 175 ? lastColumnMaxWidth + 10 : 175);
                margins[1] = (header ? 50 : 40) + (newColumn ? 0 : 45);
                lastColumnMaxWidth = 0;
            }

            return margins;
        }

        private static double[] CreateColumn(string head, double[] margins, int unitID, Unit unit,
            ref bool notFirstColumn, ref double lastColumnMaxWidth)
        {
            if (notFirstColumn)
                margins[1] += 10;

            margins = CheckColumn(margins, ref lastColumnMaxWidth, header: true, newColumn: notFirstColumn);

            margins[1] += AddLabel(head, margins, 25, ref lastColumnMaxWidth, bold: true);

            margins[1] += 10;

            int mountAlreadyOn = (unit.MountOn > 0 ? unit.GetMountOption() : 0);

            Option.OnlyForType mountTypeAlreadyFixed = unit.GetMountTypeAlreadyFixed();

            if (head == "SPECIAL RULES")
            {
                foreach (string rule in unit.GetSpecialRules())
                {
                    margins = CheckColumn(margins, ref lastColumnMaxWidth);

                    margins[1] += AddLabel((rule == "FC" ? "FULL COMMAND" : rule), margins, 15, ref lastColumnMaxWidth);

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
                        if (head == "OPTION" && (!option.IsOption() || option.FullCommand))
                            continue;

                        if (head == "COMMAND" && !option.FullCommand)
                            continue;

                        if (head == "MAGIC ITAMS" && (!option.IsMagicItem() || (option.Points <= 0)))
                            continue;

                        margins = CheckColumn(margins, ref lastColumnMaxWidth);

                        int alredyUsedBy = (option.OnlyOneInArmy ? ArmyChecks.IsOptionAlreadyUsed(option.Name) : 0);
                        bool canBeUsed = (!option.OnlyOneInArmy || (alredyUsedBy == 0) || (alredyUsedBy == unitID));

                        margins[1] += AddButton(option.Name, margins, 25, ref lastColumnMaxWidth, String.Format("{0}|{1}", unitID, option.ID),
                            option, mountAlreadyOn: mountAlreadyOn, mountTypeAlreadyFixed: mountTypeAlreadyFixed, unit: unit,
                            mustBeEnabled: canBeUsed);

                        margins[1] += 20;
                    }
                    else
                    {
                        bool thisIsStandartEquipment = !option.IsMagicItem() || (option.Points != 0) || String.IsNullOrEmpty(option.Name);
                        bool thisIsSpecialRuleOrMount = option.Realised && !option.Mount && option.SpecialRuleDescription.Length == 0;

                        if (head == "WEAPONS & ARMOUR" && thisIsStandartEquipment && !thisIsSpecialRuleOrMount)
                            continue;

                        margins = CheckColumn(margins, ref lastColumnMaxWidth);

                        margins[1] += AddLabel(option.Name, margins, 15, ref lastColumnMaxWidth);

                        margins[1] += 5;
                    }

                    notFirstColumn = true;
                }
            }

            return margins;
        }

        public static void AddOptionsList(int unitID, Unit unit)
        {
            double[] margins = new double[] { Interface.main.unitName.Margin.Left, Interface.main.unitName.Margin.Top + 35 };

            List<FrameworkElement> elementsForRemoving = new List<FrameworkElement>();

            foreach (FrameworkElement element in Interface.main.unitDetail.Children)
                if (element.Name != "closeUnitDetail" && element.Name != "unitName")
                    elementsForRemoving.Add(element);

            foreach (FrameworkElement element in elementsForRemoving)
                Interface.main.unitDetail.Children.Remove(element);

            bool notFirstColumn = false;

            double lastColumnMaxWidth = 0;

            if (unit.Wizard > 0)
            {
                AddLabel(String.Format("Wizard Level {0}", unit.GetUnitWizard()),
                    new double[] { Interface.main.unitName.Margin.Left + Interface.main.unitName.ActualWidth + 5, Interface.main.unitName.Margin.Top },
                    25, ref lastColumnMaxWidth);
            }

            if (unit.ExistsOptions())
                margins = CreateColumn("OPTION", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsCommand())
                margins = CreateColumn("COMMAND", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsMagicItems())
                margins = CreateColumn("MAGIC ITAMS", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.ExistsOrdinaryItems())
                margins = CreateColumn("WEAPONS & ARMOUR", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            if (unit.GetSpecialRules().Count > 0)
                margins = CreateColumn("SPECIAL RULES", margins, unitID, unit, ref notFirstColumn, ref lastColumnMaxWidth);

            Interface.main.unitDetail.Width = margins[0] + lastColumnMaxWidth + 10;

            if (Interface.main.unitDetail.Width > Interface.main.unitDetailScroll.Width)
                Interface.main.unitDetailScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private static void AddOption_Click(object sender, RoutedEventArgs e)
        {
            string id_tag = (sender as Label).Tag.ToString();

            string[] id = id_tag.Split('|');

            int optionID = InterfaceOther.IntParse(id[1]);
            int unitID = InterfaceOther.IntParse(id[0]);

            Army.Units[unitID].AddOption(optionID, Army.Units[unitID], unitID);
            Army.Units[unitID].ThrowAwayIncompatibleOption();

            InterfaceReload.ReloadArmyData();
            InterfaceMod.SetArtefactAlreadyUsed(InterfaceOther.IntParse(id[1]), false);
            UpdateUnitDescription(unitID, Army.Units[unitID]);
        }

        public static void UpdateUnitDescription(int unitID, Unit unit)
        {
            Interface.main.unitName.Content = unit.Name.ToUpper();

            Interface.main.unitName.Foreground = Brushes.White;
            Interface.main.unitName.Background = ArmyBook.MainColor;
            Interface.main.unitName.FontWeight = FontWeights.Bold;

            Interface.main.UpdateLayout();

            AddOptionsList(unitID, unit);
        }

        private static double AddLabel(string caption, double[] margins, double height, ref double lastColumnMaxWidth,
            bool selected = false, double points = 0, bool perModel = false, bool bold = false)
        {
            Label newOption = new Label();
            newOption.Content = caption;
            newOption.Margin = Interface.Thick(newOption, margins[0], margins[1]);

            if (selected)
                newOption.Foreground = ArmyBook.AdditionalColor;

            if (selected || bold)
                newOption.FontWeight = FontWeights.Bold;

            if (bold)
            {
                newOption.Foreground = Brushes.White;
                newOption.Background = ArmyBook.MainColor;
            }

            Interface.main.unitDetail.Children.Add(newOption);

            Interface.main.UpdateLayout();

            double actualWidth = newOption.ActualWidth;

            if (points > 0)
            {
                Label optionPoints = new Label();
                optionPoints.Content = points.ToString() + " pts" + (perModel ? "/m" : String.Empty);
                optionPoints.Margin = Interface.Thick(optionPoints, margins[0] + newOption.ActualWidth - 5, margins[1]);
                optionPoints.Foreground = ArmyBook.MainColor;
                Interface.main.unitDetail.Children.Add(optionPoints);

                Interface.main.UpdateLayout();

                actualWidth += optionPoints.ActualWidth - 5;
            }

            if (actualWidth > lastColumnMaxWidth)
                lastColumnMaxWidth = actualWidth;

            return height;
        }

        private static double AddButtonPart(string caption, double[] margins, double actualPrevPartWidth,
            string id, Brush background, double? partWidth = null, bool enabled = true)
        {
            Label newPart = new Label();

            newPart.Content = caption;
            newPart.HorizontalContentAlignment = HorizontalAlignment.Center;
            newPart.Margin = Interface.Thick(newPart, margins[0] + 2 + actualPrevPartWidth, margins[1] + 20);

            newPart.Foreground = Brushes.White;
            newPart.Background = background;

            if (enabled)
                newPart.MouseDown += AddOption_Click;

            newPart.Width = partWidth ?? 77;
            newPart.Tag = id;

            Interface.main.unitDetail.Children.Add(newPart);
            Interface.main.UpdateLayout();

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
                AddButtonPart("drop", margins, 0, id, ArmyBook.MainColor, 154);
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
                    backgroundFirst: ArmyBook.MainColor,
                    backgroundSecond: ArmyBook.BackgroundColor,
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
