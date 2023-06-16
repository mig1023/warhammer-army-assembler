using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml;
using static WarhammerArmyAssembler.ArmyBook.Parsers;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Load
    {
        public static int GetNextIndex() => Data.MaxIDindex++;

        private static void LoadUnitsFromXml(XmlDocument xmlFile, string path,
            ref Dictionary<int, Unit> dict, string currentArmyLimit = "")
        {
            XmlNodeList xmlNodes = xmlFile.SelectNodes(path);

            bool dogsOfWar = !String.IsNullOrEmpty(currentArmyLimit);

            foreach (XmlNode xmlUnit in xmlNodes)
            {
                if (dogsOfWar && CurrentArmyLimitFail(xmlUnit, currentArmyLimit))
                    continue;

                int newID = GetNextIndex(); 
                dict.Add(newID, LoadUnit(newID, xmlUnit, xmlFile));
            }
        }

        private static bool CurrentArmyLimitFail(XmlNode xmlUnit, string currentArmyLimit)
        {
            string hireLimits = xmlUnit["Hire"].InnerText;
            bool negativeLogic = hireLimits.Contains("!");
            bool any = hireLimits.Contains("*");
            hireLimits = hireLimits.Replace("!", String.Empty);

            List<string> limits = hireLimits
                .Split(',')
                .Select(x => x.Trim())
                .ToList();

            if (!any && !negativeLogic && !limits.Contains(currentArmyLimit))
                return true;

            if (!any && negativeLogic && limits.Contains(currentArmyLimit))
                return true;

            return false;
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
            Data.AddStyle = LoadStyle(xmlFile, "Buttons/Add", defaultValue: "add");
            Data.DropStyle = LoadStyle(xmlFile, "Buttons/Drop", defaultValue: "drop");
            Data.MagicItemsStyle = LoadStyle(xmlFile, "MagicItems", defaultValue: "MAGIC ITEMS").ToUpper();
            Data.MagicPowersStyle = LoadStyle(xmlFile, "MagicPowers", defaultValue: "MAGIC POWERS").ToUpper();
        }

        private static string UnitsPath(string pathLine, out string name)
        {
            List<string> pathList = pathLine.Split('\\').ToList();

            string path = pathList[0];
            name = pathList[1];

            return String.Format("ArmyBook/Content/{0}/{1}", Constants.EnemyPathTypes[path], path);
        }

        public static Unit ArmyUnitOnly(string xmlFileName, string unitName, Unit target,
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

        public static string ArmyUnitImageOnly(string xmlFileName, string unitName, string homologue, bool isHero)
        {
            XmlDocument xmlFile = new XmlDocument();

            xmlFile.Load(xmlFileName);

            string imagePath = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Styles/Images/Units")));
                
            string unitType = isHero ? "Heroes/*" : "Units/*";
            XmlNodeList xmlNodes = xmlFile.SelectNodes(String.Format("ArmyBook/Content/{0}", unitType));

            foreach (XmlNode xmlUnit in xmlNodes)
            {
                string xmlUnitName = StringParse(xmlUnit["Name"]);
                string xmlUnitHomologue = StringParse(xmlUnit["Homologue"]);

                bool equal = xmlUnitName == unitName;
                bool isHomologue = !String.IsNullOrEmpty(homologue) && (xmlUnitHomologue == homologue);

                if ((!equal && !isHomologue) || (xmlUnit["Image"] == null))
                    continue;

                string image = StringParse(xmlUnit["Image"]);

                if (String.IsNullOrEmpty(image))
                    return ImagePathByName(xmlUnit, imagePath);
                else 
                    return FullImagePath(image, imagePath);
            }

            return String.Empty;
        }

        private static void LoadCommonXmlOptionFromFile(XmlDocument xmlFile, string path,
            Dictionary<string, string> commonXmlOption)
        {
            foreach (XmlNode option in xmlFile.SelectNodes(path))
            {
                string title = option.Attributes["Name"]?.InnerText ?? String.Empty;
                    
                if (String.IsNullOrEmpty(title))
                    title = Services.CamelNameSplit(option.Name);
                    
                string value = String.Format("{0}|{1}", title, option.InnerText);
                commonXmlOption.Add(option.Name, value);
            }
        }

        public static Dictionary<string, string> CommonXmlOption(XmlDocument armybook)
        {
            Dictionary<string, string> commonXmlOption = new Dictionary<string, string>();

            if (armybook != null)
                LoadCommonXmlOptionFromFile(armybook, "ArmyBook/Introduction/LocalOption/*", commonXmlOption);

            if (String.IsNullOrEmpty(Constants.CommonXmlOptionPath))
                return commonXmlOption;

            if ((armybook == null) && (Constants.CommonXmlOption != null))
                return Constants.CommonXmlOption;

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.CommonXmlOptionPath);
            LoadCommonXmlOptionFromFile(xmlFile, "Options/*/*", commonXmlOption);

            if (armybook == null)
                Constants.CommonXmlOption = commonXmlOption;

            return commonXmlOption;
        }

        public static Dictionary<string, string> CommonXmlSpecialRules(XmlDocument armybook)
        {
            Dictionary<string, string> ruleList = new Dictionary<string, string>();

            XmlNodeList specialRules = armybook.SelectNodes("ArmyBook/Introduction/LocalSpecialRules/*");

            if (specialRules == null)
                return ruleList;

            foreach (XmlNode option in specialRules)
            {
                string value = option.InnerText;
                
                if (String.IsNullOrEmpty(value))
                {
                    List<string> multiWords = Regex.Split(option.Name, @"(?<!^)(?=[A-Z])").ToList();

                    if (multiWords.Count < 2)
                        value = option.Name;
                    else
                        value = String.Join(" ", multiWords);
                };
                
                ruleList.Add(option.Name, value);
            }

            return ruleList;
        }

        public static void Enemies()
        {
            if (Enemy.AlreadyLoaded() || Enemy.CantBeLoaded())
                return;

            Enemy.Clean();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.EnemiesOptionPath);

            foreach (XmlNode enemy in xmlFile.SelectNodes("Enemies/Enemy"))
            {
                XmlAttributeCollection attr = enemy.Attributes;
                Enemy.Add(attr["Path"], attr["Size"], attr["Type"]);
            }
        }
        private static void LoadDogsOfWar(string army)
        {
            XmlDocument dogsFile = new XmlDocument();
            dogsFile.Load(Constants.DogOfWarPath);
            LoadUnitsFromXml(dogsFile, "ArmyBook/Content/Units/*", ref Data.Units, army);
            LoadUnitsFromXml(dogsFile, "ArmyBook/Content/Mounts/*", ref Data.Mounts);
        }

        public static void Armybook(string xmlFileName)
        {
            Data.Magic.Clear();
            Data.Dispell.Clear();

            Data.Units.Clear();
            Data.Mounts.Clear();
            Data.Artefact.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            Constants.CommonXmlOption = CommonXmlOption(xmlFile);
            Constants.CommonXmlSpecialRules = CommonXmlSpecialRules(xmlFile);

            XmlNode armyFile = Services.Intro(xmlFile, "Styles/Images/Symbol");
            Interface.Changes.LoadArmyImage(armyFile, xmlFileName);

            Army.Data.Name = LoadString(xmlFile, "Info/Army");
            Army.Data.Internal = LoadString(xmlFile, "Internal");
            Army.Data.ArmyEdition = LoadInt(xmlFile, "Info/Edition");

            Data.FrontColor = LoadColor(xmlFile, "Front");
            Data.BackColor = LoadColor(xmlFile, "Back");
            Data.GridColor = LoadColor(xmlFile, "Grid");
            Data.TooltipColor = LoadColor(xmlFile, "Tooltip");

            Data.Upgraded = Services.StyleColor(xmlFile, "Upgraded").InnerText;

            Data.DemonicMortal = BoolParse(Services.Intro(xmlFile, "DemonicMortal"));

            Army.Data.UnitsImagesDirectory = String.Format("{0}\\Images\\{1}\\", Path.GetDirectoryName(xmlFileName),
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Styles/Images/Units")));

            Interface.Mod.SetArmyGridAltColor(Data.GridColor);

            LoadStyles(xmlFile);

            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Units/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Heroes/*", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Mounts/*", ref Data.Mounts);

            LoadDogsOfWar(Army.Data.Internal);

            foreach (XmlNode xmlArtefactGroup in xmlFile.SelectNodes("ArmyBook/Content/Artefacts/Group"))
            {
                string groupName = xmlArtefactGroup.Attributes["Name"].Value;

                foreach (XmlNode xmlArtefact in xmlArtefactGroup.SelectNodes("*"))
                {
                    int newID = GetNextIndex();
                    Data.Artefact.Add(newID, LoadOption(newID, xmlArtefact, xmlFile, groupName));
                }
            }

            LoadMagic(xmlFile);
        }

        private static void LoadMagic(XmlDocument xmlFile)
        {
            XmlNode loreBook = xmlFile.SelectSingleNode("ArmyBook/Introduction/Magic");

            if (loreBook == null)
                return;

            Data.MagicLoreName = StringParse(loreBook["Name"]);
            Data.MagicOptions = StringParse(loreBook["Options"]);

            string enemyMagicLine = StringParse(loreBook["Enemy"]);
            List<string> enemyMagic = enemyMagicLine.Split(',').Select(x => x.Trim()).ToList();

            if (enemyMagic.Count < 2)
                return;

            Data.EnemyMagicName = enemyMagic[0];
            Data.EnemyMagicLoreName = enemyMagic[1];

            LoadSpells(xmlFile, "Magic", ref Data.Magic);
            LoadSpells(xmlFile, "Dispell", ref Data.Dispell);
        }

        private static void LoadSpells(XmlDocument xmlFile, string magic, ref Dictionary<string, int> spells)
        {
            foreach (XmlNode spell in xmlFile.SelectNodes(String.Format("ArmyBook/Introduction/Magic/{0}/Spell", magic)))
            {
                string spellName = spell.Attributes["Name"].Value;

                if (!int.TryParse(spell.Attributes["Cast"]?.Value, out int spellDifficulty))
                    continue;

                spells.Add(spellName, spellDifficulty);
            }
        }

        private static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml,
            Unit target = null, Dictionary<string, string> enemyCommonXmlOption = null)
        {
            string description = StringParse(xmlUnit["Description"]);

            if (String.IsNullOrEmpty(description))
                description = StringParse(xmlUnit["Name"]);

            Unit newUnit = target ?? new Unit();

            newUnit.ID = id;
            newUnit.IDView = id.ToString();

            newUnit.Name = StringParse(xmlUnit["Name"]);
            newUnit.Homologue = StringParse(xmlUnit["Homologue"]);
            newUnit.Type = UnitTypeParse(xmlUnit);
            newUnit.Points = DoubleParse(xmlUnit["Points"]);
            newUnit.StaticPoints = DoubleParse(xmlUnit["StaticPoints"]);
            newUnit.Singleton = BoolParse(xmlUnit["Singleton"]);
            newUnit.Wizard = IntParse(xmlUnit["Wizard"]);
            newUnit.MountOn = IntParse(xmlUnit["MountOn"]);
            newUnit.MountInit = StringParse(xmlUnit["Mount"]);
            newUnit.ModelsInPack = IntParse(xmlUnit["ModelsInPack"], byDefault: 1);
            newUnit.Description = description;
            newUnit.Character = BoolParse(xmlUnit["Character"]);
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

                if (Constants.CommonXmlSpecialRules != null)
                    foreach (XmlNode specialRule in additionalParam.SelectNodes("*"))
                        if (Constants.CommonXmlSpecialRules.ContainsKey(specialRule.Name))
                            newUnit.Options.Add(AddCommonXmlSpecialRules(GetNextIndex(), specialRule.Name));

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

                if (additionalParam["Original"] != null)
                {
                    if (additionalParam["Original"].SelectNodes("*").Count == 0)
                    {
                        XmlElement option = xml.CreateElement("Rule");
                        option.InnerText = additionalParam["Original"].InnerText;
                        additionalParam["Original"].AppendChild(option);
                    }

                    newUnit.Options.Add(LoadOption(GetNextIndex(), additionalParam["Original"],
                        xml, commonXmlOption: enemyCommonXmlOption));
                }

                AddCommonXmlOptionBySpecialRules(xml, additionalParam, ref newUnit,
                    enemyCommonXmlOption ?? Constants.CommonXmlOption);
            }

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Equipments/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition, xml,
                    category: Option.OptionCategory.Equipment, commonXmlOption: enemyCommonXmlOption));

            foreach (XmlNode xmlOption in xmlUnit.SelectNodes("Options/*"))
            {
                if (xmlOption.Name == "Command")
                {
                    foreach (XmlNode xmlCommand in xmlOption.SelectNodes("*"))
                        newUnit.Options.Add(LoadOption(GetNextIndex(), xmlCommand, xml));
                }
                else
                {
                    newUnit.Options.Add(LoadOption(GetNextIndex(), xmlOption, xml));
                }
            }

            newUnit.SizableType = !newUnit.IsHero() && (newUnit.Type != UnitType.Mount) && (newUnit.MaxSize != newUnit.MinSize);
            newUnit.VisibleType = newUnit.SizableType ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            newUnit.ArmyColor = (SolidColorBrush)Data.FrontColor;
            newUnit.TooltipColor = (SolidColorBrush)Data.TooltipColor;

            newUnit.Image = TryFindImage(xmlUnit, newUnit);

            return newUnit;
        }

        private static string TryFindImage(XmlNode xmlUnit, Unit newUnit)
        {
            string image = StringParse(xmlUnit["Image"]);

            if (xmlUnit["Image"] == null)
                return Interface.Changes.TryHomologueImage(newUnit);

            else if (String.IsNullOrEmpty(image))
                return ImagePathByName(xmlUnit);

            else
                return FullImagePath(image);
        }

        private static string ImagePathByName(XmlNode xmlUnit, string imagePath = "")
        {
            string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(StringParse(xmlUnit["Name"]).ToLower());
            string pathByName = FullImagePath(name.Replace(" ", String.Empty), imagePath);

            return File.Exists(pathByName) ? pathByName : String.Empty;
        }

        private static string FullImagePath(string image, string imagePath = "")
        {
            string path = String.IsNullOrEmpty(imagePath) ? Army.Data.UnitsImagesDirectory : imagePath;
            return String.Format("{0}{1}.jpg", path, image);
        }

        private static string AddCommonXmlSpecialRules(string specialRule) =>
            Constants.CommonXmlSpecialRules[specialRule];

        private static Option AddCommonXmlSpecialRules(int id, string specialRule)
        {
            string rule = Constants.CommonXmlSpecialRules[specialRule];

            Option newOption = new Option
            {
                ID = id,
                IDView = id.ToString(),
                Name = specialRule,
                Type = Option.OptionType.Info,
                Category = Option.OptionCategory.SpecialRule,
                SpecialRuleDescription = new string[] { rule },
            };

            return newOption;
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
            AddToOption(xmlDocument, ref xmlNode, "Points", xmlNode.Attributes["Points"]?.InnerText ?? "0");
            AddToOption(xmlDocument, ref xmlNode, "Type", "Option");
            AddToOption(xmlDocument, ref xmlNode, "Mount", "True");

            if ((xmlNode.Attributes["Dependencies"] != null) || (xmlNode.Attributes["For"] != null))
                AddDependencies(xmlDocument, ref xmlNode);
        }

        private static void CreateWizardOption(XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            AddToOption(xmlDocument, ref xmlNode, "Points", xmlNode.Attributes["Points"]?.InnerText ?? "0");
            AddToOption(xmlDocument, ref xmlNode, "Type", "Option");

            int level = int.Parse(xmlNode.Attributes["Level"].InnerText);

            AddToOption(xmlDocument, ref xmlNode, "Name", String.Format("Wizard Level {0}", level));
            AddToOption(xmlDocument, ref xmlNode, "AddToCast", "1");
            AddToOption(xmlDocument, ref xmlNode, "AddToWizard", "1");

            if ((level == 1) || (level == 3))
                AddToOption(xmlDocument, ref xmlNode, "AddToDispell", "1");
        }

        private static string GetDependenciesLine(XmlNode xmlNode)
        {
            if (xmlNode.Attributes["For"] != null)
                return String.Format("On: {0}", xmlNode.Attributes["For"].InnerText);
            else
                return xmlNode.Attributes["Dependencies"].InnerText;
        }

        private static void AddDependencies(XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            string dependencies = GetDependenciesLine(xmlNode);

            XmlElement xmlDependencies = xmlDocument.CreateElement("Dependencies");

            List<string> allDependencies = dependencies.Split(';').ToList();

            foreach (string dependenciesWithType in allDependencies)
            {
                string dependencyType, dependencyLine;

                if (dependenciesWithType.Contains(":"))
                {
                    List<string> typeAndDependencies = dependenciesWithType.Split(':').ToList();
                    dependencyType = typeAndDependencies[0].Trim();
                    dependencyLine = typeAndDependencies[1];
                }
                else
                {
                    dependencyType = "Off";
                    dependencyLine = dependenciesWithType;
                }

                List<string> optionDependencies = dependencyLine.Split(',').ToList();

                foreach (string optionDependency in optionDependencies)
                {
                    XmlElement option = xmlDocument.CreateElement(dependencyType);
                    option.InnerText = optionDependency.Trim();
                    xmlDependencies.AppendChild(option);
                }
            }

            xmlNode.AppendChild(xmlDependencies);
        }

        private static bool EmptyNameSpecific(string name) =>
            (name == "Hand weapon") || (name == "Range weapon") || (name == "Mark");

        private static void CreateOption(string name, string attributes, XmlDocument xmlDocument, ref XmlNode xmlNode)
        {
            if (xmlNode["Name"] != null)
                return;

            if (((name == "Champion") || (name == "Musician")) && (xmlNode.Attributes["Name"] != null))
                name = xmlNode.Attributes["Name"].InnerText;

            if (EmptyNameSpecific(name) && !String.IsNullOrEmpty(xmlNode.InnerText))
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
                {
                    AddToOption(xmlDocument, ref xmlNode, attributeLine);
                }
            }

            string points = 
                xmlNode.Attributes["PointsPerModel"]?.InnerText ??
                xmlNode.Attributes["Points"]?.InnerText ??
                String.Empty;

            if (!String.IsNullOrEmpty(points))
            {
                if (!typesIncluded)
                    AddToOption(xmlDocument, ref xmlNode, "Type", "Option");

                string pointsSource = xmlNode.Attributes["PointsPerModel"] != null ?
                    "PointsPerModel" : "Points";

                AddToOption(xmlDocument, ref xmlNode, pointsSource, points);
            }

            if (xmlNode.Attributes["Dependency"] != null)
            {
                string dependency = String.Format("Group:{0}", xmlNode.Attributes["Dependency"].InnerText);
                AddToOption(xmlDocument, ref xmlNode, "Dependency", attributes: dependency);
            }

            if ((xmlNode.Attributes["Dependencies"] != null) || (xmlNode.Attributes["For"] != null))
                AddDependencies(xmlDocument, ref xmlNode);

            if (xmlNode.Attributes["Magic"] != null)
                AddToOption(xmlDocument, ref xmlNode, "MagicItems", attributes: xmlNode.Attributes["Magic"].InnerText);

            if (xmlNode.Attributes["Only"] != null)
                AddToOption(xmlDocument, ref xmlNode, "Only", xmlNode.Attributes["Only"].InnerText);

            if (xmlNode.Attributes["OnlyGroup"] != null)
                AddToOption(xmlDocument, ref xmlNode, "OnlyGroup", xmlNode.Attributes["OnlyGroup"].InnerText);
        }

        private static Option LoadOption(int id, XmlNode xmlNode, XmlDocument xmlDocument,
            string artefactGroup = null, Option.OptionCategory category = Option.OptionCategory.Nope,
            Dictionary<string, string> commonXmlOption = null)
        {
            if (Services.GetCommonXmlOption(xmlNode.Name, out string commonOption, commonXmlOption))
            {
                List<string> xmlOption = commonOption.Split('|').ToList();
                CreateOption(xmlOption[0], xmlOption[1], xmlDocument, ref xmlNode);
            }

            if ((xmlNode.Name == "Mount") || (xmlNode.Name == "Crew"))
                CreateMountOption(xmlDocument, ref xmlNode);

            if (xmlNode.Name == "Wizard")
                CreateWizardOption(xmlDocument, ref xmlNode);

            Option newOption = new Option
            {
                ID = id,
                IDView = id.ToString(),
                Type = OptionTypeParse(xmlNode),
                Only = StringParse(xmlNode["Only"]),
                Dependencies = AllStringParse(xmlNode["Dependencies"], "On", comma: true),
                InverseDependencies = AllStringParse(xmlNode["Dependencies"], "Off", comma: true),
                Realised = BoolParse(xmlNode["RealisedByDefault"]),
                Countable = CountableParse(xmlNode["Countable"]),
                ImpactHitByFront = BoolParse(xmlNode["ImpactHitByFront"]) ? 1 : 0,
                ParamTests = ParamParse(xmlNode),
                MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItemsType"]),
                ArtefactGroup = artefactGroup ?? String.Empty,
                TooltipColor = (SolidColorBrush)Data.TooltipColor,
            };

            if (xmlNode["PointsPerModel"] != null)
            {
                newOption.Points = DoubleParse(xmlNode["PointsPerModel"]);
                newOption.PerModel = true;
            }
            else
            {
                newOption.Points = DoubleParse(xmlNode["Points"]);
                newOption.VirtueOriginalPoints = DoubleParse(xmlNode["Points"]);
            }

            if (category != Option.OptionCategory.Nope)
                newOption.Category = category;

            if (xmlNode["Dependency"] != null)
            {
                string group = StringParse(xmlNode["Dependency"]?.Attributes["Group"]);
                newOption.DependencyGroup = String.IsNullOrEmpty(group) ? "Default" : group;
            }

            foreach (string name in Constants.ProfilesNames.Keys)
            {
                SetProperty(newOption, xmlNode, String.Format("AddTo{0}", name));
                SetProperty(newOption, xmlNode, String.Format("{0}To", name));
            }

            foreach (string name in Constants.OptionProperties)
                SetProperty(newOption, xmlNode, name);

            newOption.Description = StringParse(xmlNode["Description"]);
            newOption.SpecialRuleDescription = AllStringParse(xmlNode, "Rule");

            if (Constants.CommonXmlSpecialRules != null)
            {
                foreach (XmlNode specialRule in xmlNode.SelectNodes("*"))
                {
                    if (!Constants.CommonXmlSpecialRules.ContainsKey(specialRule.Name))
                        continue;

                    List<string> rulesList = newOption.SpecialRuleDescription.ToList();
                    rulesList.Add(AddCommonXmlSpecialRules(specialRule.Name));
                    newOption.SpecialRuleDescription = rulesList.ToArray();
                }
            }

            if (String.IsNullOrEmpty(newOption.Description) && (newOption.SpecialRuleDescription.Length > 0))
                newOption.Description = newOption.SpecialRuleDescription[0];

            if (!String.IsNullOrEmpty(newOption.Description) && !Regex.IsMatch(newOption.Description, @"\.$"))
                newOption.Description = String.Format("{0}.", newOption.Description);

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

        private static XmlNode XmlValueSemiParser(XmlNode value, string name)
        {
            XmlNode xmlNode = value[name];

            if (xmlNode == null)
                return null;

            if (!String.IsNullOrEmpty(xmlNode.InnerText))
                return xmlNode;

            string valueAttr = xmlNode.Attributes["Val"]?.InnerText ?? String.Empty;

            if (!String.IsNullOrEmpty(valueAttr))
                xmlNode.InnerText = valueAttr;

            return xmlNode;
        }

        private static void SetProperty(object element, XmlNode value, string name, string byAttr = "")
        {
            XmlNode xmlNode = null;
            bool notByAttr = String.IsNullOrEmpty(byAttr);

            if (notByAttr)
                xmlNode = XmlValueSemiParser(value, name);
            else
                xmlNode = (XmlNode)value.Attributes[byAttr];

            object propetyValue = PropertyByType(element, xmlNode, name, !notByAttr);

            if (propetyValue != null)
                element.GetType().GetProperty(name).SetValue(element, propetyValue);
        }
    }
}
