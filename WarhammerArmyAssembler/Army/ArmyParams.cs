using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler
{
    class ArmyParams
    {
        public static double GetArmyPoints()
        {
            double points = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                points += entry.Value.GetUnitPoints();

            return points;
        }

        public static int GetArmySize()
        {
            int size = 0;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
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

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                units[entry.Value.Type] += 1;

                units[Unit.UnitType.Lord] += entry.Value.SlotsOfLords;
                units[Unit.UnitType.Hero] += entry.Value.SlotsOfHero;
                units[Unit.UnitType.Special] += entry.Value.SlotsOfSpecial;
                units[Unit.UnitType.Rare] += entry.Value.SlotsOfRare;
            }

            return units[type];
        }

        public static int GetArmyMaxPoints()
        {
            return Army.MaxPoints;
        }

        public static int GetArmyMaxUnitsNumber(Unit.UnitType type)
        {
            switch (type)
            {
                case Unit.UnitType.Lord:
                    return (Army.MaxPoints < 2000 ? 0 : 1 + ((Army.MaxPoints - 2000) / 1000));
                case Unit.UnitType.Hero:
                    return (Army.MaxPoints < 2000 ? 3 : (Army.MaxPoints / 1000) * 2);
                case Unit.UnitType.Core:
                    return (Army.MaxPoints < 2000 ? 2 : 1 + (Army.MaxPoints / 1000));
                case Unit.UnitType.Special:
                    return (Army.MaxPoints < 2000 ? 3 : 2 + (Army.MaxPoints / 1000));
                case Unit.UnitType.Rare:
                    return (Army.MaxPoints < 2000 ? 1 : (Army.MaxPoints / 1000));
                default:
                    return 0;
            }
        }

        public static int GetArmyCast()
        {
            int cast = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                cast += entry.Value.Wizard;

                foreach (Option option in entry.Value.Options)
                    if (option.IsActual())
                        cast += option.AddToCast;
            }

            return cast;
        }

        public static int GetArmyDispell()
        {
            int dispell = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                if (entry.Value.Wizard > 2)
                    dispell += 2;
                else if (entry.Value.Wizard > 0)
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

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                if (entry.Value.Type != Unit.UnitType.Mount)
                    categories[(int)entry.Value.Type].Items.Add(ReloadArmyUnit(entry.Key, entry.Value));

                if (
                    (entry.Value.MountOn > 0)
                    &&
                        (
                            (Army.Units[entry.Value.MountOn].Wounds != 1)
                            ||
                            (Army.Units[entry.Value.MountOn].WeaponTeam)
                        )
                    )
                    categories[(int)entry.Value.Type].Items.Add(
                        ReloadArmyUnit(entry.Value.MountOn, Army.Units[entry.Value.MountOn])
                    );
            }

            return categories;
        }

        public static Unit ReloadArmyUnit(int id, Unit unit)
        {
            Unit newUnit = unit.Clone().GetOptionRules();

            newUnit.RulesView = newUnit.GetSpecialRulesLine();
            newUnit.PointsView = newUnit.GetUnitPoints().ToString();
            newUnit.ID = id;

            return newUnit;
        }

        public static List<Unit> GetArmyCategories()
        {
            return new List<Unit>
            {
                new Unit() { Name = "Lords" },
                new Unit() { Name = "Heroes" },
                new Unit() { Name = "Core" },
                new Unit() { Name = "Special" },
                new Unit() { Name = "Rare" },
            };
        }

        public static Unit GetArmyGeneral()
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.ArmyGeneral)
                    return entry.Value;

            return null;
        }
    }
}
