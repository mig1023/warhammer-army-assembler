using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Army
{
    class Params
    {
        public enum BasesTypes { normal, large, cavalry, chariot };

        public static double GetArmyPoints() => Data.Units.Values.Sum(x => x.GetUnitPoints());

        public static int GetArmySize()
        {
            int size = 0;

            foreach (Unit entry in Data.Units.Values)
            {
                int modelsInPack = entry.ModelsInPack;

                foreach (Option option in entry.Options)
                {
                    if (option.IsOption() && (option.AddToModelsInPack > 0) && option.Realised)
                        modelsInPack += option.AddToModelsInPack;

                    if ((option.Countable != null) && (option.Countable.ExportToUnitSize))
                        size += option.Countable.Value;
                }

                if (!((entry.Type == Unit.UnitType.Mount) && (entry.Wounds.Value <= 1)))
                    size += entry.Size * modelsInPack;
            }

            return size;
        }

        public static int GetArmyUnitsNumber(Unit.UnitType type)
        {
            Dictionary<Unit.UnitType, int> units = new Dictionary<Unit.UnitType, int>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (Unit entry in Data.Units.Values)
            {
                if ((entry.Type != Unit.UnitType.Core) || !entry.NoCoreSlot)
                    units[entry.Type] += 1;

                if (entry.Slots != null)
                {
                    foreach (string slot in entry.Slots)
                        units[(Unit.UnitType)Enum.Parse(typeof(Unit.UnitType), slot)] += 1;
                }
            }

            return units[type];
        }

        public static int GetArmyMaxPoints() => Data.MaxPoints;

        public static int GetArmyMaxUnitsNumber(Unit.UnitType type)
        {
            switch (type)
            {
                case Unit.UnitType.Lord:
                    return (Data.MaxPoints < ArmyBook.Constants.BaseArmySize ? 0 : 1 + ((Data.MaxPoints - 2000) / 1000));

                case Unit.UnitType.Hero:
                    return (Data.MaxPoints < ArmyBook.Constants.BaseArmySize ? 3 : (Data.MaxPoints / 1000) * 2);

                case Unit.UnitType.Core:
                    return (Data.MaxPoints < ArmyBook.Constants.BaseArmySize ? 2 : 1 + (Data.MaxPoints / 1000));

                case Unit.UnitType.Special:
                    return (Data.MaxPoints < ArmyBook.Constants.BaseArmySize ? 3 : 2 + (Data.MaxPoints / 1000));

                case Unit.UnitType.Rare:
                    return (Data.MaxPoints < ArmyBook.Constants.BaseArmySize ? 1 : (Data.MaxPoints / 1000));

                default:
                    return 0;
            }
        }

        public static int GetArmyCast()
        {
            int cast = 2;

            foreach (Unit entry in Data.Units.Values)
            {
                cast += entry.Wizard;

                foreach (Option option in entry.Options)
                {
                    if (option.IsActual())
                        cast += option.AddToCast;

                    if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                        cast += option.GetWizardLevelBonus();
                }
            }

            return cast;
        }

        public static int GetArmyDispell()
        {
            int dispell = 2;

            foreach (Unit entry in Data.Units.Values)
            {
                int wizard = entry.Wizard;

                foreach (Option option in entry.Options)
                    if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                        wizard += option.GetWizardLevelBonus();

                if (wizard > 2)
                    dispell += 2;

                else if (wizard > 0)
                    dispell += 1;

                foreach (Option option in entry.Options)
                    if (option.IsActual())
                        dispell += option.AddToDispell;
            }

            return dispell;
        }

        public static List<Unit> GetArmyUnitsByCategories()
        {
            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in Data.Units)
            {
                if (entry.Value.Type != Unit.UnitType.Mount)
                    categories[(int)entry.Value.Type].Items.Add(ReloadArmyUnit(entry.Key, entry.Value));

                if (entry.Value.MountOn <= 0)
                    continue;

                bool multiWounds = (Data.Units[entry.Value.MountOn].Wounds.Value != 1) ||
                    (Data.Units[entry.Value.MountOn].WeaponTeam);

                if (multiWounds)
                    categories[(int)entry.Value.Type].Items.Add(
                        ReloadArmyUnit(entry.Value.MountOn, Data.Units[entry.Value.MountOn]));
            }

            return categories;
        }

        public static Unit ReloadArmyUnit(int id, Unit unit)
        {
            Unit newUnit = unit.Clone().GetOptionRules(hasMods: out _);

            newUnit.RulesView = newUnit.GetSpecialRulesLine(withCommandData: true);
            newUnit.PointsView = newUnit.GetUnitPoints().ToString();
            newUnit.ID = id;

            return newUnit;
        }

        public static List<Unit> GetArmyCategories() => new List<Unit>
        {
            new Unit() { Name = "Lords" },
            new Unit() { Name = "Heroes" },
            new Unit() { Name = "Core" },
            new Unit() { Name = "Special" },
            new Unit() { Name = "Rare" },
            new Unit() { Name = "Dogs of War" },
        };

        public static Unit GetArmyGeneral() => Data.Units.Values.Where(x => x.CurrentGeneral).FirstOrDefault();

        public static int GetUnitsNumberByBase(BasesTypes type)
        {
            int number = 0;

            foreach (Unit entry in Data.Units.Values)
            {
                if (entry.Type == Unit.UnitType.Mount)
                    continue;

                bool cavalry = (entry.MountOn > 1 || !String.IsNullOrEmpty(entry.MountInit));
                bool chariot = entry.Chariot > 0;

                bool chariotBase = (type == BasesTypes.chariot) && chariot;
                bool largeBase = (type == BasesTypes.large) && entry.LargeBase;
                bool cavalryBase = (type == BasesTypes.cavalry) && cavalry && !chariot;
                bool normalBase = (type == BasesTypes.normal) && !entry.LargeBase && !cavalry && !chariot;

                if (largeBase || cavalryBase || normalBase || chariotBase)
                    number += entry.Size;
            }

            return number;
        }

        public static string GetUnitsListByType(Unit.UnitType type)
        {
            List<string> units = new List<string>();

            foreach (Unit entry in Data.Units.Values.Where(x => x.Type == type).GroupBy(x => x.Name).Select(x => x.First()))
            {
                int count = Data.Units.Values.Count(x => x.Name == entry.Name);

                if (count > 1)
                    units.Add($"{entry.Name} x {count}");
                else
                    units.Add(entry.Name);
            }
                

            return (units.Count == 0 ? "empty yet" : String.Join("\n", units));
        }

        public static double GetVirtuePoints(int id, bool nextPricePreview = false)
        {
            int count = Data.Units.Values.Sum(x => x.Options.Where(y => y.Name == ArmyBook.Data.Artefact[id].Name).Count());

            if (count == 0)
                return ArmyBook.Data.Artefact[id].VirtueOriginalPoints;
            else
                return ArmyBook.Data.Artefact[id].VirtueOriginalPoints * (count + 1 + (nextPricePreview ? 1 : 0));
        }

        public static int GetArmyDispellScroll() =>
            Data.Units.Values.Sum(x => x.Options.Where(y => y.Name == "Dispell Scroll").Count());
    }
}
