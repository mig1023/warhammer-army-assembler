using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Army
{
    class Checks
    {
        public static bool IsUnitExistInArmy(int unitID)
        {
            if (!Army.Data.Units.ContainsKey(unitID))
                return false;

            return true;
        }

        public static bool IsUnitExistInArmyByArmyBookID(int UnitID)
        {
            foreach (Unit entry in Army.Data.Units.Values)
                if (entry.ID == UnitID)
                    return true;

            return false;
        }

        public static bool IsRunicCombinationAlreadyExist(Unit unit, Option newOption)
        {
            Dictionary<string, int> currentCombination = new Dictionary<string, int> {};

            if (newOption != null)
                currentCombination.Add(newOption.Name, 1);

            foreach (Option option in unit.Options)
                if (option.MasterRunic)
                    currentCombination.Add(option.Name, 1);
                else if ((option.Runic > 0) && currentCombination.ContainsKey(option.Name))
                    currentCombination[option.Name] += option.Runic;
                else if (option.Runic > 0)
                    currentCombination.Add(option.Name, option.Runic);

            foreach (Unit entry in Army.Data.Units.Values)
                if (entry.ExistsRunicCombinationInUnit(currentCombination))
                    return true;

            return false;
        }

        public static Dictionary<Unit.UnitType, double> UnitsPointsPercent()
        {
            Dictionary<Unit.UnitType, double> units = new Dictionary<Unit.UnitType, double>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (Unit entry in Army.Data.Units.Values)
                units[entry.Type] += entry.GetUnitPoints();

            return units;
        }

        public static Dictionary<Unit.UnitType, double> UnitsMaxPointsPercent() => new Dictionary<Unit.UnitType, double>
        {
            [Unit.UnitType.Lord] = 0.25,
            [Unit.UnitType.Hero] = 0.25,
            [Unit.UnitType.Core] = 0.25,
            [Unit.UnitType.Special] = 0.50,
            [Unit.UnitType.Rare] = 0.25,
        };

        public static bool IsArmyUnitsPointsPercentOk(Unit.UnitType type, double points)
        {
            Dictionary<Unit.UnitType, double> units = UnitsPointsPercent();

            int twentyFivePercent = (int)(Army.Data.MaxPoints * 0.25);

            if (type == Unit.UnitType.Lord || type == Unit.UnitType.Hero || type == Unit.UnitType.Rare)
                return (units[type] + points <= twentyFivePercent);

            if (type == Unit.UnitType.Special)
                return units[type] + points <= (twentyFivePercent * 2);

            return true;
        }

        public static bool IsArmyDublicationOk(Unit unit)
        {
            int alreadyInArmy = 0;

            foreach (Unit armyUnit in Army.Data.Units.Values)
                if (armyUnit.ID == unit.ID)
                    alreadyInArmy += 1;

            int limitForArmy = -1;

            if (unit.Type == Unit.UnitType.Special)
                limitForArmy = (Army.Data.MaxPoints >= 3000 ? 6 : 3);
            else if (unit.Type == Unit.UnitType.Rare)
                limitForArmy = (Army.Data.MaxPoints >= 3000 ? 4 : 2);

            return (limitForArmy < 0 ? true : (alreadyInArmy < limitForArmy));
        }
        
        public static bool IsArmyUnitMaxLimitOk(Unit newUnit)
        {
            if (!newUnit.UniqueUnits)
                return true;

            foreach (Unit unit in Army.Data.Units.Values)
                if (unit.ID == newUnit.ID)
                    return false;

            return true;
        }

        public static int IsOptionAlreadyUsed(string optionName, int requestFromUnit, string unitName, bool byUnitType)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
                if ((entry.Key != requestFromUnit) && (!byUnitType || (entry.Value.Name == unitName)))
                    foreach (Option option in entry.Value.Options)
                        if ((option.Name == optionName) && (option.Realised || option.IsMagicItem()))
                            return entry.Key;

            return 0;
        }

        public static string ArmyProblems()
        {
            bool problem = (Army.Params.GetArmyUnitsNumber(Unit.UnitType.Core) < Army.Params.GetArmyMaxUnitsNumber(Unit.UnitType.Core));

            return (problem ? "Not enough core unit in army" : String.Empty);
        } 

        public static bool IsArmyValid()
        {
            string armyProblems = ArmyProblems();

            return String.IsNullOrEmpty(armyProblems);
        }

        public static bool IsArmyFullForTypeIcrease(Unit unit)
        {
            Unit.UnitType type;

            if (unit.Type == Unit.UnitType.Rare)
                return true;

            else if (unit.Type == Unit.UnitType.Special)
                type = Unit.UnitType.Rare;

            else if (unit.Type == Unit.UnitType.Core)
                type = Unit.UnitType.Special;

            else
                return false;

            return Army.Params.GetArmyUnitsNumber(type) >= Army.Params.GetArmyMaxUnitsNumber(type);
        }
    }
}
