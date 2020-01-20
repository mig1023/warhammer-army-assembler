using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class ArmyChecks
    {
        public static bool IsUnitExistInArmy(int unitID)
        {
            if (!Army.Units.ContainsKey(unitID))
                return false;

            return true;
        }

        public static bool IsUnitExistInArmyByArmyBookID(int UnitID)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.ID == UnitID)
                    return true;

            return false;
        }

        public static Dictionary<Unit.UnitType, double> UnitsPointsPercent()
        {
            Dictionary<Unit.UnitType, double> units = new Dictionary<Unit.UnitType, double>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                units[entry.Value.Type] += entry.Value.GetUnitPoints();

            return units;
        }

        public static Dictionary<Unit.UnitType, double> UnitsMaxPointsPercent()
        {
            return new Dictionary<Unit.UnitType, double>
            {
                [Unit.UnitType.Lord] = 0.25,
                [Unit.UnitType.Hero] = 0.25,
                [Unit.UnitType.Core] = 0.25,
                [Unit.UnitType.Special] = 0.50,
                [Unit.UnitType.Rare] = 0.25,
            };
        }

        public static bool IsArmyUnitsPointsPercentOk(Unit.UnitType type, double points)
        {
            Dictionary<Unit.UnitType, double> units = UnitsPointsPercent();

            int twentyFivePercent = (int)(Army.MaxPoints * 0.25);

            if (type == Unit.UnitType.Lord || type == Unit.UnitType.Hero || type == Unit.UnitType.Rare)
                return (units[type] + points > twentyFivePercent ? false : true);

            if (type == Unit.UnitType.Special)
                return (units[type] + points > (twentyFivePercent * 2) ? false : true);

            return true;
        }

        public static bool IsArmyDublicationOk(Unit unit)
        {
            int alreadyInArmy = 0;

            foreach (KeyValuePair<int, Unit> armyUnit in Army.Units)
                if (armyUnit.Value.ID == unit.ID)
                    alreadyInArmy += 1;

            int limitForArmy = -1;

            if (unit.Type == Unit.UnitType.Special)
                limitForArmy = (Army.MaxPoints >= 3000 ? 6 : 3);
            else if (unit.Type == Unit.UnitType.Rare)
                limitForArmy = (Army.MaxPoints >= 3000 ? 4 : 2);

            return (limitForArmy < 0 ? true : (alreadyInArmy < limitForArmy));
        }

        public static int IsOptionAlreadyUsed(string optionName)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                foreach (Option option in entry.Value.Options)
                    if ((option.Name == optionName) && option.Realised)
                        return entry.Key;

            return 0;
        }
    }
}
