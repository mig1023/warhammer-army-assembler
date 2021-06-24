using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Unit : IComparable<Unit>
    {
        public enum UnitType { Lord, Hero, Core, Special, Rare, Mount, ToCore, ToSpecial }
        public enum MagicItemsTypes { Hero, Wizard, Unit }
        public enum TestTypeTypes { Unit, Enemy }

        public string Name { get; set; }
        string Group { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }
        public int ArmyID { get; set; }

        public UnitType Type { get; set; }
        public bool SizableType { get; set; }
        public Visibility VisibleType { get; set; }

        public int Size { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public int ModelsInPack { get; set; }
        public bool UniqueUnits { get; set; }

        public double Points { get; set; }

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
        public bool ImmuneToPoison { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool BloodFrenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }
        public bool Stupidity { get; set; }
        public bool Undead { get; set; }
        public bool StrengthInNumbers { get; set; }
        public string AddToCloseCombat { get; set; }

        public bool LargeBase { get; set; }
        public bool NoKillingBlow { get; set; }
        public bool NoMultiWounds { get; set; }

        public bool AutoHit { get; set; }
        public bool AutoWound { get; set; }
        public bool AutoDeath { get; set; }
        public bool HitFirst { get; set; }
        public bool HitLast { get; set; }
        public bool Regeneration { get; set; }
        public bool KillingBlow { get; set; }
        public int ExtendedKillingBlow { get; set; }
        public bool HeroicKillingBlow { get; set; }
        public bool PoisonAttack { get; set; }
        public string MultiWounds { get; set; }
        public bool NoArmour { get; set; }
        public bool NoWard { get; set; }
        public int ArmourPiercing { get; set; }
        public string Reroll { get; set; }
        public string ImpactHit { get; set; }
        public bool SteamTank { get; set; }
        public bool HellPitAbomination { get; set; }
        public bool Giant { get; set; }
        public bool Lance { get; set; }
        public bool Flail { get; set; }
        public int ChargeStrengthBonus { get; set; }
        public bool Resolute { get; set; }
        public bool PredatoryFighter { get; set; }
        public bool MurderousProwess { get; set; }
        public bool Bloodroar { get; set; }
        public int AddToHit { get; set; }
        public int SubOpponentToHit { get; set; }
        public int AddToWound { get; set; }
        public int SubOpponentToWound { get; set; }
        public int HitOn { get; set; }
        public int OpponentHitOn { get; set; }
        public int WoundOn { get; set; }
        public int WardForFirstWound { get; set; }
        public int WardForLastWound { get; set; }
        public bool FirstWoundDiscount { get; set; }

        public List<Test.Param> ParamTests { get; set; }

        public List<string> SlotOf { get; set; }
        public bool NoCoreSlot { get; set; }

        public int MagicItems { get; set; }
        public int MagicItemCount { get; set; }
        public MagicItemsTypes MagicItemsType { get; set; }
        public int MagicPowers { get; set; }
        public int MagicPowersCount { get; set; }

        public int MountOn { get; set; }
        public string MountInit { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }

        public bool PersonifiedHero { get; set; }
        public bool ArmyGeneral { get; set; }
        public bool WeaponTeam { get; set; }
        public bool NotALeader { get; set; }
        public bool MustBeGeneral { get; set; }
        public bool Chariot { get; set; }

        public List<Option> Options = new List<Option>();

        public TestTypeTypes TestType { get; set; }
        public Unit Mount { get; set; }
        public bool WoundedWithKillingBlow { get; set; }
        public bool PassThisRound { get; set; }
        public string TestListName { get; set; }

        public ObservableCollection<Unit> Items { get; set; }
        public SolidColorBrush ArmyColor { get; set; }
        public SolidColorBrush ArmyBackgroundColor { get; set; }

        public string RulesView { get; set; }

        public Unit() => this.Items = new ObservableCollection<Unit>();

        public int CompareTo(Unit anotherUnit) => (Test.Fight.CheckInitiative(this, anotherUnit) ? -1 : 1);

        public double GetUnitPoints()
        {
            double points = Size * Points;

            foreach (Option option in Options)
            {
                if (option.Countable != null)
                    points += option.Points * option.Countable.Value;

                else if (!option.IsOption() || (option.IsOption() && option.Realised && !option.IsSlannOption()))
                    points += option.Points * (option.PerModel ? Size : 1);
            }

            bool firstSlannOptionAlreadyIsFree = false;

            foreach (Option option in Options.Where(x => x.IsSlannOption() && x.Realised))
            {
                if (firstSlannOptionAlreadyIsFree)
                    points += option.Points;
                else
                    firstSlannOptionAlreadyIsFree = true;
            }

            return points;
        }

        public string GetGroup()
        {
            Option option = Options.Where(x => (!x.IsOption() || (x.IsOption() && x.Realised)) && !String.IsNullOrEmpty(x.Group)).FirstOrDefault();
            return (option == null ? String.Empty : option.Group);
        }

        public void SetGroup(string newGroup) => this.Group = newGroup;

        public int GetUnitMagicPoints()
        {
            int unitAllMagicPoints = MagicItems;

            if (MagicItemCount > 0)
                return MagicItemCount;

            foreach (Option option in Options.Where(x => x.IsActual()))
                unitAllMagicPoints += option.MagicItems;

            return unitAllMagicPoints;
        }

        public double MagicPointsAlreadyUsed()
        {
            double alreayUsed = 0;

            foreach (Option option in Options)
            {
                if (option.IsMagicItem() && (MagicItemCount <= 0))
                    alreayUsed += option.Points;

                else if (option.IsMagicItem() && (MagicItemCount > 0) && (option.Points > 0))
                    alreayUsed += 1;
            }

            return alreayUsed;
        }

        public double GetUnitMagicPowersPoints() => MagicPowers;

        public double MagicPowersPointsAlreadyUsed() => Options.Where(x => x.IsPowers()).Sum(x => x.Points);

        public double GetMagicPowersCount() => MagicPowersCount;

        public double MagicPowersCountAlreadyUsed() => Options.Where(x => x.IsPowers()).Count();

        public int GetUnitWizard()
        {
            int wizard = Wizard;

            foreach (Option option in Options)
            {
                if (!option.IsOption() || option.IsActual())
                    wizard += option.AddToWizard;

                if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                    wizard += option.Countable.Value;
            }

            return wizard;
        }

        public Unit Clone(bool full = false)
        {
            Unit newUnit = new Unit
            {
                Name = this.Name,
                Group = this.Group,
                ID = this.ID,
                IDView = this.IDView,
                ArmyID = this.ArmyID,
                Type = this.Type,
                Size = this.Size,
                MinSize = this.MinSize,
                MaxSize = this.MaxSize,
                ModelsInPack = this.ModelsInPack,
                UniqueUnits = this.UniqueUnits,
                Points = this.Points,
                MountOn = this.MountOn,
                MountInit = this.MountInit,
                Description = this.Description,

                Movement = this.Movement,
                WeaponSkill = this.WeaponSkill,
                BallisticSkill = this.BallisticSkill,
                Strength = this.Strength,
                Toughness = this.Toughness,
                Wounds = this.Wounds,
                Initiative = this.Initiative,
                Attacks = this.Attacks,
                Leadership = this.Leadership,
                Armour = this.Armour,
                Ward = this.Ward,
                Wizard = this.Wizard,

                OriginalWounds = this.OriginalWounds,
                OriginalAttacks = this.OriginalAttacks,

                NoKillingBlow = this.NoKillingBlow,
                NoMultiWounds = this.NoMultiWounds,
                LargeBase = this.LargeBase,

                ImmuneToPsychology = this.ImmuneToPsychology,
                ImmuneToPoison = this.ImmuneToPoison,
                Stubborn = this.Stubborn,
                Hate = this.Hate,
                Fear = this.Fear,
                Terror = this.Terror,
                Frenzy = this.Frenzy,
                BloodFrenzy = this.BloodFrenzy,
                Unbreakable = this.Unbreakable,
                ColdBlooded = this.ColdBlooded,
                Stupidity = this.Stupidity,
                Undead = this.Undead,
                StrengthInNumbers = this.StrengthInNumbers,
                AutoHit = this.AutoHit,
                AutoWound = this.AutoWound,
                AutoDeath = this.AutoDeath,
                HitFirst = this.HitFirst,
                HitLast = this.HitLast,
                Regeneration = this.Regeneration,
                KillingBlow = this.KillingBlow,
                ExtendedKillingBlow = this.ExtendedKillingBlow,
                HeroicKillingBlow = this.HeroicKillingBlow,
                PoisonAttack = this.PoisonAttack,
                MultiWounds = this.MultiWounds,
                NoArmour = this.NoArmour,
                NoWard = this.NoWard,
                ArmourPiercing = this.ArmourPiercing,
                Reroll = this.Reroll,
                ImpactHit = this.ImpactHit,
                SteamTank = this.SteamTank,
                HellPitAbomination = this.HellPitAbomination,
                Giant = this.Giant,
                Lance = this.Lance,
                Flail = this.Flail,
                ChargeStrengthBonus = this.ChargeStrengthBonus,
                Resolute = this.Resolute,
                PredatoryFighter = this.PredatoryFighter,
                MurderousProwess = this.MurderousProwess,
                AddToCloseCombat = this.AddToCloseCombat,
                Bloodroar = this.Bloodroar,
                AddToHit = this.AddToHit,
                SubOpponentToHit = this.SubOpponentToHit,
                AddToWound = this.AddToWound,
                SubOpponentToWound = this.SubOpponentToWound,
                HitOn = this.HitOn,
                OpponentHitOn = this.OpponentHitOn,
                WoundOn = this.WoundOn,
                WardForFirstWound = this.WardForFirstWound,
                WardForLastWound = this.WardForLastWound,
                FirstWoundDiscount = this.FirstWoundDiscount,

                ParamTests = Test.Param.Clone(this.ParamTests),

                NoCoreSlot = this.NoCoreSlot,

                MagicItems = this.MagicItems,
                MagicItemCount = this.MagicItemCount,
                MagicItemsType = this.MagicItemsType,
                MagicPowers = this.MagicPowers,
                MagicPowersCount = this.MagicPowersCount,

                SizableType = this.SizableType,
                VisibleType = this.VisibleType,
                PersonifiedHero = this.PersonifiedHero,
                ArmyGeneral = this.ArmyGeneral,
                WeaponTeam = this.WeaponTeam,
                NotALeader = this.NotALeader,
                MustBeGeneral = this.MustBeGeneral,
                Chariot = this.Chariot,

                TestType = this.TestType,
                WoundedWithKillingBlow = false,
                PassThisRound = false,
                TestListName = this.TestListName,

                ArmyColor = this.ArmyColor,
                ArmyBackgroundColor = this.ArmyBackgroundColor,
            };

            if (this.SlotOf != null)
                newUnit.SlotOf = new List<string>(this.SlotOf);

            if (this.Mount != null)
                newUnit.Mount = this.Mount.Clone();

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

            foreach (Option option in this.Options)
                newUnit.Options.Add(option.Clone());

            return newUnit;
        }

        private bool ContainsCaseless(string line, string subline) => line.IndexOf(subline, StringComparison.OrdinalIgnoreCase) >= 0;

        private bool OptionTypeAlreadyUsed(Option option, ref bool alreadyArmour, ref bool alreadyShield)
        {
            if (option.NativeArmour && IsArmourOptionAdded())
                return true;

            else if (alreadyArmour && (option.Type == Option.OptionType.Option) && ContainsCaseless(option.Name, "Armour"))
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

        private void GetParamTestsFromOptions()
        {
            foreach (Option option in this.Options.Where(x => x.ParamTests.Count > 0))
                this.ParamTests.AddRange(option.ParamTests);
        }

        public static string GetRandomAttacksLine(int? attack)
        {
            string param = attack.ToString();
            string addToDice = (param[2] == '0' ? String.Empty : "+" + param[2].ToString());

            return String.Format("{0}D{1}{2}", param[0], param[1], addToDice);
        }

        private string AddFromAnyOption(string name, bool reversParam = false,
            bool mountParam = false, bool doNotCombine = false)
        {
            PropertyInfo unitParam = typeof(Unit).GetProperty(name);
            object paramObject = unitParam.GetValue(this);
            int? paramValue = (int?)paramObject;

            if (paramValue > 100)
                return GetRandomAttacksLine(paramValue);

            else if (paramValue <= 0)
                return "-";

            string paramModView = String.Empty;

            List<Option> allOption = new List<Option>(Options);

            if (mountParam && (MountOn > 0))
                allOption.AddRange(Army.Data.Units[MountOn].Options);

            bool alreadyArmour = false, alreadyShield = false;

            foreach (Option option in allOption.Where(x => x.IsActual()))
            {
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

                if (optionValue != 0 && reversParam)
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
                else if (optionValue != 0)
                {
                    paramModView += '*';
                    paramValue += optionValue;
                    paramValue = ParamNormalization(paramValue ?? 0);
                }
            }

            if (reversParam && (paramValue != null))
                paramModView += '+';

            return paramValue.ToString() + paramModView;
        }

        private void SetUnitParamByOption(string paramName, bool directModification = false)
        {
            bool value = RuleFromAnyOption(paramName, out string stringValue, out int intValue, directModification: directModification);

            if (!value)
                return;

            if (intValue > 0)
                typeof(Unit).GetProperty(paramName).SetValue(this, intValue);

            else if (!String.IsNullOrEmpty(stringValue))
                typeof(Unit).GetProperty(paramName).SetValue(this, stringValue);

            else
                typeof(Unit).GetProperty(paramName).SetValue(this, value);
        }

        public Unit GetOptionRules(bool directModification = false)
        {
            Unit unit = this.Clone();

            foreach (string name in SpecialRules.UnitParam)
            {
                bool reverse = (name == "Armour" || name == "Ward");
                bool mount = (name == "Armour");
                bool combine = (name == "Ward");

                string newParamLine = AddFromAnyOption(name, reversParam: reverse, mountParam: mount, doNotCombine: combine);

                typeof(Unit).GetProperty(String.Format("{0}View", name)).SetValue(unit, newParamLine);

                if (directModification && !String.IsNullOrEmpty(newParamLine))
                {
                    string cleanParamLine = newParamLine.Replace("+", String.Empty).Replace("*", String.Empty);

                    if (cleanParamLine.Contains("-") && (name != "Armour"))
                        cleanParamLine = "0";

                    if (!cleanParamLine.Contains("D"))
                        typeof(Unit).GetProperty(name).SetValue(unit, int.Parse(cleanParamLine));
                }
            }

            if (directModification)
            {
                foreach (string specialRule in SpecialRules.All.Keys)
                    unit.SetUnitParamByOption(specialRule, directModification);

                unit.GetParamTestsFromOptions();
            }

            return unit;
        }

        public Unit GetUnitMultiplier(int? baseSize = null)
        {
            Unit unit = this.Clone(full: true);

            if (unit.Chariot)
                unit.Size = 2;

            unit.Size = baseSize ?? unit.Size;
            unit.OriginalWounds = unit.Wounds;
            unit.Wounds *= unit.Size;

            return unit;
        }

        public int GetFront()
        {
            int frontSize = this.Size;

            Dictionary<int, int> ratio = new Dictionary<int, int>
            {
                [5] = 3,
                [7] = 4,
                [9] = 5,
                [24] = 6,
                [28] = 7,
                [32] = 8,
            };

            foreach (KeyValuePair<int, int> r in ratio)
                if (this.Size >= r.Key)
                    frontSize = r.Value;

            return frontSize;
        }

        public int GetRank()
        {
            if (!this.IsUnit())
                return 1;

            int front = this.GetFront();

            if (front < 5)
                return 1;

            int ranks = (this.Wounds / front) + (this.Wounds % front >= 5 ? 1 : 0);

            if (ranks > 3)
                return 3;

            return ranks;
        }

        public Option GetCurrentRunicItemByName(string name) => Options.Where(x => x.Name == name).FirstOrDefault();

        public int GetCurrentRunicItemsByCount()
        {
            int count = 0;

            foreach (Option option in Options)
            {
                if (option.MasterRunic)
                    count += 1;

                else if (option.Runic > 0)
                    count += option.Runic;
            }

            return count;
        }

        public void AddAmmunition(int id)
        {
            Options.Insert(0, ArmyBook.Data.Artefact[id].Clone());

            if (ArmyBook.Data.Artefact[id].Virtue)
            {
                ArmyBook.Data.Artefact[id].Points = Army.Params.GetVirtuePoints(id);
                Interface.Reload.LoadArmyList(fastReload: true);
            }
        }

        public void ChangeCountableOption(int optionID, string direction)
        {
            for (int i = 0; i < this.Options.Count; i++)
            {
                Option option = this.Options[i];

                if (option.ID == optionID)
                {
                    if (direction == "-")
                        option.Countable.Value -= 1;
                    else
                    {
                        if (!Army.Checks.IsArmyUnitsPointsPercentOk(this.Type, option.Points))
                            Interface.Changes.Error(String.Format("The {0} has reached a point cost limit", this.UnitTypeName()));

                        else if (!Interface.Checks.EnoughUnitPointsForAddOption(option.Points))
                            Interface.Changes.Error(String.Format("Not enough points to add", this.UnitTypeName()));

                        else
                            option.Countable.Value += 1;
                    }

                    return;
                }
            }
        }

        public void AddOption(int optionID)
        {
            for(int i = 0; i < this.Options.Count; i++)
            {
                Option option = this.Options[i];

                if (option.ID == optionID)
                {
                    bool realise = false;

                    if (option.IsMagicItem() || option.IsPowers())
                    {
                        this.Options.Remove(option);

                        Option artefact = ArmyBook.Data.Artefact[option.ID];

                        if ((artefact.TypeUnitIncrese) && (this.Type == Unit.UnitType.Special))
                            this.Type = Unit.UnitType.Core;

                        else if ((artefact.TypeUnitIncrese) && (this.Type == Unit.UnitType.Rare))
                            this.Type = Unit.UnitType.Special;

                        if (option.Virtue)
                        {
                            ArmyBook.Data.Artefact[option.ID].Points = Army.Params.GetVirtuePoints(option.ID);
                            Interface.Reload.LoadArmyList(fastReload: true);
                        }
                    }
                    else
                    {
                        if (option.Realised)
                            option.Realised = false;
                        else
                        {
                            double optionPoints = (option.PerModel ? option.Points * this.Size : option.Points);

                            if (!Army.Checks.IsArmyUnitsPointsPercentOk(this.Type, option.Points))
                            {
                                Interface.Changes.Error(String.Format("The {0} has reached a point cost limit", this.UnitTypeName()));
                                return;
                            }
                            else if (!Interface.Checks.EnoughUnitPointsForAddOption(optionPoints))
                            {
                                Interface.Changes.Error(String.Format("Not enough points to add", this.UnitTypeName()));
                                return;
                            }
                            else
                                realise = true;
                        }
                    }

                    if (option.Mount && realise)
                    {
                        foreach (KeyValuePair<int, Unit> mount in ArmyBook.Data.Mounts.Where(x => x.Value.Name == option.Name))
                            Interface.Changes.ArmyGridDrop(mount.Key, points: option.Points, unit: ArmyID);
                    }
                    else if (option.Mount && !realise)
                    {
                        Army.Mod.DeleteUnitByID(this.MountOn);
                        this.MountOn = 0;
                    }

                    option.Realised = realise;
                    return;
                }
            }
        }

        public string GetSpecialRulesLine(bool withCommandData = false, bool onlyUnitParam = false, bool noNeedToDoubleBSB = false)
        {
            string rules = (withCommandData ? GetFullCommandLine() : String.Empty);

            foreach (string rule in GetSpecialRules(onlyUnitParam))
                if (!(noNeedToDoubleBSB && (rule == "Battle Standard Bearer")))
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
                    equipment += String.Format("{0}; ", option.FullName());
            }

            foreach (Option option in Options.Where(x => (x.Countable != null) && (x.Countable.Value > 0)))
                equipment += String.Format("{0} {1}; ", option.Countable.Value, option.Name);

            foreach (Option option in Options.Where(x => !String.IsNullOrEmpty(x.Name) && x.IsPowers()))
                equipment += String.Format("{0}; ", option.Name);

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

                if (UniqueUnits)
                    describe += "\nOnly one unit of this type can be in the army";
            }

            int wizard = GetUnitWizard();

            if (wizard > 0)
                describe += String.Format("\nWizard: {0}", wizard);

            if (!String.IsNullOrEmpty(Group))
                describe += String.Format("\nGroup: {0}", Group);

            return describe;
        }

        public string UnitTypeName()
        {
            Dictionary<UnitType, string> typesNames = new Dictionary<UnitType, string>
            {
                [Unit.UnitType.Lord] = "lords",
                [Unit.UnitType.Hero] = "heroes",
                [Unit.UnitType.Core] = "core units",
                [Unit.UnitType.Special] = "special units",
                [Unit.UnitType.Rare] = "rare units",
            };

            return (typesNames.ContainsKey(Type) ? typesNames[Type] : String.Empty);
        }

        public static int ParamNormalization(int param, bool onlyZeroCheck = false)
        {
            if (param < 0)
                return 0;
            
            if ((param > 10) && !onlyZeroCheck)
                return 10;

            return param;
        }

        private bool GetUnitValueTrueOrFalse(object unitValue, out string additionalParam, out int intValue)
        {
            additionalParam = String.Empty;
            intValue = 0;

            if (unitValue is bool)
                return (bool)unitValue;

            else if (unitValue is string)
            {
                additionalParam = unitValue.ToString();
                return !String.IsNullOrEmpty(additionalParam);
            }
            else if (unitValue is int)
            {
                intValue = (int)unitValue;
                return intValue > 0;
            }
            else
                return false;
        }

        public bool RuleFromAnyOption(string name, out string additionalParam, out int intValue,
            bool onlyUnitParam = false, bool directModification = false)
        {
            PropertyInfo unitField = typeof(Unit).GetProperty(name);
            bool anyIsTrue = GetUnitValueTrueOrFalse(unitField.GetValue(this), out string lineParamValue, out int intParamValue);

            if (!onlyUnitParam)
                foreach (Option option in Options.Where(x => (!x.IsOption() || x.Realised)))
                {
                    PropertyInfo optionField = typeof(Option).GetProperty(name);

                    bool fromParamValue = GetUnitValueTrueOrFalse(
                        optionField.GetValue(option), out string lineOptionValue, out int intOptionValue
                    );

                    anyIsTrue = (fromParamValue ? true : anyIsTrue);

                    if ((name == "Reroll") && !directModification && fromParamValue && !String.IsNullOrEmpty(lineOptionValue))
                    {
                        string[] allRerolls = lineOptionValue.Split(';');

                        foreach(string reroll in allRerolls)
                        {
                            string secondElement = (String.IsNullOrEmpty(lineParamValue) ? String.Empty : "; ");
                            string[] rerollsParams = reroll.Split('(');
                            lineParamValue += String.Format("{0}{1}", secondElement, SpecialRules.RerollsLines[rerollsParams[0]]);

                            if (rerollsParams.Length > 1)
                                lineParamValue += "(" + rerollsParams[1];
                        }
                    }
                    else if (fromParamValue && !String.IsNullOrEmpty(lineOptionValue))
                        lineParamValue = lineOptionValue;

                    if (fromParamValue && (intOptionValue > 0))
                        intParamValue = intOptionValue;
                }

            additionalParam = lineParamValue;
            intValue = intParamValue;

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
                foreach (Option option in Options.Where(x => (x.FullCommand && x.Realised)))
                    rules.Add(option.Name);

            string rulesLine = String.Empty;

            foreach (string rule in rules)
                rulesLine += String.Format("{0}; ", rule);

            return rulesLine;
        }

        public List<string> GetSpecialRules(bool onlyUnitParam = false)
        {
            List<string> rules = new List<string>();

            if (ArmyGeneral)
                rules.Add("General");

            if (MountOn > 0)
                rules.Add(Army.Data.Units[MountOn].Name);

            foreach (Option option in Options.Where(x => (x.Realised && x.SpecialRuleDescription.Length > 0)))
                rules.Add(option.Name);

            foreach (KeyValuePair<string, string> specialRule in SpecialRules.All) 
                if (RuleFromAnyOption(specialRule.Key, out string additionalParam, out int intParam, onlyUnitParam: onlyUnitParam))
                    rules.Add(specialRule.Value.Replace("[X]", (intParam > 0 ? intParam.ToString() : additionalParam)));

            Test.Param.Describe(ParamTests, ref rules);

            if (!onlyUnitParam)
                foreach (Option option in Options)
                    Test.Param.Describe(option.ParamTests, ref rules);

            foreach (Option option in Options.Where(x => (x.SpecialRuleDescription.Length > 0) && (!x.IsOption() || x.Realised)))
                foreach (string specialRule in option.SpecialRuleDescription)
                    rules.Add(specialRule);

            return rules;
        }

        public bool IsHero() => (Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero);

        public bool IsHeroOrHisMount() => (LargeBase || Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero || Type == Unit.UnitType.Mount);

        public bool IsUnit() => (this.Type == Unit.UnitType.Core || this.Type == Unit.UnitType.Special || this.Type == Unit.UnitType.Rare);

        public bool ExistsOptions() => Options.Where(x => x.IsOption() && !x.FullCommand).FirstOrDefault() != null;

        public bool ExistsCommand() => Options.Where(x => x.FullCommand).FirstOrDefault() != null;

        public bool ExistsMagicItems() => Options.Where(x => x.IsMagicItem() && ((x.Points > 0) || x.Honours)).FirstOrDefault() != null;

        public bool ExistsMagicPowers() => Options.Where(x => x.Type == Option.OptionType.Powers).FirstOrDefault() != null; 

        public bool ExistsRunicCombinationInUnit(Dictionary<string, int> runicItems)
        {
            foreach (KeyValuePair<string, int> item in runicItems)
            {
                bool exist = false;

                foreach (Option option in Options)
                    if ((item.Key == option.Name) && (item.Value == option.Runic))
                    {
                        exist = true;
                        break;
                    }

                if (!exist)
                    return false;
            }

            return true;
        }

        public bool ExistsOrdinaryItems() =>
            Options.Where(x => x.IsMagicItem() && !String.IsNullOrEmpty(x.Name) && (x.Points == 0)).FirstOrDefault() != null;

        public int GetMountOn()
        {
            if (MountOn > 0)
                return MountOn;

            Option option = Options.Where(x => x.Mount && x.IsActual()).FirstOrDefault();

            return (option == null ? 0 : option.ID);
        }

        public int GetMountOption()
        {
            if (!Army.Data.Units.ContainsKey(MountOn))
                return 0;

            Option option = Options.Where(x => x.Name == Army.Data.Units[MountOn].Name).FirstOrDefault();

            return (option == null ? 0 : option.ID);
        }

        public void ThrowAwayIncompatibleOption()
        {
            for(int i = 0; i < Options.Count; i++)
            {
                bool incompatible = !IsOptionEnabled(Options[i], GetMountOn(), GetMountTypeAlreadyFixed(), postCheck: true);
                bool notCompitableMore = IsNotCompitableMore(Options[i]);
                bool isCountable = (Options[i].Countable != null);

                if ((incompatible || notCompitableMore) && (Options[i].IsActual() || isCountable))
                {
                    Interface.Mod.SetArtefactAlreadyUsed(Options[i].ID, false);

                    if (isCountable)
                        Options[i].Countable.Value = 0;

                    AddOption(Options[i].ID);
                }
            }
        }

        public bool IsNotCompitableMore(Option option)
        {
            if ((option.MasterRunic || (option.Runic > 0)) && Army.Checks.IsRunicCombinationAlreadyExist(this, null))
                return true;

            return !Interface.Checks.EnoughUnitPointsForAddArtefact(option.ID, this, addOption: false);
        }

        public bool IsArmourOptionAdded()
        {
            foreach (Option option in Options)
            {
                if ((option.Type == Option.OptionType.Armour) && !option.NativeArmour)
                    return true;

                bool optionAndNotNative = (option.Type == Option.OptionType.Option) && !option.NativeArmour;

                if (optionAndNotNative && ContainsCaseless(option.Name, "Armour") && option.Realised)
                    return true;
            }

            return false;
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

            if (IsAnotherOptionIsIncompatible(option))
                return false;

            if (option.IsSlannOption() && !option.Realised && IsMaxSlannOption())
                return false;

            return true;
        }

        public Option.OnlyForType GetMountTypeAlreadyFixed()
        {
            foreach (Option option in Options.Where(x => x.IsActual()))
            {
                if (option.OnlyFor == Option.OnlyForType.Mount)
                    return Option.OnlyForType.Mount;

                if (option.OnlyFor == Option.OnlyForType.Infantry)
                    return Option.OnlyForType.Infantry;
            }

            return Option.OnlyForType.All;
        }

        public bool IsOptionRealised(string optionName) =>
            Options.Where(x => (x.Name.ToUpper() == optionName.ToUpper()) && (x.Realised || x.IsMagicItem() || x.IsPowers())).FirstOrDefault() != null;
 
        public bool IsAnotherOptionRealised(string[] optionNames, bool defaultResult)
        {
            if (optionNames.Length <= 0)
                return defaultResult;

            return optionNames.Where(x => IsOptionRealised(x)).FirstOrDefault() != null;
        }

        public bool IsAnotherOptionIsIncompatible(Option option)
        {
            bool yesWhenNecessaryNo = !IsAnotherOptionRealised(option.OnlyIfAnotherService, defaultResult: true);
            bool noWhenNecessaryYes = IsAnotherOptionRealised(option.OnlyIfNotAnotherService, defaultResult: false);

            return (yesWhenNecessaryNo || noWhenNecessaryYes);
        }

        public bool IsMaxSlannOption() => Options.Where(x => x.IsSlannOption() && x.Realised).Count() >= 4;

        public Unit SetTestType(TestTypeTypes testType)
        {
            TestType = testType;

            return this;
        }

        public bool IsSimpleMount() => (this.Type == Unit.UnitType.Mount) && (this.OriginalWounds == 1);

        public bool IsNotSimpleMount() => (this.Type != Unit.UnitType.Mount) || (this.OriginalWounds != 1);

        public bool IsFearOrTerror() => (this.Terror || this.Fear || this.Undead);

        public bool IsAlready(string name) => Options.Where(x => x.Name == name).FirstOrDefault() != null;
    }
}
