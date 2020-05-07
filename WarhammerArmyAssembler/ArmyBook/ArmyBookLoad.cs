using System;
using System.Collections.Generic;
using System.Xml;
using static WarhammerArmyAssembler.Unit;
using static WarhammerArmyAssembler.ArmyBookParsers;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class ArmyBookLoad
    {
        public static int GetNextIndex()
        {
            return ArmyBook.MaxIDindex++;
        }

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
            ArmyBook.Units.Clear();
            ArmyBook.Mounts.Clear();
            ArmyBook.Artefact.Clear();

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(xmlFileName);

            XmlNode armyFile = xmlFile.SelectSingleNode("ArmyBook/Info/SymbolImage");
            Interface.LoadArmyImage(armyFile, xmlFileName);

            Army.ArmyName = StringParse(xmlFile.SelectSingleNode("ArmyBook/Info/ArmyName"));

            ArmyBook.MainColor = InterfaceOther.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/MainColor"));
            ArmyBook.AdditionalColor = InterfaceOther.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/AdditionalColor"));
            ArmyBook.BackgroundColor = InterfaceOther.BrushFromXml(xmlFile.SelectSingleNode("ArmyBook/Info/BackgroundColor"));

            ArmyBook.DemonicMortal = BoolParse(xmlFile.SelectSingleNode("ArmyBook/Info/DemonicMortal"));

            InterfaceMod.SetArmyGridAltColor(ArmyBook.BackgroundColor);

            LoadUnitsFromXml(xmlFile, "ArmyBook/Units/Unit", ref ArmyBook.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Heroes/Hero", ref ArmyBook.Units);
            LoadUnitsFromXml(xmlFile, "ArmyBook/Mounts/Mount", ref ArmyBook.Mounts);

            foreach (XmlNode xmlArtefactGroup in xmlFile.SelectNodes("ArmyBook/Artefacts/ArtefactsGroup"))
            {
                string groupName = xmlArtefactGroup.Attributes["Name"].Value;

                foreach (XmlNode xmlArtefact in xmlArtefactGroup.SelectNodes("Artefact"))
                {
                    int newID = GetNextIndex();
                    ArmyBook.Artefact.Add(newID, LoadOption(newID, xmlArtefact, groupName));
                }
            }
        }

        public static Unit LoadUnit(int id, XmlNode xmlUnit, XmlDocument xml)
        {
            Unit newUnit = new Unit();

            newUnit.ID = id;
            newUnit.IDView = id.ToString();

            newUnit.Name = StringParse(xmlUnit["Name"]);
            newUnit.Group = StringParse(xmlUnit["Group"]);
            newUnit.Type = UnitTypeParse(xmlUnit["Type"]);
            newUnit.Points = IntParse(xmlUnit["Points"]);
            newUnit.Size = IntParse(xmlUnit["MinSize"]);
            newUnit.MinSize = newUnit.Size;
            newUnit.MaxSize = IntParse(xmlUnit["MaxSize"]);
            newUnit.Wizard = IntParse(xmlUnit["Wizard"]);
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

            newUnit.PersonifiedHero = BoolParse(xmlUnit["PersonifiedHero"]);
            newUnit.WeaponTeam = BoolParse(xmlUnit["WeaponTeam"]);
            newUnit.Chariot = BoolParse(xmlUnit["Chariot"]);

            XmlNode additionalParam = xmlUnit["AdditionalParam"];

            if (additionalParam != null)
            {
                newUnit.UnitStrength = IntParse(additionalParam["UnitStrength"], byDefault: 1);

                newUnit.ImmuneToPsychology = BoolParse(additionalParam["ImmuneToPsychology"]);
                newUnit.Stubborn = BoolParse(additionalParam["Stubborn"]);
                newUnit.Hate = BoolParse(additionalParam["Hate"]);
                newUnit.Fear = BoolParse(additionalParam["Fear"]);
                newUnit.Terror = BoolParse(additionalParam["Terror"]);
                newUnit.Frenzy = BoolParse(additionalParam["Frenzy"]);
                newUnit.Unbreakable = BoolParse(additionalParam["Unbreakable"]);
                newUnit.ColdBlooded = BoolParse(additionalParam["ColdBlooded"]);
                newUnit.Stupidity = BoolParse(additionalParam["Stupidity"]);
                newUnit.Undead = BoolParse(additionalParam["Undead"]);
                newUnit.StrengthInNumbers = BoolParse(additionalParam["StrengthInNumbers"]);
                newUnit.AutoHit = BoolParse(additionalParam["AutoHit"]);
                newUnit.AutoWound = BoolParse(additionalParam["AutoWound"]);
                newUnit.HitFirst = BoolParse(additionalParam["HitFirst"]);
                newUnit.HitLast = BoolParse(additionalParam["HitLast"]);
                newUnit.Regeneration = BoolParse(additionalParam["Regeneration"]);
                newUnit.KillingBlow = BoolParse(additionalParam["KillingBlow"]);
                newUnit.PoisonAttack = BoolParse(additionalParam["PoisonAttack"]);
                newUnit.MultiWounds = StringParse(additionalParam["MultiWounds"]);
                newUnit.NoArmour = BoolParse(additionalParam["NoArmour"]);
                newUnit.ArmourPiercing = IntParse(additionalParam["ArmourPiercing"]);
                newUnit.MagicItems = IntParse(additionalParam["MagicItems"]);
                newUnit.MagicItemsType = MagicItemsTypeParse(additionalParam["MagicItemsType"]);
                newUnit.NotALeader = BoolParse(additionalParam["NotALeader"]);
                newUnit.MustBeGeneral = BoolParse(additionalParam["MustBeGeneral"]);
                newUnit.Reroll = StringParse(additionalParam["Reroll"]);
                newUnit.ImpactHit = StringParse(additionalParam["ImpactHit"]);

                newUnit.SlotsOfLords = IntParse(additionalParam["SlotsOfLords"]);
                newUnit.SlotsOfHero = IntParse(additionalParam["SlotsOfHero"]);
                newUnit.SlotsOfSpecial = IntParse(additionalParam["SlotsOfSpecial"]);
                newUnit.SlotsOfRare = IntParse(additionalParam["SlotsOfRare"]);
                newUnit.NoSlotsOfCore = BoolParse(additionalParam["NoSlotsOfCore"]);

                if (newUnit.Frenzy)
                    xmlUnit.SelectSingleNode("SpecialRulesAndAmmunition").AppendChild(ArmyBookOther.AddFrenzyAttack(xml));
            }

            if (newUnit.UnitStrength == 0)
                newUnit.UnitStrength = 1;

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("SpecialRulesAndAmmunition/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            foreach (XmlNode xmlAmmunition in xmlUnit.SelectNodes("Options/*"))
                newUnit.Options.Add(LoadOption(GetNextIndex(), xmlAmmunition));

            newUnit.SizableType = (!newUnit.IsHero() && (newUnit.Type != UnitType.Mount));

            newUnit.ArmyColor = (SolidColorBrush)ArmyBook.MainColor;

            return newUnit;
        }

        public static Option LoadOption(int id, XmlNode xmlNode, string artefactGroup = null)
        {
            Option newWeapon = new Option();

            newWeapon.ID = id;
            newWeapon.IDView = id.ToString();

            newWeapon.Name = StringParse(xmlNode["Name"]);
            newWeapon.Description = StringParse(xmlNode["Description"]);
            newWeapon.Type = OptionTypeParse(xmlNode["Type"]);
            newWeapon.OnlyFor = OnlyForParse(xmlNode["OnlyFor"]);
            newWeapon.OnlyIfAnotherService = AllStringParse(xmlNode["OnlyIfAnotherService"], "OnlyIf");
            newWeapon.OnlyIfNotAnotherService = AllStringParse(xmlNode["OnlyIfAnotherService"], "OnlyIfNot");
            newWeapon.OnlyOneInArmy = BoolParse(xmlNode["OnlyOneInArmy"]);
            newWeapon.OnlyOneForSuchUnits = BoolParse(xmlNode["OnlyOneForSuchUnits"]);
            newWeapon.OnlyForGroup = StringParse(xmlNode["OnlyForGroup"]);
            newWeapon.Realised = false;
            newWeapon.Multiple = BoolParse(xmlNode["Multiple"]);
            newWeapon.SpecialRuleDescription = AllStringParse(xmlNode, "SpecialRuleDescription");

            newWeapon.AutoHit = BoolParse(xmlNode["AutoHit"]);
            newWeapon.HitFirst = BoolParse(xmlNode["HitFirst"]);
            newWeapon.HitLast = BoolParse(xmlNode["HitLast"]);
            newWeapon.KillingBlow = BoolParse(xmlNode["KillingBlow"]);
            newWeapon.PoisonAttack = BoolParse(xmlNode["PoisonAttack"]);
            newWeapon.MultiWounds = StringParse(xmlNode["MultiWounds"]);
            newWeapon.NoArmour = BoolParse(xmlNode["NoArmour"]);
            newWeapon.ArmourPiercing = IntParse(xmlNode["ArmourPiercing"]);

            newWeapon.Regeneration = BoolParse(xmlNode["Regeneration"]);
            newWeapon.ImmuneToPsychology = BoolParse(xmlNode["ImmuneToPsychology"]);
            newWeapon.Stubborn = BoolParse(xmlNode["Stubborn"]);
            newWeapon.Hate = BoolParse(xmlNode["Hate"]);
            newWeapon.Fear = BoolParse(xmlNode["Fear"]);
            newWeapon.Terror = BoolParse(xmlNode["Terror"]);
            newWeapon.Frenzy = BoolParse(xmlNode["Frenzy"]);
            newWeapon.Unbreakable = BoolParse(xmlNode["Unbreakable"]);
            newWeapon.ColdBlooded = BoolParse(xmlNode["ColdBlooded"]);
            newWeapon.Reroll = StringParse(xmlNode["Reroll"]);
            newWeapon.Stupidity = BoolParse(xmlNode["Stupidity"]);
            newWeapon.Undead = BoolParse(xmlNode["Undead"]);
            newWeapon.StrengthInNumbers = BoolParse(xmlNode["StrengthInNumbers"]);
            newWeapon.ImpactHit = StringParse(xmlNode["ImpactHit"]);

            newWeapon.Points = DoubleParse(xmlNode["Points"]);
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
            newWeapon.AddToWizard = IntParse(xmlNode["AddToWizard"]);
            newWeapon.AddToModelsInPack = IntParse(xmlNode["AddToModelsInPack"]);

            newWeapon.MovementTo = IntParse(xmlNode["MovementTo"]);
            newWeapon.WeaponSkillTo = IntParse(xmlNode["WeaponSkillTo"]);
            newWeapon.BallisticSkillTo = IntParse(xmlNode["BallisticSkillTo"]);
            newWeapon.StrengthTo = IntParse(xmlNode["StrengthTo"]);
            newWeapon.ToughnessTo = IntParse(xmlNode["ToughnessTo"]);
            newWeapon.WoundsTo = IntParse(xmlNode["WoundsTo"]);
            newWeapon.InitiativeTo = IntParse(xmlNode["InitiativeTo"]);
            newWeapon.AttacksTo = IntParse(xmlNode["AttacksTo"]);
            newWeapon.LeadershipTo = IntParse(xmlNode["LeadershipTo"]);
            newWeapon.ArmourTo = IntParse(xmlNode["ArmourTo"]);
            newWeapon.WizardTo = IntParse(xmlNode["WizardTo"]);

            newWeapon.MagicItems = IntParse(xmlNode["MagicItems"]);
            newWeapon.MagicItemsType = MagicItemsTypeParse(xmlNode["MagicItemsType"]);

            newWeapon.FullCommand = BoolParse(xmlNode["FullCommand"]);
            newWeapon.PersonifiedCommander = BoolParse(xmlNode["PersonifiedCommander"]);

            newWeapon.Mount = BoolParse(xmlNode["Mount"]);

            newWeapon.ArtefactGroup = artefactGroup ?? String.Empty;

            return newWeapon;
        }
    }
}
