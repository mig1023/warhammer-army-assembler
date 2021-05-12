using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler.Army
{
    class Params
    {
        public enum BasesTypes { normal, large, cavalry };


        public static double GetArmyPoints()
        {
            double points = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
                points += entry.Value.GetUnitPoints();

            return points;
        }

        public static int GetArmySize()
        {
            int size = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                int modelsInPack = entry.Value.ModelsInPack;

                foreach (Option option in entry.Value.Options)
                    if (option.IsOption() && (option.AddToModelsInPack > 0) && option.Realised)
                        modelsInPack += option.AddToModelsInPack;

                if (!((entry.Value.Type == Unit.UnitType.Mount) && (entry.Value.Wounds <= 1)))
                    size += entry.Value.Size * modelsInPack;
            }

            return size;
        }

        public static int GetArmyUnitsNumber(Unit.UnitType type)
        {
            Dictionary<Unit.UnitType, int> units = new Dictionary<Unit.UnitType, int>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                if ((entry.Value.Type != Unit.UnitType.Core) || !entry.Value.NoCoreSlot)
                    units[entry.Value.Type] += 1;

                if (entry.Value.SlotOf != null)
                    foreach (string slot in entry.Value.SlotOf)
                        units[(Unit.UnitType)Enum.Parse(typeof(Unit.UnitType), slot)] += 1;
            }

            return units[type];
        }

        public static int GetArmyMaxPoints() => Army.Data.MaxPoints;

        public static int GetArmyMaxUnitsNumber(Unit.UnitType type)
        {
            switch (type)
            {
                case Unit.UnitType.Lord:
                    return (Army.Data.MaxPoints < 2000 ? 0 : 1 + ((Army.Data.MaxPoints - 2000) / 1000));

                case Unit.UnitType.Hero:
                    return (Army.Data.MaxPoints < 2000 ? 3 : (Army.Data.MaxPoints / 1000) * 2);

                case Unit.UnitType.Core:
                    return (Army.Data.MaxPoints < 2000 ? 2 : 1 + (Army.Data.MaxPoints / 1000));

                case Unit.UnitType.Special:
                    return (Army.Data.MaxPoints < 2000 ? 3 : 2 + (Army.Data.MaxPoints / 1000));

                case Unit.UnitType.Rare:
                    return (Army.Data.MaxPoints < 2000 ? 1 : (Army.Data.MaxPoints / 1000));

                default:
                    return 0;
            }
        }

        public static int GetArmyCast()
        {
            int cast = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                cast += entry.Value.Wizard;

                foreach (Option option in entry.Value.Options)
                {
                    if (option.IsActual())
                        cast += option.AddToCast;

                    if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                        cast += option.Countable.Value;
                }
            }

            return cast;
        }

        public static int GetArmyDispell()
        {
            int dispell = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                int wizard = entry.Value.Wizard;

                foreach (Option option in entry.Value.Options)
                    if ((option.Countable != null) && (option.Countable.ExportToWizardLevel))
                        wizard += option.Countable.Value;

                if (wizard > 2)
                    dispell += 2;

                else if (wizard > 0)
                    dispell += 1;

                foreach (Option option in entry.Value.Options)
                    if (option.IsActual())
                        dispell += option.AddToDispell;
            }

            return dispell;
        }

        public static List<Unit> GetArmyUnitsByCategories()
        {
            List<Unit> categories = GetArmyCategories();

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                if (entry.Value.Type != Unit.UnitType.Mount)
                    categories[(int)entry.Value.Type].Items.Add(ReloadArmyUnit(entry.Key, entry.Value));

                if (entry.Value.MountOn <= 0)
                    continue;

                bool multiWounds = (Army.Data.Units[entry.Value.MountOn].Wounds != 1) || (Army.Data.Units[entry.Value.MountOn].WeaponTeam);

                if (multiWounds)
                    categories[(int)entry.Value.Type].Items.Add(ReloadArmyUnit(entry.Value.MountOn, Army.Data.Units[entry.Value.MountOn]));
            }

            return categories;
        }

        public static Unit ReloadArmyUnit(int id, Unit unit)
        {
            Unit newUnit = unit.Clone().GetOptionRules();

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
        };

        public static Unit GetArmyGeneral()
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
                if (entry.Value.ArmyGeneral)
                    return entry.Value;

            return null;
        }

        public static int GetUnitsNumberByBase(BasesTypes type)
        {
            int number = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
            {
                bool cavalryBase = entry.Value.MountOn > 1 || !String.IsNullOrEmpty(entry.Value.MountInit);

                if (
                    ((type == BasesTypes.large) && entry.Value.LargeBase)
                    ||
                    ((type == BasesTypes.cavalry) && cavalryBase)
                    ||
                    ((type == BasesTypes.normal) && !entry.Value.LargeBase && !cavalryBase)
                )
                    number += entry.Value.Size;
            }

            return number;
        }

        public static string GetUnitsListByType(Unit.UnitType type)
        {
            List<string> units = new List<string>();

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
                if (entry.Value.Type == type)
                    units.Add(entry.Value.Name);

            return (units.Count == 0 ? "empty yet" : String.Join(", ", units));
        }

        public static double GetVirtuePoints(int id, bool nextPricePreview = false)
        {
            int count = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Data.Units)
                foreach (Option option in entry.Value.Options)
                    if (option.Name == ArmyBook.Data.Artefact[id].Name)
                        count += 1;

            if (count == 0)
                return ArmyBook.Data.Artefact[id].VirtueOriginalPoints;
            else
                return ArmyBook.Data.Artefact[id].VirtueOriginalPoints * (count + 1 + (nextPricePreview ? 1 : 0));
        }

        public static string MagicPowersName() => (String.IsNullOrEmpty(Army.Data.MagicPowers) ? "MAGIC POWERS" : Army.Data.MagicPowers.ToUpper());
    }
}
