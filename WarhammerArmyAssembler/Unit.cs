using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace WarhammerArmyAssembler
{
    public class Unit
    {
        public enum UnitType { Lord, Hero, Core, Special, Rare, Mount }

        public enum MagicItemsTypes { Hero, Wizard, Unit }

        public string Name { get; set; }
        public string Group { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }

        public UnitType Type { get; set; }
        public bool SizableType { get; set; }

        public int Size { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public int ModelsInPack { get; set; }

        public int Points { get; set; }

        public string PointsView { get; set; }

        public string Description { get; set; }

        public int Movement { get; set; }
        public int WeaponSkill { get; set; }
        public int BallisticSkill { get; set; }
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Wounds { get; set; }
        public int Initiative { get; set; }
        public int Attacks { get; set; }
        public int Leadership { get; set; }
        public int? Armour { get; set; }
        public int? Ward { get; set; }

        public int Wizard { get; set; }

        public string MovementView { get; set; }
        public string WeaponSkillView { get; set; }
        public string BallisticSkillView { get; set; }
        public string StrengthView { get; set; }
        public string ToughnessView { get; set; }
        public string WoundsView { get; set; }
        public string InitiativeView { get; set; }
        public string AttacksView { get; set; }
        public string LeadershipView { get; set; }
        public string ArmourView { get; set; }
        public string WardView { get; set; }

        public bool ImmuneToPsychology { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }
        public bool Stupidity { get; set; }

        public bool HitFirst { get; set; }
        public bool Regeneration { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }

        public int SlotsOfLords { get; set; }
        public int SlotsOfHero { get; set; }
        public int SlotsOfSpecial { get; set; }
        public int SlotsOfRare { get; set; }

        public int MagicItems { get; set; }
        public MagicItemsTypes MagicItemsType { get; set; }

        public int MountOn { get; set; }
        public string MountInit { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }

        public bool PersonifiedHero { get; set; }
        public bool WeaponTeam { get; set; }

        public List<Option> Options = new List<Option>();

        public ObservableCollection<Unit> Items { get; set; }

        public string RulesView { get; set; }

        public Unit()
        {
            this.Items = new ObservableCollection<Unit>();
        }

        public double GetUnitPoints()
        {
            double points = Size * Points;

            foreach (Option option in Options)
                if (!option.IsOption() || (option.IsOption() && option.Realised && !option.IsSlannOption()))
                    points += option.Points * (option.PerModel ? Size : 1);

            bool firstSlannOptionAlreadyIsFree = false;

            foreach (Option option in Options)
                if (option.IsSlannOption() && option.Realised)
                    if (firstSlannOptionAlreadyIsFree)
                        points += option.Points;
                    else
                        firstSlannOptionAlreadyIsFree = true;

            return points;
        }

        public int GetUnitWizard()
        {
            int wizard = Wizard;

            foreach (Option option in Options)
                if (!option.IsOption() || (option.IsOption() && option.Realised))
                    wizard += option.AddToWizard;

            return wizard;
        }

        public Unit Clone()
        {
            Unit newUnit = new Unit();

            newUnit.Name = this.Name;
            newUnit.Group = this.Group;
            newUnit.ID = this.ID;
            newUnit.IDView = this.IDView;
            newUnit.Type = this.Type;
            newUnit.Size = this.Size;
            newUnit.MinSize = this.MinSize;
            newUnit.MaxSize = this.MaxSize;
            newUnit.ModelsInPack = this.ModelsInPack;
            newUnit.Points = this.Points;
            newUnit.MountOn = this.MountOn;
            newUnit.MountInit = this.MountInit;
            newUnit.Description = this.Description;

            newUnit.Movement = this.Movement;
            newUnit.WeaponSkill = this.WeaponSkill;
            newUnit.BallisticSkill = this.BallisticSkill;
            newUnit.Strength = this.Strength;
            newUnit.Toughness = this.Toughness;
            newUnit.Wounds = this.Wounds;
            newUnit.Initiative = this.Initiative;
            newUnit.Attacks = this.Attacks;
            newUnit.Leadership = this.Leadership;
            newUnit.Armour = this.Armour;
            newUnit.Ward = this.Ward;
            newUnit.Wizard = this.Wizard;

            newUnit.ImmuneToPsychology = this.ImmuneToPsychology;
            newUnit.Stubborn = this.Stubborn;
            newUnit.Hate = this.Hate;
            newUnit.Fear = this.Fear;
            newUnit.Terror = this.Terror;
            newUnit.Frenzy = this.Frenzy;
            newUnit.Unbreakable = this.Unbreakable;
            newUnit.ColdBlooded = this.ColdBlooded;
            newUnit.Stupidity = this.Stupidity;
            newUnit.HitFirst = this.HitFirst;
            newUnit.Regeneration = this.Regeneration;
            newUnit.KillingBlow = this.KillingBlow;
            newUnit.PoisonAttack = this.PoisonAttack;

            newUnit.SlotsOfLords = this.SlotsOfLords;
            newUnit.SlotsOfHero = this.SlotsOfHero;
            newUnit.SlotsOfSpecial = this.SlotsOfSpecial;
            newUnit.SlotsOfRare = this.SlotsOfRare;

            newUnit.MagicItems = this.MagicItems;
            newUnit.MagicItemsType = this.MagicItemsType;

            newUnit.SizableType = this.SizableType;
            newUnit.PersonifiedHero = this.PersonifiedHero;
            newUnit.WeaponTeam = this.WeaponTeam;

            List <Option> Option = new List<Option>();
            foreach (Option option in this.Options)
                newUnit.Options.Add(option.Clone());

            return newUnit;
        }

        public string AddFromAnyOption(string name, bool reversParam = false,
            bool mountParam = false, bool doNotCombine = false)
        {
            PropertyInfo unitParam = typeof(Unit).GetProperty(name);
            object paramObject = unitParam.GetValue(this);
            int? paramValue = (int?)paramObject;

            string paramModView = String.Empty;

            List<Option> allOption = new List<Option>(Options);

            if (mountParam && (MountOn > 0))
                allOption.AddRange(Army.Units[MountOn].Options);

            foreach (Option option in allOption)
                if (option.IsMagicItem() || (option.IsOption() && option.Realised))
                {
                    PropertyInfo optionParam = typeof(Option).GetProperty(String.Format("AddTo{0}", name));
                    object optionObject = optionParam.GetValue(option);
                    int optionValue = (int)optionObject;

                    if (optionValue > 0 && reversParam)
                    {
                        if (paramValue == null)
                            paramValue = 7;

                        if (doNotCombine)
                        {
                            if (optionValue < paramValue)
                                paramValue = optionValue;
                        }
                        else
                            paramValue -= (7 - optionValue);

                        paramModView = "+";
                    }
                    else if (optionValue > 0)
                    {
                        paramModView += '*';
                        paramValue += optionValue;

                        if (paramValue > 10)
                            paramValue = 10;
                    }
                }

            return paramValue.ToString() + paramModView;
        }

        public Unit GetOptionRules()
        {
            Unit unit = this.Clone();

            unit.MovementView = AddFromAnyOption("Movement");
            unit.WeaponSkillView = AddFromAnyOption("WeaponSkill");
            unit.BallisticSkillView = AddFromAnyOption("BallisticSkill");
            unit.StrengthView = AddFromAnyOption("Strength");
            unit.ToughnessView = AddFromAnyOption("Toughness");
            unit.WoundsView = AddFromAnyOption("Wounds");
            unit.InitiativeView = AddFromAnyOption("Initiative");
            unit.AttacksView = AddFromAnyOption("Attacks");
            unit.LeadershipView = AddFromAnyOption("Leadership");

            unit.ArmourView = AddFromAnyOption("Armour", reversParam: true, mountParam: true);
            unit.WardView = AddFromAnyOption("Ward", reversParam: true, doNotCombine: true);

            return unit;
        }

        public void AddAmmunition(int id)
        {
            Options.Add(ArmyBook.Artefact[id].Clone());
        }

        public void AddOption(int optionID, Unit unit, int unitID)
        {
            for(int i = 0; i < unit.Options.Count; i++)
            {
                Option option = unit.Options[i];
                if (option.ID == optionID)
                {
                    bool realise = false;

                    if (option.IsMagicItem())
                        unit.Options.Remove(option);
                    else
                    {
                        if (option.Realised)
                            option.Realised = false;
                        else
                        {
                            double optionPoints = (option.PerModel ? option.Points * Army.Units[unitID].Size : option.Points);

                            if (!Army.IsArmyUnitsPointsPercentOk(Army.Units[unitID].Type, option.Points))
                            {
                                Interface.Error(String.Format("The {0} has reached a point cost limit", Army.UnitTypeName(Army.Units[unitID].Type)));
                                return;
                            }
                            else if (!Interface.EnoughUnitPointsForAddOption(optionPoints))
                            {
                                Interface.Error(String.Format("Not enough points to add", Army.UnitTypeName(Army.Units[unitID].Type)));
                                return;
                            }
                            else
                                realise = true;
                        }
                    }

                    if (option.Mount && realise)
                    {
                        foreach (KeyValuePair<int, Unit> mount in ArmyBook.Mounts)
                            if (mount.Value.Name == option.Name)
                                Interface.ArmyGridDrop(mount.Key, points: option.Points, unit: unitID);
                    }
                    else if (option.Mount && !realise)
                    {
                        Army.DeleteUnitByID(Army.Units[unitID].MountOn);
                        unit.MountOn = 0;
                    }

                    option.Realised = realise;

                    return;
                }
            }
        }

        public string GetSpecialRulesLine()
        {
            string rules = String.Empty;

            foreach (string rule in GetSpecialRules())
                rules += rule + "; ";

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }

        public bool RuleFromAnyOption(string name)
        {
            PropertyInfo unitField = typeof(Unit).GetProperty(name);
            object unitValue = unitField.GetValue(this);

            bool anyIsTrue = (bool)unitValue ? true : false;

            foreach(Option option in Options)
            {
                if (option.IsOption() && !option.Realised)
                    continue;

                PropertyInfo optionField = typeof(Option).GetProperty(name);
                object fieldValue = optionField.GetValue(option);

                bool isValueTrue = (bool)fieldValue;
                anyIsTrue = isValueTrue ? true : anyIsTrue;
            }

            return anyIsTrue;
        }

        public List<string> GetSpecialRules()
        {
            List<string> rules = new List<string>();

            if (!IsHero())
            {
                int fullCommand = 0;

                foreach (Option option in Options)
                    if (option.FullCommand && option.Realised)
                        fullCommand += 1;

                if (fullCommand == 3)
                    rules.Add("FC");
                else
                    foreach (Option option in Options)
                        if (option.FullCommand && option.Realised)
                            rules.Add(option.Name);
            }

            if (MountOn > 0)
                rules.Add(Army.Units[MountOn].Name);

            foreach (Option option in Options)
                if (!option.FullCommand && option.Realised && !option.Mount)
                    rules.Add(option.Name);

            Dictionary<string, string> allSpecialRules = new Dictionary<string, string>()
            {
                ["ImmuneToPsychology"] = "Immune to Psychology",
                ["Stubborn"] = "Stubborn",
                ["Hate"] = "Hate",
                ["Fear"] = "Fear",
                ["Terror"] = "Terror",
                ["Frenzy"] = "Frenzy",
                ["Unbreakable"] = "Unbreakable",
                ["ColdBlooded"] = "ColdBlooded",
                ["HitFirst"] = "Hit First",
                ["Regeneration"] = "Regeneration",
                ["KillingBlow"] = "Killing Blow",
                ["PoisonAttack"] = "Poison Attack",
            };

            foreach(KeyValuePair<string, string> specialRule in allSpecialRules)
                if (RuleFromAnyOption(specialRule.Key))
                    rules.Add(specialRule.Value);

            foreach (Option option in Options)
            {
                if (option.SpecialRuleDescription.Length <= 0)
                    continue;

                if (option.IsOption() && !option.Realised)
                    continue;

                foreach (string specialRule in option.SpecialRuleDescription)
                    rules.Add(specialRule);
            }

            return rules;
        }

        public bool IsHero()
        {
            return (Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero ? true : false);
        }

        public bool ExistsOptions()
        {
            foreach (Option option in Options)
                if (option.IsOption() && !option.FullCommand)
                    return true;

            return false;
        }

        public bool ExistsCommand()
        {
            foreach (Option option in Options)
                if (option.FullCommand)
                    return true;

            return false;
        }

        public bool ExistsMagicItems()
        {
            foreach (Option option in Options)
                if (option.IsMagicItem() && (option.Points > 0))
                    return true;

            return false;
        }

        public bool ExistsOrdinaryItems()
        {
            foreach (Option option in Options)
                if (option.IsMagicItem() && (option.Points == 0))
                    return true;

            return false;
        }

        public int GetMountOn()
        {
            if (MountOn > 0)
                return MountOn;

            foreach (Option option in Options)
                if (option.Mount && ((option.IsOption() && option.Realised) || option.IsMagicItem()))
                    return option.ID;

            return 0;
        }

        public int GetMountOption()
        {
            Unit mount = null;
            foreach (KeyValuePair<int, Unit> armyUnit in Army.Units)
                if (armyUnit.Key == MountOn)
                    mount = armyUnit.Value;

            if (mount == null)
                return 0;

            foreach (Option option in Options)
                if (option.Name == mount.Name)
                    return option.ID;

            return 0;
        }

        public void ThrowAwayIncompatibleOption()
        {
            for(int i = 0; i < Options.Count; i++)
            {
                bool incompatible = !IsOptionEnabled(Options[i], GetMountOn(), GetMountTypeAlreadyFixed(), postCheck: true);

                if (incompatible && (Options[i].Realised || Options[i].IsMagicItem()))
                {
                    Interface.SetArtefactAlreadyUsed(Options[i].ID, false);
                    AddOption(Options[i].ID, this, this.ID);
                }
            }
        }

        public bool IsOptionEnabled(Option option, int mountAlreadyOn, Option.OnlyForType mountTypeAlreadyFixed, bool postCheck = false)
        {
            if (!postCheck && option.Mount && (mountAlreadyOn > 0) && (option.ID != mountAlreadyOn))
                return false;

            if (option.Mount && (mountTypeAlreadyFixed == Option.OnlyForType.Infantry))
                return false;

            if ((option.OnlyFor == Option.OnlyForType.Mount) && ((mountAlreadyOn == 0) || (mountTypeAlreadyFixed == Option.OnlyForType.Infantry)))
                return false;

            if ((option.OnlyFor == Option.OnlyForType.Infantry) && ((mountAlreadyOn > 0) || (mountTypeAlreadyFixed == Option.OnlyForType.Mount)))
                return false;

            if (!IsAnotherOptionRealised(option.OnlyIfAnotherService, defaultResult: true) 
                ||
                IsAnotherOptionRealised(option.OnlyIfNotAnotherService, defaultResult: false))
                return false;

            if (option.IsSlannOption() && !option.Realised && IsMaxSlannOption())
                return false;

            return true;
        }

        public Option.OnlyForType GetMountTypeAlreadyFixed()
        {
            foreach (Option option in Options)
                if ((option.IsOption() && option.Realised) || option.IsMagicItem())
                {
                    if (option.OnlyFor == Option.OnlyForType.Mount)
                        return Option.OnlyForType.Mount;

                    if (option.OnlyFor == Option.OnlyForType.Infantry)
                        return Option.OnlyForType.Infantry;
                }

            return Option.OnlyForType.All;
        }

        public bool IsAnotherOptionRealised(string[] optionNames, bool defaultResult)
        {
            if (optionNames.Length <= 0)
                return defaultResult;

            foreach (string optionName in optionNames)
                foreach (Option option in Options)
                    if ((option.Name == optionName) && option.Realised)
                        return true;

            return false;
        }

        public bool IsMaxSlannOption()
        {
            int slannOption = 0;

            foreach (Option option in Options)
                if (option.IsSlannOption() && option.Realised)
                    slannOption += 1;

            return (slannOption >= 4 ? true : false);
        }
    }
}
