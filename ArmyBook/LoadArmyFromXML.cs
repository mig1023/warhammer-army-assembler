using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WarhammerArmyAssembler.ArmyBook
{
    class LoadArmyFromXml
    {
        public static List<string> GetAllXmlArmyBooks()
        {
            List<string> allXmlFiles = FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory);

            List<string> allXmlArmyBooks = new List<string>();

            foreach(string xmlName in allXmlFiles)
            {
                XmlDocument xmlFile = new XmlDocument();
                xmlFile.Load(xmlName);

                XmlNode armyName = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName");
                allXmlArmyBooks.Add(armyName.InnerText);
            }

            return allXmlArmyBooks;
        }

        public static List<string> FindAllXmlFiles(string s)
        {
            List<string> files = new List<string>(); 
            try
            {
                foreach(string file in Directory.GetFiles(s))
                    if (file.EndsWith(".xml"))
                        files.Add(file);

                foreach (string directory in Directory.GetDirectories(s))
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
