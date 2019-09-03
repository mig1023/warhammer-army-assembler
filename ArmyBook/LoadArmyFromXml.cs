﻿using System;
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
                ArmyBook.Units.Add(GetUnitID(xmlUnit), LoadUnit(xmlUnit));
        }

        public static string GetUnitID(XmlNode xmlUnit)
        {
            XmlNode genaralParam = xmlUnit["General"];

            return genaralParam["ID"].InnerText;
        }

        public static Unit LoadUnit(XmlNode xmlUnit)
        {
            Unit newUnit = new Unit();

            XmlNode generalParam = xmlUnit["General"];

            newUnit.Type = TypeParse(generalParam["Name"]);
            newUnit.Name = generalParam["Type"].InnerText;
            newUnit.Price = IntParse(generalParam["Price"]);

            XmlNode mainParam = xmlUnit["MainParam"];

            newUnit.Movement = IntParse(mainParam["Movement"]);
            newUnit.WeaponSkill = IntParse(mainParam["WeaponSkill"]);
            newUnit.BallisticSkill = IntParse(mainParam["BallisticSkill"]);
            newUnit.Strength = IntParse(mainParam["Strength"]);
            newUnit.Toughness = IntParse(mainParam["Toughness"]);
            newUnit.Wounds = IntParse(mainParam["Wounds"]);
            newUnit.Initiative = IntParse(mainParam["Initiative"]);
            newUnit.Attacks = IntParse(mainParam["Attacks"]);
            newUnit.Leadership = IntParse(mainParam["Leadership"]);

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

            newUnit.HitFirst = BoolParse(additionalParam["HitFirst"]);
            newUnit.Regeneration = BoolParse(additionalParam["Regeneration"]);
            newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
            newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);

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
