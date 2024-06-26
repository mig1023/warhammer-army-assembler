﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class Reload
    {
        public static void LoadArmyList(bool fastReload = false)
        {
            MainWindow main = Changes.main;
            main.ArmyList.Items.Clear();

            if (!fastReload)
            {
                Changes.AllUnitDelete();

                main.armyMainLabel.Content = Army.Data.Name;
                main.armyEditionLabel.Content = $"{Army.Data.ArmyEdition} Edition";

                main.armyMainLabelPlace.Background = ArmyBook.Data.FrontColor;
                main.armyEditionLabel.Background = ArmyBook.Data.FrontColor;
                main.unitDetailHead.Background = ArmyBook.Data.FrontColor;

                main.armyMainMenu.Content = '\u2630';
                main.armyMainMenu.Foreground = Brushes.White;
                main.armyMainMenu.Background = ArmyBook.Data.BackColor;

                main.armyMainLabel.Foreground = Brushes.White;
                main.armyMainLabel.Background = ArmyBook.Data.FrontColor;
            }

            List<Unit> categories = Army.Params.GetArmyCategories(withMercenary: true);

            foreach (Unit entry in ArmyBook.Data.Units.Values)
            {
                Unit unit = entry.Clone();
                unit.InterfaceColor = ArmyBook.Data.FrontColor;
                unit.PointsView = $" {unit.Points} pts";

                int category = (int)unit.Type;
                bool dogsHeroIsEnabled = Settings.Values.IsTrue("DogsOfWarCharacter");
                bool dogsUnitIsEnabled = Settings.Values.IsTrue("DogsOfWarEnabled");
                int dogsCategory = ArmyBook.Constants.DogsOfWarCategory;

                if (unit.DogsOfWar)
                {
                    if (unit.IsHero())
                    {
                        if (!dogsHeroIsEnabled || ArmyBook.Data.NoDogsOfWar)
                            continue;

                        categories[dogsCategory].Items[(int)unit.Type].Items.Add(unit);
                    }
                    else
                    {
                        if (!dogsUnitIsEnabled || ArmyBook.Data.NoDogsOfWar)
                            continue;

                        if (unit.Prepayment != 0)
                            unit.PointsView += $" (+{unit.Prepayment} pts)";

                        if (dogsHeroIsEnabled)
                        {
                            int dogsUnitAfterHeroes = ArmyBook.Constants.DogsOfWarUnitsAfterHeroes;
                            categories[dogsCategory].Items[dogsUnitAfterHeroes].Items.Add(unit);
                        }
                        else
                        {
                            categories[dogsCategory].Items.Add(unit);
                        }
                    }
                }
                else
                {
                    categories[category].Items.Add(unit);
                }
            }

            foreach (Unit unitType in categories)
                main.ArmyList.Items.Add(unitType);

            List<string> artefactsTypes = ArmyBook.Data.Artefact.Values
                .Select(x => x.ArtefactGroup)
                .Distinct()
                .ToList();

            string lastRandomGroup = String.Empty;

            foreach (string artefactType in artefactsTypes)
            {
                Option artefacts = Army.Params.GetCategoryArtefact(artefactType);

                foreach (Option entry in ArmyBook.Data.Artefact.Values)
                {
                    if ((entry.ArtefactGroup != artefactType) || (entry.Runic > 1))
                        continue;

                    if (!String.IsNullOrEmpty(entry.RandomGroup) && (entry.RandomGroup == lastRandomGroup))
                        continue;

                    Option artefact = entry.Clone();

                    if (!String.IsNullOrEmpty(entry.RandomGroup))
                    {
                        artefact.Name = entry.RandomGroup + " (random)";
                        lastRandomGroup = entry.RandomGroup;
                    }

                    artefact.PointsView = PointsView(artefact);
                    artefact.InterfaceColor = ArmyBook.Data.FrontColor;
                    artefacts.Items.Add(artefact);
                }

                main.ArmyList.Items.Add(artefacts);
            }

            TreeViewItem item = main.ArmyList.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;

            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }
        }

        private static string PointsView(Option artefact)
        {
            string points = artefact.Runic > 0 ? RunicPoints(artefact) : artefact.Points.ToString();
            return $" {points} pts";
        }

        private static string RunicPoints(Option artefact)
        {
            List<string> points = new List<string>();

            Dictionary<int, Option> runicVersions = artefact.AllRunicVersions();

            foreach (Option runic in runicVersions.Values)
                points.Add(runic.Points.ToString());

            return String.Join("/", points.ToArray());
        }

        public static void ReloadArmyData()
        {
            Changes.ArmyInInterface.Clear();

            List<Unit> categories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in categories)
            {
                foreach (Unit unit in unitType.Items)
                {
                    Changes.ArmyInInterface.Add(unit);
                }
            }
                
            Changes.main.ArmyGrid.ItemsSource = Changes.ArmyInInterface;

            int lord = UnitsNumber(Unit.UnitType.Lord);
            int maxLord = MaxUnits(Unit.UnitType.Lord);

            int hero = UnitsNumber(Unit.UnitType.Hero);
            int maxHero = MaxUnits(Unit.UnitType.Hero);

            Changes.main.armyHeroes.Content = $"Heroes: {lord}/{hero} [ {maxLord}/{maxHero} ]";

            int core = UnitsNumber(Unit.UnitType.Core);
            int maxCore = MaxUnits(Unit.UnitType.Core);

            int special = UnitsNumber(Unit.UnitType.Special);
            int maxSpecial = MaxUnits(Unit.UnitType.Special);

            int rare = UnitsNumber(Unit.UnitType.Rare);
            int maxRare = MaxUnits(Unit.UnitType.Rare);

            Changes.main.armyUnits.Content = $"Units: {core}/{special}/{rare} " +
                $"[ {maxCore}+/{maxSpecial}/{maxRare} ]";

            double points = Army.Params.GetArmyPoints();
            int maxPoints = Army.Params.GetArmyMaxPoints();

            Changes.main.armyPoints.Content = $"Points: {points} [ {maxPoints} ]";
            Changes.main.armySize.Content = $"Models: {Army.Params.GetArmySize()}";
            Changes.main.armyCasting.Content = $"Cast: {Army.Params.GetArmyCast()}";
            Changes.main.armyDispell.Content = $"Dispell: {Army.Params.GetArmyDispell()}";
        }

        private static int UnitsNumber(Unit.UnitType type) =>
            Army.Params.GetArmyUnitsNumber(type);
        private static int MaxUnits(Unit.UnitType type) =>
            Army.Params.GetArmyMaxUnitsNumber(type);

        public static void LoadArmySize(int points, string armyName)
        {
            Army.Data.MaxPoints = points;
            Army.Data.RosterName = armyName;

            Changes.main.dragWindowBottom.Content = "Warhammer Army Assembler (WAAgh)" +
                (String.IsNullOrWhiteSpace(armyName) ? String.Empty : $" // {armyName}");

            ArmyBook.Load.Armybook(Changes.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            Changes.DetailResize(open: false);
            Changes.Move(Interface.Changes.MovingType.ToMain);
        }
    }
}
