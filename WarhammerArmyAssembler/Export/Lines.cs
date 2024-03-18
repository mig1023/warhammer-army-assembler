using System;

namespace WarhammerArmyAssembler.Export
{
    class Lines
    {
        public static string GetArmyName()
        {
            if (String.IsNullOrWhiteSpace(Army.Data.RosterName))
                return "warhammer fantasy battles";
            else
                return Army.Data.RosterName;
        }

        public static string GetUnitName(Unit unit) =>
            String.IsNullOrEmpty(unit.Personification) ? unit.Name : $"{unit.Personification} / {unit.Name}";

        static public string UnitSizeIfNeed(Unit unit)
        {
            string unitSize = unit.Size.ToString();

            foreach (Option option in unit.Options)
            {
                if (option.Countable == null)
                    continue;

                bool exportSize = option.Countable.ExportToUnitSize;
                bool isValued = option.Countable.Value > 0;

                if ((option.Countable != null) && exportSize && isValued)
                    unitSize += $"+{option.Countable.Value}";
            }

            return (unit.IsHeroOrHisMount() ? String.Empty : unitSize + ' ');
        }

        static public string UnitPointsLine(Unit unit) =>
            unit.GetUnitPoints() > 0 ? $" ({unit.GetUnitPoints()} pts)" : String.Empty;

        static public string AllArmyName() =>
            $"{Army.Data.Name} // {GetArmyName()}";

        static public string AllArmyPointsAndEdition() =>
            $"{Army.Data.MaxPoints} pts, {Army.Data.ArmyEdition}th Edition";
    }
}
