using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler
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

            newUnit.Name = generalParam["Name"].InnerText;
            newUnit.ID = generalParam["ID"].InnerText;
            newUnit.Type = TypeParse(generalParam["Type"]);
            newUnit.Points = IntParse(generalParam["Points"]);
            newUnit.Size = IntParse(generalParam["MinSize"]);

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

            newUnit.ImmuneToPsychology = BoolParse(psychology["ImmuneToPsychology"]);
            newUnit.Stubborn = BoolParse(psychology["Stubborn"]);
            newUnit.Hate = BoolParse(psychology["Hate"]);
            newUnit.Fear = BoolParse(psychology["Fear"]);
            newUnit.Terror = BoolParse(psychology["Terror"]);
            newUnit.Frenzy = BoolParse(psychology["Frenzy"]);
            newUnit.Unbreakable = BoolParse(psychology["Unbreakable"]);
            newUnit.ColdBlooded = BoolParse(psychology["ColdBlooded"]);

            XmlNode additionalParam = xmlUnit["AdditionalParam"];

            newUnit.HitFirst = BoolParse(additionalParam["HitFirst"]);
            newUnit.Regeneration = BoolParse(additionalParam["Regeneration"]);
            newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
            newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Ammunition/*"))
                newUnit.Weapons.Add(LoadWeapon(xmlAmmunition));

            return newUnit;
        }

        private static int IntParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            int value = 0;

            bool success = int.TryParse(xmlNode.InnerText, out value);

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

            return (xmlNode.InnerText == "true" ? true : false);
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

        public static Ammunition LoadWeapon(XmlNode xmlNode)
        {
            Ammunition newWeapon = new Ammunition();

            newWeapon.Name = xmlNode["Name"].InnerText;

            newWeapon.HitFirst = BoolParse(xmlNode["HitFirst"]);
            newWeapon.KillingBlow = BoolParse(xmlNode["KillingBlow"]);
            newWeapon.PoisonAttack = BoolParse(xmlNode["PoisonAttack"]);

            newWeapon.AddToMovement = IntParse(xmlNode["AddToMovement"]);
            newWeapon.AddToWeaponSkill = IntParse(xmlNode["AddToWeaponSkill"]);
            newWeapon.AddToBallisticSkill = IntParse(xmlNode["AddToBallisticSkill"]);
            newWeapon.AddToStrength = IntParse(xmlNode["AddToStrength"]);
            newWeapon.AddToToughness = IntParse(xmlNode["AddToToughness"]);
            newWeapon.AddToWounds = IntParse(xmlNode["AddToWounds"]);
            newWeapon.AddToInitiative = IntParse(xmlNode["AddToInitiative"]);
            newWeapon.AddToAttacks = IntParse(xmlNode["AddToAttacks"]);
            newWeapon.AddToLeadership = IntParse(xmlNode["AddToLeadership"]);

            return newWeapon;
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
