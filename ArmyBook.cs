using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using static WarhammerArmyAssembler.Option;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler
{
    public class ArmyBook
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
        public static Dictionary<int, Unit> Mounts = new Dictionary<int, Unit>();
        public static Dictionary<int, Option> Artefact = new Dictionary<int, Option>();

        private static int MaxIDindex = 0;

        public static Brush MainColor = null;
        public static Brush AdditionalColor = null;
        public static Brush BackgroundColor = null;

        public static int GetNextIndex()
        {
            return MaxIDindex++;
        }

        public static void LoadArmy(string xmlFileName)
        {
            Units.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            string mainColor = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));
            MainColor = (SolidColorBrush)new BrushConverter().ConvertFromString(mainColor);

            string additionalColor = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/AdditionalColor"));
            AdditionalColor = (SolidColorBrush)new BrushConverter().ConvertFromString(additionalColor);

            string backgroundColor = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/BackgroundColor"));
            BackgroundColor = (SolidColorBrush)new BrushConverter().ConvertFromString(backgroundColor);

            Interface.SetArmyGridAltColor(BackgroundColor);

            foreach (XmlNode xmlUnit in xmlFile.SelectNodes("ArmyBook/Units/Unit"))
            {
                int newID = GetNextIndex();
                Units.Add(newID, LoadUnit(newID, xmlUnit, xmlFile));
            }

            foreach (XmlNode xmlUnit in xmlFile.SelectNodes("ArmyBook/Heroes/Hero"))
            {
                int newID = GetNextIndex();
                Units.Add(newID, LoadUnit(newID, xmlUnit, xmlFile));
            }

            foreach (XmlNode xmlMount in xmlFile.SelectNodes("ArmyBook/Mounts/Mount"))
            {
                int newID = GetNextIndex();
                Mounts.Add(newID, LoadUnit(newID, xmlMount, xmlFile));
            }

            foreach (XmlNode xmlArtefact in xmlFile.SelectNodes("ArmyBook/Artefacts/Artefact"))
            {
                int newID = GetNextIndex();
                Artefact.Add(newID, LoadOption(newID, xmlArtefact));
            }
                
        }

        private static XmlNode AddFrenzyAttack(XmlDocument xml)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, "AdditionalAttackByFrenzy", String.Empty);
            XmlNode nodeParam = xml.CreateNode(XmlNodeType.Element, "AddToAttacks", String.Empty);
            nodeParam.InnerText = "1";
            nodeName.AppendChild(nodeParam);

            return nodeName;
        }

        public static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml)
        {
            Unit newUnit = new Unit();

            newUnit.ID = id;
            newUnit.IDView = id.ToString();

            newUnit.Name = StringParse(xmlUnit["Name"]);
            newUnit.Type = UnitTypeParse(xmlUnit["Type"]);
            newUnit.Points = IntParse(xmlUnit["Points"]);
            newUnit.Size = IntParse(xmlUnit["MinSize"]);
            newUnit.MinSize = newUnit.Size;
            newUnit.MaxSize = IntParse(xmlUnit["MaxSize"]);
            newUnit.Mage = IntParse(xmlUnit["Mage"]);
            newUnit.MountOn = IntParse(xmlUnit["MountOn"]);
            newUnit.MountInit = StringParse(xmlUnit["MountInit"]);
            newUnit.ModelsInPack = IntParse(xmlUnit["ModelsInPack"], byDefault: 1);

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

            newUnit.SlotsOfLords = IntParse(xmlUnit["SlotsOfLords"]);
            newUnit.SlotsOfHero = IntParse(xmlUnit["SlotsOfHero"]);
            newUnit.SlotsOfSpecial = IntParse(xmlUnit["SlotsOfSpecial"]);
            newUnit.SlotsOfRare = IntParse(xmlUnit["SlotsOfRare"]);

            newUnit.PersonifiedHero = BoolParse(xmlUnit["PersonifiedHero"]);

            XmlNode additionalParam = xmlUnit["AdditionalParam"];

            if (additionalParam != null)
            {
                newUnit.ImmuneToPsychology = BoolParse(additionalParam["ImmuneToPsychology"]);
                newUnit.Stubborn = BoolParse(additionalParam["Stubborn"]);
                newUnit.Hate = BoolParse(additionalParam["Hate"]);
                newUnit.Fear = BoolParse(additionalParam["Fear"]);
                newUnit.Terror = BoolParse(additionalParam["Terror"]);
                newUnit.Frenzy = BoolParse(additionalParam["Frenzy"]);
                newUnit.Unbreakable = BoolParse(additionalParam["Unbreakable"]);
                newUnit.ColdBlooded = BoolParse(additionalParam["ColdBlooded"]);
                newUnit.Stupidity = BoolParse(additionalParam["Stupidity"]);
                newUnit.HitFirst = BoolParse(additionalParam["HitFirst"]);
                newUnit.Regeneration = BoolParse(additionalParam["Regeneration"]);
                newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
                newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);
                newUnit.MagicItems = IntParse(additionalParam["MagicItems"]);
                newUnit.MagicItemsType = MagicItemsTypeParse(additionalParam["MagicItemsType"]);

                if (newUnit.Frenzy)
                    xmlUnit.SelectSingleNode("SpecialRulesAndAmmunition").AppendChild(AddFrenzyAttack(xml));
            }

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("SpecialRulesAndAmmunition/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Options/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            newUnit.SizableType = (!newUnit.IsHero() && (newUnit.Type != UnitType.Mount));

            return newUnit;
        }

        private static int IntParse(XmlNode xmlNode, int? byDefault = null)
        {
            if (xmlNode == null)
                return byDefault ?? 0;

            int value = 0;

            bool success = int.TryParse(xmlNode.InnerText, out value);

            return (success ? value : (byDefault ?? 0));
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

        private static string[] AllStringParse(XmlNode xmlNode)
        {
            List<string> allString = new List<string>();

            foreach (XmlNode xmlSpecialRule in xmlNode.SelectNodes("SpecialRuleDescription"))
                allString.Add(xmlSpecialRule.InnerText);

            return allString.ToArray();
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

        private static OnlyForType OnlyForParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            OnlyForType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : Option.OnlyForType.All);
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

        public static Option LoadOption(int id, XmlNode xmlNode)
        {
            Option newWeapon = new Option();

            newWeapon.ID = id;
            newWeapon.IDView = id.ToString();

            newWeapon.Name = StringParse(xmlNode["Name"]);
            newWeapon.Description = StringParse(xmlNode["Description"]);
            newWeapon.Type = OptionTypeParse(xmlNode["Type"]);
            newWeapon.OnlyFor = OnlyForParse(xmlNode["OnlyFor"]);
            newWeapon.OnlyIfAnotherService = StringParse(xmlNode["OnlyIfAnotherService"]);
            newWeapon.OnlyIfNotAnotherService = StringParse(xmlNode["OnlyIfNotAnotherService"]);
            newWeapon.Realised = false;
            newWeapon.Multiple = BoolParse(xmlNode["Multiple"]);
            newWeapon.OrdinaryArtefact = BoolParse(xmlNode["OrdinaryArtefact"]);
            newWeapon.OnlyOneInArmy = BoolParse(xmlNode["OnlyOneInArmy"]);

            newWeapon.SpecialRuleDescription = AllStringParse(xmlNode);

            newWeapon.HitFirst = BoolParse(xmlNode["HitFirst"]);
            newWeapon.KillingBlow = BoolParse(xmlNode["KillingBlow"]);
            newWeapon.PoisonAttack = BoolParse(xmlNode["PoisonAttack"]);
            newWeapon.Regeneration = BoolParse(xmlNode["Regeneration"]);
            newWeapon.ImmuneToPsychology = BoolParse(xmlNode["ImmuneToPsychology"]);
            newWeapon.Stubborn = BoolParse(xmlNode["Stubborn"]);
            newWeapon.Hate = BoolParse(xmlNode["Hate"]);
            newWeapon.Fear = BoolParse(xmlNode["Fear"]);
            newWeapon.Terror = BoolParse(xmlNode["Terror"]);
            newWeapon.Frenzy = BoolParse(xmlNode["Frenzy"]);
            newWeapon.Unbreakable = BoolParse(xmlNode["Unbreakable"]);
            newWeapon.ColdBlooded = BoolParse(xmlNode["ColdBlooded"]);

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
            newWeapon.AddToCast = IntParse(xmlNode["AddToCast"]);
            newWeapon.AddToDispell = IntParse(xmlNode["AddToDispell"]);
            newWeapon.AddToModelsInPack = IntParse(xmlNode["AddToModelsInPack"]);

            newWeapon.FullCommand = BoolParse(xmlNode["FullCommand"]);

            newWeapon.Mount = BoolParse(xmlNode["Mount"]);

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
