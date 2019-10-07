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
        public enum UnitType { Lord, Hero, Core, Special, Rare }

        public enum MagicItemsTypes { Hero, Mage, Unit }

        public string Name { get; set; }
        public int ID { get; set; }
        public string IDView { get; set; }

        public UnitType Type { get; set; }

        public int Size { get; set; }

        public int Points { get; set; }

        public string PointsView { get; set; }

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
        public int? Armour { get; set; }
        public int? Ward { get; set; }

        public int Mage { get; set; }

        public string MovementView { get; set; }
        public string WeaponSkillView { get; set; }
        public string BallisticSkillView { get; set; }
        public string StrengthView { get; set; }
        public string ToughnessView { get; set; }
        public string WoundsView { get; set; }
        public string InitiativeView { get; set; }
        public string AttacksView { get; set; }
        public string LeadershipView { get; set; }
        public string ArmourView { get; set; }
        public string WardView { get; set; }

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
        public MagicItemsTypes MagicItemsType { get; set; }

        public Brush InterfaceColor { get; set; }
        public bool GroopBold { get; set; }

        public List<Option> Option = new List<Option>();

        public ObservableCollection<Unit> Items { get; set; }

        public string RulesView { get; set; }

        public Unit()
        {
            this.Items = new ObservableCollection<Unit>();
        }

        public int GetUnitPoints()
        {
            int points = Size * Points;

            foreach (Option option in Option)
                if (!option.IsOption() || (option.IsOption() && option.Realised))
                    points += option.Points * (option.PerModel ? Size : 1);

            return points;
        }

        public Unit Clone()
        {
            Unit newUnit = new Unit();

            newUnit.Name = this.Name;
            newUnit.ID = this.ID;
            newUnit.IDView = this.IDView;
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
            newUnit.Mage = this.Mage;

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
            newUnit.MagicItemsType = this.MagicItemsType;

            List <Option> Option = new List<Option>();
            foreach (Option option in this.Option)
                newUnit.Option.Add(option.Clone());

            return newUnit;
        }

        public Unit GetOptionRules()
        {
            Unit unit = this.Clone();

            foreach (Option option in this.Option)
                if (option.IsMagicItem() || (option.IsOption() && option.Realised))
                {
                    if (option.AddToMovement > 0)
                    {
                        unit.MovementView += '*';
                        unit.Movement += option.AddToMovement;

                        if (unit.Movement > 10)
                            unit.Movement = 10;
                    }

                    if (option.AddToWeaponSkill > 0)
                    {
                        unit.WeaponSkillView += '*';
                        unit.WeaponSkill += option.AddToWeaponSkill;

                        if (unit.WeaponSkill > 10)
                            unit.WeaponSkill = 10;
                    }

                    if (option.AddToBallisticSkill > 0)
                    {
                        unit.BallisticSkillView += '*';
                        unit.BallisticSkill += option.AddToBallisticSkill;

                        if (unit.BallisticSkill > 10)
                            unit.BallisticSkill = 10;
                    }

                    if (option.AddToStrength > 0)
                    {
                        unit.StrengthView += '*';
                        unit.Strength += option.AddToStrength;

                        if (unit.Strength > 10)
                            unit.Strength = 10;
                    }
                    
                    if (option.AddToToughness > 0)
                    {
                        unit.ToughnessView += '*';
                        unit.Toughness += option.AddToToughness;

                        if (unit.Toughness > 10)
                            unit.Toughness = 10;
                    }
                    
                    if (option.AddToWounds > 0)
                    {
                        unit.WoundsView += '*';
                        unit.Wounds += option.AddToWounds;

                        if (unit.Wounds > 10)
                            unit.Wounds = 10;
                    }

                    if (option.AddToInitiative > 0)
                    {
                        unit.InitiativeView += '*';
                        unit.Initiative += option.AddToInitiative;

                        if (unit.Initiative > 10)
                            unit.Initiative = 10;
                    }

                    if (option.AddToAttacks > 0)
                    {
                        unit.AttacksView += '*';
                        unit.Attacks += option.AddToAttacks;

                        if (unit.Attacks > 10)
                            unit.Attacks = 10;
                    }

                    if (option.AddToLeadership > 0)
                    {
                        unit.LeadershipView += '*';
                        unit.Leadership += option.AddToLeadership;

                        if (unit.Leadership > 10)
                            unit.Leadership = 10;
                    }

                    if (option.AddToArmour > 0)
                    {
                        if (unit.Armour == null)
                            unit.Armour = 7;

                        unit.Armour -= (7 - option.AddToArmour);
                        unit.ArmourView = "+";
                    }
                    
                    if (option.AddToWard > 0)
                    {
                        if (unit.Ward == null)
                            unit.Ward = 7;

                        unit.Ward -= (7 - option.AddToWard);
                        unit.WardView = "+";
                    }
                }

            unit.MovementView = unit.Movement.ToString() + unit.MovementView;
            unit.WeaponSkillView = unit.WeaponSkill.ToString() + unit.WeaponSkillView;
            unit.BallisticSkillView = unit.BallisticSkill.ToString() + unit.BallisticSkillView;
            unit.StrengthView = unit.Strength.ToString() + unit.StrengthView;
            unit.ToughnessView = unit.Toughness.ToString() + unit.ToughnessView;
            unit.WoundsView = unit.Wounds.ToString() + unit.WoundsView;
            unit.InitiativeView = unit.Initiative.ToString() + unit.InitiativeView;
            unit.AttacksView = unit.Attacks.ToString() + unit.AttacksView;
            unit.LeadershipView = unit.Leadership.ToString() + unit.LeadershipView;
            unit.ArmourView = unit.Armour.ToString() + unit.ArmourView;
            unit.WardView = unit.Ward.ToString() + unit.WardView;

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

            return description;
        }

        public void AddAmmunition(int id)
        {
            Option.Add(ArmyBook.Artefact[id].Clone());
        }

        public void AddOption(int id, Unit unit)
        {
            foreach (Option option in unit.Option)
                if (option.ID == id)
                {
                    if (option.IsMagicItem())
                        unit.Option.Remove(option);
                    else
                        option.Realised = !option.Realised;

                    if (!String.IsNullOrEmpty(option.MountOn))
                        foreach(KeyValuePair<int, Unit> mount in ArmyBook.Mounts)
                            if (mount.Value.Name == option.Name)
                                Interface.ArmyGridDrop(mount.Key, points: option.Points);

                    return;
                }
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

        public bool ExistsOptions()
        {
            foreach (Option option in Option)
                if (option.IsOption())
                    return true;

            return false;
        }

        public bool ExistsMagicItems()
        {
            foreach (Option option in Option)
                if (option.IsMagicItem() && (option.Points > 0))
                    return true;

            return false;
        }

        public bool ExistsOrdinaryItems()
        {
            foreach (Option option in Option)
                if (option.IsMagicItem() && (option.Points == 0))
                    return true;

            return false;
        }
    }
}
