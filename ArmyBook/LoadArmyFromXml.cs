using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WarhammerArmyAssembler.Units;

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

        public static IUnit LoadUnit(XmlNode xmlUnit)
        {
            Unit newUnit;

            if (xmlUnit["Type"].InnerText == "Rare")
                newUnit = new Rare();
            else if (xmlUnit["Type"].InnerText == "Special")
                newUnit = new Special();
            else
                newUnit = new Core();

            newUnit.Name = xmlUnit["Name"].InnerText;

            newUnit.Movement = Int32.Parse(xmlUnit["M"].InnerText);
            newUnit.WeaponSkill = Int32.Parse(xmlUnit["WS"].InnerText);
            newUnit.BallisticSkill = Int32.Parse(xmlUnit["BS"].InnerText);
            newUnit.Strength = Int32.Parse(xmlUnit["S"].InnerText);
            newUnit.Toughness = Int32.Parse(xmlUnit["T"].InnerText);
            newUnit.Wounds = Int32.Parse(xmlUnit["W"].InnerText);
            newUnit.Initiative = Int32.Parse(xmlUnit["I"].InnerText);
            newUnit.Attacks = Int32.Parse(xmlUnit["A"].InnerText);
            newUnit.Leadership = Int32.Parse(xmlUnit["LD"].InnerText);

            return newUnit;
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
