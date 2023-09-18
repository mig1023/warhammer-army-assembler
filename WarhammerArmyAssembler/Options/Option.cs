﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Option : INotifyPropertyChanged
    {
        public enum OptionType
        {
            Weapon,
            Armour,
            Additional,
            Shield,
            Arcane,
            Banner,
            Option,
            SlannOption,
            Powers,
            Info,
        }

        public enum OptionCategory
        {
            Option,
            Equipment,
            SpecialRule,
            Nope,
        }

        public string Name { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }
        public OptionType Type { get; set; }
        public string Only { get; set; }
        public OptionCategory Category { get; set; }
        public string[] Dependencies { get; set; }
        public string[] InverseDependencies { get; set; }
        public string DependencyGroup { get; set; }
        public bool OnlyOneInArmy { get; set; }
        public bool OnlyOneSuchUnits { get; set; }
        public bool Realised { get; set; }
        public bool Multiple { get; set; }
        public bool Virtue { get; set; }
        public bool Honours { get; set; }
        public Countable Countable { get; set; }
        public int Runic { get; set; }
        public bool MasterRunic { get; set; }
        public string RandomGroup { get; set; }
        public bool TypeUnitIncrese { get; set; }

        public double Points { get; set; }
        public bool PerModel { get; set; }
        public double VirtueOriginalPoints { get; set; }
        public bool MagicItemsPoints { get; set; }

        public string PointsView { get; set; }

        public string Description { get; set; }

        public string Group { get; set; }
        public bool AutoHit { get; set; }
        public bool AutoWound { get; set; }
        public bool AutoDeath { get; set; }
        public bool HitFirst { get; set; }
        public bool HitLast { get; set; }
        public bool KillingBlow { get; set; }
        public int ExtendedKillingBlow { get; set; }
        public bool HeroicKillingBlow { get; set; }
        public bool PoisonAttack { get; set; }
        public string MultiWounds { get; set; }
        public bool NoArmour { get; set; }
        public bool NoWard { get; set; }
        public bool NoKillingBlow { get; set; }
        public bool NoMultiWounds { get; set; }
        public int ArmourPiercing { get; set; }
        public string Reroll { get; set; }
        public bool Regeneration { get; set; }
        public int ExtendedRegeneration { get; set; }
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
        public string ImpactHit { get; set; }
        public int ImpactHitByFront { get; set; }
        public bool SteamTank { get; set; }
        public bool Lance { get; set; }
        public bool Flail { get; set; }
        public int ChargeStrengthBonus { get; set; }
        public bool Resolute { get; set; }
        public bool PredatoryFighter { get; set; }
        public bool MurderousProwess { get; set; }
        public bool BigWeapon { get; set; }
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
        public bool NotALeader { get; set; }
        public bool NativeArmour { get; set; }
        public bool DogsOfWar { get; set; }
        public bool CloseTreeView { get; set; }
        public bool General { get; set; }
        public bool Scout { get; set; }
        public bool Scouts { get; set; }
        public bool FastCavalry { get; set; }

        public List<Test.Param> ParamTests { get; set; }

        public int AddToMovement { get; set; }
        public int AddToWeaponSkill { get; set; }
        public int AddToBallisticSkill { get; set; }
        public int AddToStrength { get; set; }
        public int AddToToughness { get; set; }
        public int AddToWounds { get; set; }
        public int AddToInitiative { get; set; }
        public int AddToAttacks { get; set; }
        public int AddToLeadership { get; set; }
        public int AddToArmour { get; set; }
        public int AddToWard { get; set; }
        public int AddToCast { get; set; }
        public int AddToDispell { get; set; }
        public int AddToWizard { get; set; }
        public string AddToCloseCombat { get; set; }

        public int MovementTo { get; set; }
        public int WeaponSkillTo { get; set; }
        public int BallisticSkillTo { get; set; }
        public int StrengthTo { get; set; }
        public int ToughnessTo { get; set; }
        public int WoundsTo { get; set; }
        public int InitiativeTo { get; set; }
        public int AttacksTo { get; set; }
        public int LeadershipTo { get; set; }
        public int ArmourTo { get; set; }
        public int WizardTo { get; set; }
        public int MagicResistance { get; set; }

        public int AddToModelsInPack { get; set; }
        public bool Command { get; set; }
        public bool PersonifiedCommander { get; set; }

        public string[] SpecialRuleDescription { get; set; }

        public bool Mount { get; set; }

        public ObservableCollection<Option> Items { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroupBold { get; set; }
        public string ArtefactGroup { get; set; }
        public bool Artefacts { get; set; }

        public int MagicItems { get; set; }
        public Unit.MagicItemsTypes MagicItemsType { get; set; }
        public SolidColorBrush TooltipColor { get; set; }

        public bool OnlyRuleOption { get; set; }

        private bool artefactAlreadyUsed = false;
        public bool ArtefactAlreadyUsed
        {
            get => artefactAlreadyUsed;
            set
            {
                artefactAlreadyUsed = value;
                OnPropertyChanged("ArtefactAlreadyUsed");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public Option() =>
            this.Items = new ObservableCollection<Option>();

        public Option Clone() => new Option
        {
            Name = this.Name,
            ID = this.ID,
            IDView = this.IDView,
            Points = this.Points,
            VirtueOriginalPoints = this.VirtueOriginalPoints,
            PerModel = this.PerModel,
            MagicItemsPoints = this.MagicItemsPoints,
            Type = this.Type,
            Category = this.Category,
            Description = this.Description,
            SpecialRuleDescription = this.SpecialRuleDescription,
            Realised = this.Realised,
            Multiple = this.Multiple,
            Virtue = this.Virtue,
            Honours = this.Honours,
            OnlyOneInArmy = this.OnlyOneInArmy,
            OnlyOneSuchUnits = this.OnlyOneSuchUnits,
            Only = this.Only,
            Dependencies = this.Dependencies,
            InverseDependencies = this.InverseDependencies,
            DependencyGroup = this.DependencyGroup,
            Group = this.Group,
            AutoHit = this.AutoHit,
            AutoWound = this.AutoWound,
            AutoDeath = this.AutoDeath,
            HitFirst = this.HitFirst,
            HitLast = this.HitLast,
            KillingBlow = this.KillingBlow,
            ExtendedKillingBlow = this.ExtendedKillingBlow,
            HeroicKillingBlow = this.HeroicKillingBlow,
            PoisonAttack = this.PoisonAttack,
            MultiWounds = this.MultiWounds,
            NoArmour = this.NoArmour,
            NoWard = this.NoWard,
            NoMultiWounds = this.NoMultiWounds,
            NoKillingBlow = this.NoKillingBlow,
            ArmourPiercing = this.ArmourPiercing,
            Regeneration = this.Regeneration,
            ExtendedRegeneration = this.ExtendedRegeneration,
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
            Reroll = this.Reroll,
            Stupidity = this.Stupidity,
            Undead = this.Undead,
            StrengthInNumbers = this.StrengthInNumbers,
            ImpactHit = this.ImpactHit,
            ImpactHitByFront = this.ImpactHitByFront,
            SteamTank = this.SteamTank,
            PredatoryFighter = this.PredatoryFighter,
            MurderousProwess = this.MurderousProwess,
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
            NotALeader = this.NotALeader,
            Runic = this.Runic,
            MasterRunic = this.MasterRunic,
            RandomGroup = this.RandomGroup,
            TypeUnitIncrese = this.TypeUnitIncrese,
            NativeArmour = this.NativeArmour,
            DogsOfWar = this.DogsOfWar,
            CloseTreeView = this.CloseTreeView,
            General = this.General,
            Scout = this.Scout,
            Scouts = this.Scouts,
            FastCavalry = this.FastCavalry,

            Lance = this.Lance,
            Flail = this.Flail,
            ChargeStrengthBonus = this.ChargeStrengthBonus,
            Resolute = this.Resolute,
            BigWeapon = this.BigWeapon,
                
            ParamTests = Test.Param.Clone(this.ParamTests),

            AddToMovement = this.AddToMovement,
            AddToWeaponSkill = this.AddToWeaponSkill,
            AddToBallisticSkill = this.AddToBallisticSkill,
            AddToStrength = this.AddToStrength,
            AddToToughness = this.AddToToughness,
            AddToWounds = this.AddToWounds,
            AddToInitiative = this.AddToInitiative,
            AddToAttacks = this.AddToAttacks,
            AddToLeadership = this.AddToLeadership,
            AddToArmour = this.AddToArmour,
            AddToWard = this.AddToWard,
            AddToCast = this.AddToCast,
            AddToDispell = this.AddToDispell,
            AddToWizard = this.AddToWizard,
            AddToCloseCombat = this.AddToCloseCombat,

            MovementTo = this.MovementTo,
            WeaponSkillTo = this.WeaponSkillTo,
            BallisticSkillTo = this.BallisticSkillTo,
            StrengthTo = this.StrengthTo,
            ToughnessTo = this.ToughnessTo,
            WoundsTo = this.WoundsTo,
            InitiativeTo = this.InitiativeTo,
            AttacksTo = this.AttacksTo,
            LeadershipTo = this.LeadershipTo,
            ArmourTo = this.ArmourTo,
            WizardTo = this.WizardTo,
            MagicResistance = this.MagicResistance,

            AddToModelsInPack = this.AddToModelsInPack,
            Command = this.Command,
            PersonifiedCommander = this.PersonifiedCommander,

            MagicItems = this.MagicItems,
            MagicItemsType = this.MagicItemsType,
            TooltipColor = this.TooltipColor,

            Mount = this.Mount,

            InterfaceColor = this.InterfaceColor,
            Countable = this.Countable?.Clone(),
            OnlyRuleOption = this.OnlyRuleOption,
        };

        public Dictionary<int, Option> AllRunicVersions()
        {
            Dictionary<int, Option> options = ArmyBook.Data.Artefact
                .Where(x => x.Value.Name == this.Name)
                .ToDictionary(x => x.Value.Runic, x => x.Value);

            return options;
        }

        public List<Option> AllRandomByGroup()
        {
            List<Option> options = ArmyBook.Data.Artefact
                .Where(x => x.Value.RandomGroup == this.RandomGroup)
                .Select(x => x.Value)
                .ToList();

            return options;
        }

        public int GetWizardLevelBonus() =>
            this.Countable.Value - (this.Countable.Nullable && (this.Countable.Value > 0) ? 1 : 0);

        public string FullName()
        {
            string runic = Runic > 2 ? "Three" : "Two";
            string name = Name.Replace("Rune", "runes");

            return Runic > 1 ? $"{runic} {name}" : Name;
        }

        public string SelfDescription()
        {
            string describe = String.Empty;

            if (!String.IsNullOrEmpty(Only))
                describe += $"Only for: {Only}\n";

            if (OnlyOneInArmy)
                describe += "Only one in army\n";

            if (OnlyOneSuchUnits)
                describe += "Only one for each type units\n";

            return describe;
        }

        public bool IsMagicItem()
        {
            bool weapons = this.Type == OptionType.Weapon || this.Type == OptionType.Info;
            bool armour = this.Type == OptionType.Armour || this.Type == OptionType.Shield;
            bool additional = this.Type == OptionType.Additional;
            bool otherStuffs = this.Type == OptionType.Arcane || this.Type == OptionType.Banner;

            return weapons || armour || additional || otherStuffs;
        }

        public bool IsPowers() =>
            this.Type == OptionType.Powers;

        public bool IsOption() =>
            this.Type == OptionType.Option || this.Type == OptionType.SlannOption;

        public bool IsEquipment() =>
            this.Category == OptionCategory.Equipment;

        public bool IsSpecaialRule() =>
            this.Category == OptionCategory.SpecialRule;

        public bool IsSlannOption() =>
            this.Type == OptionType.SlannOption;

        private bool TypeAndPointsSatisfy(Unit.MagicItemsTypes itemsType, int itemsPoints, int itemsCout)
        {
            if ((itemsPoints < Points) && (itemsCout <= 0))
                return false;

            if ((itemsType == Unit.MagicItemsTypes.Unit) && (Type != OptionType.Banner))
                return false;

            if ((itemsType == Unit.MagicItemsTypes.Wizard) && (Type == OptionType.Banner))
                return false;

            if ((itemsType == Unit.MagicItemsTypes.Hero) && (Type == OptionType.Arcane))
                return false;

            return true;
        }

        public bool IsActual() =>
            (IsOption() && Realised) || IsMagicItem() || IsPowers();

        public bool IsUsableByUnit(Unit unit, bool addOption = true, bool dragOverCheck = false)
        {
            string group = ArmyBook.Services.ExistsInOnly(Only, unit.GetGroup());

            if (!String.IsNullOrEmpty(Only) && (group != unit.GetGroup()))
                return false;

            if ((Runic > 0) && dragOverCheck)
            {
                Dictionary<int, Option> versions = AllRunicVersions();

                Option currentItem = unit.GetCurrentRunicItemByName(this.Name);

                if ((currentItem != null) && (currentItem.Runic >= versions.Count))
                    return false;

                if (unit.GetCurrentRunicItemsByCount() >= 3)
                    return false;
            }

            if (IsPowers() && (unit.MagicPowersCount > 0))
            {
                if (unit.MagicPowersCountAlreadyUsed() > unit.GetMagicPowersCount())
                    return false;
            }
            else if (IsPowers())
            {
                if (unit.MagicPowers <= 0)
                    return false;

                Option option = unit.Options.Where(x => addOption && (x.Name == this.Name)).FirstOrDefault();
                
                if (option != null)
                    return false;
            }

            if ((Virtue || Honours) && addOption && unit.IsAlready(this.Name))
                return false;

            if (unit.IsAnotherOptionIsIncompatible(this))
                return false;

            if ((Type == OptionType.Powers) && (unit.GetMagicPowersCount() > unit.MagicPowersCountAlreadyUsed()))
                return true;

            if (TypeAndPointsSatisfy(unit.MagicItemsType, unit.MagicItemsPoints, unit.MagicItemCount))
                return true;

            foreach (Option option in unit.Options.Where(x => x.IsActual()))
            {
                if (TypeAndPointsSatisfy(option.MagicItemsType, option.MagicItems, unit.MagicItemCount))
                    return true;
            }

            return false;
        }
    }
}
