using System;
using System.Collections.Generic;
using System.Xml;
using static WarhammerArmyAssembler.Unit;
using static WarhammerArmyAssembler.ArmyBook.Parsers;
using System.Windows.Media;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Load
    {
        public static int GetNextIndex() => ArmyBook.Data.MaxIDindex++;

        private static void LoadUnitsFromXml(XmlDocument xmlFile, string path, ref Dictionary<int, Unit> dict)
        {
            XmlNodeList xmlNodes = xmlFile.SelectNodes(path);

            foreach (XmlNode xmlUnit in xmlNodes)
            {
                int newID = GetNextIndex();
                dict.Add(newID, LoadUnit(newID, xmlUnit, xmlFile));
            }
        }

        public static void LoadArmy(string xmlFileName)
        {
            ArmyBook.Data.Units.Clear();
            ArmyBook.Data.Mounts.Clear();
            ArmyBook.Data.Artefact.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/SymbolImage");
            Interface.Changes.LoadArmyImage(armyFile, xmlFileName);

            Army.Data.Name = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName"));
            Army.Data.ArmyVersion = IntParse(xmlFile.SelectSingleNode("ArmyBook/Info/ArmyBookVersion"));
            Army.Data.MagicPowers = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/MagicPowers"));

            ArmyBook.Data.MainColor = Interface.Other.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));
            ArmyBook.Data.AdditionalColor = Interface.Other.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/AdditionalColor"));
            ArmyBook.Data.BackgroundColor = Interface.Other.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/BackgroundColor"));

            ArmyBook.Data.DemonicMortal = BoolParse(xmlFile.SelectSingleNode("ArmyBook/Info/DemonicMortal"));

            Interface.Mod.SetArmyGridAltColor(ArmyBook.Data.BackgroundColor);

            LoadUnitsFromXml(xmlFile, "ArmyBook/Units/Unit", ref ArmyBook.Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Heroes/Hero", ref ArmyBook.Data.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Mounts/Mount", ref ArmyBook.Data.Mounts);

            foreach (XmlNode xmlArtefactGroup in xmlFile.SelectNodes("ArmyBook/Artefacts/ArtefactsGroup"))
            {
                string groupName = xmlArtefactGroup.Attributes["Name"].Value;

                foreach (XmlNode xmlArtefact in xmlArtefactGroup.SelectNodes("Artefact"))
                {
                    int newID = GetNextIndex();
                    ArmyBook.Data.Artefact.Add(newID, LoadOption(newID, xmlArtefact, groupName));
                }
            }
        }

        public static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml)
        {
            string description = StringParse(xmlUnit["Description"]);

            if (String.IsNullOrEmpty(description))
                description = StringParse(xmlUnit["Name"]);

            XmlNode mainParam = xmlUnit["MainParam"];

            Unit newUnit = new Unit
            {
                ID = id,
                IDView = id.ToString(),

                Name = StringParse(xmlUnit["Name"]),
                Type = UnitTypeParse(xmlUnit["Type"]),
                Points = DoubleParse(xmlUnit["Points"]),
                Size = IntParse(xmlUnit["MinSize"]),
                MinSize = IntParse(xmlUnit["MinSize"]),
                MaxSize = IntParse(xmlUnit["MaxSize"]),
                UniqueUnits = BoolParse(xmlUnit["UniqueUnits"]),
                Wizard = IntParse(xmlUnit["Wizard"]),
                MountOn = IntParse(xmlUnit["MountOn"]),
                MountInit = StringParse(xmlUnit["MountInit"]),
                ModelsInPack = IntParse(xmlUnit["ModelsInPack"], byDefault: 1),

                Description = description,

                Movement = IntParse(mainParam["Movement"]),
                WeaponSkill = IntParse(mainParam["WeaponSkill"]),
                BallisticSkill = IntParse(mainParam["BallisticSkill"]),
                Strength = IntParse(mainParam["Strength"]),
                Toughness = IntParse(mainParam["Toughness"]),
                Wounds = IntParse(mainParam["Wounds"]),
                Initiative = IntParse(mainParam["Initiative"]),
                Attacks = IntParse(mainParam["Attacks"]),
                Leadership = IntParse(mainParam["Leadership"]),

                Armour = IntNullableParse(mainParam["Armour"]),
                Ward = IntNullableParse(mainParam["Ward"]),

                PersonifiedHero = BoolParse(xmlUnit["PersonifiedHero"]),
                WeaponTeam = BoolParse(xmlUnit["WeaponTeam"]),
                Chariot = BoolParse(xmlUnit["Chariot"]),
            };

            XmlNode additionalParam = xmlUnit["AdditionalParam"];

            newUnit.SetGroup(StringParse(xmlUnit["Group"]));

            if (additionalParam != null)
            {
                newUnit.NoKillingBlow = BoolParse(additionalParam["NoKillingBlow"]);
                newUnit.NoMultiWounds = BoolParse(additionalParam["NoMultiWounds"]);
                newUnit.LargeBase = BoolParse(additionalParam["LargeBase"]);
                newUnit.ImmuneToPsychology = BoolParse(additionalParam["ImmuneToPsychology"]);
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
                newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
                newUnit.ExtendedKillingBlow = IntParse(additionalParam["ExtendedKillingBlow"]);
                newUnit.HeroicKillingBlow = BoolParse(additionalParam["HeroicKillingBlow"]);
                newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);
                newUnit.MultiWounds = StringParse(additionalParam["MultiWounds"]);
                newUnit.NoArmour = BoolParse(additionalParam["NoArmour"]);
                newUnit.NoWard = BoolParse(additionalParam["NoWard"]);
                newUnit.ArmourPiercing = IntParse(additionalParam["ArmourPiercing"]);
                newUnit.MagicItems = IntParse(additionalParam["MagicItems"]);
                newUnit.MagicItemCount = IntParse(additionalParam["MagicItemCount"]);
                newUnit.MagicItemsType = MagicItemsTypeParse(additionalParam["MagicItemsType"]);
                newUnit.MagicPowers = IntParse(additionalParam["MagicPowers"]);
                newUnit.MagicPowersCount = IntParse(additionalParam["MagicPowersCount"]);
                newUnit.NotALeader = BoolParse(additionalParam["NotALeader"]);
                newUnit.MustBeGeneral = BoolParse(additionalParam["MustBeGeneral"]);
                newUnit.Reroll = StringParse(additionalParam["Reroll"]);
                newUnit.ImpactHit = StringParse(additionalParam["ImpactHit"]);
                newUnit.SteamTank = BoolParse(additionalParam["SteamTank"]);
                newUnit.HellPitAbomination = BoolParse(additionalParam["HellPitAbomination"]);
                newUnit.Giant = BoolParse(additionalParam["Giant"]);
                newUnit.Lance = BoolParse(additionalParam["Lance"]);
                newUnit.Flail = BoolParse(additionalParam["Flail"]);
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

                newUnit.ParamTests = ParamParse(additionalParam);

                newUnit.SlotOf = SlotsParse(additionalParam);
                newUnit.NoCoreSlot = BoolParse(additionalParam["NoCoreSlot"]);

                if (newUnit.Frenzy)
                    xmlUnit.SelectSingleNode("SpecialRulesAndAmmunition").AppendChild(ArmyBook.Other.AddFrenzyAttack(xml));
            }

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("SpecialRulesAndAmmunition/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Options/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            newUnit.SizableType = (!newUnit.IsHero() && (newUnit.Type != UnitType.Mount));
            newUnit.VisibleType = (newUnit.SizableType ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);

            newUnit.ArmyColor = (SolidColorBrush)ArmyBook.Data.MainColor;
            newUnit.ArmyBackgroundColor = (SolidColorBrush)ArmyBook.Data.BackgroundColor;

            return newUnit;
        }

        public static Option LoadOption(int id, XmlNode xmlNode, string artefactGroup = null) => new Option
        {
            ID = id,
            IDView = id.ToString(),

            Name = StringParse(xmlNode["Name"]),
            Description = StringParse(xmlNode["Description"]),
            Type = OptionTypeParse(xmlNode["Type"]),
            OnlyFor = OnlyForParse(xmlNode["OnlyFor"]),
            OnlyIfAnotherService = AllStringParse(xmlNode["OnlyIfAnotherService"], "OnlyIf"),
            OnlyIfNotAnotherService = AllStringParse(xmlNode["OnlyIfAnotherService"], "OnlyIfNot"),
            OnlyOneInArmy = BoolParse(xmlNode["OnlyOneInArmy"]),
            OnlyOneForSuchUnits = BoolParse(xmlNode["OnlyOneForSuchUnits"]),
            OnlyForGroup = StringParse(xmlNode["OnlyForGroup"]),
            Realised = BoolParse(xmlNode["RealisedByDefault"]),
            Multiple = BoolParse(xmlNode["Multiple"]),
            Virtue = BoolParse(xmlNode["Virtue"]),
            Honours = BoolParse(xmlNode["Honours"]),

            Countable = CountableParse(xmlNode),

            SpecialRuleDescription = AllStringParse(xmlNode, "SpecialRuleDescription"),

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
            ImmuneToPsychology = BoolParse(xmlNode["ImmuneToPsychology"]),
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
            SteamTank = BoolParse(xmlNode["SteamTank"]),
            Lance = BoolParse(xmlNode["Lance"]),
            Flail = BoolParse(xmlNode["Flail"]),
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
            TypeUnitIncrese = BoolParse(xmlNode["TypeUnitIncrese"]),
            WardForFirstWound = IntParse(xmlNode["WardForFirstWound"]),
            WardForLastWound = IntParse(xmlNode["WardForLastWound"]),
            FirstWoundDiscount = BoolParse(xmlNode["FirstWoundDiscount"]),
            NotALeader = BoolParse(xmlNode["NotALeader"]),

            ParamTests = ParamParse(xmlNode),

            Points = DoubleParse(xmlNode["Points"]),
            PerModel = BoolParse(xmlNode["PerModel"]),
            VirtueOriginalPoints = DoubleParse(xmlNode["Points"]),

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

            FullCommand = BoolParse(xmlNode["FullCommand"]),
            PersonifiedCommander = BoolParse(xmlNode["PersonifiedCommander"]),

            Mount = BoolParse(xmlNode["Mount"]),

            ArtefactGroup = artefactGroup ?? String.Empty,
            ArmyBackgroundColor = (SolidColorBrush)ArmyBook.Data.BackgroundColor,
        };
    }
}
