using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Interface
{
    class Info
    {
        public static string armyPoints()
        {
            Dictionary<Unit.UnitType, double> units = Army.Checks.UnitsPointsPercent();
            Dictionary<Unit.UnitType, double> unitPercents = Army.Checks.UnitsMaxPointsPercent();

            double armyCurrentPoint = Army.Params.GetArmyPoints();
            double availablePoints = (Army.Params.GetArmyMaxPoints() - armyCurrentPoint);

            string pointsMsg = String.Format("All points:\t\t{0} pts\n\nAlready used:\t\t{1} pts / {2}%\n\nAvailable:\t\t{3} pts / {4}%\n\n\n\n",
                Army.Params.GetArmyMaxPoints(),
                armyCurrentPoint, Interface.Other.CalcPercent(armyCurrentPoint, Army.Params.GetArmyMaxPoints()),
                availablePoints, Interface.Other.CalcPercent(availablePoints, Army.Params.GetArmyMaxPoints()));

            foreach (KeyValuePair<Unit.UnitType, double> entry in unitPercents)
                pointsMsg += String.Format("{0}:\t{1,10} pts / {2}%\t( {3} {4} pts / {5}% )\n\n",
                    entry.Key, units[entry.Key], Interface.Other.CalcPercent(units[entry.Key],
                    Army.Data.MaxPoints), (entry.Key == Unit.UnitType.Core ? "min" : "max"),
                    (int)(Army.Data.MaxPoints * entry.Value), entry.Value * 100);

            return pointsMsg;
        }

        public static string armyUnits()
        {
            string unitsMsg = String.Format("CORE UNITS:\n{0}\n\nSPECIAL UNITS:\n{1}\n\nRARE UNITS:\n{2}",
                Army.Params.GetUnitsListByType(Unit.UnitType.Core),
                Army.Params.GetUnitsListByType(Unit.UnitType.Special),
                Army.Params.GetUnitsListByType(Unit.UnitType.Rare));

            return unitsMsg;
        }

        public static string armyHeroes()
        {
            string heroMsg = String.Format("LORDS:\n{0}\n\nHEROES:\n{1}",
                Army.Params.GetUnitsListByType(Unit.UnitType.Lord),
                Army.Params.GetUnitsListByType(Unit.UnitType.Hero)
            );

            return heroMsg;
        }

        public static string armyModels()
        {
            string baseMsg = String.Format("Normal base:\t{0}\n\nCavalry base:\t{1}\n\nLarge base:\t{2}",
                Army.Params.GetUnitsNumberByBase(Army.Params.BasesTypes.normal),
                Army.Params.GetUnitsNumberByBase(Army.Params.BasesTypes.cavalry),
                Army.Params.GetUnitsNumberByBase(Army.Params.BasesTypes.large)
            );

            return baseMsg;
        }
    }
}
