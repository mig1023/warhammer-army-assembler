using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class InterfaceReload
    {
        public static void LoadArmyList()
        {
            Interface.main.ArmyList.Items.Clear();

            Interface.AllUnitDelete();

            Interface.main.armyMainLabel.Content = Army.Data.Name;
            Interface.main.armyVersionLabel.Content = String.Format("{0}ed", Army.Data.ArmyVersion); 

            Interface.main.armyMainLabelPlace.Background = ArmyBook.MainColor;
            Interface.main.armyVersionLabel.Background = ArmyBook.MainColor;
            Interface.main.unitDetailHead.Background = ArmyBook.MainColor;

            Interface.main.armyMainMenu.Content = '\u2630';
            Interface.main.armyMainMenu.Foreground = Brushes.White;
            Interface.main.armyMainMenu.Background = ArmyBook.AdditionalColor;

            Interface.main.armyMainLabel.Foreground = Brushes.White;
            Interface.main.armyMainLabel.Background = ArmyBook.MainColor;

            List<Unit> categories = Army.Params.GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in ArmyBook.Units)
            {
                Unit unit = entry.Value.Clone();
                unit.PointsView = String.Format(" {0} pts", unit.Points);
                unit.InterfaceColor = ArmyBook.MainColor;
                categories[(int)unit.Type].Items.Add(unit);
            }

            foreach (Unit unitType in categories)
            {
                unitType.Name = Army.Mod.CategoryNameModification(unitType.Name);
                unitType.GroopBold = true;
                Interface.main.ArmyList.Items.Add(unitType);
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
                Interface.main.ArmyList.Items.Add(artefacts);
            }
        }

        public static void ReloadArmyData()
        {
            Interface.ArmyInInterface.Clear();

            List<Unit> categories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in categories)
                foreach (Unit unit in unitType.Items)
                    Interface.ArmyInInterface.Add(unit);

            Interface.main.ArmyGrid.ItemsSource = Interface.ArmyInInterface;
            Interface.main.armyHeroes.Content = String.Format("Heroes: {0}/{1} [ {2}/{3} ]",
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Lord),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Hero),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            Interface.main.armyUnits.Content = String.Format("Units: {0}/{1}/{2} [ {3}+/{4}/{5} ]",
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Core),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Special),
                Army.Params.GetArmyUnitsNumber(Unit.UnitType.Rare),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            Interface.main.armyPoints.Text = String.Format("Points: {0} [ {1} ]", Army.Params.GetArmyPoints(), Army.Params.GetArmyMaxPoints());
            Interface.main.armySize.Text = String.Format("Models: {0}", Army.Params.GetArmySize());
            Interface.main.armyCasting.Content = String.Format("Cast: {0}", Army.Params.GetArmyCast());
            Interface.main.armyDispell.Content = String.Format("Dispell: {0}", Army.Params.GetArmyDispell());
        }

        public static void LoadArmySize(int points, string armyName)
        {
            Army.Data.MaxPoints = points;
            Army.Data.AdditionalName = armyName;

            string windowsHeaderWithName = "Warhammer Army Assembler (WAAgh)";
            Interface.main.dragWindowBottom.Content = windowsHeaderWithName +
                (String.IsNullOrWhiteSpace(armyName) ? String.Empty : String.Format(" // {0}", armyName));

            ArmyBookLoad.LoadArmy(Interface.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            Interface.DetailResize(open: false);
            Interface.Move(Interface.MovingType.ToMain);
        }
    }
}
