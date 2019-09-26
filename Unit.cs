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

        public List<Option> Option = new List<Option>();

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

            foreach (Option option in Option)
                if (!option.IsOption() || (option.IsOption() && option.Realised))
                    points += option.Points;

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
            newUnit.Description = this.Description;

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

            List <Option> Option = new List<Option>();
            foreach (Option option in this.Option)
                newUnit.Option.Add(option);

            return newUnit;
        }

        public Unit GetOptionRules()
        {
            Unit unit = this.Clone();

            Brush modificationColor = Brushes.Aquamarine;

            foreach (Option option in this.Option)
            {
                if (option.AddToMovement > 0)
                {
                    unit.Movement += option.AddToMovement;
                    unit.MovementModifecated += '*';
                }

                if (option.AddToWeaponSkill > 0)
                {
                    unit.WeaponSkill += option.AddToWeaponSkill;
                    unit.WeaponSkillModifecated += '*';
                }

                if (option.AddToBallisticSkill > 0)
                {
                    unit.BallisticSkill += option.AddToBallisticSkill;
                    unit.BallisticSkillModifecated += '*';
                }

                if (option.AddToStrength > 0)
                {
                    unit.Strength += option.AddToStrength;
                    unit.StrengthModifecated += '*';
                }
                    
                if (option.AddToToughness > 0)
                {
                    unit.Toughness += option.AddToToughness;
                    unit.ToughnessModifecated += '*';
                }
                    
                if (option.AddToWounds > 0)
                {
                    unit.Wounds += option.AddToWounds;
                    unit.WoundsModifecated += '*';
                }

                if (option.AddToInitiative > 0)
                {
                    unit.Initiative += option.AddToInitiative;
                    unit.InitiativeModifecated += '*';
                }

                if (option.AddToAttacks > 0)
                {
                    unit.Attacks += option.AddToAttacks;
                    unit.AttacksModifecated += '*';
                }

                if (option.AddToLeadership > 0)
                {
                    unit.Leadership += option.AddToLeadership;
                    unit.LeadershipModifecated += '*';
                }

                if (option.AddToArmour > 0)
                {
                    unit.Armour += option.AddToArmour;
                    unit.ArmourModifecated += '*';
                }
                    
                if (option.AddToWard > 0)
                {
                    unit.Ward += option.AddToWard;
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

        public string GetFullDescription()
        {
            string description = String.Empty;

            if (GetSpecialRules().Count > 0)
            {
                description += "СПЕЦИАЛЬНЫЕ ПРАВИЛА:\n";

                foreach (string rule in GetSpecialRules())
                    description += rule + "\n";

                description += "\n";
            }

            if (Option.Count > 0)
            {
                description += "СНАРЯЖЕНИЕ:\n";

                foreach (Option option in Option)
                    if (option.IsMagicItem())
                        description += option.Name + "\n";

                description += "\n";
            }

            if (Option.Count > 0)
            {
                description += "ОПЦИИ:\n";

                foreach (Option option in Option)
                    if (option.IsOption())
                        description += option.Name + (option.Realised ? " (удалить)" : " (добавить)") + "\n";

                description += "\n";
            }

            return description;
        }

        public void AddAmmunition(string id)
        {
            Option.Add(ArmyBook.Artefact[id].Clone());
        }

        public string GetSpecialRulesLine()
        {
            string rules = String.Empty;

            foreach (string rule in GetSpecialRules())
                rules += rule + "; ";

            if (!String.IsNullOrEmpty(rules))
                rules = rules.Remove(rules.Length - 2);

            return rules;
        }

        public List<string> GetSpecialRules()
        {
            List<string> rules = new List<string>();

            if (ImmuneToPsychology)
                rules.Add("иммунен к психологии");

            if (Stubborn)
                rules.Add("упорность;");

            if (Hate)
                rules.Add("ненависть");

            if (Fear)
                rules.Add("страх");

            if (Terror)
                rules.Add("ужас");

            if (Frenzy)
                rules.Add("бешенство");

            if (Unbreakable)
                rules.Add("несломимость");

            if (ColdBlooded)
                rules.Add("хладнокровие");

            if (HitFirst)
                rules.Add("всегда бьёт первым");

            if (Regeneration)
                rules.Add("регенерация");

            if (KillingBlow)
                rules.Add("смертельный удар");

            if (PoisonAttack)
                rules.Add("ядовитые атаки");

            return rules;
        }

        public bool IsHero()
        {
            return (Type == Unit.UnitType.Lord || Type == Unit.UnitType.Hero ? true : false);
        }
    }
}
