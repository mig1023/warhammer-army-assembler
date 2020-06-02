using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WarhammerArmyAssembler.ArmyBook
{
    class XmlBook
    {
        public static string GetXmlArmyBooks(bool next = false, bool prev = false)
        {
            List<string> allXmlFiles = FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory);

            string newArmyList = Interface.CurrentSelectedArmy ?? allXmlFiles[0];

            string firstFile = allXmlFiles[0];
            string prevFile = String.Empty;
            string lastFile = allXmlFiles[allXmlFiles.Count - 1];
            bool nextName = false;

            int indexName = 0;

            foreach (string xmlName in allXmlFiles)
            {
                XmlDocument xmlFile = new XmlDocument();
                xmlFile.Load(xmlName);

                XmlNode armyName = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName");

                if (nextName)
                    return xmlName;

                if ((newArmyList == xmlName) && prev && String.IsNullOrEmpty(prevFile))
                    return lastFile;
                else if ((newArmyList == xmlName) && prev)
                    return prevFile;

                if ((newArmyList == xmlName) && next && (indexName >= allXmlFiles.Count))
                    return firstFile;
                else if ((newArmyList == xmlName) && next)
                    nextName = true;
                else if (newArmyList == xmlName)
                    return xmlName;

                prevFile = xmlName;

                indexName += 1;
            }

            if (nextName)
                return firstFile;

            return String.Empty;
        }

        public static List<string> FindAllXmlFiles(string programDirectory)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(programDirectory))
                    if (file.EndsWith(".xml") && !file.Contains("itextsharp.xml"))
                        files.Add(file);

                foreach (string directory in Directory.GetDirectories(programDirectory))
                    files.AddRange(FindAllXmlFiles(directory));
            }
            catch
            {
                return new List<string>();
            }

            return files;
        }
    }
}
