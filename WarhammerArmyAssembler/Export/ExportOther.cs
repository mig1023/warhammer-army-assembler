using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class ExportOther
    {
        static string ARMYLIST_DIR = "armylists";

        public static string GetFileName()
        {
            ExportDirectory();

            int fileIndex = 0;

            string fileName = String.Empty;

            while (File.Exists(NewFileName(fileIndex, out fileName)))
                fileIndex += 1;

            return fileName;
        }

        private static string NewFileName(int newIndex, out string newFileName)
        {
            string name = ARMYLIST_DIR + '\\' + Army.ArmyName.Replace(" ", "_") + '_' + Army.MaxPoints.ToString();
            newFileName = name + (newIndex > 0 ? '_' + newIndex.ToString() : String.Empty) + ".pdf";

            return newFileName;
        }

        private static void ExportDirectory()
        {
            if (!Directory.Exists(ARMYLIST_DIR))
                Directory.CreateDirectory(ARMYLIST_DIR);
        }
    }
}
