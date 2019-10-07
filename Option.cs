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
        public enum OptionType { Weapon, Armour, Arcane, Banner, Option }

        public string Name { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }
        public OptionType Type { get; set; }

        public bool Realised { get; set; }
        public bool Multiple { get; set; }
        public bool OnlyOneInArmy { get; set; }

        public int Points { get; set; }
        public bool PerModel { get; set; }

        public string PointsView { get; set; }

        public string Description { get; set; }

        public bool HitFirst { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }

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

        public string MountOn { get; set; }

        public ObservableCollection<Option> Items { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }

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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propName));
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
            newOption.Realised = this.Realised;
            newOption.Multiple = this.Multiple;
            newOption.OnlyOneInArmy = this.OnlyOneInArmy;

            newOption.HitFirst = this.HitFirst;
            newOption.KillingBlow = this.KillingBlow;
            newOption.PoisonAttack = this.PoisonAttack;

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

            newOption.MountOn = this.MountOn;

            newOption.InterfaceColor = this.InterfaceColor;

            return newOption;
        }

        public bool IsMagicItem()
        {
            if (this.Type == OptionType.Weapon || this.Type == OptionType.Armour || this.Type == OptionType.Arcane || this.Type == OptionType.Banner)
                return true;
            else
                return false;
        }

        public bool IsOption()
        {
            if (this.Type == OptionType.Option)
                return true;
            else
                return false;
        }

        public bool IsUsableByUnit(Unit.MagicItemsTypes unitType)
        {
            if ((unitType == Unit.MagicItemsTypes.Unit) && (Type != OptionType.Banner))
                return false;

            if ((unitType == Unit.MagicItemsTypes.Mage) && (Type == OptionType.Banner))
                return false;

            if ((unitType == Unit.MagicItemsTypes.Hero) && (Type == OptionType.Arcane))
                return false;

            return true;
        }
    }
}
