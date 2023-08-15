using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Army
{
    class Checks
    {
        public static bool IsUnitExistInArmy(int unitID) =>
            Data.Units.ContainsKey(unitID);

        public static bool ThisIsAftefact(int id) =>
            ArmyBook.Data.Artefact.ContainsKey(id);

        public static bool IsUnitExistInArmyByArmyBookID(int UnitID) =>
            Data.Units.Values.Where(x => x.ID == UnitID).FirstOrDefault() != null;

        public static bool IsRunicCombinationAlreadyExist(Unit unit, Option newOption)
        {
            Dictionary<string, int> currentCombination = new Dictionary<string, int> {};

            if (newOption != null)
                currentCombination.Add(newOption.Name, 1);

            foreach (Option option in unit.Options)
            {
                if (option.MasterRunic)
                    currentCombination.Add(option.Name, 1);

                else if ((option.Runic > 0) && currentCombination.ContainsKey(option.Name))
                    currentCombination[option.Name] += option.Runic;

                else if (option.Runic > 0)
                    currentCombination.Add(option.Name, option.Runic);
            }

            if (Data.Units.Values.Where(x => x.ExistsRunicCombinationInUnit(currentCombination)).Count() > 0)
                return true;

            return false;
        }

        public static Dictionary<Unit.UnitType, double> UnitsPointsPercent()
        {
            Dictionary<Unit.UnitType, double> units = new Dictionary<Unit.UnitType, double>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (Unit entry in Data.Units.Values)
                units[entry.Type] += entry.GetUnitPoints();

            return units;
        }

        public static Dictionary<Unit.UnitType, double> UnitsMaxPointsPercent() => new Dictionary<Unit.UnitType, double>
        {
            [Unit.UnitType.Lord] = ArmyBook.Constants.Quarter,
            [Unit.UnitType.Hero] = ArmyBook.Constants.Quarter,
            [Unit.UnitType.Core] = ArmyBook.Constants.Quarter,
            [Unit.UnitType.Special] = ArmyBook.Constants.Hulf,
            [Unit.UnitType.Rare] = ArmyBook.Constants.Quarter,
        };

        public static bool IsArmyUnitsPointsPercentOk(Unit.UnitType type, double points, double prepayment)
        {
            Dictionary<Unit.UnitType, double> units = UnitsPointsPercent();

            int twentyFivePercent = (int)(Data.MaxPoints * ArmyBook.Constants.Quarter);

            if (type == Unit.UnitType.Lord || type == Unit.UnitType.Hero || type == Unit.UnitType.Rare)
                return units[type] + points + prepayment <= twentyFivePercent;

            if (type == Unit.UnitType.Special)
                return units[type] + points + prepayment <= (twentyFivePercent * 2);

            return true;
        }

        public static bool IsArmyDublicationOk(Unit unit)
        {
            int alreadyInArmy = Data.Units.Values.Where(x => x.ID == unit.ID).Count();

            int limitForArmy = -1;

            if (unit.Type == Unit.UnitType.Special)
                limitForArmy = Data.MaxPoints >= ArmyBook.Constants.LargeArmySize ? 6 : 3;

            else if (unit.Type == Unit.UnitType.Rare)
                limitForArmy = Data.MaxPoints >= ArmyBook.Constants.LargeArmySize ? 4 : 2;

            return limitForArmy < 0 ? true : (alreadyInArmy < limitForArmy);
        }

        public static bool IsArmyUnitMaxLimitOk(Unit newUnit) =>
            !newUnit.Singleton || !(Data.Units.Values.Where(x => x.ID == newUnit.ID).FirstOrDefault() != null);

        public static int OptionsAlreadyUsed(string optionName, int requestFromUnit, string unitName, bool byUnitType)
        {
            foreach (KeyValuePair<int, Unit> entry in Data.Units)
            {
                if ((entry.Key == requestFromUnit) || (byUnitType && (entry.Value.Name != unitName)))
                    continue;

                foreach (Option option in entry.Value.Options)
                {
                    if ((option.Name == optionName) && (option.Realised || option.IsMagicItem()))
                        return entry.Key;
                }
            }

            return 0;
        }

        public static string ArmyProblems()
        {
            if (Params.GetArmyUnitsNumber(Unit.UnitType.Core) < Params.GetArmyMaxUnitsNumber(Unit.UnitType.Core))
                return "Not enough core unit in army";

            else if (Params.GetArmyUnitsNumber(Unit.UnitType.Hero) + Params.GetArmyUnitsNumber(Unit.UnitType.Lord) <= 0)
                return "Not enough characters in army";

            else
                return String.Empty;
        }

        public static bool IsArmyValid() =>
            String.IsNullOrEmpty(ArmyProblems());

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

            return Params.GetArmyUnitsNumber(type) >= Params.GetArmyMaxUnitsNumber(type);
        }
    }
}
