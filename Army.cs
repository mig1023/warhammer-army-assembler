﻿using System;
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

            Units.Add(GetNextIndex(), unit);
        }

        public static void AddMountByID(int id, int points, int unit)
        {
            Unit mount = ArmyBook.Mounts[id].Clone();
            mount.Points = points;

            int newID = GetNextIndex();
            Army.Units[unit].MountOn = newID;
            Units.Add(newID, mount);
        }

        public static void DeleteUnitByID(int id)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.MountOn == id)
                {
                    foreach (Option option in entry.Value.Option)
                        if (option.Name == Units[id].Name)
                            option.Realised = false;

                    entry.Value.MountOn = 0;
                }

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
                size += entry.Value.Size;

            return size;
        }

        public static int GetArmyUnitsNumber(Unit.UnitType type)
        {
            Dictionary<Unit.UnitType, int> units = new Dictionary<Unit.UnitType, int>();

            foreach(Unit.UnitType u in Enum.GetValues(typeof(Unit.UnitType)))
                units.Add(u, 0);

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                units[entry.Value.Type] += 1;

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

                foreach (Option option in entry.Value.Option)
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

                foreach (Option option in entry.Value.Option)
                    if (!option.IsOption() || (option.IsOption() && option.Realised))
                        dispell += option.AddToDispell;
            }

            return dispell;
        }

        public static int OptionAlreadyUsed(int id)
        {
            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                foreach (Option option in entry.Value.Option)
                    if ((option.ID == id) && option.Realised)
                        return entry.Key;

            return 0;
        }
    }
}
