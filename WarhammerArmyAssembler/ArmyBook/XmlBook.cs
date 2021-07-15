using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace WarhammerArmyAssembler.ArmyBook
{
    class XmlBook
    {
        public static string GetXmlArmyBooks(bool next = false, bool prev = false)
        {
            List<string> allXmlFiles = FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory);

            string newArmyList = Interface.Changes.CurrentSelectedArmy ?? allXmlFiles[0];

            string firstFile = allXmlFiles[0];
            string prevFile = String.Empty;
            string lastFile = allXmlFiles[allXmlFiles.Count - 1];
            bool nextName = false;

            int indexName = 0;

            foreach (string xmlName in allXmlFiles)
            {
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

        public static List<string> FindAllXmlFiles(string programDirectory) => FindAllXmlFilesInDirectories(programDirectory).Values.ToList();

        private static SortedDictionary<string, string> FindAllXmlFilesInDirectories(string programDirectory)
        {
            SortedDictionary<string, string> files = new SortedDictionary<string, string>();

            try
            {
                foreach (string file in Directory.GetFiles(programDirectory).Where(x => x.EndsWith("ed.xml")))
                {
                    XmlDocument xmlFile = new XmlDocument();

                    try
                    {
                        xmlFile.Load(file);
                    }
                    catch (System.Xml.XmlException)
                    {
                        continue;
                    }

                    XmlNode armyName = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName");

                    if (armyName == null)
                        continue;

                    XmlNode armyVersion = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion");
                    XmlNode orderName = xmlFile.SelectSingleNode("ArmyBook/Info/OrderName");

                    string armyOrderName = (orderName == null ? armyName.InnerText : orderName.InnerText);

                    if (ChangeArmybookWindow.sortedByEditions)
                        armyOrderName = armyVersion.InnerText + armyOrderName;
                    else
                        armyOrderName += armyVersion.InnerText;

                    files.Add(armyOrderName, file);
                }

                foreach (string directory in Directory.GetDirectories(programDirectory))
                    foreach (KeyValuePair<string, string> file in FindAllXmlFilesInDirectories(directory))
                        files.Add(file.Key, file.Value);
            }
            catch
            {
                return new SortedDictionary<string, string>();
            }

            return files;
        }
    }
}
