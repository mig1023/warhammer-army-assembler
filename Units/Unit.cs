using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace WarhammerArmyAssembler.Units
{
    public class Unit
    {
        public enum UnitType
        {
            Lord,
            Hero,
            Core,
            Special,
            Rare,
        }

        public string Name { get; set; }
        public string ID { get; set; }

        public UnitType Type { get; set; }

        public int Size { get; set; }

        public int Points { get; set; }

        public int Movement { get; set; }
        public int WeaponSkill { get; set; }
        public int BallisticSkill { get; set; }
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Wounds { get; set; }
        public int Initiative { get; set; }
        public int Attacks { get; set; }
        public int Leadership { get; set; }
        public int Armour { get; set; }
        public int Ward { get; set; }

        public Brush MovementModificated { get; set; }
        public Brush WeaponSkillModificated { get; set; }
        public Brush BallisticSkillModificated { get; set; }
        public Brush StrengthModificated { get; set; }
        public Brush ToughnessModificated { get; set; }
        public Brush WoundsModificated { get; set; }
        public Brush InitiativeModificated { get; set; }
        public Brush AttacksModificated { get; set; }
        public Brush LeadershipModificated { get; set; }
        public Brush ArmourModificated { get; set; }
        public Brush WardModificated { get; set; }

        public bool ImmuneToPsychology { get; set; }
        public bool Stubborn { get; set; }
        public bool Hate { get; set; }
        public bool Fear { get; set; }
        public bool Terror { get; set; }
        public bool Frenzy { get; set; }
        public bool Unbreakable { get; set; }
        public bool ColdBlooded { get; set; }

        public bool HitFirst { get; set; }
        public bool Regeneration { get; set; }
        public bool KillingBlow { get; set; }
        public bool PoisonAttack { get; set; }

        public List<Ammunition> Weapons = new List<Ammunition>();

        public string InterfaceRules { get; set; }

        public int InterfacePoints { get; set; }

        public int GetUnitPoints()
        {
            return Size * Points;
        }

        public Unit Clone()
        {
            Unit newUnit = new Unit();

            newUnit.Name = this.Name;
            newUnit.ID = this.ID;
            newUnit.Type = this.Type;
            newUnit.Size = this.Size;
            newUnit.Points = this.Points;

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

            newUnit.MovementModificated = Brushes.Gray;
            newUnit.WeaponSkillModificated = Brushes.Gray;
            newUnit.BallisticSkillModificated = Brushes.Gray;
            newUnit.StrengthModificated = Brushes.Gray;
            newUnit.ToughnessModificated = Brushes.Gray;
            newUnit.WoundsModificated = Brushes.Gray;
            newUnit.InitiativeModificated = Brushes.Gray;
            newUnit.AttacksModificated = Brushes.Gray;
            newUnit.LeadershipModificated = Brushes.Gray;
            newUnit.ArmourModificated = Brushes.Gray;
            newUnit.WardModificated = Brushes.Gray;

            newUnit.ImmuneToPsychology = this.ImmuneToPsychology;
            newUnit.Stubborn = this.Stubborn;
            newUnit.Hate = this.Hate;
            newUnit.Fear = this.Fear;
            newUnit.Terror = this.Terror;
            newUnit.Frenzy = this.Frenzy;
            newUnit.Unbreakable = this.Unbreakable;
            newUnit.ColdBlooded = this.ColdBlooded;
            newUnit.HitFirst = this.HitFirst;
            newUnit.Regeneration = this.Regeneration;
            newUnit.KillingBlow = this.KillingBlow;
            newUnit.PoisonAttack = this.PoisonAttack;

            List <Ammunition> Weapons = new List<Ammunition>();
            foreach (Ammunition ammunition in this.Weapons)
                newUnit.Weapons.Add(ammunition);

            return newUnit;
        }

        public Unit GetWeaponsRules()
        {
            Unit unit = this.Clone();

            foreach (Ammunition ammunition in this.Weapons)
            {
                if (ammunition.AddToMovement > 0)
                {
                    unit.Movement += ammunition.AddToMovement;
                    unit.MovementModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToWeaponSkill > 0)
                {
                    unit.WeaponSkill += ammunition.AddToWeaponSkill;
                    unit.WeaponSkillModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToBallisticSkill > 0)
                {
                    unit.BallisticSkill += ammunition.AddToBallisticSkill;
                    unit.BallisticSkillModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToStrength > 0)
                {
                    unit.Strength += ammunition.AddToStrength;
                    unit.StrengthModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToToughness > 0)
                {
                    unit.Toughness += ammunition.AddToToughness;
                    unit.ToughnessModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToWounds > 0)
                {
                    unit.Wounds += ammunition.AddToWounds;
                    unit.WoundsModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToInitiative > 0)
                {
                    unit.Initiative += ammunition.AddToInitiative;
                    unit.InitiativeModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToAttacks > 0)
                {
                    unit.Attacks += ammunition.AddToAttacks;
                    unit.AttacksModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToLeadership > 0)
                {
                    unit.Leadership += ammunition.AddToLeadership;
                    unit.LeadershipModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToArmour > 0)
                {
                    unit.Armour += ammunition.AddToArmour;
                    unit.ArmourModificated = Brushes.LimeGreen;
                };

                if (ammunition.AddToWard > 0)
                {
                    unit.Ward += ammunition.AddToWard;
                    unit.WardModificated = Brushes.LimeGreen;
                };
            }

            return unit;
        }

        public string GetSpecialRules()
        {
            string rules = String.Empty;

            if (ImmuneToPsychology)
                rules += "иммунен к психологии; ";

            if (Stubborn)
                rules += "упорность; ";

            if (Hate)
                rules += "ненависть; ";

            if (Fear)
                rules += "страх; ";

            if (Terror)
                rules += "ужас; ";

            if (Frenzy)
                rules += "бешенство; ";

            if (Unbreakable)
                rules += "несломимость; ";

            if (ColdBlooded)
                rules += "хладнокровие; ";

            if (HitFirst)
                rules += "всегда бьёт первым; ";

            if (Regeneration)
                rules += "регенерация; ";

            if (KillingBlow)
                rules += "смертельный удар; ";

            if (PoisonAttack)
                rules += "ядовитые атаки; ";

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }
    }
}
