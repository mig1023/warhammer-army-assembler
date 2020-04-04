using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    public class Option : INotifyPropertyChanged
    {
        public enum OptionType { Weapon, Armour, AdditionalArmour, Shield, Arcane, Banner, Option, SlannOption }

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
        

        public double Points { get; set; }
        public bool PerModel { get; set; }

        public string PointsView { get; set; }

        public string Description { get; set; }

        public bool HitFirst { get; set; }
        public bool HitLast { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }
        public string MultiWounds { get; set; }
        public bool Regeneration { get; set; }
        public bool ImmuneToPsychology { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }

        public bool BigWeapon { get; set; }

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
            Option newOption = new Option();

            newOption.Name = this.Name;
            newOption.ID = this.ID;
            newOption.IDView = this.IDView;
            newOption.Points = this.Points;
            newOption.PerModel = this.PerModel;
            newOption.Type = this.Type;
            newOption.Description = this.Description;
            newOption.SpecialRuleDescription = this.SpecialRuleDescription;
            newOption.Realised = this.Realised;
            newOption.Multiple = this.Multiple;
            newOption.OnlyOneInArmy = this.OnlyOneInArmy;
            newOption.OnlyOneForSuchUnits = this.OnlyOneForSuchUnits;
            newOption.OnlyFor = this.OnlyFor;
            newOption.OnlyIfAnotherService = this.OnlyIfAnotherService;
            newOption.OnlyIfNotAnotherService = this.OnlyIfNotAnotherService;
            newOption.OnlyForGroup = this.OnlyForGroup;

            newOption.HitFirst = this.HitFirst;
            newOption.HitLast = this.HitLast;
            newOption.KillingBlow = this.KillingBlow;
            newOption.PoisonAttack = this.PoisonAttack;
            newOption.MultiWounds = this.MultiWounds;
            newOption.Regeneration = this.Regeneration;
            newOption.ImmuneToPsychology = this.ImmuneToPsychology;
            newOption.Stubborn = this.Stubborn;
            newOption.Hate = this.Hate;
            newOption.Fear = this.Fear;
            newOption.Terror = this.Terror;
            newOption.Frenzy = this.Frenzy;
            newOption.Unbreakable = this.Unbreakable;
            newOption.ColdBlooded = this.ColdBlooded;

            newOption.BigWeapon = this.BigWeapon;

            newOption.AddToMovement = this.AddToMovement;
            newOption.AddToWeaponSkill = this.AddToWeaponSkill;
            newOption.AddToBallisticSkill = this.AddToBallisticSkill;
            newOption.AddToStrength = this.AddToStrength;
            newOption.AddToToughness = this.AddToToughness;
            newOption.AddToWounds = this.AddToWounds;
            newOption.AddToInitiative = this.AddToInitiative;
            newOption.AddToAttacks = this.AddToAttacks;
            newOption.AddToLeadership = this.AddToLeadership;
            newOption.AddToArmour = this.AddToArmour;
            newOption.AddToWard = this.AddToWard;
            newOption.AddToCast = this.AddToCast;
            newOption.AddToDispell = this.AddToDispell;
            newOption.AddToWizard = this.AddToWizard;

            newOption.MovementTo = this.MovementTo;
            newOption.WeaponSkillTo = this.WeaponSkillTo;
            newOption.BallisticSkillTo = this.BallisticSkillTo;
            newOption.StrengthTo = this.StrengthTo;
            newOption.ToughnessTo = this.ToughnessTo;
            newOption.WoundsTo = this.WoundsTo;
            newOption.InitiativeTo = this.InitiativeTo;
            newOption.AttacksTo = this.AttacksTo;
            newOption.LeadershipTo = this.LeadershipTo;
            newOption.ArmourTo = this.ArmourTo;
            newOption.WizardTo = this.WizardTo;

            newOption.AddToModelsInPack = this.AddToModelsInPack;
            newOption.FullCommand = this.FullCommand;
            newOption.PersonifiedCommander = this.PersonifiedCommander;

            newOption.MagicItems = this.MagicItems;
            newOption.MagicItemsType = this.MagicItemsType;

            newOption.Mount = this.Mount;

            newOption.InterfaceColor = this.InterfaceColor;

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

        public bool IsOption()
        {
            if (this.Type == OptionType.Option || this.Type == OptionType.SlannOption)
                return true;
            else
                return false;
        }

        public bool IsSlannOption()
        {
            if (this.Type == OptionType.SlannOption)
                return true;
            else
                return false;
        }

        private bool TypeAndPointsSatisfy(Unit.MagicItemsTypes itemsType, int itemsPoints)
        {
            if (itemsPoints < Points)
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
            return (IsOption() && Realised) || IsMagicItem();
        }

        public bool IsUsableByUnit(Unit unit)
        {
            if (!String.IsNullOrEmpty(OnlyForGroup) && (OnlyForGroup != unit.Group))
                return false;

            if (TypeAndPointsSatisfy(unit.MagicItemsType, unit.MagicItems))
                return true;

            foreach (Option option in unit.Options)
                if (option.IsActual())
                    if (TypeAndPointsSatisfy(option.MagicItemsType, option.MagicItems))
                        return true;

            return false;
        }
    }
}
