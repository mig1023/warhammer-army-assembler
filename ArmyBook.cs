using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static WarhammerArmyAssembler.Option;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler
{
    public class ArmyBook
    {
        public static Dictionary<string, Unit> Units = new Dictionary<string, Unit>();
        public static Dictionary<string, Option> Artefact = new Dictionary<string, Option>();

        public static void LoadArmy(string xmlFileName)
        {
            Units.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            foreach (XmlNode xmlUnit in xmlFile.SelectNodes("ArmyBook/Units/Unit"))
                Units.Add(GetID(xmlUnit), LoadUnit(xmlUnit));

            foreach (XmlNode xmlUnit in xmlFile.SelectNodes("ArmyBook/Heroes/Hero"))
                Units.Add(GetID(xmlUnit), LoadUnit(xmlUnit));

            foreach (XmlNode xmlArtefact in xmlFile.SelectNodes("ArmyBook/Artefacts/Artefact"))
                Artefact.Add(GetID(xmlArtefact), LoadOption(xmlArtefact));
        }

        public static string GetID(XmlNode xmlUnit)
        {
            return xmlUnit["ID"].InnerText;
        }

        public static Unit LoadUnit(XmlNode xmlUnit)
        {
            Unit newUnit = new Unit();

            newUnit.Name = StringParse(xmlUnit["Name"]);
            newUnit.ID = StringParse(xmlUnit["ID"]);
            newUnit.Type = UnitTypeParse(xmlUnit["Type"]);
            newUnit.Points = IntParse(xmlUnit["Points"]);
            newUnit.Size = IntParse(xmlUnit["MinSize"]);

            newUnit.Description = StringParse(xmlUnit["Description"]);

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

            newUnit.Armour = IntNullableParse(mainParam["Armour"]);
            newUnit.Ward = IntNullableParse(mainParam["Ward"]);

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

            newUnit.MagicItems = IntParse(additionalParam["MagicItems"]);
            newUnit.MagicItemsType = MagicItemsTypeParse(additionalParam["MagicItemsType"]);

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Ammunition/*"))
                newUnit.Option.Add(LoadOption(xmlAmmunition));

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Options/*"))
                newUnit.Option.Add(LoadOption(xmlAmmunition));

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

        private static int? IntNullableParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            int value = 0;

            bool success = int.TryParse(xmlNode.InnerText, out value);

            return (success ? value : (int?)null);
        }

        private static string StringParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return String.Empty;

            return xmlNode.InnerText.Replace("|", "\n");
        }

        private static UnitType UnitTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return UnitType.Core;

            UnitType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : UnitType.Core);
        }

        private static MagicItemsTypes MagicItemsTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return MagicItemsTypes.Hero;

            MagicItemsTypes value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : MagicItemsTypes.Hero);
        }

        private static OptionType OptionTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            OptionType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : Option.OptionType.Option);
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

            foreach (string xmlName in allXmlFiles)
            {
                XmlDocument xmlFile = new XmlDocument();
                xmlFile.Load(xmlName);

                XmlNode armyName = xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName");
                allXmlArmyBooks.Add(armyName.InnerText);
            }

            return allXmlArmyBooks;
        }

        public static Option LoadOption(XmlNode xmlNode)
        {
            Option newWeapon = new Option();

            newWeapon.Name = StringParse(xmlNode["Name"]);
            newWeapon.Description = StringParse(xmlNode["Description"]);
            newWeapon.Type = OptionTypeParse(xmlNode["Type"]);
            newWeapon.Realised = false;
            newWeapon.Multiple = BoolParse(xmlNode["Multiple"]);

            newWeapon.ID = StringParse(xmlNode["ID"]);

            newWeapon.HitFirst = BoolParse(xmlNode["HitFirst"]);
            newWeapon.KillingBlow = BoolParse(xmlNode["KillingBlow"]);
            newWeapon.PoisonAttack = BoolParse(xmlNode["PoisonAttack"]);

            newWeapon.Points = IntParse(xmlNode["Points"]);
            newWeapon.PerModel = BoolParse(xmlNode["PerModel"]);

            newWeapon.AddToMovement = IntParse(xmlNode["AddToMovement"]);
            newWeapon.AddToWeaponSkill = IntParse(xmlNode["AddToWeaponSkill"]);
            newWeapon.AddToBallisticSkill = IntParse(xmlNode["AddToBallisticSkill"]);
            newWeapon.AddToStrength = IntParse(xmlNode["AddToStrength"]);
            newWeapon.AddToToughness = IntParse(xmlNode["AddToToughness"]);
            newWeapon.AddToWounds = IntParse(xmlNode["AddToWounds"]);
            newWeapon.AddToInitiative = IntParse(xmlNode["AddToInitiative"]);
            newWeapon.AddToAttacks = IntParse(xmlNode["AddToAttacks"]);
            newWeapon.AddToLeadership = IntParse(xmlNode["AddToLeadership"]);
            newWeapon.AddToArmour = IntParse(xmlNode["AddToArmour"]);
            newWeapon.AddToWard = IntParse(xmlNode["AddToWard"]);

            return newWeapon;
        }

        public static List<string> FindAllXmlFiles(string programDirectory)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(programDirectory))
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
