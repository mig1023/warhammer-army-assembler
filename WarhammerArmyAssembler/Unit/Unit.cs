﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Unit : SpecialRules, IComparable<Unit>
    {
        public enum UnitType
        {
            Lord,
            Hero,
            Core,
            Special,
            Rare,
            Mount,
            Crew,
            ToCore,
            ToSpecial
        }

        public enum MagicItemsTypes
        {
            Hero,
            Wizard,
            Unit,
        }

        public enum TestTypeTypes
        {
            Unit,
            Enemy,
        }

        public string Name { get; set; }

        public string Personification { get; set; }

        public string NameInGrid
        {
            get
            {
                string mount = this.Type == Unit.UnitType.Mount ? "↳ " : String.Empty;

                if (!String.IsNullOrEmpty(this.Personification))
                    return $"{mount}{this.Personification} ({Name})";

                else
                    return $"{mount}{Name}";
            }
        }

        public string Homologue { get; set; }
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
        public bool Singleton { get; set; }

        public double Points { get; set; }
        public double Prepayment { get; set; }
        public string PointsView { get; set; }

        public string Lore { get; set; }
        public string TooltipText { get => Lore; }

        public Profile Movement { get; set; }
        public Profile WeaponSkill { get; set; }
        public Profile BallisticSkill { get; set; }
        public Profile Strength { get; set; }
        public Profile Toughness { get; set; }
        public Profile Wounds { get; set; }
        public Profile Initiative { get; set; }
        public Profile Attacks { get; set; }
        public Profile Leadership { get; set; }
        public Profile Armour { get; set; }
        public Profile Ward { get; set; }

        public int Wizard { get; set; }

        public string AddToCloseCombat { get; set; }

        public bool LargeBase { get; set; }
        public bool HellPitAbomination { get; set; }
        public bool Giant { get; set; }

        public List<Test.Param> ParamTests { get; set; }

        public List<string> Slots { get; set; }
        public bool NoCoreSlot { get; set; }

        public int MagicItemsPoints { get; set; }
        public int MagicItemCount { get; set; }
        public MagicItemsTypes MagicItemsType { get; set; }
        public int MagicPowers { get; set; }
        public int MagicPowersCount { get; set; }

        public int MountOn { get; set; }
        public string MountInit { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroupBold { get; set; }

        public bool Character { get; set; }
        public bool CurrentGeneral { get; set; }
        public bool WeaponTeam { get; set; }
        public int Chariot { get; set; }

        public List<Option> Options = new List<Option>();

        public TestTypeTypes TestType { get; set; }
        public Unit Mount { get; set; }
        public bool WoundedWithKillingBlow { get; set; }
        public bool PassThisRound { get; set; }
        public string Armybook { get; set; }

        public ObservableCollection<Unit> Items { get; set; }
        public SolidColorBrush ArmyColor { get; set; }
        public SolidColorBrush TooltipColor { get; set; }
        public string Image { get; set; }
        public bool ImageFromAnotherEdition { get; set; }

        public string RulesView { get; set; }

        public Unit() =>
            this.Items = new ObservableCollection<Unit>();

        public int CompareTo(Unit anotherUnit) =>
            (Test.Fight.CheckInitiative(this, anotherUnit) ? -1 : 1);

        public double GetUnitPoints()
        {
            double points = Prepayment + (Size * Points);

            foreach (Option option in Options)
            {
                if ((option.Countable != null) && option.Countable.Nullable)
                {
                    int value = (option.Countable.Value - option.Countable.Min + 1);
                    points += option.Points * (value > 0 ? value : 0);
                }
                else if (option.Countable != null)
                {
                    points += option.Points * option.Countable.Value;
                }
                else if (!option.IsOption() || (option.IsOption() && option.Realised && !option.IsSlannOption()))
                {
                    points += option.Points * (option.PerModel ? Size : 1);
                }
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

        public string GetGroup(Option option = null)
        {
            if (option == null)
            {
                Option change = Options
                    .Where(x => IsGroupOfRealisedOption(x))
                    .FirstOrDefault();

                return change?.Group ?? Group;
            }
            else
            {
                return option.Group;
            }
        }

        private bool IsGroupOfRealisedOption(Option option)
        {
            if (option.IsOption() && !option.Realised)
            {
                return false;
            }
            else if (String.IsNullOrEmpty(option.Group))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetGroup(string newGroup) =>
            this.Group = newGroup;

        public int GetUnitMagicPoints()
        {
            int unitAllMagicPoints = MagicItemsPoints;

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
                if (option.MagicItemsPoints && option.Realised)
                {
                    alreayUsed += option.Points;
                }
                else if (option.IsMagicItem() && (MagicItemCount <= 0))
                {
                    alreayUsed += option.Points;
                }
                else if (option.IsMagicItem() && (MagicItemCount > 0) && (option.Points > 0))
                {
                    alreayUsed += 1;
                }
            }

            return alreayUsed;
        }

        public double GetUnitMagicPowersPoints() =>
            MagicPowers;

        public double MagicPowersPointsAlreadyUsed() =>
            Options.Where(x => x.IsPowers()).Sum(x => x.Points);

        public double GetMagicPowersCount() =>
            MagicPowersCount;

        public double MagicPowersCountAlreadyUsed() =>
            Options.Where(x => x.IsPowers()).Count();
            
        public double MagicItemSlotsAlreadyUsed() =>
            Options.Where(x => x.IsMagicItem() && (x.Points > 0)).Count();

        public int GetUnitWizard()
        {
            int wizard = Wizard;

            foreach (Option option in Options)
            {
                if (!option.IsOption() || option.IsActual())
                    wizard += option.AddTo.ContainsKey("Wizard") ? option.AddTo["Wizard"] : 0;

                if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                    wizard += option.GetWizardLevelBonus();
            }

            return wizard;
        }

        public Unit Clone(bool full = false)
        {
            Unit newUnit = new Unit
            {
                Name = this.Name,
                Personification = this.Personification,
                Group = this.Group,
                Homologue = this.Homologue,
                ID = this.ID,
                IDView = this.IDView,
                ArmyID = this.ArmyID,
                Type = this.Type,
                Size = this.Size,
                MinSize = this.MinSize,
                MaxSize = this.MaxSize,
                ModelsInPack = this.ModelsInPack,
                Singleton = this.Singleton,
                Points = this.Points,
                Prepayment = this.Prepayment,
                MountOn = this.MountOn,
                MountInit = this.MountInit,
                Lore = this.Lore,

                Wizard = this.Wizard,

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
                ExtendedRegeneration = this.ExtendedRegeneration,
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
                ImpactHitByFront = this.ImpactHitByFront,
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
                DogsOfWar = this.DogsOfWar,
                CloseTreeView = this.CloseTreeView,
                Scout = this.Scout,
                Scouts = this.Scouts,
                FastCavalry = this.FastCavalry,

                ParamTests = Test.Param.Clone(this.ParamTests),

                NoCoreSlot = this.NoCoreSlot,

                MagicItemsPoints = this.MagicItemsPoints,
                MagicItemCount = this.MagicItemCount,
                MagicItemsType = this.MagicItemsType,
                MagicPowers = this.MagicPowers,
                MagicPowersCount = this.MagicPowersCount,
                MagicResistance = this.MagicResistance,

                SizableType = this.SizableType,
                VisibleType = this.VisibleType,
                Character = this.Character,
                CurrentGeneral = this.CurrentGeneral,
                WeaponTeam = this.WeaponTeam,
                NotALeader = this.NotALeader,
                General = this.General,
                Chariot = this.Chariot,

                TestType = this.TestType,
                WoundedWithKillingBlow = false,
                PassThisRound = false,
                Armybook = this.Armybook,

                ArmyColor = this.ArmyColor,
                TooltipColor = this.TooltipColor,
                Image = this.Image,
                ImageFromAnotherEdition = this.ImageFromAnotherEdition,
            };

            newUnit.Movement = this.Movement.Clone();
            newUnit.WeaponSkill = this.WeaponSkill.Clone();
            newUnit.BallisticSkill = this.BallisticSkill.Clone();
            newUnit.Strength = this.Strength.Clone();
            newUnit.Toughness = this.Toughness.Clone();
            newUnit.Wounds = this.Wounds.Clone();
            newUnit.Initiative = this.Initiative.Clone();
            newUnit.Attacks = this.Attacks.Clone();
            newUnit.Leadership = this.Leadership.Clone();

            newUnit.Armour = this.Armour?.Clone();
            newUnit.Ward = this.Ward?.Clone();
            newUnit.Mount = this.Mount?.Clone();

            if (this.Slots != null)
                newUnit.Slots = new List<string>(this.Slots);

            foreach (Option option in this.Options)
                newUnit.Options.Add(option.Clone());

            return newUnit;
        }

        private bool ContainsCaseless(string line, string subline) =>
            line.IndexOf(subline, StringComparison.OrdinalIgnoreCase) >= 0;

        private bool OptionTypeAlreadyUsed(Option option, ref bool alreadyArmour, ref bool alreadyShield)
        {
            bool optionType = option.Type == Option.OptionType.Option;
            bool armourType = option.Type == Option.OptionType.Armour;
            bool shieldType = option.Type == Option.OptionType.Shield;
            bool containsArmour = ContainsCaseless(option.Name, "Armour");
            bool containsShield = ContainsCaseless(option.Name, "Shield");

            if (option.NativeArmour && IsArmourOptionAdded())
            {
                return true;
            }
            else if (alreadyArmour && optionType && containsArmour)
            {
                return true;
            }
            else if (optionType && containsArmour)
            {
                alreadyArmour = true;
            }
            else if (alreadyShield && optionType && containsShield)
            {
                return true;
            }
            else if (optionType && containsShield)
            {
                alreadyShield = true;
            }
            else if (alreadyArmour && armourType)
            {
                return true;
            }
            else if (armourType)
            {
                alreadyArmour = true;
            }
            else if (alreadyShield && shieldType)
            {
                return true;
            }
            else if (shieldType)
            {
                alreadyShield = true;
            }

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

            return $"{param[0]}D{param[1]}{addToDice}";
        }

        private string AddFromAnyOption(ref Unit unit, string name, bool reversParam = false,
            bool mountParam = false, bool doNotCombine = false)
        {

            Profile paramValue = (Profile)typeof(Unit).GetProperty(name).GetValue(unit);

            if ((paramValue == null) && reversParam)
            {
                return String.Empty;
            }
            else if (paramValue == null)
            {
                return "-";
            }

            if (paramValue.Value > 100)
                return GetRandomAttacksLine(paramValue.Value);

            string paramModView = String.Empty;

            List<Option> allOption = new List<Option>(Options);

            if (mountParam && (MountOn > 0))
                allOption.AddRange(Army.Data.Units[MountOn].Options);

            bool alreadyArmour = false, alreadyShield = false;
            int? newValue = null;

            if (!paramValue.Null)
                newValue = paramValue.Value;

            foreach (Option option in allOption.Where(x => x.IsActual()))
            {
                if (OptionTypeAlreadyUsed(option, ref alreadyArmour, ref alreadyShield))
                    continue;

                if (option.ChangeTo.ContainsKey(name))
                {
                    int optionToValue = option.ChangeTo[name];
                    return optionToValue.ToString() + (reversParam ? "+" : "*");
                }

                if (option.AddTo.ContainsKey(name))
                {
                    int optionValue = option.AddTo[name];

                    if ((optionValue != 0) && reversParam)
                    {
                        if (newValue == null)
                            newValue = 7;

                        if (doNotCombine)
                        {
                            if (optionValue < newValue)
                                newValue = optionValue;
                        }
                        else
                        {
                            newValue -= (7 - optionValue);
                        }
                    }
                    else if (optionValue != 0)
                    {
                        paramModView += '*';
                        newValue = ParamNormalization((int)newValue + optionValue);
                    }
                }
            }

            if ((newValue == null) && reversParam)
                return String.Empty;

            if ((newValue == 0) && !reversParam)
                return "-";

            if (reversParam)
                paramModView += '+';

            return newValue.ToString() + paramModView;
        }

        private void SetUnitParamByOption(string paramName, bool directModification = false)
        {
            bool value = RuleFromAnyOption(paramName, out string stringValue,
                out int intValue, directMods: directModification);

            if (!value)
                return;

            if (intValue > 0)
            {
                typeof(Unit).GetProperty(paramName).SetValue(this, intValue);
            }
            else if (!String.IsNullOrEmpty(stringValue))
            {
                typeof(Unit).GetProperty(paramName).SetValue(this, stringValue);
            }
            else
            {
                typeof(Unit).GetProperty(paramName).SetValue(this, value);
            }
        }

        private SolidColorBrush ColorByMods(string newParamLine)
        {
            string[] colors = ArmyBook.Data.Upgraded
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();

            int count = newParamLine.Count(x => x == '*') - 1;

            if (count > 2)
                count = 2;

            return (SolidColorBrush)new BrushConverter().ConvertFromString($"#{colors[count]}");
        }

        public Unit GetOptionRules(out bool hasMods, bool directModification = false)
        {
            Unit unit = this.Clone();
            hasMods = false;

            List<string> unitsParams = ArmyBook.Constants.ProfilesNames
                .Keys
                .Where(x => !String.IsNullOrEmpty(ArmyBook.Constants.ProfilesNames[x]))
                .ToList();

            foreach (string name in unitsParams)
            {
                bool reverse = name == "Armour" || name == "Ward";
                bool mount = name == "Armour";
                bool combine = name == "Ward";

                string newParamLine = AddFromAnyOption(ref unit, name,
                    reversParam: reverse, mountParam: mount, doNotCombine: combine);

                Profile param = (Profile)typeof(Unit).GetProperty(name).GetValue(unit);

                if (param == null)
                    continue;

                param.View = newParamLine;

                if (newParamLine.Contains("*"))
                {
                    param.Color = ColorByMods(newParamLine);
                    hasMods = true;
                }

                if (directModification && !String.IsNullOrEmpty(newParamLine))
                {
                    string cleanParamLine = newParamLine.Replace("+", String.Empty).Replace("*", String.Empty);

                    if (name == "Armour")
                        param.Null = false;

                    if (cleanParamLine.Contains("-") && (name != "Armour"))
                        cleanParamLine = "0";

                    param.Value = ArmyBook.Parsers.IntParse(cleanParamLine);
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

            unit.Size = baseSize ?? unit.Size;
            unit.Wounds.Original = unit.Wounds.Value;
            unit.Wounds.Value *= unit.Size;

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
            {
                if (this.Size >= r.Key)
                    frontSize = r.Value;
            }

            return frontSize;
        }

        public int GetRank()
        {
            if (!this.IsUnit())
                return 1;

            int front = this.GetFront();

            if (front < 5)
                return 1;

            bool largeFront = this.Wounds.Value % front >= 5;
            int ranks = (this.Wounds.Value / front) + (largeFront ? 1 : 0);

            if (ranks > 3)
                return 3;

            return ranks;
        }

        public Option GetCurrentRunicItemByName(string name) =>
            Options.Where(x => x.Name == name).FirstOrDefault();

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
            Option option = this.Options
                .Where(x => x.ID == optionID)
                .FirstOrDefault();

            if (option == null)
                return;

            if (direction == "-")
            {
                if ((option.Countable.Value == option.Countable.Min) && option.Countable.Nullable)
                {
                    option.Countable.Value = 0;
                }
                else
                {
                    option.Countable.Value -= 1;
                }
            }
            else
            {
                if (!Army.Checks.IsArmyUnitsPointsPercentOk(this.Type, option.Points, 0))
                {
                    Interface.Changes.Error($"The {this.UnitTypeName()} has reached a point cost limit");
                }
                else if (!Interface.Checks.EnoughUnitPointsForAddOption(option.Points))
                {
                    Interface.Changes.Error($"Not enough points to add {this.UnitTypeName()}");
                }
                else if (option.Countable.Nullable && (option.Countable.Value == 0) && (option.Countable.Min > 1))
                {
                    option.Countable.Value = option.Countable.Min;
                }
                else
                {
                    option.Countable.Value += 1;
                }
            }
        }

        public void AddOption(int optionID)
        {
            Option option = this.Options
                .Where(x => x.ID == optionID)
                .FirstOrDefault();

            if (option == null)
                return;

            bool realise = false;

            if (option.IsMagicItem() || option.IsPowers())
            {
                this.Options.Remove(option);

                Option artefact = ArmyBook.Data.Artefact[option.ID];

                if (artefact.TypeUnitIncrese && (this.Type == Unit.UnitType.Special))
                {
                    this.Type = Unit.UnitType.Core;
                }
                else if (artefact.TypeUnitIncrese && (this.Type == Unit.UnitType.Rare))
                {
                    this.Type = Unit.UnitType.Special;
                }

                if (option.Virtue)
                {
                    ArmyBook.Data.Artefact[option.ID].Points = Army.Params.GetVirtuePoints(option.ID);
                    Interface.Reload.LoadArmyList(fastReload: true);
                }
            }
            else
            {
                if (option.Realised)
                {
                    option.Realised = false;
                }
                else
                {
                    double optionPoints = option.PerModel ?
                        option.Points * this.Size : option.Points;

                    if (!Army.Checks.IsArmyUnitsPointsPercentOk(this.Type, option.Points, 0))
                    {
                        Interface.Changes.Error($"The {this.UnitTypeName()} " +
                            $"has reached a point cost limit");

                        return;
                    }
                    else if (!Interface.Checks.EnoughUnitPointsForAddOption(optionPoints))
                    {
                        Interface.Changes.Error($"Not enough points " +
                            $"to add {this.UnitTypeName()}");

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
        }

        public string GetWizardLevelLine()
        {
            int wizard = GetUnitWizard();
            return wizard > 0 ? $"Wizard Level {wizard}" : String.Empty;
        }

        public string GetSpecialRulesLine(bool withCommandData = false,
            bool onlyUnitParam = false, bool withoutWizards = false,
            bool detail = false)
        {
            string rules = withCommandData ? GetCommandGroupLine() : String.Empty;
            string specialRules = String.Empty;
            int index = 0;

            foreach (string rule in GetSpecialRules(onlyUnitParam, withoutWizards))
            {
                index += 1;

                if (detail)
                    specialRules += $"{index}. {rule}\n";
                else
                    specialRules += $"{rule}; ";
            }

            if (detail && !String.IsNullOrEmpty(specialRules))
            {
                rules += $"Special rules:\n{specialRules}\n";
            }
            else
            {
                rules += specialRules;
            }

            if (!detail && !String.IsNullOrEmpty(rules))
            {
                rules = rules.Remove(rules.Length - 2);
            }

            if (detail)
            {
                index = 0;
                string equipments = "Equipments:\n";

                foreach (Option option in Options.Where(x => (x.IsEquipment())))
                {
                    index += 1;
                    equipments += $"{index}. {option.Name}\n";
                }

                if (index > 0)
                    rules += equipments;
            }

            return rules;
        }

        private bool WizardBonus(Dictionary<string, int> changes, string line) =>
            changes.ContainsKey(line) && changes[line] > 0;

        public string GetEquipmentLine()
        {
            string equipment = GetCommandGroupLine();

            foreach (Option option in Options)
            {
                bool named = !String.IsNullOrEmpty(option.Name);
                bool thisIsRealised = (option.Realised || option.IsMagicItem()) && option.Points != 0;
                bool thisIsNotMountOrFC = !(option.Mount || option.Command);
                bool wizard = WizardBonus(option.ChangeTo, "WizardTo") || WizardBonus(option.AddTo, "Wizard");

                if (named && thisIsRealised && thisIsNotMountOrFC && !wizard)
                    equipment += $"{option.FullName()}; ";
            }

            List<Option> countableOptions = Options
                .Where(x => (x.Countable != null) && (x.Countable.Value > 0))
                .ToList();

            foreach (Option option in countableOptions)
            {
                if (!option.Countable?.ExportToWizardLevel ?? false)
                    equipment += $"{option.Countable.Value} {option.Name}; ";
            }

            List<Option> powersOptions = Options
                .Where(x => !String.IsNullOrEmpty(x.Name) && x.IsPowers())
                .ToList();

            foreach (Option option in powersOptions)
                equipment += $"{option.Name}; ";

            if (!String.IsNullOrEmpty(equipment))
                equipment = equipment.Remove(equipment.Length - 2);

            return equipment;
        }

        private string GetModifiedParam(string shortName, string param)
        {
            if (!param.Contains("*") && !param.Contains("+"))
                return String.Empty;

            string cleanParam = param.Replace("*", String.Empty);
            return $"{shortName}={cleanParam}, ";
        }

        public string GetModifiedParamsLine()
        {
            Unit unit = GetOptionRules(hasMods: out _);

            string paramLine = String.Empty;

            Dictionary<string, string> unitParams = new Dictionary<string, string>
            {
                ["M"] = unit.Movement.View,
                ["WS"] = unit.WeaponSkill.View,
                ["BS"] = unit.BallisticSkill.View,
                ["S"] = unit.Strength.View,
                ["T"] = unit.Toughness.View,
                ["W"] = unit.Wounds.View,
                ["I"] = unit.Initiative.View,
                ["A"] = unit.Attacks.View,
                ["LD"] = unit.Leadership.View,
                ["AS"] = unit.Armour.View,
                ["Ward"] = unit.Ward.View,
            };

            foreach(KeyValuePair<string, string> paramPair in unitParams)
                paramLine += GetModifiedParam(paramPair.Key, paramPair.Value);

            if (!String.IsNullOrEmpty(paramLine))
                paramLine = paramLine.Remove(paramLine.Length - 2);

            return paramLine;
        }

        public string SelfDescription()
        {
            string describe = $"\nUnit type: {Type}";

            if (Character)
                describe += " (character)";

            if (!IsHeroOrHisMount())
            {
                string minAndMax = $"\nUnit size: {MinSize} - {MaxSize}";
                string plus = MinSize == MaxSize ? String.Empty : "+";
                string minOnly = $"\nUnit size: {MinSize}{plus}";

                describe += (MaxSize > 0) && (MinSize != MaxSize) ? minAndMax : minOnly;

                if (Singleton)
                    describe += "\nOnly one unit of this type can be in the army";
            }

            int wizard = GetUnitWizard();

            if (wizard > 0)
                describe += $"\nWizard: Level {wizard}";

            if (!String.IsNullOrEmpty(Group))
                describe += $"\nGroup: {Group}";

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

            return typesNames.ContainsKey(Type) ? typesNames[Type] : String.Empty;
        }

        public static int ParamNormalization(int param, bool onlyZeroCheck = false)
        {
            if (param < 0)
                return 0;
            
            if ((param > 10) && !onlyZeroCheck)
                return 10;

            return param;
        }

        private bool GetUnitValueTrueOrFalse(object unitValue,
            out string additionalParam, out int intValue)
        {
            additionalParam = String.Empty;
            intValue = 0;

            if (unitValue is bool)
            {
                return (bool)unitValue;
            }
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
            {
                return false;
            }
        }

        public bool RuleFromAnyOption(string name, out string additionalParam,
            out int intValue, bool onlyUnitParam = false, bool directMods = false)
        {
            object property = typeof(Unit).GetProperty(name).GetValue(this);

            bool anyIsTrue = GetUnitValueTrueOrFalse(property,
                out string lineParamValue, out int intParamValue);

            if (!onlyUnitParam)
            {
                List<Option> options = Options
                    .Where(x => !x.IsOption() || x.Realised)
                    .ToList();

                foreach (Option option in options)
                {
                    PropertyInfo optionField = typeof(Option).GetProperty(name);

                    bool fromParamValue = GetUnitValueTrueOrFalse(optionField.GetValue(option),
                        out string lineOptionValue, out int intOptionValue);

                    anyIsTrue = fromParamValue || anyIsTrue;
                    bool impactHits = option.ImpactHitByFront > 0;
                    bool lineOption = !String.IsNullOrEmpty(lineOptionValue);

                    if ((name == "ImpactHitByFront") && (((int)property > 0) || impactHits))
                    {
                        intParamValue = GetFront();
                        anyIsTrue = true;
                    }
                    else if ((name == "Reroll") && !directMods && fromParamValue && lineOption)
                    {
                        string[] allRerolls = lineOptionValue.Split(';');

                        foreach (string reroll in allRerolls)
                        {
                            string secondElement = String.IsNullOrEmpty(lineParamValue) ?
                                String.Empty : "; ";

                            string[] rerollsParams = reroll.Split('(');

                            lineParamValue += $"{secondElement}" +
                                $"{SpecialRules.RerollsLines[rerollsParams[0]]}";

                            if (rerollsParams.Length > 1)
                                lineParamValue += "(" + rerollsParams[1];
                        }
                    }
                    else if (fromParamValue && !String.IsNullOrEmpty(lineOptionValue))
                    {
                        lineParamValue = lineOptionValue;
                    }

                    if (fromParamValue && (intOptionValue > 0))
                        intParamValue = intOptionValue;
                }
            }

            additionalParam = lineParamValue;
            intValue = intParamValue;

            return anyIsTrue;
        }

        private string GetCommandGroupLine()
        {
            List<string> rules = new List<string>();

            int fullCommand = 0;
            string characterCommander = String.Empty;

            foreach (Option option in Options)
            {
                if (!option.Command || !option.Realised)
                    continue;

                fullCommand += 1;

                if (option.PersonifiedCommander)
                    characterCommander = option.Name;
            }

            if ((fullCommand == 3) && !String.IsNullOrEmpty(characterCommander))
            {
                rules.Add($"FC ({characterCommander})");
            }
            else if (fullCommand == 3)
            {
                rules.Add("FC");
            }
            else
            {
                foreach (Option option in Options.Where(x => (x.Command && x.Realised)))
                    rules.Add(option.Name);
            }
                
            string rulesLine = String.Empty;

            foreach (string rule in rules)
                rulesLine += $"{rule}; ";

            return rulesLine;
        }

        public List<string> GetSpecialRules(bool onlyUnitParam = false, bool withoudWizards = false)
        {
            List<string> rules = new List<string>();

            if (CurrentGeneral)
                rules.Add("General");

            if (MountOn > 0)
                rules.Add(Army.Data.Units[MountOn].Name);

            List<Option> options = Options
                .Where(x => x.Realised && x.SpecialRuleDescription.Length > 0)
                .ToList();

            foreach (Option option in options)
            {
                bool wizExport = option.Countable?.ExportToWizardLevel ?? false;

                bool noWizardBonus = WizardBonus(option.ChangeTo, "WizardTo") ||
                    WizardBonus(option.AddTo, "Wizard");

                if (!withoudWizards || (noWizardBonus && !wizExport))
                    rules.Add(option.Name);
            }

            foreach (KeyValuePair<string, string> specialRule in SpecialRules.All)
            {
                if (String.IsNullOrEmpty(specialRule.Value))
                    continue;

                bool isRuled = RuleFromAnyOption(specialRule.Key, out string additionalParam,
                    out int intParam, onlyUnitParam: onlyUnitParam);

                if (!isRuled)
                    continue;

                if (SpecialRules.IncompatibleRules(specialRule.Key, this))
                    continue;

                string paramValue = intParam > 0 ? intParam.ToString() : additionalParam;
                rules.Add(specialRule.Value.Replace("[X]", paramValue));
            }

            if (ParamTests != null)
            {
                Test.Param.Describe(ParamTests, ref rules);

                if (!onlyUnitParam)
                {
                    foreach (Option option in Options)
                        if (option.ParamTests != null)
                            Test.Param.Describe(option.ParamTests, ref rules);
                }
            }

            List<Option> specialRules = Options
                .Where(x => (x.SpecialRuleDescription.Length > 0) && (!x.IsOption() || x.Realised))
                .ToList();

            foreach (Option rule in specialRules) 
                foreach (string specialRule in rule.SpecialRuleDescription)
                    rules.Add(specialRule);

            return rules;
        }

        public bool IsHero() =>
            Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero;

        public bool IsHeroOrHisMount()
        {
            bool hero = Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero;
            bool mount = Type == Unit.UnitType.Mount;
            bool monster = LargeBase && (MaxSize == MinSize);

            return hero || mount || monster;
        }

        public bool IsUnit() =>
            this.Type == Unit.UnitType.Core || this.Type == Unit.UnitType.Special || this.Type == Unit.UnitType.Rare;

        public bool ExistsOptions() =>
            Options.Where(x => x.IsOption() && !x.MagicItemsPoints && !x.Command).FirstOrDefault() != null;

        public bool ExistsCommand() =>
            Options.Where(x => x.Command).FirstOrDefault() != null;

        public bool ExistsMagicItems() =>
            Options.Where(x => x.IsMagicItem() && ((x.Points > 0) || x.Honours)).FirstOrDefault() != null;

        public bool ExistsMagicPowers() =>
            Options.Where(x => x.Type == Option.OptionType.Powers).FirstOrDefault() != null; 

        public bool ExistsRunicCombinationInUnit(Dictionary<string, int> runicItems)
        {
            foreach (KeyValuePair<string, int> item in runicItems)
            {
                bool exist = false;

                foreach (Option option in Options)
                {
                    if ((item.Key == option.Name) && (item.Value == option.Runic))
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                    return false;
            }

            return true;
        }

        public bool ExistsEquipmentsItems() =>
            Options.Where(x => x.IsEquipment() && !String.IsNullOrEmpty(x.Name)).FirstOrDefault() != null;

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
                string mount = GetMountTypeAlreadyFixed();
                bool incompatible = !IsOptionEnabled(Options[i], GetMountOn(), mount, postCheck: true);
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

        public bool IsOptionEnabled(Option option, int mountAlreadyOn, string mountTypeAlreadyFixed, bool postCheck = false)
        {
            if (!postCheck && option.Mount && (mountAlreadyOn > 0) && (option.ID != mountAlreadyOn))
                return false;

            if (option.Mount && (mountTypeAlreadyFixed == "Infantry"))
                return false;

            string only = ArmyBook.Services.ExistsInOnly(option.Only, "Infantry, Mount");

            if ((only == "Mount") && ((mountAlreadyOn == 0) || (mountTypeAlreadyFixed == "Infantry")))
                return false;

            if ((only == "Infantry") && ((mountAlreadyOn > 0) || (mountTypeAlreadyFixed == "Mount")))
                return false;

            if (IsAnotherOptionIsIncompatible(option, postCheck))
                return false;

            if (option.IsSlannOption() && !option.Realised && IsMaxSlannOption())
                return false;

            double alreadyRealised = Options
                .Where(x => x.MagicItemsPoints && x.Realised)
                .Sum(x => x.Points) + (postCheck ? 0 : option.Points);

            if (option.MagicItemsPoints && !option.Realised && (alreadyRealised > MagicItemsPoints))
                return false;

            return true;
        }

        public string GetMountTypeAlreadyFixed()
        {
            foreach (Option option in Options.Where(x => x.IsActual()))
            {
                if (!String.IsNullOrEmpty(option.Only))
                {
                    string fixedType = ArmyBook.Services.ExistsInOnly(option.Only, "Infantry, Mount");

                    if (!String.IsNullOrEmpty(fixedType))
                        return fixedType;
                }
            }

            return String.Empty;
        }

        public bool IsOptionRealised(string optionName)
        {
            Option option = Options
                 .Where(x => (x.Name.ToUpper() == optionName.ToUpper()) && x.IsActual())
                 .FirstOrDefault();
                
            return option != null;
        }
 
        public bool IsAnotherOptionRealised(string[] optionNames, bool defaultResult)
        {
            if ((optionNames == null) || (optionNames.Length <= 0))
                return defaultResult;

            return optionNames.Where(x => IsOptionRealised(x)).FirstOrDefault() != null;
        }

        private bool IsAnyInGroupUsed(string groupName, Option currentOption)
        {
            IEnumerable<Option> options = Options
                .Where(x => (x.DependencyGroup == groupName) && x.IsActual());

            if (currentOption == null)
                return options.FirstOrDefault() != null;

            IEnumerable<Option> byName = options.Where(y => y.Name != currentOption.Name);
            return byName.FirstOrDefault() != null;

        }

        public bool IsGroupAlreadyUsed(string groupName, Option currentOption, bool postCheck)
        {
            if (String.IsNullOrEmpty(groupName))
                return false;

            return IsAnyInGroupUsed(groupName, postCheck ? null : currentOption);
        }
            
        public bool IsAnotherOptionIsIncompatible(Option option, bool postCheck = false)
        {
            bool yesWhenNecessaryNo =
                !IsAnotherOptionRealised(option.Dependencies, defaultResult: true);

            bool noWhenNecessaryYes =
                IsAnotherOptionRealised(option.InverseDependencies, defaultResult: false);

            bool groopAlreadyUsed = postCheck ?
                false : IsGroupAlreadyUsed(option.DependencyGroup, option, postCheck);

            return (yesWhenNecessaryNo || noWhenNecessaryYes || groopAlreadyUsed);
        }

        public bool IsMaxSlannOption() =>
            Options.Where(x => x.IsSlannOption() && x.Realised).Count() >= 4;

        public Unit SetTestType(TestTypeTypes testType)
        {
            TestType = testType;
            return this;
        }

        public bool IsSimpleMount() =>
            (this.Type == Unit.UnitType.Mount) && (this.Wounds.Original == 1);

        public bool IsNotSimpleMount() =>
            (this.Type != Unit.UnitType.Mount) || (this.Wounds.Original != 1);

        public bool IsFearOrTerror() =>
            (this.Terror || this.Fear || this.Undead);

        public bool IsAlready(string name) =>
            Options.Where(x => x.Name == name).FirstOrDefault() != null;
    }
}
