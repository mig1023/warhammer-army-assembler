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
    public class Unit : IComparable<Unit>
    {
        public enum UnitType { Lord, Hero, Core, Special, Rare, Mount, ToCore, ToSpecial }
        public enum MagicItemsTypes { Hero, Wizard, Unit }
        public enum TestTypeTypes { Unit, Enemy }

        private static List<string> UnitParam = new List<string> {
            "Movement", "WeaponSkill", "BallisticSkill", "Strength", "Toughness",
            "Wounds", "Initiative", "Attacks", "Leadership", "Armour", "Ward"
        };

        private static Dictionary<string, string> AllSpecialRules = new Dictionary<string, string>()
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
            ["HitLast"] = "Hit Last",
            ["Regeneration"] = "Regeneration",
            ["KillingBlow"] = "Killing Blow",
            ["PoisonAttack"] = "Poison Attack",
            ["MultiWounds"] = "Multiple wounds ([X])",
        };

        public string Name { get; set; }
        public string Group { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }
        public int ArmyID { get; set; }

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

        public int OriginalWounds { get; set; }
        public int OriginalAttacks { get; set; }

        public bool ImmuneToPsychology { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }
        public bool Stupidity { get; set; }

        public int UnitStrength { get; set; }

        public bool HitFirst { get; set; }
        public bool HitLast { get; set; }
        public bool Regeneration { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }
        public string MultiWounds { get; set; } 

        public int SlotsOfLords { get; set; }
        public int SlotsOfHero { get; set; }
        public int SlotsOfSpecial { get; set; }
        public int SlotsOfRare { get; set; }
        public bool NoSlotsOfCore { get; set; }

        public int MagicItems { get; set; }
        public MagicItemsTypes MagicItemsType { get; set; }

        public int MountOn { get; set; }
        public string MountInit { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }

        public bool PersonifiedHero { get; set; }
        public bool ArmyGeneral { get; set; }
        public bool WeaponTeam { get; set; }
        public bool NotALeader { get; set; }
        public bool MustBeGeneral { get; set; }

        public List<Option> Options = new List<Option>();

        public TestTypeTypes TestType { get; set; }
        public Unit EnemyMount { get; set; }

        public ObservableCollection<Unit> Items { get; set; }
        public SolidColorBrush ArmyColor { get; set; }

        public string RulesView { get; set; }

        public Unit()
        {
            this.Items = new ObservableCollection<Unit>();
        }

        public int CompareTo(Unit anotherUnit)
        {
            return (TestFight.CheckInitiative(this, anotherUnit) ? -1 : 1);
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

        public int GetUnitMagicPoints()
        {
            int unitAllMagicPoints = MagicItems;

            foreach (Option option in Options)
                if (option.IsActual())
                    unitAllMagicPoints += option.MagicItems;

            return unitAllMagicPoints;
        }

        public int GetUnitWizard()
        {
            int wizard = Wizard;

            foreach (Option option in Options)
                if (!option.IsOption() || option.IsActual())
                    wizard += option.AddToWizard;

            return wizard;
        }

        public Unit Clone(bool full = false)
        {
            Unit newUnit = new Unit();

            newUnit.Name = this.Name;
            newUnit.Group = this.Group;
            newUnit.ID = this.ID;
            newUnit.IDView = this.IDView;
            newUnit.ArmyID = this.ArmyID;
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

            if (full)
            {
                newUnit.MovementView = this.MovementView;
                newUnit.WeaponSkillView = this.WeaponSkillView;
                newUnit.BallisticSkillView = this.BallisticSkillView;
                newUnit.StrengthView = this.StrengthView;
                newUnit.ToughnessView = this.ToughnessView;
                newUnit.WoundsView = this.WoundsView;
                newUnit.InitiativeView = this.InitiativeView;
                newUnit.AttacksView = this.AttacksView;
                newUnit.LeadershipView = this.LeadershipView;
                newUnit.ArmourView = this.ArmourView;
                newUnit.WardView = this.WardView;
            }

            newUnit.OriginalWounds = this.OriginalWounds;
            newUnit.OriginalAttacks = this.OriginalAttacks;

            newUnit.UnitStrength = this.UnitStrength;

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
            newUnit.HitLast = this.HitLast;
            newUnit.Regeneration = this.Regeneration;
            newUnit.KillingBlow = this.KillingBlow;
            newUnit.PoisonAttack = this.PoisonAttack;
            newUnit.MultiWounds = this.MultiWounds;

            newUnit.SlotsOfLords = this.SlotsOfLords;
            newUnit.SlotsOfHero = this.SlotsOfHero;
            newUnit.SlotsOfSpecial = this.SlotsOfSpecial;
            newUnit.SlotsOfRare = this.SlotsOfRare;
            newUnit.NoSlotsOfCore = this.NoSlotsOfCore;

            newUnit.MagicItems = this.MagicItems;
            newUnit.MagicItemsType = this.MagicItemsType;

            newUnit.SizableType = this.SizableType;
            newUnit.PersonifiedHero = this.PersonifiedHero;
            newUnit.ArmyGeneral = this.ArmyGeneral;
            newUnit.WeaponTeam = this.WeaponTeam;
            newUnit.NotALeader = this.NotALeader;
            newUnit.MustBeGeneral = this.MustBeGeneral;

            newUnit.TestType = this.TestType;
            newUnit.EnemyMount = this.EnemyMount;

            List <Option> Option = new List<Option>();
            foreach (Option option in this.Options)
                newUnit.Options.Add(option.Clone());

            newUnit.ArmyColor = this.ArmyColor;

            return newUnit;
        }

        private bool ContainsCaseless(string line, string subline)
        {
            return line.IndexOf(subline, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool OptionTypeAlreadyUsed(Option option, ref bool alreadyArmour, ref bool alreadyShield)
        {
            if (alreadyArmour && (option.Type == Option.OptionType.Option) && ContainsCaseless(option.Name, "Armour"))
                return true;
            else if ((option.Type == Option.OptionType.Option) && ContainsCaseless(option.Name, "Armour"))
                alreadyArmour = true;
            else if (alreadyShield && (option.Type == Option.OptionType.Option) && ContainsCaseless(option.Name, "Shield"))
                return true;
            else if ((option.Type == Option.OptionType.Option) && ContainsCaseless(option.Name, "Shield"))
                alreadyShield = true;
            else if (alreadyArmour && (option.Type == Option.OptionType.Armour))
                return true;
            else if (option.Type == Option.OptionType.Armour)
                alreadyArmour = true;
            else if (alreadyShield && (option.Type == Option.OptionType.Shield))
                return true;
            else if (option.Type == Option.OptionType.Shield)
                alreadyShield = true;

            return false;
        }

        public string AddFromAnyOption(string name, bool reversParam = false,
            bool mountParam = false, bool doNotCombine = false)
        {
            PropertyInfo unitParam = typeof(Unit).GetProperty(name);
            object paramObject = unitParam.GetValue(this);
            int? paramValue = (int?)paramObject;

            if (paramValue == 16)
                return "D6";
            else if ((paramValue > 10) && ((paramValue % 6)== 0))
                return ((int)(paramValue / 6)).ToString() + "D6";
            else if (paramValue < 0)
                return "-";

            string paramModView = String.Empty;

            List<Option> allOption = new List<Option>(Options);

            if (mountParam && (MountOn > 0))
                allOption.AddRange(Army.Units[MountOn].Options);

            bool alreadyArmour = false;
            bool alreadyShield = false;

            foreach (Option option in allOption)
            {
                if (!option.IsActual())
                    continue;

                if (OptionTypeAlreadyUsed(option, ref alreadyArmour, ref alreadyShield))
                    continue;

                PropertyInfo optionToParam = typeof(Option).GetProperty(String.Format("{0}To", name));
                if (optionToParam != null)
                {
                    object optionToObject = optionToParam.GetValue(option);
                    int optionToValue = (int)optionToObject;

                    if (optionToValue > 0)
                        return optionToValue.ToString() + (reversParam ? "+" : "*");
                }

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
                }
                else if (optionValue > 0)
                {
                    paramModView += '*';
                    paramValue += optionValue;

                    if (paramValue > 10)
                        paramValue = 10;
                }
            }

            if (reversParam && (paramValue != null))
                paramModView += '+';

            return paramValue.ToString() + paramModView;
        }

        private void SetUnitParamByOption(Unit unit, string paramName)
        {
            string stringValue = String.Empty;

            bool value = RuleFromAnyOption(paramName, out stringValue);

            if (!value)
                return;

            if (!String.IsNullOrEmpty(stringValue))
                typeof(Unit).GetProperty(paramName).SetValue(unit, stringValue);
            else
                typeof(Unit).GetProperty(paramName).SetValue(unit, value);
        }

        public Unit GetOptionRules(bool directModification = false)
        {
            Unit unit = this.Clone();

            foreach (string name in UnitParam)
            {
                bool reverse = ((name == "Armour" || name == "Ward") ? true : false);
                bool mount = (name == "Armour" ? true : false);
                bool combine = (name == "Ward" ? true : false);

                string newParamLine = AddFromAnyOption(name, reversParam: reverse, mountParam: mount, doNotCombine: combine);

                typeof(Unit).GetProperty(String.Format("{0}View", name)).SetValue(unit, newParamLine);

                if (directModification && !String.IsNullOrEmpty(newParamLine))
                {
                    string cleanParamLine = newParamLine.Replace("+", String.Empty).Replace("*", String.Empty);
                    typeof(Unit).GetProperty(name).SetValue(unit, int.Parse(cleanParamLine));
                }
            }

            if (directModification)
                foreach (KeyValuePair<string, string> specialRule in AllSpecialRules)
                    SetUnitParamByOption(unit, specialRule.Key);

            return unit;
        }

        public Unit GetUnitMultiplier()
        {
            Unit unit = this.Clone(full: true);

            unit.OriginalAttacks = unit.Attacks;
            unit.OriginalWounds = unit.Wounds;

            Dictionary<int, int> ratio = new Dictionary<int, int>
            {
                [4] = 3, [7] = 4, [10] = 5, [24] = 6, [28] = 7, [32] = 8,
            };

            int frontSize = unit.Size;

            foreach(KeyValuePair<int, int> r in ratio)
                if (unit.Size >= r.Key)
                    frontSize = r.Value;

            unit.Attacks *= frontSize;

            unit.Wounds *= unit.Size;

            return unit;
        }

        public void AddAmmunition(int id)
        {
            Options.Insert(0, ArmyBook.Artefact[id].Clone());
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

                            if (!ArmyChecks.IsArmyUnitsPointsPercentOk(Army.Units[unitID].Type, option.Points))
                            {
                                Interface.Error(String.Format("The {0} has reached a point cost limit", Army.Units[unitID].UnitTypeName()));
                                return;
                            }
                            else if (!InterfaceChecks.EnoughUnitPointsForAddOption(optionPoints))
                            {
                                Interface.Error(String.Format("Not enough points to add", Army.Units[unitID].UnitTypeName()));
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
                        ArmyMod.DeleteUnitByID(Army.Units[unitID].MountOn);
                        unit.MountOn = 0;
                    }

                    option.Realised = realise;

                    return;
                }
            }
        }

        public string GetSpecialRulesLine(bool withCommandData = false)
        {
            string rules = (withCommandData ? GetFullCommandLine() : String.Empty);

            foreach (string rule in GetSpecialRules())
                rules += String.Format("{0}; ", rule);

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }

        public string GetEquipmentLine()
        {
            string equipment = GetFullCommandLine();

            foreach (Option option in Options)
            {
                bool thisIsRealised = (option.Realised || option.IsMagicItem()) && option.Points != 0;
                bool thisIsNotMountOrFC = !(option.Mount || option.FullCommand);

                if (!String.IsNullOrEmpty(option.Name) && thisIsRealised && thisIsNotMountOrFC)
                    equipment += String.Format("{0}; ", option.Name);
            }

            if (!String.IsNullOrEmpty(equipment))
                equipment = equipment.Remove(equipment.Length - 2);

            return equipment;
        }

        private string GetModifiedParam(string shortName, string param)
        {
            if (!param.Contains("*") && !param.Contains("+"))
                return String.Empty;

            string cleanParam = param.Replace("*", String.Empty);

            return String.Format("{0}={1}, ", shortName, cleanParam);
        }

        public string GetModifiedParamsLine()
        {
            Unit unit = GetOptionRules();

            string paramLine = String.Empty;

            Dictionary<string, string> unitParams = new Dictionary<string, string>
            {
                ["M"] = unit.MovementView,
                ["WS"] = unit.WeaponSkillView,
                ["BS"] = unit.BallisticSkillView,
                ["S"] = unit.StrengthView,
                ["T"] = unit.ToughnessView,
                ["W"] = unit.WoundsView,
                ["I"] = unit.InitiativeView,
                ["A"] = unit.AttacksView,
                ["LD"] = unit.LeadershipView,
                ["AS"] = unit.ArmourView,
                ["Ward"] = unit.WardView
            };

            foreach(KeyValuePair<string, string> paramPair in unitParams)
                paramLine += GetModifiedParam(paramPair.Key, paramPair.Value);

            if (!String.IsNullOrEmpty(paramLine))
                paramLine = paramLine.Remove(paramLine.Length - 2);

            return paramLine;
        }

        public string SelfDescription()
        {
            string describe = String.Format("\nUnit type: {0}", Type);

            if (PersonifiedHero)
                describe += " (personified)";

            if (!IsHeroOrHisMount())
            {
                string minAndMax = String.Format("\nUnit size: {0} - {1}", MinSize, MaxSize);
                string plus = (MinSize == MaxSize ? String.Empty : "+");
                string minOnly = String.Format("\nUnit size: {0}{1}", MinSize, plus);

                describe += ((MaxSize > 0) && (MinSize != MaxSize) ? minAndMax : minOnly);
            }

            if (Wizard > 0)
                describe += String.Format("\nWizard: {0}", Wizard);


            if (!String.IsNullOrEmpty(Group))
                describe += String.Format("\nGroup: {0}", Group);

            return describe;
        }

        public string UnitTypeName()
        {
            if (Type == Unit.UnitType.Lord)
                return "lords";
            else if (Type == Unit.UnitType.Hero)
                return "heroes";
            else if (Type == Unit.UnitType.Core)
                return "core units";
            else if (Type == Unit.UnitType.Special)
                return "special units";
            else if (Type == Unit.UnitType.Rare)
                return "rare units";

            return String.Empty;
        }

        private bool GetUnitValueTrueOrFalse(object unitValue, out string additionalParam)
        {
            additionalParam = String.Empty;

            if (unitValue is bool)
                return ((bool)unitValue ? true : false);
            else if (unitValue is string)
            {
                additionalParam = (string)unitValue;
                return (String.IsNullOrEmpty((string)unitValue) ? false : true);
            }
            else
                return false;
        }

        public bool RuleFromAnyOption(string name, out string additionalParam)
        {
            string lineParamValue = String.Empty;

            PropertyInfo unitField = typeof(Unit).GetProperty(name);
            bool anyIsTrue = GetUnitValueTrueOrFalse(unitField.GetValue(this), out lineParamValue);

            foreach (Option option in Options)
            {
                if (option.IsOption() && !option.Realised)
                    continue;

                string lineOptionValue = String.Empty;

                PropertyInfo optionField = typeof(Option).GetProperty(name);
                bool fromParamValue = GetUnitValueTrueOrFalse(optionField.GetValue(option), out lineOptionValue);
                anyIsTrue = (fromParamValue ? true : anyIsTrue);

                if (fromParamValue && String.IsNullOrEmpty(lineParamValue))
                    lineParamValue = lineOptionValue;
            }

            additionalParam = lineParamValue;

            return anyIsTrue;
        }

        private string GetFullCommandLine()
        {
            List<string> rules = new List<string>();

            int fullCommand = 0;
            string personifiedCommander = String.Empty;

            foreach (Option option in Options)
            {
                if (option.FullCommand && option.Realised)
                    fullCommand += 1;

                if (option.PersonifiedCommander && option.Realised)
                    personifiedCommander = option.Name;
            }

            if (fullCommand == 3)
            {
                rules.Add("FC");

                if (!String.IsNullOrEmpty(personifiedCommander))
                    rules.Add(personifiedCommander);
            }
            else
                foreach (Option option in Options)
                    if (option.FullCommand && option.Realised)
                        rules.Add(option.Name);

            string rulesLine = String.Empty;

            foreach (string rule in rules)
                rulesLine += String.Format("{0}; ", rule);

            return rulesLine;
        }

        public List<string> GetSpecialRules()
        {
            List<string> rules = new List<string>();

            string tmp = this.Name;

            if (ArmyGeneral)
                rules.Add("General");

            if (MountOn > 0)
                rules.Add(Army.Units[MountOn].Name);

            foreach (Option option in Options)
                if (option.Realised && option.SpecialRuleDescription.Length > 0)
                    rules.Add(option.Name);

            string additionalParam = String.Empty;

            foreach(KeyValuePair<string, string> specialRule in AllSpecialRules) 
                if (RuleFromAnyOption(specialRule.Key, out additionalParam))
                    rules.Add(specialRule.Value.Replace("[X]", additionalParam));

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

        public bool IsHeroOrHisMount()
        {
            return (Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero || Type == Unit.UnitType.Mount ? true : false);
        }

        public bool IsUnit()
        {
            bool core = (this.Type == Unit.UnitType.Core);
            bool special = (this.Type == Unit.UnitType.Special);
            bool rare = (this.Type == Unit.UnitType.Rare);

            return (core || special || rare ? true : false);
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
                if (option.IsMagicItem() && !String.IsNullOrEmpty(option.Name) && (option.Points == 0))
                    return true;

            return false;
        }

        public int GetMountOn()
        {
            if (MountOn > 0)
                return MountOn;

            foreach (Option option in Options)
                if (option.Mount && (option.IsActual()))
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

                bool notCompitableMore = IsNotCompitableMore(Options[i]);

                if ((incompatible || notCompitableMore) && (Options[i].IsActual()))
                {
                    InterfaceMod.SetArtefactAlreadyUsed(Options[i].ID, false);
                    AddOption(Options[i].ID, this, this.ArmyID);
                }
            }
        }

        public bool IsNotCompitableMore(Option option)
        {
            return !InterfaceChecks.EnoughUnitPointsForAddArtefact(option.ID, this, addOption: false);
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
                if (option.IsActual())
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

        public Unit SetTestType(TestTypeTypes testType)
        {
            TestType = testType;

            return this;
        } 
    }
}
