using System;
using System.IO;

namespace WarhammerArmyAssembler
{
    class ExportOther
    {
        static string ARMYLIST_DIR = "armylists";

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
            return (String.IsNullOrWhiteSpace(Army.Data.AdditionalName) ? "warhammer fantasy battles" : Army.Data.AdditionalName);
        }

        private static string NewFileName(int newIndex, string fileType, out string newFileName)
        {
            string name = ARMYLIST_DIR + '\\' + Army.Data.Name.Replace(" ", "_") + '_' + Army.Data.MaxPoints.ToString();
            name += (String.IsNullOrWhiteSpace(Army.Data.AdditionalName) ? String.Empty : '_' + Army.Data.AdditionalName.Replace(" ", "_"));
            newFileName = name + (newIndex > 0 ? '_' + newIndex.ToString() : String.Empty) + '.' + fileType;

            return newFileName;
        }

        private static void ExportDirectory()
        {
            if (!Directory.Exists(ARMYLIST_DIR))
                Directory.CreateDirectory(ARMYLIST_DIR);
        }

        static public string UnitSizeIfNeed(Unit unit)
        {
            return (unit.IsHeroOrHisMount() ? String.Empty : unit.Size.ToString() + ' ');
        }
    }
}
