using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class Army
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        private static int MaxPoints = 2000;

        private static int MaxIDindex = 0;

        public static int GetNextIndex()
        {
            return MaxIDindex += 1;
        }

        public static void AddUnitByID(int id)
        {
            Unit unit = ArmyBook.Units[id].Clone();

            int newUnitID = GetNextIndex();

            Units.Add(newUnitID, unit);

            if (!String.IsNullOrEmpty(unit.MountInit))
            {
                foreach(KeyValuePair<int, Unit> mount in ArmyBook.Mounts)
                    if (mount.Value.Name == unit.MountInit)
                    {
                        int newMountID = GetNextIndex();
                        Units[newUnitID].MountOn = newMountID;
                        Units.Add(newMountID, mount.Value.Clone());
                    }
            }
        }

        public static void AddMountByID(int id, int unit)
        {
            Unit mount = ArmyBook.Mounts[id].Clone();

            int newID = GetNextIndex();
            Army.Units[unit].MountOn = newID;
            Units.Add(newID, mount);
        }

        public static void DeleteAllUnits()
        {
            for (int i = (Units.Count - 1); i >= 0 ; i--) 
                DeleteUnitByID(Units.ElementAt(i).Key, onlyDirectlyHim: true);
        }

        public static void DeleteUnitByID(int id, bool onlyDirectlyHim = false)
        {
            int? removeUnitAlso = null;

            if (!onlyDirectlyHim)
                foreach (KeyValuePair<int, Unit> entry in Army.Units)
                    if (entry.Value.MountOn == id)
                    {
                        foreach (Option option in entry.Value.Options)
                            if (option.Name == Units[id].Name)
                                option.Realised = false;

                        entry.Value.MountOn = 0;

                        if (!String.IsNullOrEmpty(entry.Value.MountInit))
                            removeUnitAlso = entry.Key;
                    }

            foreach (Option option in Army.Units[id].Options)
                if (option.IsMagicItem())
                    Interface.SetArtefactAlreadyUsed(option.ID, false);

            if (removeUnitAlso != null)
                Units.Remove((int)removeUnitAlso);

            Units.Remove(id);
        }

        public static int GetArmyPoints()
        {
            int points = 0;

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

            foreach(Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
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
            return MaxPoints;
        }

        public static int GetArmyMaxUnitsNumber(Unit.UnitType type)
        {
            switch(type)
            {
                case Unit.UnitType.Lord:
                    return (MaxPoints < 2000 ? 0 : 1 + ((MaxPoints - 2000) / 1000));
                case Unit.UnitType.Hero:
                    return (MaxPoints < 2000 ? 3 : (MaxPoints / 1000) * 2);
                case Unit.UnitType.Core:
                    return (MaxPoints < 2000 ? 2 : 1 + (MaxPoints / 1000));
                case Unit.UnitType.Special:
                    return (MaxPoints < 2000 ? 3 : 2 + (MaxPoints / 1000));
                case Unit.UnitType.Rare:
                    return (MaxPoints < 2000 ? 1 : (MaxPoints / 1000));
                default:
                    return 0;
            }
        }

        public static int GetArmyCast()
        {
            int cast = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                cast += entry.Value.Mage;

                foreach (Option option in entry.Value.Options)
                    if (!option.IsOption() || (option.IsOption() && option.Realised))
                        cast += option.AddToCast;
            }
                
            return cast;
        }

        public static int GetArmyDispell()
        {
            int dispell = 2;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                if (entry.Value.Mage > 2)
                    dispell += 2;
                else if (entry.Value.Mage > 0)
                    dispell += 1;

                foreach (Option option in entry.Value.Options)
                    if (!option.IsOption() || (option.IsOption() && option.Realised))
                        dispell += option.AddToDispell;
            }

            return dispell;
        }

        public static int OptionAlreadyUsed(int id)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                foreach (Option option in entry.Value.Options)
                    if ((option.ID == id) && option.Realised)
                        return entry.Key;

            return 0;
        }

        public static int GetMountOption(Unit unit)
        {
            foreach (KeyValuePair<int, Unit> armyUnit in Army.Units)
                if (armyUnit.Key == unit.MountOn)
                    foreach (Option option in unit.Options)
                        if (option.Name == armyUnit.Value.Name)
                            return option.ID;

            return 0;
        }

        public static string UnitTypeName(Unit.UnitType type)
        {
            if (type == Unit.UnitType.Lord)
                return "лордов";
            else if (type == Unit.UnitType.Hero)
                return "героев";
            else if (type == Unit.UnitType.Core)
                return "основных подразделений";
            else if (type == Unit.UnitType.Special)
                return "специальных подразделений";
            else if (type == Unit.UnitType.Rare)
                return "редких подразделений";

            return String.Empty;
        }

        public static bool IsArmyUnitsPointsPercentOk(Unit.UnitType type, int points)
        {
            Dictionary<Unit.UnitType, int> units = new Dictionary<Unit.UnitType, int>();

            foreach (Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                units[entry.Value.Type] += entry.Value.GetUnitPoints();

            int twentyFivePercent = (int)(MaxPoints * 0.25);

            if (type == Unit.UnitType.Lord || type == Unit.UnitType.Hero || type == Unit.UnitType.Rare)
                return (units[type] + points > twentyFivePercent ? false : true);

            if (type == Unit.UnitType.Special)
                return (units[type] + points > (twentyFivePercent * 2) ? false : true);

            return true;
        }

        public static bool IsArmyDublicationOk(Unit unit)
        {
            int alreadyInArmy = 0;

            foreach (KeyValuePair<int, Unit> armyUnit in Army.Units)
                if (armyUnit.Value.ID == unit.ID)
                    alreadyInArmy += 1;

            int limitForArmy = -1;

            if (unit.Type == Unit.UnitType.Special)
                limitForArmy = (Army.MaxPoints >= 3000 ? 6 : 3);
            else if (unit.Type == Unit.UnitType.Rare)
                limitForArmy = (Army.MaxPoints >= 3000 ? 4 : 2);

            return (limitForArmy < 0 ? true : (alreadyInArmy < limitForArmy));
        }
    }
}
