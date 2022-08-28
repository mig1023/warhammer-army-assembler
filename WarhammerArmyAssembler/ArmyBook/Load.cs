using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using static WarhammerArmyAssembler.ArmyBook.Parsers;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Load
    {
        public static int GetNextIndex() => Data.MaxIDindex++;

        private static void LoadUnitsFromXml(XmlDocument xmlFile, string path, ref Dictionary<int, Unit> dict)
        {
            XmlNodeList xmlNodes = xmlFile.SelectNodes(path);

            foreach (XmlNode xmlUnit in xmlNodes)
            {
                int newID = GetNextIndex(); 
                dict.Add(newID, LoadUnit(newID, xmlUnit, xmlFile));
            }
        }

        private static string LoadString(XmlDocument xmlFile, string node) =>
            StringParse(Services.Intro(xmlFile, node));

        private static int LoadInt(XmlDocument xmlFile, string node) =>
            IntParse(Services.Intro(xmlFile, node));

        private static Brush LoadColor(XmlDocument xmlFile, string node) =>
            Interface.Services.BrushFromXml(Services.Intro(xmlFile, String.Format("Styles/Colors/{0}", node)));

        private static string LoadStyle(XmlDocument xmlFile, string node, string defaultValue) =>
            xmlFile.SelectSingleNode(String.Format("ArmyBook/Introduction/Styles/{0}", node))?.InnerText ?? defaultValue;

        private static void LoadStyles(XmlDocument xmlFile)
        {
            Data.AddStyle = LoadStyle(xmlFile, "Add", defaultValue: "add");
            Data.DropStyle = LoadStyle(xmlFile, "Drop", defaultValue: "drop");
            Data.MagicItemsStyle = LoadStyle(xmlFile, "MagicItems", defaultValue: "MAGIC ITEMS").ToUpper();
            Data.MagicPowersStyle = LoadStyle(xmlFile, "MagicPowers", defaultValue: "MAGIC POWERS").ToUpper();
        }

        public static string LoadArmyUnitImageOnly(string xmlFileName, string unitName, bool isHero)
        {
            XmlDocument xmlFile = new XmlDocument();

            xmlFile.Load(xmlFileName);

            string imagePath = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsIn")));
                
            string unitType = (isHero ? "Heroes/Hero" : "Units/Unit");
            XmlNodeList xmlNodes =  xmlFile.SelectNodes(String.Format("ArmyBook/Content/{0}", unitType));

            foreach (XmlNode xmlUnit in xmlNodes)
            {
                string xmlUnitName = StringParse(xmlUnit["Name"]);

                if (xmlUnitName != unitName)
                    continue;

                string image = StringParse(xmlUnit["Image"]);

                if (!String.IsNullOrEmpty(image))
                    return imagePath + image;
            }

            return String.Empty;
        }

        private static void LoadCommonXmlOptionFromFile(XmlDocument xmlFile, string path)
        {
            foreach (XmlNode option in xmlFile.SelectNodes(path))
            {
                string name = option.Attributes["Name"]?.InnerText ?? option.Name;
                string title = option.Attributes["Title"]?.InnerText ?? name;
                string value = String.Format("{0}|{1}", title, option.InnerText);
                Constants.CommonXmlOption.Add(name, value);
            }
        }

        private static void LoadCommonXmlOption(XmlDocument armybook)
        {
            if (String.IsNullOrEmpty(Constants.CommonXmlOptionPath))
                return;

            Constants.CommonXmlOption = new Dictionary<string, string>();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.CommonXmlOptionPath);
            LoadCommonXmlOptionFromFile(xmlFile, "Options/*/*");

            LoadCommonXmlOptionFromFile(armybook, "ArmyBook/Introduction/LocalXmlOption/*");
        }

        private static void LoadEnemies()
        {
            if (String.IsNullOrEmpty(Constants.EnemiesOptionPath))
                return;

            Enemy.CleanEnemies();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.EnemiesOptionPath);

            foreach (XmlNode genus in xmlFile.SelectNodes("Enemies/Genus"))
                foreach (XmlNode enemy in genus.SelectNodes("Enemy"))
                    Enemy.AddEnemies(genus.Attributes["Name"].InnerText, enemy.InnerText);
        }

        public static void LoadArmy(string xmlFileName)
        {
            Data.Magic.Clear();
            Data.Dispell.Clear();

            Data.Units.Clear();
            Data.Mounts.Clear();
            Data.Artefact.Clear();

            LoadEnemies();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            LoadCommonXmlOption(xmlFile);

            XmlNode armyFile = Services.Intro(xmlFile, "Images/Symbol");
            Interface.Changes.LoadArmyImage(armyFile, xmlFileName);

            Army.Data.Name = LoadString(xmlFile, "Name");
            Army.Data.InternalName = LoadString(xmlFile, "InternalName");
            Army.Data.ArmyEdition = LoadInt(xmlFile, "Edition");

            Data.FrontColor = LoadColor(xmlFile, "Front");
            Data.BackColor = LoadColor(xmlFile, "Back");
            Data.GridColor = LoadColor(xmlFile, "Grid");
            Data.TooltipColor = LoadColor(xmlFile, "Tooltip");

            Data.Upgraded = Services.StyleColor(xmlFile, "Upgraded").InnerText;

            Data.DemonicMortal = BoolParse(Services.Intro(xmlFile, "DemonicMortal"));

            Interface.Mod.SetArmyGridAltColor(Data.GridColor);

            LoadStyles(xmlFile);

            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Units/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Heroes/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Mounts/*", ref Data.Mounts);

            foreach (XmlNode xmlArtefactClass in xmlFile.SelectNodes("ArmyBook/Content/Artefacts/Class"))
            {
                string className = xmlArtefactClass.Attributes["Name"].Value;

                foreach (XmlNode xmlArtefact in xmlArtefactClass.SelectNodes("*"))
                {
                    int newID = GetNextIndex();
                    Data.Artefact.Add(newID, LoadOption(newID, xmlArtefact, xmlFile, className));
                }
            }

            LoadMagic(xmlFile, "Magic");
            LoadMagic(xmlFile, "Dispell", enemy: true);

            Army.Data.UnitsImagesDirectory = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsIn")));
        }

        private static void LoadMagic(XmlDocument xmlFile, string magic, bool enemy = false)
        {
            XmlNode loreBook = xmlFile.SelectSingleNode(String.Format("ArmyBook/Introduction/{0}", magic));

            if (loreBook == null)
                return;

            if (!enemy)
            {
                Data.MagicLoreName = loreBook.Attributes["Name"]?.Value ?? String.Empty;
                Data.MagicAlternative = loreBook.Attributes["Alternative"]?.Value ?? String.Empty;
            }
            else
            {
                Data.EnemyMagicLoreName = loreBook.Attributes["Name"]?.Value ?? String.Empty;
                Data.EnemyMagicName = loreBook.Attributes["Enemy"]?.Value ?? String.Empty;
            }

            foreach (XmlNode spell in xmlFile.SelectNodes(String.Format("ArmyBook/Introduction/{0}/Spell", magic)))
            {
                string spellName = spell.Attributes["Name"].Value;
                int spellDifficulty = int.Parse(spell.InnerText);

                if (!enemy)
                    Data.Magic.Add(spellName, spellDifficulty);
                else
                    Data.Dispell.Add(spellName, spellDifficulty);
            }
        }

        public static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml)
        {
            string description = StringParse(xmlUnit["Description"]);

            if (String.IsNullOrEmpty(description))
                description = StringParse(xmlUnit["Name"]);

            Unit newUnit = new Unit
            {
                ID = id,
                IDView = id.ToString(),

                Name = StringParse(xmlUnit["Name"]),
                Type = UnitTypeParse(xmlUnit),
                Points = DoubleParse(xmlUnit["Points"]),
                UniqueUnits = BoolParse(xmlUnit["UniqueUnits"]),
                Wizard = IntParse(xmlUnit["Wizard"]),
                MountOn = IntParse(xmlUnit["MountOn"]),
                MountInit = StringParse(xmlUnit["Mount"]),
                ModelsInPack = IntParse(xmlUnit["ModelsInPack"], byDefault: 1),

                Description = description,
                Image = StringParse(xmlUnit["Image"]),

                Personified = BoolParse(xmlUnit["Personified"]),
                WeaponTeam = BoolParse(xmlUnit["WeaponTeam"]),
                Chariot = IntParse(xmlUnit["Chariot"]),
            };

            int min = IntParse(xmlUnit["Min"]);
            int max = IntParse(xmlUnit["Max"]);

            if ((min == 0) && (max == 0))
            {
                newUnit.Size = 1;
                newUnit.MinSize = 1;
                newUnit.MaxSize = 1;
            }
            else
            {
                newUnit.Size = min;
                newUnit.MinSize = min;
                newUnit.MaxSize = max;
            }

            XmlNode profile = xmlUnit["Profile"];

            foreach (string name in Constants.ProfilesNames.Keys)
                SetProperty(newUnit, profile, name, byAttr: Constants.ProfilesNames[name]);

            newUnit.Ward = IntNullableParse(profile.Attributes["Ward"]);
            XmlNode additionalParam = xmlUnit["SpecialRules"];
            newUnit.SetGroup(StringParse(xmlUnit["Group"]));

            if (additionalParam != null)
            {
                newUnit.NoKillingBlow = BoolParse(additionalParam["NoKillingBlow"]);
                newUnit.NoMultiWounds = BoolParse(additionalParam["NoMultiWounds"]);
                newUnit.LargeBase = BoolParse(additionalParam["LargeBase"]);
                newUnit.ImmuneToPsychology = BoolParse(additionalParam["ImmuneToPsychology"]);
                newUnit.ImmuneToPoison = BoolParse(additionalParam["ImmuneToPoison"]);
                newUnit.Stubborn = BoolParse(additionalParam["Stubborn"]);
                newUnit.Hate = BoolParse(additionalParam["Hate"]);
                newUnit.Fear = BoolParse(additionalParam["Fear"]);
                newUnit.Terror = BoolParse(additionalParam["Terror"]);
                newUnit.Frenzy = BoolParse(additionalParam["Frenzy"]);
                newUnit.BloodFrenzy = BoolParse(additionalParam["BloodFrenzy"]);
                newUnit.Unbreakable = BoolParse(additionalParam["Unbreakable"]);
                newUnit.ColdBlooded = BoolParse(additionalParam["ColdBlooded"]);
                newUnit.Stupidity = BoolParse(additionalParam["Stupidity"]);
                newUnit.Undead = BoolParse(additionalParam["Undead"]);
                newUnit.StrengthInNumbers = BoolParse(additionalParam["StrengthInNumbers"]);
                newUnit.AutoHit = BoolParse(additionalParam["AutoHit"]);
                newUnit.AutoWound = BoolParse(additionalParam["AutoWound"]);
                newUnit.AutoDeath = BoolParse(additionalParam["AutoDeath"]);
                newUnit.HitFirst = BoolParse(additionalParam["HitFirst"]);
                newUnit.HitLast = BoolParse(additionalParam["HitLast"]);
                newUnit.Regeneration = BoolParse(additionalParam["Regeneration"]);
                newUnit.ExtendedRegeneration = IntParse(additionalParam["ExtendedRegeneration"]);
                newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
                newUnit.ExtendedKillingBlow = IntParse(additionalParam["ExtendedKillingBlow"]);
                newUnit.HeroicKillingBlow = BoolParse(additionalParam["HeroicKillingBlow"]);
                newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);
                newUnit.MultiWounds = StringParse(additionalParam["MultiWounds"]);
                newUnit.NoArmour = BoolParse(additionalParam["NoArmour"]);
                newUnit.NoWard = BoolParse(additionalParam["NoWard"]);
                newUnit.ArmourPiercing = IntParse(additionalParam["ArmourPiercing"]);
                newUnit.MagicPowers = IntParse(additionalParam["MagicPowers"]);
                newUnit.MagicPowersCount = IntParse(additionalParam["MagicPowersCount"]);
                newUnit.NotALeader = BoolParse(additionalParam["NotALeader"]);
                newUnit.MustBeGeneral = BoolParse(additionalParam["MustBeGeneral"]);
                newUnit.Reroll = StringParse(additionalParam["Reroll"]);
                newUnit.ImpactHit = StringParse(additionalParam["ImpactHit"]);
                newUnit.ImpactHitByFront = (BoolParse(additionalParam["ImpactHitByFront"]) ? 1 : 0);
                newUnit.SteamTank = BoolParse(additionalParam["SteamTank"]);
                newUnit.HellPitAbomination = BoolParse(additionalParam["HellPitAbomination"]);
                newUnit.Giant = BoolParse(additionalParam["Giant"]);
                newUnit.Lance = BoolParse(additionalParam["Lance"]);
                newUnit.Flail = BoolParse(additionalParam["Flail"]);
                newUnit.ChargeStrengthBonus = IntParse(additionalParam["ChargeStrengthBonus"]);
                newUnit.Resolute = BoolParse(additionalParam["Resolute"]);
                newUnit.PredatoryFighter = BoolParse(additionalParam["PredatoryFighter"]);
                newUnit.MurderousProwess = BoolParse(additionalParam["MurderousProwess"]);
                newUnit.AddToCloseCombat = StringParse(additionalParam["AddToCloseCombat"]);
                newUnit.Bloodroar = BoolParse(additionalParam["Bloodroar"]);
                newUnit.AddToHit = IntParse(additionalParam["AddToHit"]);
                newUnit.SubOpponentToHit = IntParse(additionalParam["SubOpponentToHit"]);
                newUnit.AddToWound = IntParse(additionalParam["AddToWound"]);
                newUnit.SubOpponentToWound = IntParse(additionalParam["SubOpponentToWound"]);
                newUnit.HitOn = IntParse(additionalParam["HitOn"]);
                newUnit.OpponentHitOn = IntParse(additionalParam["OpponentHitOn"]);
                newUnit.WoundOn = IntParse(additionalParam["WoundOn"]);
                newUnit.WardForFirstWound = IntParse(additionalParam["WardForFirstWound"]);
                newUnit.WardForLastWound = IntParse(additionalParam["WardForLastWound"]);
                newUnit.FirstWoundDiscount = BoolParse(additionalParam["FirstWoundDiscount"]);

                if (additionalParam["MagicItems"] != null)
                {
                    newUnit.MagicItemsPoints = IntParse(additionalParam["MagicItems"].Attributes["Points"]);
                    newUnit.MagicItemCount = IntParse(additionalParam["MagicItems"].Attributes["Count"]);
                    newUnit.MagicItemsType = MagicItemsTypeParse(additionalParam["MagicItems"].Attributes["Type"]);
                }

                newUnit.ParamTests = ParamParse(additionalParam);

                newUnit.Slots = SlotsParse(additionalParam);
                newUnit.NoCoreSlot = BoolParse(additionalParam["NoCoreSlot"]);

                if (newUnit.Frenzy)
                {
                    if (xmlUnit["Equipments"] == null)
                        xmlUnit.AppendChild(xml.CreateNode(XmlNodeType.Element, "Equipments", String.Empty));

                    xmlUnit["Equipments"].AppendChild(Services.AddFrenzyAttack(xml));
                }

                if (additionalParam["Individual"] != null)
                    newUnit.Options.Add(LoadOption(GetNextIndex(), additionalParam["Individual"], xml));

                AddCommonXmlOptionBySpecialRules(xml, additionalParam, ref newUnit);
            }

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Equipments/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition, xml));

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Options/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition, xml));

            newUnit.SizableType = (!newUnit.IsHero() && (newUnit.Type != UnitType.Mount) && (newUnit.MaxSize != newUnit.MinSize));
            newUnit.VisibleType = (newUnit.SizableType ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);

            newUnit.ArmyColor = (SolidColorBrush)Data.FrontColor;
            newUnit.TooltipColor = (SolidColorBrush)Data.TooltipColor;

            return newUnit;
        }

        private static void AddCommonXmlOptionBySpecialRules(XmlDocument xml, XmlNode xmlUnitRules, ref Unit newUnit)
        {
            foreach (string option in Constants.CommonXmlOption.Keys)
                if (xmlUnitRules[option] != null)
                    newUnit.Options.Add(LoadOption(GetNextIndex(), Services.CreateRuleOnlyOption(xml, option), xml));
        }

        private static void AddToOption(XmlDocument xmlDocument, ref XmlNode xmlNode,
            string name, string value = "", string attributes = "")
        {
            XmlElement option = xmlDocument.CreateElement(name);

            if (!String.IsNullOrEmpty(value))
                option.InnerText = value;

            if (!String.IsNullOrEmpty(attributes))
            {
                foreach (string attributeLine in attributes.Split(',').Select(x => x.Trim()))
                {
                    List<string> attribute = attributeLine.Split(':').Select(x => x.Trim()).ToList();
                    option.SetAttribute(attribute[0], attribute[1]);
                }
            }

            xmlNode.AppendChild(option);
        }

        private static void CreateMountOption(XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            AddToOption(xmlDocument, ref xmlNode, "Name", xmlNode.Attributes["Name"].InnerText);
            AddToOption(xmlDocument, ref xmlNode, "Points", xmlNode.Attributes["Points"].InnerText);
            AddToOption(xmlDocument, ref xmlNode, "Type", "Option");
            AddToOption(xmlDocument, ref xmlNode, "Mount", "True");
        }

        private static void CreateWizardOption(XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            AddToOption(xmlDocument, ref xmlNode, "Points", xmlNode.Attributes["Points"].InnerText);
            AddToOption(xmlDocument, ref xmlNode, "Type", "Option");

            int level = int.Parse(xmlNode.Attributes["Level"].InnerText);

            AddToOption(xmlDocument, ref xmlNode, "Name", String.Format("Wizard Level {0}", level));
            AddToOption(xmlDocument, ref xmlNode, "AddToCast", "1");
            AddToOption(xmlDocument, ref xmlNode, "AddToWizard", "1");

            if ((level == 1) || (level == 3))
                AddToOption(xmlDocument, ref xmlNode, "AddToDispell", "1");
        }

        private static void AddDependencies(XmlDocument xmlDocument, ref XmlNode xmlNode, string dependencies)
        {
            XmlElement xmlDependencies = xmlDocument.CreateElement("Dependencies");

            List<string> allDependencies = dependencies.Split(';').ToList();

            foreach (string dependenciesWithType in allDependencies)
            {
                List<string> typeAndDependencies = dependenciesWithType.Split(':').ToList();
                List<string> optionDependencies = typeAndDependencies[1].Split(',').ToList();

                foreach (string optionDependency in optionDependencies)
                {
                    XmlElement option = xmlDocument.CreateElement(typeAndDependencies[0].Trim());
                    option.InnerText = optionDependency.Trim();
                    xmlDependencies.AppendChild(option);
                }
            }

            xmlNode.AppendChild(xmlDependencies);
        }

        private static void CreateOption(string name, string attributes, XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            if (xmlNode["Name"] != null)
                return;

            if (name == "Command")
                name = xmlNode.Attributes["Position"].InnerText;

            AddToOption(xmlDocument, ref xmlNode, "Name", name);

            bool typesIncluded = false;

            foreach (string attributeLine in attributes.Split(';').Select(x => x.Trim('\n').Trim()))
            {
                if (String.IsNullOrEmpty(attributeLine))
                    continue;

                if (attributeLine.Contains(":"))
                {
                    List<string> attribute = attributeLine.Split(':').Select(x => x.Trim()).ToList();
                    AddToOption(xmlDocument, ref xmlNode, attribute[0], attribute[1]);

                    if (attribute[0] == "Type")
                        typesIncluded = true;
                }
                else
                    AddToOption(xmlDocument, ref xmlNode, attributeLine);
            }

            string points = 
                xmlNode.Attributes["PointsPerModel"]?.InnerText ??
                xmlNode.Attributes["Points"]?.InnerText ??
                String.Empty;

            if (!String.IsNullOrEmpty(points))
            {
                if (!typesIncluded)
                    AddToOption(xmlDocument, ref xmlNode, "Type", "Option");

                AddToOption(xmlDocument, ref xmlNode, "Points", points);

                if (xmlNode.Attributes["PointsPerModel"] != null)
                    AddToOption(xmlDocument, ref xmlNode, "PerModel", "True");
            }

            if (xmlNode.Attributes["Dependencies"] != null)
                AddDependencies(xmlDocument, ref xmlNode, xmlNode.Attributes["Dependencies"].InnerText);

            if (xmlNode.Attributes["Magic"] != null)
                AddToOption(xmlDocument, ref xmlNode, "MagicItems", attributes: xmlNode.Attributes["Magic"].InnerText);

            if (xmlNode.Attributes["Only"] != null)
                AddToOption(xmlDocument, ref xmlNode, "Only", xmlNode.Attributes["Only"].InnerText);

            if (xmlNode.Attributes["OnlyGroup"] != null)
                AddToOption(xmlDocument, ref xmlNode, "OnlyGroup", xmlNode.Attributes["OnlyGroup"].InnerText);
        }

        public static Option LoadOption(int id, XmlNode xmlNode, XmlDocument xmlDocument, string artefactGroup = null)
        {
            if (Services.GetCommonXmlOption(xmlNode.Name, out string commonOption))
            {
                List<string> xmlOption = commonOption.Split('|').ToList();
                CreateOption(xmlOption[0], xmlOption[1], xmlDocument, ref xmlNode);
            }

            if (xmlNode.Name == "Mount")
                CreateMountOption(xmlDocument, ref xmlNode);

            if (xmlNode.Name == "Wizard")
                CreateWizardOption(xmlDocument, ref xmlNode);

            Option newOption = new Option
            {
                ID = id,
                IDView = id.ToString(),
                Type = OptionTypeParse(xmlNode),
                Only = OnlyForParse(xmlNode["Only"]),
                ServiceDependencies = AllStringParse(xmlNode["Dependencies"], "On"),
                ServiceInverseDependencies = AllStringParse(xmlNode["Dependencies"], "Off"),
                Realised = BoolParse(xmlNode["RealisedByDefault"]),
                Countable = CountableParse(xmlNode["Countable"]),
                ImpactHitByFront = BoolParse(xmlNode["ImpactHitByFront"]) ? 1 : 0,
                ParamTests = ParamParse(xmlNode),
                Points = DoubleParse(xmlNode["Points"]),
                VirtueOriginalPoints = DoubleParse(xmlNode["Points"]),
                MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItemsType"]),
                ArtefactGroup = artefactGroup ?? String.Empty,
                TooltipColor = (SolidColorBrush)Data.TooltipColor,
            };

            foreach (string name in Constants.ProfilesNames.Keys)
            {
                SetProperty(newOption, xmlNode, String.Format("AddTo{0}", name));
                SetProperty(newOption, xmlNode, String.Format("{0}To", name));
            }

            foreach (string name in Constants.OptionProperties)
                SetProperty(newOption, xmlNode, name);

            newOption.Description = StringParse(xmlNode["Description"]);
            newOption.SpecialRuleDescription = AllStringParse(xmlNode, "Rule");

            if (String.IsNullOrEmpty(newOption.Description) && (newOption.SpecialRuleDescription.Length > 0))
                newOption.Description = newOption.SpecialRuleDescription[0];

            if (xmlNode["MagicItems"] != null)
            {
                newOption.MagicItems = IntParse(xmlNode["MagicItems"].Attributes["Points"]);
                newOption.MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItems"].Attributes["Type"]);
            }

            return newOption;
        }

        private static object PropertyByType(object element, XmlNode value, string paramName)
        {
            PropertyInfo param = element.GetType().GetProperty(paramName);

            if (param.PropertyType == typeof(Profile))
                return ProfileParse(value);

            else if (param.PropertyType == typeof(bool))
                return BoolParse(value);

            else if (param.PropertyType == typeof(int?))
                return IntNullableParse(value);

            else if (param.PropertyType == typeof(int))
                return IntParse(value);

            else if (param.PropertyType == typeof(string))
                return StringParse(value);

            else
                return null;
        }

        public static void SetProperty(object element, XmlNode value, string name, string byAttr = "")
        {
            bool isByAttr = !String.IsNullOrEmpty(byAttr);

            XmlNode xmlNode = isByAttr ? (XmlNode)value.Attributes[byAttr] : value[name];

            if (isByAttr && (xmlNode == null))
                return;

            object propetyValue = PropertyByType(element, xmlNode, name);

            if (propetyValue != null)
                element.GetType().GetProperty(name).SetValue(element, propetyValue);
        }
    }
}
