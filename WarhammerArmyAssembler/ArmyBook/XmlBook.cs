using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace WarhammerArmyAssembler.ArmyBook
{
    class XmlBook
    {
        private static Dictionary<string, List<string>> HomologousSeries = new Dictionary<string, List<string>>();

        private static void AddHomologue(string head, string file)
        {
            if (HomologousSeries.ContainsKey(head))
                HomologousSeries[head].Add(file);
            else
                HomologousSeries.Add(head, new List<string> { file });
        }

        public static List<string> GetHomologue(string army, string unit, bool isHero)
        {
            List<string> images = new List<string>();

            foreach (string homologue in HomologousSeries[army])
            {
                string homologueImage = Load.LoadArmyUnitImageOnly(homologue, unit, isHero);

                if (!String.IsNullOrEmpty(homologueImage))
                    images.Add(homologueImage);
            }

            return images;
        }

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

        public static List<string> FindAllXmlFiles(string programDirectory) =>
            FindAllXmlFilesInDirectories(programDirectory).Values.ToList();

        private static SortedDictionary<string, string> FindAllXmlFilesInDirectories(string programDirectory)
        {
            Constants.CommonXmlOptionPath = Directory
                .GetFiles(programDirectory, "CommonXmlOption.xml", SearchOption.AllDirectories)
                .ToList()
                .FirstOrDefault();

            SortedDictionary<string, string> files = new SortedDictionary<string, string>();

            try
            {
                foreach (string file in Directory.GetFiles(programDirectory, "*ed.xml", SearchOption.AllDirectories))
                {
                    XmlDocument xmlFile = new XmlDocument();

                    try
                    {
                        xmlFile.Load(file);
                    }
                    catch (XmlException)
                    {
                        continue;
                    }

                    XmlNode armyName = Services.Intro(xmlFile, "Name");

                    if (armyName == null)
                        continue;

                    XmlNode armyInternalName = Services.Intro(xmlFile, "InternalName");
                    XmlNode armyEdition = Services.Intro(xmlFile, "Edition");
                    string armyOrderName = armyInternalName.InnerText;

                    if (ChangeArmybookWindow.sortedByEditions)
                        armyOrderName = armyEdition.InnerText + armyOrderName;
                    else
                        armyOrderName += armyEdition.InnerText;

                    files.Add(armyOrderName, file);
                    AddHomologue(armyInternalName.InnerText, file);
                }
            }
            catch
            {
                return new SortedDictionary<string, string>();
            }

            return files;
        }
    }
}
