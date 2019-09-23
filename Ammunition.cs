using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    public class Ammunition
    {
        public string Name { get; set; }

        public int Points { get; set; }

        public string PointsModifecated { get; set; }

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

        public ObservableCollection<Ammunition> Items { get; set; }

        public Ammunition()
        {
            this.Items = new ObservableCollection<Ammunition>();
        }

        public Ammunition Clone()
        {
            Ammunition newAmmunition = new Ammunition();

            newAmmunition.Name = this.Name;
            newAmmunition.Points = this.Points;

            newAmmunition.HitFirst = this.HitFirst;
            newAmmunition.KillingBlow = this.KillingBlow;
            newAmmunition.PoisonAttack = this.PoisonAttack;

            newAmmunition.BigWeapon = this.BigWeapon;

            newAmmunition.AddToMovement = this.AddToMovement;
            newAmmunition.AddToWeaponSkill = this.AddToWeaponSkill;
            newAmmunition.AddToBallisticSkill = this.AddToBallisticSkill;
            newAmmunition.AddToStrength = this.AddToStrength;
            newAmmunition.AddToToughness = this.AddToToughness;
            newAmmunition.AddToWounds = this.AddToWounds;
            newAmmunition.AddToInitiative = this.AddToInitiative;
            newAmmunition.AddToAttacks = this.AddToAttacks;
            newAmmunition.AddToLeadership = this.AddToLeadership;
            newAmmunition.AddToArmour = this.AddToArmour;
            newAmmunition.AddToWard = this.AddToWard;

            return newAmmunition;
        }
    }
}
