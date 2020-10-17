using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class Reload
    {
        public static void LoadArmyList()
        {
            Interface.Changes.main.ArmyList.Items.Clear();

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

            List<Unit> categories = Army.Params.GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in ArmyBook.Data.Units)
            {
                Unit unit = entry.Value.Clone();
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

            foreach (KeyValuePair<int, Option> entry in ArmyBook.Data.Artefact)
                if (!artefactsTypes.Contains(entry.Value.ArtefactGroup))
                    artefactsTypes.Add(entry.Value.ArtefactGroup);

            foreach (string artefactType in artefactsTypes)
            {
                Option artefacts = new Option() { Name = artefactType };

                foreach (KeyValuePair<int, Option> entry in ArmyBook.Data.Artefact)
                {
                    if ((entry.Value.ArtefactGroup != artefactType) || (entry.Value.Runic > 1))
                        continue;

                    Option artefact = entry.Value.Clone();
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

                foreach (KeyValuePair<int, Option> runic in runicVersions)
                    points.Add(runic.Value.Points.ToString());

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
            Interface.Changes.main.armyHeroes.Content = String.Format("Heroes: {0}/{1} [ {2}/{3} ]",
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Lord),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Hero),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            Interface.Changes.main.armyUnits.Content = String.Format("Units: {0}/{1}/{2} [ {3}+/{4}/{5} ]",
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Core),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Special),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Rare),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            Interface.Changes.main.armyPoints.Text = String.Format("Points: {0} [ {1} ]", Army.Params.GetArmyPoints(), Army.Params.GetArmyMaxPoints());
            Interface.Changes.main.armySize.Text = String.Format("Models: {0}", Army.Params.GetArmySize());
            Interface.Changes.main.armyCasting.Content = String.Format("Cast: {0}", Army.Params.GetArmyCast());
            Interface.Changes.main.armyDispell.Content = String.Format("Dispell: {0}", Army.Params.GetArmyDispell());
        }

        public static void LoadArmySize(int points, string armyName)
        {
            Army.Data.MaxPoints = points;
            Army.Data.AdditionalName = armyName;

            string windowsHeaderWithName = "Warhammer Army Assembler (WAAgh)";
            Interface.Changes.main.dragWindowBottom.Content = windowsHeaderWithName +
                (String.IsNullOrWhiteSpace(armyName) ? String.Empty : String.Format(" // {0}", armyName));

            ArmyBook.Load.LoadArmy(Interface.Changes.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            Interface.Changes.DetailResize(open: false);
            Interface.Changes.Move(Interface.Changes.MovingType.ToMain);
        }
    }
}
