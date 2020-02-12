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

            Interface.main.armyMainLabel.Content = Army.ArmyName;
            Interface.main.armyMainLabelPlace.Background = ArmyBook.MainColor;
            Interface.main.unitDetailHead.Background = ArmyBook.MainColor;

            Interface.main.armyMainMenu.Content = '\u2630';
            Interface.main.armyMainMenu.Foreground = Brushes.White;
            Interface.main.armyMainMenu.Background = ArmyBook.AdditionalColor;

            foreach(Label label in new List<Label> {
                Interface.main.armyMainLabel, Interface.main.toNewArmy, Interface.main.saveArmyToPDF
            })
            {
                label.Foreground = Brushes.White;
                label.Background = ArmyBook.MainColor;
            }

            Interface.main.saveArmyToPDF.Foreground = Brushes.White;
            Interface.main.saveArmyToPDF.Background = ArmyBook.MainColor;

            List<Unit> categories = ArmyParams.GetArmyCategories();

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

            List<Unit> categories = ArmyParams.GetArmyUnitsByCategories();

            foreach (Unit unitType in categories)
                foreach (Unit unit in unitType.Items)
                    Interface.ArmyInInterface.Add(unit);

            Interface.main.ArmyGrid.ItemsSource = Interface.ArmyInInterface;
            Interface.main.armyHeroes.Content = String.Format("Heroes: {0}/{1} [ {2}/{3} ]",
                ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Lord),
                ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Hero),
                ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Lord),
                ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Hero)
            );
            Interface.main.armyUnits.Content = String.Format("Units: {0}/{1}/{2} [ {3}+/{4}/{5} ]",
                ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Core),
                ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Special),
                ArmyParams.GetArmyUnitsNumber(Unit.UnitType.Rare),
                ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Core),
                ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Special),
                ArmyParams.GetArmyMaxUnitsNumber(Unit.UnitType.Rare)
            );
            Interface.main.armyPoints.Text = String.Format("Points: {0} [ {1} ]", ArmyParams.GetArmyPoints(), ArmyParams.GetArmyMaxPoints());
            Interface.main.armySize.Content = String.Format("Models: {0}", ArmyParams.GetArmySize());
            Interface.main.armyCasting.Content = String.Format("Cast: {0}", ArmyParams.GetArmyCast());
            Interface.main.armyDispell.Content = String.Format("Dispell: {0}", ArmyParams.GetArmyDispell());
        }

        public static void LoadArmySize(int points, bool onlyReload = false)
        {
            Army.MaxPoints = points;
            ArmyBookLoad.LoadArmy(Interface.CurrentSelectedArmy);

            LoadArmyList();
            ReloadArmyData();

            if (onlyReload)
                return;

            Interface.DetailResize(open: false);
            Interface.Move(Interface.MovingType.ToMain);
        }
    }
}
