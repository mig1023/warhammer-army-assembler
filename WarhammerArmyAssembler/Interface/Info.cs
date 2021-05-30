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

            double armyCurrentPoint = Army.Params.GetArmyPoints();
            double availablePoints = (Army.Params.GetArmyMaxPoints() - armyCurrentPoint);

            string pointsMsg = String.Format("All points:\t\t{0} pts\n\nAlready used:\t\t{1} pts / {2}%\n\nAvailable:\t\t{3} pts / {4}%\n\n\n\n",
                Army.Params.GetArmyMaxPoints(),
                armyCurrentPoint, Interface.Other.CalcPercent(armyCurrentPoint, Army.Params.GetArmyMaxPoints()),
                availablePoints, Interface.Other.CalcPercent(availablePoints, Army.Params.GetArmyMaxPoints()));

            foreach (KeyValuePair<Unit.UnitType, double> entry in Army.Checks.UnitsMaxPointsPercent())
                pointsMsg += String.Format("{0}:\t{1,10} pts / {2}%\t( {3} {4} pts / {5}% )\n\n",
                    entry.Key,
                    units[entry.Key],
                    Interface.Other.CalcPercent(units[entry.Key], Army.Data.MaxPoints),
                    (entry.Key == Unit.UnitType.Core ? "min" : "max"),
                    (int)(Army.Data.MaxPoints * entry.Value), entry.Value * 100
                );

            return pointsMsg;
        }

        public static string armyUnits() => String.Format("CORE UNITS:\n{0}\n\nSPECIAL UNITS:\n{1}\n\nRARE UNITS:\n{2}",
            UnitsByType(Unit.UnitType.Core), UnitsByType(Unit.UnitType.Special), UnitsByType(Unit.UnitType.Rare));

        public static string armyHeroes() => String.Format("LORDS:\n{0}\n\nHEROES:\n{1}",
            UnitsByType(Unit.UnitType.Lord), UnitsByType(Unit.UnitType.Hero));

        public static string armyModels() => String.Format("Normal base:\t{0}\n\nCavalry base:\t{1}\n\nLarge base:\t{2}",
            UnitsByBase(Army.Params.BasesTypes.normal), UnitsByBase(Army.Params.BasesTypes.cavalry), UnitsByBase(Army.Params.BasesTypes.large));

        private static string UnitsByType(Unit.UnitType u) => Army.Params.GetUnitsListByType(u);

        private static int UnitsByBase(Army.Params.BasesTypes u) => Army.Params.GetUnitsNumberByBase(u);
    }
}
