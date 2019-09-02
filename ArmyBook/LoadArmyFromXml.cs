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

            newUnit.Type = TypeParse(xmlUnit["Name"]);
            newUnit.Name = xmlUnit["Type"].InnerText;

            XmlNode mainParam = xmlUnit["MainParam"];

            newUnit.Movement = IntParse(mainParam["M"]);
            newUnit.WeaponSkill = IntParse(mainParam["WS"]);
            newUnit.BallisticSkill = IntParse(mainParam["BS"]);
            newUnit.Strength = IntParse(mainParam["S"]);
            newUnit.Toughness = IntParse(mainParam["T"]);
            newUnit.Wounds = IntParse(mainParam["W"]);
            newUnit.Initiative = IntParse(mainParam["I"]);
            newUnit.Attacks = IntParse(mainParam["A"]);
            newUnit.Leadership = IntParse(mainParam["LD"]);

            XmlNode psychology = xmlUnit["Psychology"];

            newUnit.ImmuneToPsychology = BoolParse(mainParam["ImmuneToPsychology"]);
            newUnit.Stubborn = BoolParse(mainParam["Stubborn"]);
            newUnit.Hate = BoolParse(mainParam["Hate"]);
            newUnit.Fear = BoolParse(mainParam["Fear"]);
            newUnit.Terror = BoolParse(mainParam["Terror"]);
            newUnit.Frenzy = BoolParse(mainParam["Frenzy"]);
            newUnit.Unbreakable = BoolParse(mainParam["Unbreakable"]);
            newUnit.ColdBlooded = BoolParse(mainParam["ColdBlooded"]);

            XmlNode additionalParam = xmlUnit["AdditionalParam"];

            return newUnit;
        }

        private static int IntParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            int value = 0;

            bool success = Int32.TryParse(xmlNode.InnerText, out value);

            return (success ? value : 0);
        }

        private static UnitType TypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            UnitType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : UnitType.Core);
        }

        private static bool BoolParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return false;

            bool value;

            bool success = bool.TryParse(xmlNode.InnerText, out value);

            return (success ? value : false);
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
