using System;
using System.IO;

namespace WarhammerArmyAssembler.Export
{
    class File
    {
        static string ARMYLIST_DIR = "ArmyLists";

        public static string GetName(string fileType)
        {
            if (!Directory.Exists(ARMYLIST_DIR))
                Directory.CreateDirectory(ARMYLIST_DIR);

            int fileIndex = 0;
            string fileName = String.Empty;

            while (System.IO.File.Exists(NewName(fileIndex, fileType, out fileName)))
                fileIndex += 1;

            return fileName;
        }

        private static string NewName(int newIndex, string fileType, out string newFileName)
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
    }
}
