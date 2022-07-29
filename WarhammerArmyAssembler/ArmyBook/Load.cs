using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Media;
using System.IO;
using static WarhammerArmyAssembler.Unit;
using static WarhammerArmyAssembler.ArmyBook.Parsers;
using System.Linq;

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

            string imagePath = Path.GetDirectoryName(xmlFileName) + "\\" +
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsIn")) + "\\";

            string unitType = (isHero ? "Heroes/Hero" : "Units/Unit");
            XmlNodeList xmlNodes =  xmlFile.SelectNodes("ArmyBook/Content/" + unitType);

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
                string title = (option.Attributes["Title"] == null ? "Name" : "Title");
                string value = String.Format("{0}|{1}", option.Attributes[title].InnerText, option.InnerText);
                Constants.CommonXmlOption.Add(option.Attributes["Name"].InnerText, value);
            }
        }

        private static void LoadCommonXmlOption(XmlDocument armybook)
        {
            if (String.IsNullOrEmpty(Constants.CommonXmlOptionPath))
                return;

            Constants.CommonXmlOption = new Dictionary<string, string>();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.CommonXmlOptionPath);
            LoadCommonXmlOptionFromFile(xmlFile, "Options/*/Option");

            LoadCommonXmlOptionFromFile(armybook, "ArmyBook/Introduction/CommonXml/Option");
        }

        private static void LoadEnemies()
        {
            if (String.IsNullOrEmpty(Constants.EnemiesOptionPath))
                return;

            Enemy.CleanEnemies();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(Constants.EnemiesOptionPath);

            foreach (XmlNode group in xmlFile.SelectNodes("Enemies/Group"))
                foreach (XmlNode enemy in group.SelectNodes("Enemy"))
                    Enemy.AddEnemies(group.Attributes["List"].InnerText, enemy.InnerText);
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

            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Units/Unit", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Heroes/Hero", ref Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Content/Mounts/Mount", ref Data.Mounts);

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

            Army.Data.UnitsImagesDirectory = Path.GetDirectoryName(xmlFileName) + "\\" +
                StringParse(xmlFile.SelectSingleNode("ArmyBook/Introduction/Images/UnitsIn")) + "\\";
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
                Type = UnitTypeParse(xmlUnit["Type"]),
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

            newUnit.Movement = ProfileParse(profile.Attributes["M"]);
            newUnit.WeaponSkill = ProfileParse(profile.Attributes["WS"]);
            newUnit.BallisticSkill = ProfileParse(profile.Attributes["BS"]);
            newUnit.Strength = ProfileParse(profile.Attributes["S"]);
            newUnit.Toughness = ProfileParse(profile.Attributes["T"]);
            newUnit.Wounds = ProfileParse(profile.Attributes["W"]);
            newUnit.Initiative = ProfileParse(profile.Attributes["I"]);
            newUnit.Attacks = ProfileParse(profile.Attributes["A"]);
            newUnit.Leadership = ProfileParse(profile.Attributes["Ld"]);

            newUnit.Armour = IntNullableParse(profile.Attributes["AS"]);
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

            if (String.IsNullOrEmpty(attributes))
                return;

            bool typesIncluded = false;

            foreach (string attributeLine in attributes.Split(';').Select(x => x.Trim('\n').Trim()))
            {
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

                Name = StringParse(xmlNode["Name"]),
                Description = StringParse(xmlNode["Description"]),
                Type = OptionTypeParse(xmlNode["Type"]),
                Only = OnlyForParse(xmlNode["Only"]),
                ServiceDependencies = AllStringParse(xmlNode["Dependencies"], "On"),
                ServiceInverseDependencies = AllStringParse(xmlNode["Dependencies"], "Off"),
                OnlyOneInArmy = BoolParse(xmlNode["OnlyOneInArmy"]),
                OnlyOneSuchUnits = BoolParse(xmlNode["OnlyOneSuchUnits"]),
                OnlyGroup = StringParse(xmlNode["OnlyGroup"]),
                Realised = BoolParse(xmlNode["RealisedByDefault"]),
                Multiple = BoolParse(xmlNode["Multiple"]),
                Virtue = BoolParse(xmlNode["Virtue"]),
                Honours = BoolParse(xmlNode["Honours"]),

                Countable = CountableParse(xmlNode["Countable"]),

                SpecialRuleDescription = AllStringParse(xmlNode, "Rule"),

                NativeArmour = BoolParse(xmlNode["NativeArmour"]),
                Group = StringParse(xmlNode["Group"]),
                AutoHit = BoolParse(xmlNode["AutoHit"]),
                AutoWound = BoolParse(xmlNode["AutoWound"]),
                AutoDeath = BoolParse(xmlNode["AutoDeath"]),
                HitFirst = BoolParse(xmlNode["HitFirst"]),
                HitLast = BoolParse(xmlNode["HitLast"]),
                KillingBlow = BoolParse(xmlNode["KillingBlow"]),
                ExtendedKillingBlow = IntParse(xmlNode["ExtendedKillingBlow"]),
                HeroicKillingBlow = BoolParse(xmlNode["HeroicKillingBlow"]),
                PoisonAttack = BoolParse(xmlNode["PoisonAttack"]),
                MultiWounds = StringParse(xmlNode["MultiWounds"]),
                NoArmour = BoolParse(xmlNode["NoArmour"]),
                NoWard = BoolParse(xmlNode["NoWard"]),
                NoMultiWounds = BoolParse(xmlNode["NoMultiWounds"]),
                NoKillingBlow = BoolParse(xmlNode["NoKillingBlow"]),
                ArmourPiercing = IntParse(xmlNode["ArmourPiercing"]),

                Regeneration = BoolParse(xmlNode["Regeneration"]),
                ExtendedRegeneration = IntParse(xmlNode["ExtendedRegeneration"]),
                ImmuneToPsychology = BoolParse(xmlNode["ImmuneToPsychology"]),
                ImmuneToPoison = BoolParse(xmlNode["ImmuneToPoison"]),
                Stubborn = BoolParse(xmlNode["Stubborn"]),
                Hate = BoolParse(xmlNode["Hate"]),
                Fear = BoolParse(xmlNode["Fear"]),
                Terror = BoolParse(xmlNode["Terror"]),
                Frenzy = BoolParse(xmlNode["Frenzy"]),
                BloodFrenzy = BoolParse(xmlNode["BloodFrenzy"]),
                Unbreakable = BoolParse(xmlNode["Unbreakable"]),
                ColdBlooded = BoolParse(xmlNode["ColdBlooded"]),
                Reroll = StringParse(xmlNode["Reroll"]),
                Stupidity = BoolParse(xmlNode["Stupidity"]),
                Undead = BoolParse(xmlNode["Undead"]),
                StrengthInNumbers = BoolParse(xmlNode["StrengthInNumbers"]),
                ImpactHit = StringParse(xmlNode["ImpactHit"]),
                ImpactHitByFront = (BoolParse(xmlNode["ImpactHitByFront"]) ? 1 : 0),
                SteamTank = BoolParse(xmlNode["SteamTank"]),
                Lance = BoolParse(xmlNode["Lance"]),
                Flail = BoolParse(xmlNode["Flail"]),
                ChargeStrengthBonus = IntParse(xmlNode["ChargeStrengthBonus"]),
                Resolute = BoolParse(xmlNode["Resolute"]),
                PredatoryFighter = BoolParse(xmlNode["PredatoryFighter"]),
                MurderousProwess = BoolParse(xmlNode["MurderousProwess"]),
                AddToCloseCombat = StringParse(xmlNode["AddToCloseCombat"]),
                Bloodroar = BoolParse(xmlNode["Bloodroar"]),
                AddToHit = IntParse(xmlNode["AddToHit"]),
                SubOpponentToHit = IntParse(xmlNode["SubOpponentToHit"]),
                AddToWound = IntParse(xmlNode["AddToWound"]),
                SubOpponentToWound = IntParse(xmlNode["SubOpponentToWound"]),
                HitOn = IntParse(xmlNode["HitOn"]),
                OpponentHitOn = IntParse(xmlNode["OpponentHitOn"]),
                WoundOn = IntParse(xmlNode["WoundOn"]),
                Runic = IntParse(xmlNode["Runic"]),
                MasterRunic = BoolParse(xmlNode["MasterRunic"]),
                RandomGroup = StringParse(xmlNode["RandomGroup"]),
                TypeUnitIncrese = BoolParse(xmlNode["TypeUnitIncrese"]),
                WardForFirstWound = IntParse(xmlNode["WardForFirstWound"]),
                WardForLastWound = IntParse(xmlNode["WardForLastWound"]),
                FirstWoundDiscount = BoolParse(xmlNode["FirstWoundDiscount"]),
                NotALeader = BoolParse(xmlNode["NotALeader"]),

                ParamTests = ParamParse(xmlNode),

                Points = DoubleParse(xmlNode["Points"]),
                PerModel = BoolParse(xmlNode["PerModel"]),
                VirtueOriginalPoints = DoubleParse(xmlNode["Points"]),
                MagicItemsPoints = BoolParse(xmlNode["MagicItemsPoints"]),

                AddToMovement = IntParse(xmlNode["AddToMovement"]),
                AddToWeaponSkill = IntParse(xmlNode["AddToWeaponSkill"]),
                AddToBallisticSkill = IntParse(xmlNode["AddToBallisticSkill"]),
                AddToStrength = IntParse(xmlNode["AddToStrength"]),
                AddToToughness = IntParse(xmlNode["AddToToughness"]),
                AddToWounds = IntParse(xmlNode["AddToWounds"]),
                AddToInitiative = IntParse(xmlNode["AddToInitiative"]),
                AddToAttacks = IntParse(xmlNode["AddToAttacks"]),
                AddToLeadership = IntParse(xmlNode["AddToLeadership"]),
                AddToArmour = IntParse(xmlNode["AddToArmour"]),
                AddToWard = IntParse(xmlNode["AddToWard"]),
                AddToCast = IntParse(xmlNode["AddToCast"]),
                AddToDispell = IntParse(xmlNode["AddToDispell"]),
                AddToWizard = IntParse(xmlNode["AddToWizard"]),
                AddToModelsInPack = IntParse(xmlNode["AddToModelsInPack"]),

                MovementTo = IntParse(xmlNode["MovementTo"]),
                WeaponSkillTo = IntParse(xmlNode["WeaponSkillTo"]),
                BallisticSkillTo = IntParse(xmlNode["BallisticSkillTo"]),
                StrengthTo = IntParse(xmlNode["StrengthTo"]),
                ToughnessTo = IntParse(xmlNode["ToughnessTo"]),
                WoundsTo = IntParse(xmlNode["WoundsTo"]),
                InitiativeTo = IntParse(xmlNode["InitiativeTo"]),
                AttacksTo = IntParse(xmlNode["AttacksTo"]),
                LeadershipTo = IntParse(xmlNode["LeadershipTo"]),
                ArmourTo = IntParse(xmlNode["ArmourTo"]),
                WizardTo = IntParse(xmlNode["WizardTo"]),

                MagicItems = IntParse(xmlNode["MagicItems"]),
                MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItemsType"]),

                CommandGroup = BoolParse(xmlNode["CommandGroup"]),
                PersonifiedCommander = BoolParse(xmlNode["PersonifiedCommander"]),

                Mount = BoolParse(xmlNode["Mount"]),

                ArtefactGroup = artefactGroup ?? String.Empty,
                TooltipColor = (SolidColorBrush)Data.TooltipColor,
                OnlyRuleOption = BoolParse(xmlNode["OnlyRuleOption"]),
            };

            if (xmlNode["MagicItems"] != null)
            {
                newOption.MagicItems = IntParse(xmlNode["MagicItems"].Attributes["Points"]);
                newOption.MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItems"].Attributes["Type"]);
            }

            return newOption;
        }
    }
}
