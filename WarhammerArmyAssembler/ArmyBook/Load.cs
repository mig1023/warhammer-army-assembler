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

        private static string UnitsPath(string pathLine, out string name)
        {
            List<string> pathList = pathLine.Split('/').ToList();

            string path = pathList[0];
            name = pathList[1];

            return String.Format("ArmyBook/Content/{0}/{1}", Constants.EnemyPathTypes[path], path);
        }

        public static Unit LoadArmyUnitOnly(string xmlFileName, string unitName, Unit target,
            Dictionary<string, string> enemyCommonXmlOption, int size)
        {
            string filePath = Path.GetDirectoryName(Constants.EnemiesOptionPath) + "\\" + xmlFileName;
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(filePath);

            foreach (XmlNode unit in xmlFile.SelectNodes(UnitsPath(unitName, out string name)))
            {
                if (unit["Name"].InnerText != name)
                    continue;

                Unit enemy = LoadUnit(0, unit, xmlFile, target, enemyCommonXmlOption);
                enemy.Armybook = xmlFile.SelectSingleNode("ArmyBook/Introduction/Army").InnerText;

                if (unit["Mount"] != null)
                {
                    foreach (XmlNode mount in xmlFile.SelectNodes("ArmyBook/Content/Mounts/Mount"))
                        if (unit["Mount"].InnerText == mount["Name"].InnerText)
                            enemy.Mount = LoadUnit(0, mount, xmlFile, null, enemyCommonXmlOption);
                }

                if (size > 0)
                    enemy.Size = size;

                return enemy;
            }

            return null;
        }

        public static string LoadArmyUnitImageOnly(string xmlFileName, string unitName, bool isHero)
        {
            XmlDocument xmlFile = new XmlDocument();

            xmlFile.Load(xmlFileName);

            string imagePath = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsFolder")));
                
            string unitType = isHero ? "Heroes/Hero" : "Units/Unit";
            XmlNodeList xmlNodes = xmlFile.SelectNodes(String.Format("ArmyBook/Content/{0}", unitType));

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

        private static void LoadCommonXmlOptionFromFile(XmlDocument xmlFile, string path,
            Dictionary<string, string> commonXmlOption)
        {
            foreach (XmlNode option in xmlFile.SelectNodes(path))
            {
                string title = option.Attributes["Name"]?.InnerText ?? option.Name;
                string value = String.Format("{0}|{1}", title, option.InnerText);
                commonXmlOption.Add(option.Name, value);
            }
        }

        public static Dictionary<string, string> LoadCommonXmlOption(XmlDocument armybook)
        {
            if (String.IsNullOrEmpty(Constants.CommonXmlOptionPath))
                return null;

            Dictionary<string, string> commonXmlOption = new Dictionary<string, string>();

            if ((armybook == null) && (Constants.CommonXmlOption != null))
            {
                return Constants.CommonXmlOption;
            }
            else
            {
                XmlDocument xmlFile = new XmlDocument();
                xmlFile.Load(Constants.CommonXmlOptionPath);
                LoadCommonXmlOptionFromFile(xmlFile, "Options/*/*", commonXmlOption);

                if (armybook == null)
                    Constants.CommonXmlOption = commonXmlOption;
            }

            if (armybook != null)
                LoadCommonXmlOptionFromFile(armybook, "ArmyBook/Introduction/LocalXmlOption/*", commonXmlOption);

            return commonXmlOption;
        }

        private static void LoadEnemies()
        {
            if (String.IsNullOrEmpty(Constants.EnemiesOptionPath))
                return;

            Enemy.CleanEnemies();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.EnemiesOptionPath);

            foreach (XmlNode enemy in xmlFile.SelectNodes("Enemies/Enemy"))
            {
                XmlAttributeCollection attr = enemy.Attributes;
                Enemy.Add(attr["Armybook"], attr["Path"], attr["Size"], attr["Type"]);
            }
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

            Constants.CommonXmlOption = LoadCommonXmlOption(xmlFile);

            XmlNode armyFile = Services.Intro(xmlFile, "Images/Symbol");
            Interface.Changes.LoadArmyImage(armyFile, xmlFileName);

            Army.Data.Name = LoadString(xmlFile, "Army");
            Army.Data.InternalName = LoadString(xmlFile, "InternalName");
            Army.Data.ArmyEdition = LoadInt(xmlFile, "Edition");

            Data.FrontColor = LoadColor(xmlFile, "Front");
            Data.BackColor = LoadColor(xmlFile, "Back");
            Data.GridColor = LoadColor(xmlFile, "Grid");
            Data.TooltipColor = LoadColor(xmlFile, "Tooltip");

            Data.Upgraded = Services.StyleColor(xmlFile, "Upgraded").InnerText;

            Data.DemonicMortal = BoolParse(Services.Intro(xmlFile, "DemonicMortal"));

            Army.Data.UnitsImagesDirectory = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsFolder")));

            Interface.Mod.SetArmyGridAltColor(Data.GridColor);

            LoadStyles(xmlFile);

            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Units/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Heroes/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Mounts/*", ref Data.Mounts);

            foreach (XmlNode xmlArtefactGroup in xmlFile.SelectNodes("ArmyBook/Content/Artefacts/Group"))
            {
                string groupName = xmlArtefactGroup.Attributes["Name"].Value;

                foreach (XmlNode xmlArtefact in xmlArtefactGroup.SelectNodes("*"))
                {
                    int newID = GetNextIndex();
                    Data.Artefact.Add(newID, LoadOption(newID, xmlArtefact, xmlFile, groupName));
                }
            }

            LoadMagic(xmlFile, "Magic");
            LoadMagic(xmlFile, "Dispell", enemy: true);
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

                if (!int.TryParse(spell.Attributes["Cast"]?.Value, out int spellDifficulty))
                    continue;

                if (!enemy)
                    Data.Magic.Add(spellName, spellDifficulty);
                else
                    Data.Dispell.Add(spellName, spellDifficulty);
            }
        }

        public static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml,
            Unit target = null, Dictionary<string, string> enemyCommonXmlOption = null)
        {
            string description = StringParse(xmlUnit["Description"]);

            if (String.IsNullOrEmpty(description))
                description = StringParse(xmlUnit["Name"]);

            Unit newUnit = target ?? new Unit();

            newUnit.ID = id;
            newUnit.IDView = id.ToString();

            newUnit.Name = StringParse(xmlUnit["Name"]);
            newUnit.Type = UnitTypeParse(xmlUnit);
            newUnit.Points = DoubleParse(xmlUnit["Points"]);
            newUnit.UniqueUnits = BoolParse(xmlUnit["UniqueUnits"]);
            newUnit.Wizard = IntParse(xmlUnit["Wizard"]);
            newUnit.MountOn = IntParse(xmlUnit["MountOn"]);
            newUnit.MountInit = StringParse(xmlUnit["Mount"]);
            newUnit.ModelsInPack = IntParse(xmlUnit["ModelsInPack"], byDefault: 1);
            newUnit.Description = description;
            newUnit.Personified = BoolParse(xmlUnit["Personified"]);
            newUnit.WeaponTeam = BoolParse(xmlUnit["WeaponTeam"]);
            newUnit.Chariot = IntParse(xmlUnit["Chariot"]);

            Parsers.SizeParse(StringParse(xmlUnit["Size"]), out int min, out int max);

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
                foreach (string name in Constants.UnitProperties)
                    SetProperty(newUnit, additionalParam, name);

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
                    newUnit.Options.Add(LoadOption(GetNextIndex(), additionalParam["Individual"],
                        xml, commonXmlOption: enemyCommonXmlOption));

                AddCommonXmlOptionBySpecialRules(xml, additionalParam, ref newUnit,
                    enemyCommonXmlOption ?? Constants.CommonXmlOption);
            }

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Equipments/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition, xml,
                    category: Option.OptionCategory.Equipment, commonXmlOption: enemyCommonXmlOption));

            foreach (XmlNode xmlOption in xmlUnit.SelectNodes("Options/*"))
            {
                if (xmlOption.Name == "CommandGroup")
                {
                    foreach (XmlNode xmlCommand in xmlOption.SelectNodes("Leader"))
                        newUnit.Options.Add(LoadOption(GetNextIndex(), xmlCommand, xml));
                }
                else
                    newUnit.Options.Add(LoadOption(GetNextIndex(), xmlOption, xml));
            }

            newUnit.SizableType = !newUnit.IsHero() && (newUnit.Type != UnitType.Mount) && (newUnit.MaxSize != newUnit.MinSize);
            newUnit.VisibleType = newUnit.SizableType ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            newUnit.ArmyColor = (SolidColorBrush)Data.FrontColor;
            newUnit.TooltipColor = (SolidColorBrush)Data.TooltipColor;


            string image = StringParse(xmlUnit["Image"]);

            if (String.IsNullOrEmpty(image))
                newUnit.Image = Interface.Changes.TryHomologueImage(newUnit);
            else
                newUnit.Image = String.Format("{0}{1}.jpg", Army.Data.UnitsImagesDirectory, image);

            return newUnit;
        }

        private static void AddCommonXmlOptionBySpecialRules(XmlDocument xml, XmlNode xmlUnitRules,
            ref Unit newUnit, Dictionary<string, string> commonXmlOption = null)
        {
            foreach (string option in commonXmlOption.Keys)
                if (xmlUnitRules[option] != null)
                    newUnit.Options.Add(LoadOption(GetNextIndex(), Services.CreateRuleOnlyOption(xml, option),
                        xml, commonXmlOption: commonXmlOption));
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
                name = xmlNode.Attributes["Name"].InnerText;

            if (name == "Leader")
                name = xmlNode.InnerText;

            AddToOption(xmlDocument, ref xmlNode, "Name", name);

            bool typesIncluded = false;

            if (xmlNode.Attributes["Attr"] != null)
                attributes += ";" + String.Join(";", xmlNode.Attributes["Attr"].InnerText.Split(',').Select(x => x.Trim()));

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

        public static Option LoadOption(int id, XmlNode xmlNode, XmlDocument xmlDocument,
            string artefactGroup = null, Option.OptionCategory category = Option.OptionCategory.Nope,
            Dictionary<string, string> commonXmlOption = null)
        {
            if (Services.GetCommonXmlOption(xmlNode.Name, out string commonOption, commonXmlOption))
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

            if (category != Option.OptionCategory.Nope)
                newOption.Category = category;

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

        private static object PropertyByType(object element, XmlNode value,
            string paramName, bool byAttr = false)
        {
            PropertyInfo param = element.GetType().GetProperty(paramName);

            if (param.PropertyType == typeof(Profile))
                return ProfileParse(value);

            else if (byAttr)
                return null;

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

            object propetyValue = PropertyByType(element, xmlNode, name, isByAttr);

            if (propetyValue != null)
                element.GetType().GetProperty(name).SetValue(element, propetyValue);
        }
    }
}
