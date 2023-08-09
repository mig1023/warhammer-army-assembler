using System;
using System.IO;

namespace WarhammerArmyAssembler.Export
{
    class Services
    {
        static string ARMYLIST_DIR = "ArmyLists";

        public static string GetFileName(string fileType)
        {
            ExportDirectory();

            int fileIndex = 0;
            string fileName = String.Empty;

            while (File.Exists(NewFileName(fileIndex, fileType, out fileName)))
                fileIndex += 1;

            return fileName;
        }

        public static string GetArmyName()
        {
            if (String.IsNullOrWhiteSpace(Army.Data.RosterName))
                return "warhammer fantasy battles";
            else
                return Army.Data.RosterName;
        }

        private static string NewFileName(int newIndex, string fileType, out string newFileName)
        {
            string name = ARMYLIST_DIR + '\\' +
                Army.Data.Name.Replace(" ", "_") +
                '_' +
                Army.Data.MaxPoints.ToString();

            name += (String.IsNullOrWhiteSpace(Army.Data.RosterName) ?
                String.Empty : '_' + Army.Data.RosterName.Replace(" ", "_"));

            string index = newIndex > 0 ? '_' + newIndex.ToString() : String.Empty;
            newFileName = name + index + '.' + fileType;

            return newFileName;
        }

        public static string GetUnitName(Unit unit) =>
            String.IsNullOrEmpty(unit.Personification) ? unit.Name : $"{unit.Personification} / {unit.Name}";

        private static void ExportDirectory()
        {
            if (!Directory.Exists(ARMYLIST_DIR))
                Directory.CreateDirectory(ARMYLIST_DIR);
        }

        static public string UnitSizeIfNeed(Unit unit)
        {
            string unitSize = unit.Size.ToString();

            foreach (Option option in unit.Options)
            {
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
