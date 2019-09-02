using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WarhammerArmyAssembler.Units;
using static WarhammerArmyAssembler.Units.Unit;

namespace WarhammerArmyAssembler.ArmyBook
{
    class LoadArmyFromXml
    {

        public static void LoadArmy(string xmlFileName)
        {
            ArmyBook.Units.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            foreach (XmlNode xmlUnit in xmlFile.SelectNodes("ArmyBook/Units/Unit"))
                ArmyBook.Units.Add(xmlUnit["ID"].InnerText, LoadUnit(xmlUnit));
        }

        public static Unit LoadUnit(XmlNode xmlUnit)
        {
            Unit newUnit = new Unit();

            newUnit.Type = TypeParse(xmlUnit["Name"].InnerText);
            newUnit.Name = xmlUnit["Type"].InnerText;

            newUnit.Movement = IntParse(xmlUnit["M"].InnerText);
            newUnit.WeaponSkill = IntParse(xmlUnit["WS"].InnerText);
            newUnit.BallisticSkill = IntParse(xmlUnit["BS"].InnerText);
            newUnit.Strength = IntParse(xmlUnit["S"].InnerText);
            newUnit.Toughness = IntParse(xmlUnit["T"].InnerText);
            newUnit.Wounds = IntParse(xmlUnit["W"].InnerText);
            newUnit.Initiative = IntParse(xmlUnit["I"].InnerText);
            newUnit.Attacks = IntParse(xmlUnit["A"].InnerText);
            newUnit.Leadership = IntParse(xmlUnit["LD"].InnerText);

            return newUnit;
        }

        private static int IntParse(string xmlText)
        {
            return Int32.Parse(xmlText);
        }

        private static UnitType TypeParse(string xmlText)
        {
            UnitType Type;

            Enum.TryParse(xmlText, out Type);

            return Type;
        }

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

        public static List<string> FindAllXmlFiles(string programDirectory)
        {
            List<string> files = new List<string>(); 
            try
            {
                foreach(string file in Directory.GetFiles(programDirectory))
                    if (file.EndsWith(".xml"))
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
