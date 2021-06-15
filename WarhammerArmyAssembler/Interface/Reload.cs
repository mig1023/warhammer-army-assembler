using System;
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
            Interface.Changes.main.ArmyList.Items.Clear();

            if (!fastReload)
            {
                Interface.Changes.AllUnitDelete();

                Interface.Changes.main.armyMainLabel.Content = Army.Data.Name;
                Interface.Changes.main.armyVersionLabel.Content = String.Format("{0}ed", Army.Data.ArmyVersion);

                Interface.Changes.main.armyMainLabelPlace.Background = ArmyBook.Data.MainColor;
                Interface.Changes.main.armyVersionLabel.Background = ArmyBook.Data.MainColor;
                Interface.Changes.main.unitDetailHead.Background = ArmyBook.Data.MainColor;

                Interface.Changes.main.armyMainMenu.Content = '\u2630';
                Interface.Changes.main.armyMainMenu.Foreground = Brushes.White;
                Interface.Changes.main.armyMainMenu.Background = ArmyBook.Data.AdditionalColor;

                Interface.Changes.main.armyMainLabel.Foreground = Brushes.White;
                Interface.Changes.main.armyMainLabel.Background = ArmyBook.Data.MainColor;
            }

            List<Unit> categories = Army.Params.GetArmyCategories();

            foreach (Unit entry in ArmyBook.Data.Units.Values)
            {
                Unit unit = entry.Clone();
                unit.PointsView = String.Format(" {0} pts", unit.Points);
                unit.InterfaceColor = ArmyBook.Data.MainColor;
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                unitType.Name = Army.Mod.CategoryNameModification(unitType.Name);
                unitType.GroopBold = true;
                Interface.Changes.main.ArmyList.Items.Add(unitType);
            }

            List<string> artefactsTypes = new List<string>();

            foreach (Option entry in ArmyBook.Data.Artefact.Values.Where(x => !artefactsTypes.Contains(x.ArtefactGroup)))
                artefactsTypes.Add(entry.ArtefactGroup);

            foreach (string artefactType in artefactsTypes)
            {
                Option artefacts = new Option() { Name = artefactType };

                foreach (Option entry in ArmyBook.Data.Artefact.Values)
                {
                    if ((entry.ArtefactGroup != artefactType) || (entry.Runic > 1))
                        continue;

                    Option artefact = entry.Clone();
                    artefact.PointsView = PointsView(artefact);
                    artefact.InterfaceColor = ArmyBook.Data.MainColor;
                    artefacts.Items.Add(artefact);
                }

                artefacts.GroopBold = true;
                artefacts.Artefacts = true;
                Interface.Changes.main.ArmyList.Items.Add(artefacts);
            }
        }

        private static string PointsView(Option artefact)
        {
            if (artefact.Runic > 0)
            {
                List<string> points = new List<string>();

                Dictionary<int, Option> runicVersions = artefact.AllRunicVersions();

                foreach (Option runic in runicVersions.Values)
                    points.Add(runic.Points.ToString());

                return String.Format(" {0} pts", String.Join("/", points.ToArray()));
            }
            else 
                return String.Format(" {0} pts", artefact.Points);
        }

        public static void ReloadArmyData()
        {
            Interface.Changes.ArmyInInterface.Clear();

            List<Unit> categories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in categories)
                foreach (Unit unit in unitType.Items)
                    Interface.Changes.ArmyInInterface.Add(unit);

            Interface.Changes.main.ArmyGrid.ItemsSource = Interface.Changes.ArmyInInterface;
            Interface.Changes.main.armyHeroes.Text = String.Format("Heroes: {0}/{1} [ {2}/{3} ]",
                UnitsNumber(Unit.UnitType.Lord), UnitsNumber(Unit.UnitType.Hero), MaxUnits(Unit.UnitType.Lord), MaxUnits(Unit.UnitType.Hero));

            Interface.Changes.main.armyUnits.Text = String.Format("Units: {0}/{1}/{2} [ {3}+/{4}/{5} ]",
                UnitsNumber(Unit.UnitType.Core), UnitsNumber(Unit.UnitType.Special), UnitsNumber(Unit.UnitType.Rare),
                MaxUnits(Unit.UnitType.Core), MaxUnits(Unit.UnitType.Special), MaxUnits(Unit.UnitType.Rare));

            Interface.Changes.main.armyPoints.Text = String.Format("Points: {0} [ {1} ]", Army.Params.GetArmyPoints(), Army.Params.GetArmyMaxPoints());
            Interface.Changes.main.armySize.Text = String.Format("Models: {0}", Army.Params.GetArmySize());
            Interface.Changes.main.armyCasting.Content = String.Format("Cast: {0}", Army.Params.GetArmyCast());
            Interface.Changes.main.armyDispell.Content = String.Format("Dispell: {0}", Army.Params.GetArmyDispell());
        }

        private static int UnitsNumber(Unit.UnitType type) => Army.Params.GetArmyUnitsNumber(type);
        private static int MaxUnits(Unit.UnitType type) => Army.Params.GetArmyMaxUnitsNumber(type);

        public static void LoadArmySize(int points, string armyName)
        {
            Army.Data.MaxPoints = points;
            Army.Data.AdditionalName = armyName;

            Interface.Changes.main.dragWindowBottom.Content = "Warhammer Army Assembler (WAAgh)" +
                (String.IsNullOrWhiteSpace(armyName) ? String.Empty : String.Format(" // {0}", armyName));

            ArmyBook.Load.LoadArmy(Interface.Changes.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            Interface.Changes.DetailResize(open: false);
            Interface.Changes.Move(Interface.Changes.MovingType.ToMain);
        }
    }
}
