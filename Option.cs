using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class Option
    {
        public enum OptionType
        {
            Weapon,
            Armour,
            Arcane,
            Banner,
            Option
        }

        public string Name { get; set; }
        public string ID { get; set; }
        public OptionType Type { get; set; }

        public bool Realised { get; set; }

        public bool Multiple { get; set; }

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

        public ObservableCollection<Option> Items { get; set; }

        public Option()
        {
            this.Items = new ObservableCollection<Option>();
        }

        public Option Clone()
        {
            Option newOption = new Option();

            newOption.Name = this.Name;
            newOption.ID = this.ID;
            newOption.Points = this.Points;
            newOption.PerModel = this.PerModel;
            newOption.Type = this.Type;
            newOption.Description = this.Description;
            newOption.Realised = this.Realised;
            newOption.Multiple = this.Multiple;

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
