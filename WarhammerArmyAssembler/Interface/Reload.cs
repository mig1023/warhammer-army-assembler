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
            MainWindow main = Changes.main;
            main.ArmyList.Items.Clear();

            if (!fastReload)
            {
                Changes.AllUnitDelete();

                main.armyMainLabel.Content = Army.Data.Name;
                main.armyEditionLabel.Content = String.Format("{0} Edition", Army.Data.ArmyEdition);

                main.armyMainLabelPlace.Background = ArmyBook.Data.FrontColor;
                main.armyEditionLabel.Background = ArmyBook.Data.FrontColor;
                main.unitDetailHead.Background = ArmyBook.Data.FrontColor;

                main.armyMainMenu.Content = '\u2630';
                main.armyMainMenu.Foreground = Brushes.White;
                main.armyMainMenu.Background = ArmyBook.Data.BackColor;

                main.armyMainLabel.Foreground = Brushes.White;
                main.armyMainLabel.Background = ArmyBook.Data.FrontColor;
            }

            List<Unit> categories = Army.Params.GetArmyCategories();

            foreach (Unit entry in ArmyBook.Data.Units.Values)
            {
                Unit unit = entry.Clone();
                unit.PointsView = String.Format(" {0} pts", unit.Points);
                unit.InterfaceColor = ArmyBook.Data.FrontColor;
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                unitType.Name = Army.Mod.CategoryNameModification(unitType.Name);
                unitType.GroopBold = true;
                main.ArmyList.Items.Add(unitType);
            }

            List<string> artefactsTypes = new List<string>();

            foreach (Option entry in ArmyBook.Data.Artefact.Values.Where(x => !artefactsTypes.Contains(x.ArtefactGroup)))
                artefactsTypes.Add(entry.ArtefactGroup);

            string lastRandomGroup = String.Empty;

            foreach (string artefactType in artefactsTypes)
            {
                Option artefacts = new Option() { Name = artefactType };

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

                artefacts.GroopBold = true;
                artefacts.Artefacts = true;
                main.ArmyList.Items.Add(artefacts);
            }

            TreeViewItem item = main.ArmyList.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;

            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }
        }

        private static string PointsView(Option artefact) => 
            String.Format(" {0} pts", (artefact.Runic > 0 ? RunicPoints(artefact) : artefact.Points.ToString()));

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
                foreach (Unit unit in unitType.Items)
                    Changes.ArmyInInterface.Add(unit);

            Changes.main.ArmyGrid.ItemsSource = Changes.ArmyInInterface;

            int lord = UnitsNumber(Unit.UnitType.Lord);
            int hero = UnitsNumber(Unit.UnitType.Hero);

            int maxLord = MaxUnits(Unit.UnitType.Lord);
            int maxHero = MaxUnits(Unit.UnitType.Hero);

            Changes.main.armyHeroes.Content = String.Format("Heroes: {0}/{1} [ {2}/{3} ]",
                lord, hero, maxLord, maxHero);

            int core = UnitsNumber(Unit.UnitType.Core);
            int special = UnitsNumber(Unit.UnitType.Special);
            int rare = UnitsNumber(Unit.UnitType.Rare);
            
            int maxCore = MaxUnits(Unit.UnitType.Core);
            int maxSpecial = MaxUnits(Unit.UnitType.Core);
            int maxRare = MaxUnits(Unit.UnitType.Core);

            Changes.main.armyUnits.Content = String.Format("Units: {0}/{1}/{2} [ {3}+/{4}/{5} ]",
                core, special, rare, maxCore, maxSpecial, maxRare);

            double points = Army.Params.GetArmyPoints();
            int maxPoints = Army.Params.GetArmyMaxPoints();

            Changes.main.armyPoints.Content = String.Format("Points: {0} [ {1} ]", points, maxPoints);
            Changes.main.armySize.Content = String.Format("Models: {0}", Army.Params.GetArmySize());
            Changes.main.armyCasting.Content = String.Format("Cast: {0}", Army.Params.GetArmyCast());
            Changes.main.armyDispell.Content = String.Format("Dispell: {0}", Army.Params.GetArmyDispell());
        }

        private static int UnitsNumber(Unit.UnitType type) => Army.Params.GetArmyUnitsNumber(type);
        private static int MaxUnits(Unit.UnitType type) => Army.Params.GetArmyMaxUnitsNumber(type);

        public static void LoadArmySize(int points, string armyName)
        {
            Army.Data.MaxPoints = points;
            Army.Data.AdditionalName = armyName;

            Changes.main.dragWindowBottom.Content = "Warhammer Army Assembler (WAAgh)" +
                (String.IsNullOrWhiteSpace(armyName) ? String.Empty : String.Format(" // {0}", armyName));

            ArmyBook.Load.LoadArmy(Changes.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            Changes.DetailResize(open: false);
            Changes.Move(Interface.Changes.MovingType.ToMain);
        }
    }
}
