using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Windows;

namespace WarhammerArmyAssembler.ArmyBook
{
    class XmlBook
    {
        private static Dictionary<string, List<string>> HomologousSeries = new Dictionary<string, List<string>>();

        private static void AddHomologue(string head, string file)
        {
            if (HomologousSeries.ContainsKey(head))
            {
                if (!HomologousSeries[head].Contains(file))
                    HomologousSeries[head].Add(file);
            }
            else
                HomologousSeries.Add(head, new List<string> { file });
        }

        public static List<string> GetHomologue(string army, string unit, string homologue, bool isHero)
        {
            List<string> images = new List<string>();

            if ((HomologousSeries.Count == 0) || (army == null))
                return images;

            IEnumerable<string> homologousSeries = HomologousSeries[army];

            foreach (string homologueName in homologousSeries.Reverse())
            {
                string homologueImage = Load.ArmyUnitImageOnly(homologueName, unit, homologue, isHero);

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

        public static string FindXmlFileByName(string directory, string name)
        {
            return Directory
                .GetFiles(directory, name, SearchOption.AllDirectories)
                .ToList()
                .FirstOrDefault();
        }

        private static SortedDictionary<string, string> FindAllXmlFilesInDirectories(string programDirectory)
        {
            Constants.CommonXmlOptionPath = FindXmlFileByName(programDirectory, "CommonXmlOption.xml");
            Constants.EnemiesOptionPath = FindXmlFileByName(programDirectory, "Enemies.xml");
            Constants.DogOfWarPath = FindXmlFileByName(programDirectory, "DogsOfWar.xml");

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
                    catch (XmlException ex)
                    {
                        MessageBox.Show(ex.Message, String.Format("{0} xml error", Path.GetFileName(file)));
                        continue;
                    }

                    XmlNode armyName = Services.Intro(xmlFile, "Info/Army");

                    if ((armyName == null) || (armyName.InnerText == "Dogs of War"))
                        continue;

                    XmlNode armyInternalName = Services.Intro(xmlFile, "InternalName");
                    XmlNode armyEdition = Services.Intro(xmlFile, "Info/Edition");
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
