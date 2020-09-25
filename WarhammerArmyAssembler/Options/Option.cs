using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Option : INotifyPropertyChanged
    {
        public enum OptionType { Weapon, Armour, AdditionalArmour, Shield, Arcane, Banner, Option, SlannOption, Powers }

        public enum OnlyForType { All, Infantry, Mount }

        public string Name { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }
        public OptionType Type { get; set; }
        public OnlyForType OnlyFor { get; set; }
        public string[] OnlyIfAnotherService { get; set; }
        public string[] OnlyIfNotAnotherService { get; set; }
        public bool OnlyOneInArmy { get; set; }
        public bool OnlyOneForSuchUnits { get; set; }
        public string OnlyForGroup { get; set; }
        public bool Realised { get; set; }
        public bool Multiple { get; set; }
        public Countable Countable { get; set; }

        public double Points { get; set; }
        public bool PerModel { get; set; }

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
        public int ArmourPiercing { get; set; }
        public string Reroll { get; set; }
        public bool Regeneration { get; set; }
        public bool ImmuneToPsychology { get; set; }
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
        public bool SteamTank { get; set; }
        public bool Lance { get; set; }
        public bool Flail { get; set; }
        public bool Resolute { get; set; }
        public bool PredatoryFighter { get; set; }
        public bool MurderousProwess { get; set; }
        public bool BigWeapon { get; set; }
        public bool Bloodroar { get; set; }
        public int AddToHit { get; set; }
        public int SubOpponentToHit { get; set; }
        public int AddToWound { get; set; }
        public int SubOpponentToWound { get; set; }

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

        public int AddToModelsInPack { get; set; }
        public bool FullCommand { get; set; }
        public bool PersonifiedCommander { get; set; }

        public string[] SpecialRuleDescription { get; set; }

        public bool Mount { get; set; }

        public ObservableCollection<Option> Items { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }
        public string ArtefactGroup { get; set; }
        public bool Artefacts { get; set; }

        public int MagicItems { get; set; }
        public Unit.MagicItemsTypes MagicItemsType { get; set; }


        private bool artefactAlreadyUsed = false;
        public bool ArtefactAlreadyUsed
        {
            get
            {
                return artefactAlreadyUsed;
            }
            set
            {
                artefactAlreadyUsed = value;
                OnPropertyChanged("ArtefactAlreadyUsed");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public Option()
        {
            this.Items = new ObservableCollection<Option>();
        }

        public Option Clone()
        {
            Option newOption = new Option
            {
                Name = this.Name,
                ID = this.ID,
                IDView = this.IDView,
                Points = this.Points,
                PerModel = this.PerModel,
                Type = this.Type,
                Description = this.Description,
                SpecialRuleDescription = this.SpecialRuleDescription,
                Realised = this.Realised,
                Multiple = this.Multiple,
                OnlyOneInArmy = this.OnlyOneInArmy,
                OnlyOneForSuchUnits = this.OnlyOneForSuchUnits,
                OnlyFor = this.OnlyFor,
                OnlyIfAnotherService = this.OnlyIfAnotherService,
                OnlyIfNotAnotherService = this.OnlyIfNotAnotherService,
                OnlyForGroup = this.OnlyForGroup,
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
                ArmourPiercing = this.ArmourPiercing,
                Regeneration = this.Regeneration,
                ImmuneToPsychology = this.ImmuneToPsychology,
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
                SteamTank = this.SteamTank,
                PredatoryFighter = this.PredatoryFighter,
                MurderousProwess = this.MurderousProwess,
                Bloodroar = this.Bloodroar,
                AddToHit = this.AddToHit,
                SubOpponentToHit = this.SubOpponentToHit,
                AddToWound = this.AddToWound,
                SubOpponentToWound = this.SubOpponentToWound,

                Lance = this.Lance,
                Flail = this.Flail,
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

                AddToModelsInPack = this.AddToModelsInPack,
                FullCommand = this.FullCommand,
                PersonifiedCommander = this.PersonifiedCommander,

                MagicItems = this.MagicItems,
                MagicItemsType = this.MagicItemsType,

                Mount = this.Mount,

                InterfaceColor = this.InterfaceColor
            };

            if (this.Countable != null)
                newOption.Countable = this.Countable.Clone();

            return newOption;
        }

        public string SelfDescription()
        {
            string describe = String.Empty;

            if (OnlyFor != OnlyForType.All)
                describe += String.Format("\nOnly for models: {0}", OnlyFor);

            if (!String.IsNullOrEmpty(OnlyForGroup))
                describe += String.Format("\nOnly for: {0}", OnlyForGroup);

            if (OnlyOneInArmy)
                describe += "\nOnly one in army";

            if (OnlyOneForSuchUnits)
                describe += "\nOnly one for each type units";

            return describe;
        }

        public bool IsMagicItem()
        {
            if (this.Type == OptionType.Weapon || this.Type == OptionType.Armour ||
                this.Type == OptionType.AdditionalArmour || this.Type == OptionType.Shield ||
                this.Type == OptionType.Arcane || this.Type == OptionType.Banner)
                return true;
            else
                return false;
        }

        public bool IsPowers()
        {
            return (this.Type == OptionType.Powers);
        }

        public bool IsOption()
        {
            return (this.Type == OptionType.Option || this.Type == OptionType.SlannOption);
        }

        public bool IsSlannOption()
        {
            return (this.Type == OptionType.SlannOption);
        }

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

        public bool IsActual()
        {
            return (IsOption() && Realised) || IsMagicItem() || IsPowers();
        }

        public bool IsUsableByUnit(Unit unit, bool addOption = true)
        {
            if (!String.IsNullOrEmpty(OnlyForGroup) && (OnlyForGroup != unit.GetGroup()))
                return false;

            if (IsPowers())
            {
                if (unit.MagicPowers <= 0)
                    return false;

                foreach (Option option in unit.Options)
                    if (addOption && (option.Name == this.Name))
                        return false;
            }

            if (unit.IsAnotherOptionIsIncompatible(this))
                return false;

            if (TypeAndPointsSatisfy(unit.MagicItemsType, unit.MagicItems, unit.MagicItemCount))
                return true;

            foreach (Option option in unit.Options)
                if (option.IsActual())
                    if (TypeAndPointsSatisfy(option.MagicItemsType, option.MagicItems, unit.MagicItemCount))
                        return true;

            return false;
        }
    }
}
