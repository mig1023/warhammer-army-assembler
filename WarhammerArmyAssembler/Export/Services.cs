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

        public static string GetArmyName() =>
            String.IsNullOrWhiteSpace(Army.Data.RosterName) ? "warhammer fantasy battles" : Army.Data.RosterName;

        private static string NewFileName(int newIndex, string fileType, out string newFileName)
        {
            string name = ARMYLIST_DIR + '\\' + Army.Data.Name.Replace(" ", "_") + '_' + Army.Data.MaxPoints.ToString();
            name += (String.IsNullOrWhiteSpace(Army.Data.RosterName) ? String.Empty : '_' + Army.Data.RosterName.Replace(" ", "_"));
            newFileName = name + (newIndex > 0 ? '_' + newIndex.ToString() : String.Empty) + '.' + fileType;

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
                if ((option.Countable != null) && (option.Countable.ExportToUnitSize) && (option.Countable.Value > 0))
                    unitSize += String.Format("+{0}", option.Countable.Value.ToString());

            return (unit.IsHeroOrHisMount() ? String.Empty : unitSize + ' ');
        }

        static public string UnitPointsLine(Unit unit) =>
            (unit.GetUnitPoints() > 0 ? String.Format(" ({0} pts)", unit.GetUnitPoints()) : String.Empty);

        static public string AllArmyName() => String.Format("{0} // {1}", Army.Data.Name, GetArmyName());

        static public string AllArmyPointsAndEdition() =>
            String.Format("{0} pts, {1}th Edition", Army.Data.MaxPoints, Army.Data.ArmyEdition);
    }
}
