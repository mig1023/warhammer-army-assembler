using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace WarhammerArmyAssembler
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

        public string PointsModifecated { get; set; }

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

        public string MovementModifecated { get; set; }
        public string WeaponSkillModifecated { get; set; }
        public string BallisticSkillModifecated { get; set; }
        public string StrengthModifecated { get; set; }
        public string ToughnessModifecated { get; set; }
        public string WoundsModifecated { get; set; }
        public string InitiativeModifecated { get; set; }
        public string AttacksModifecated { get; set; }
        public string LeadershipModifecated { get; set; }
        public string ArmourModifecated { get; set; }
        public string WardModifecated { get; set; }

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

        public int MagicItems { get; set; }

        public List<Ammunition> Weapons = new List<Ammunition>();

        public ObservableCollection<Unit> Items { get; set; }

        public Unit()
        {
            this.Items = new ObservableCollection<Unit>();
        }

        public string InterfaceRules { get; set; }

        public int InterfacePoints { get; set; }

        public int GetUnitPoints()
        {
            int points = Size * Points;

            foreach (Ammunition ammunition in Weapons)
                points += ammunition.Points;

            return points;
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

            newUnit.MagicItems = this.MagicItems;

            List <Ammunition> Weapons = new List<Ammunition>();
            foreach (Ammunition ammunition in this.Weapons)
                newUnit.Weapons.Add(ammunition);

            return newUnit;
        }

        public Unit GetWeaponsRules()
        {
            Unit unit = this.Clone();

            Brush modificationColor = Brushes.Aquamarine;

            foreach (Ammunition ammunition in this.Weapons)
            {
                if (ammunition.AddToMovement > 0)
                {
                    unit.Movement += ammunition.AddToMovement;
                    unit.MovementModifecated += '*';
                }

                if (ammunition.AddToWeaponSkill > 0)
                {
                    unit.WeaponSkill += ammunition.AddToWeaponSkill;
                    unit.WeaponSkillModifecated += '*';
                }

                if (ammunition.AddToBallisticSkill > 0)
                {
                    unit.BallisticSkill += ammunition.AddToBallisticSkill;
                    unit.BallisticSkillModifecated += '*';
                }

                if (ammunition.AddToStrength > 0)
                {
                    unit.Strength += ammunition.AddToStrength;
                    unit.StrengthModifecated += '*';
                }
                    
                if (ammunition.AddToToughness > 0)
                {
                    unit.Toughness += ammunition.AddToToughness;
                    unit.ToughnessModifecated += '*';
                }
                    
                if (ammunition.AddToWounds > 0)
                {
                    unit.Wounds += ammunition.AddToWounds;
                    unit.WoundsModifecated += '*';
                }

                if (ammunition.AddToInitiative > 0)
                {
                    unit.Initiative += ammunition.AddToInitiative;
                    unit.InitiativeModifecated += '*';
                }

                if (ammunition.AddToAttacks > 0)
                {
                    unit.Attacks += ammunition.AddToAttacks;
                    unit.AttacksModifecated += '*';
                }

                if (ammunition.AddToLeadership > 0)
                {
                    unit.Leadership += ammunition.AddToLeadership;
                    unit.LeadershipModifecated += '*';
                }

                if (ammunition.AddToArmour > 0)
                {
                    unit.Armour += ammunition.AddToArmour;
                    unit.ArmourModifecated += '*';
                }
                    
                if (ammunition.AddToWard > 0)
                {
                    unit.Ward += ammunition.AddToWard;
                    unit.WardModifecated += '*';
                }
            }

            unit.MovementModifecated = unit.Movement.ToString() + unit.MovementModifecated;
            unit.WeaponSkillModifecated = unit.WeaponSkill.ToString() + unit.WeaponSkillModifecated;
            unit.BallisticSkillModifecated = unit.BallisticSkill.ToString() + unit.BallisticSkillModifecated;
            unit.StrengthModifecated = unit.Strength.ToString() + unit.StrengthModifecated;
            unit.ToughnessModifecated = unit.Toughness.ToString() + unit.ToughnessModifecated;
            unit.WoundsModifecated = unit.Wounds.ToString() + unit.WoundsModifecated;
            unit.InitiativeModifecated = unit.Initiative.ToString() + unit.InitiativeModifecated;
            unit.AttacksModifecated = unit.Attacks.ToString() + unit.AttacksModifecated;
            unit.LeadershipModifecated = unit.Leadership.ToString() + unit.LeadershipModifecated;
            unit.ArmourModifecated = unit.Armour.ToString() + unit.ArmourModifecated;
            unit.WardModifecated = unit.Ward.ToString() + unit.WardModifecated;

            return unit;
        }

        public string GetAmmunition()
        {
            string allAmmunition = String.Empty;

            foreach (Ammunition ammunition in this.Weapons)
                allAmmunition += ammunition.Name + "; ";

            if (!String.IsNullOrEmpty(allAmmunition))
                allAmmunition = allAmmunition.Remove(allAmmunition.Length - 2);

            return allAmmunition;
        }

        public void AddAmmunition(string id)
        {
            Weapons.Add(ArmyBook.Artefact[id].Clone());
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

        public bool IsHero()
        {
            return (Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero ? true : false);
        }
    }
}
