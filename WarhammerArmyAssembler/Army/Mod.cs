using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Army
{
    class Mod
    {
        public static int GetNextIndex() =>
            Data.MaxIDindex += 1;

        public static int AddUnitByID(int id)
        {
            Unit unit = ArmyBook.Data.Units[id].Clone();

            unit.ArmyID = GetNextIndex();

            Data.Units.Add(unit.ArmyID, unit);

            Unit mount = ArmyBook.Data.Mounts
                .Values
                .Where(x => x.Name == unit.MountInit)
                .FirstOrDefault();

            if (mount == null)
                return unit.ArmyID;

            Unit newMount = mount.Clone();

            if (Data.Units[unit.ArmyID].Chariot > 0)
                newMount.Size = Data.Units[unit.ArmyID].Chariot;

            int newMountID = GetNextIndex();
            Data.Units[unit.ArmyID].MountOn = newMountID;
            newMount.ArmyID = newMountID;
            Data.Units.Add(newMountID, newMount);

            return unit.ArmyID;
        }

        public static void AddMountByID(int id, int unit)
        {
            Unit mount = ArmyBook.Data.Mounts[id].Clone();

            int newID = GetNextIndex();
            Data.Units[unit].MountOn = newID;
            Data.Units.Add(newID, mount);
        }

        public static void DeleteAllUnits() =>
            Data.Units.Clear();

        public static void DeleteUnitByID(int id)
        {
            int? removeUnitAlso = null;

            Dictionary<int, Unit> units = Data.Units
                .Where(x => x.Value.MountOn == id)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<int, Unit> entry in units)
            {
                List<Option> options = entry.Value
                    .Options
                    .Where(x => x.Name == Data.Units[id].Name)
                    .ToList();

                foreach (Option option in options)
                    option.Realised = false;

                entry.Value.MountOn = 0;

                if (!String.IsNullOrEmpty(entry.Value.MountInit))
                    removeUnitAlso = entry.Key;
            }

            foreach (Option option in Data.Units[id].Options.Where(x => x.IsMagicItem()))
                Interface.Mod.SetArtefactAlreadyUsed(option.ID, false);

            if (removeUnitAlso != null)
                Data.Units.Remove((int)removeUnitAlso);

            Data.Units.Remove(id);

            ChangeGeneralIfNeed();
        }

        public static void ChangeGeneralIfNeed()
        {
            int maxLeadership = 0;
            int maxLeadershipOwner = -1;

            foreach (KeyValuePair<int, Unit> entry in Data.Units)
            {
                Unit unit = entry.Value;

                if (unit.CurrentGeneral)
                    unit.CurrentGeneral = false;

                int unitLeadership = unit.Leadership.Value;
                bool notALeader = false;
                bool mustBeGeneral = unit.General;

                foreach (Option option in unit.Options.Where(x => x.IsActual()))
                {
                    if (option.AddToLeadership > 0)
                        unitLeadership += option.AddToLeadership;

                    if (option.NotALeader)
                        notALeader = true;

                    if (option.General)
                        mustBeGeneral = true;
                }

                bool newChallenger = !unit.NotALeader && !notALeader && (unitLeadership > maxLeadership);

                if (mustBeGeneral)
                {
                    maxLeadershipOwner = entry.Key;
                    break;
                }
                else if (unit.IsHero() && (newChallenger || unit.General))
                {
                    maxLeadership = unitLeadership;
                    maxLeadershipOwner = entry.Key;
                }
            }

            if (maxLeadershipOwner >= 0)
            {
                Data.Units[maxLeadershipOwner].CurrentGeneral = true;

                bool newGeneralIsDemon = (Data.Units[maxLeadershipOwner].GetGroup() == "Demonic");
                bool generalIsDaemon = newGeneralIsDemon && !ArmyBook.Data.DemonicAlreadyReplaced;
                bool generalIsNotDaemon = !newGeneralIsDemon && ArmyBook.Data.DemonicAlreadyReplaced;

                if (ArmyBook.Data.DemonicMortal && (generalIsDaemon || generalIsNotDaemon))
                    ChangeCoreSpecialUnits();
            }

            Interface.Reload.ReloadArmyData();
        }

        public static string CategoryNameModification(string category)
        {
            if (!ArmyBook.Data.DemonicMortal)
                return category;

            if (category == "Core")
                return "Mortal";

            if (category == "Special")
                return "Demonic";

            return category;
        }

        private static Unit.UnitType ChangeUnitType(Unit.UnitType unitType)
        {
            Dictionary<Unit.UnitType, Unit.UnitType> types = new Dictionary<Unit.UnitType, Unit.UnitType>
            {
                [Unit.UnitType.Core] = Unit.UnitType.ToSpecial,
                [Unit.UnitType.Special] = Unit.UnitType.ToCore,
                [Unit.UnitType.ToCore] = Unit.UnitType.Core,
                [Unit.UnitType.ToSpecial] = Unit.UnitType.Special,
            };

            return (types.ContainsKey(unitType) ? types[unitType] : unitType);
        }

        private static void ChangeCoreSpecialUnits()
        {
            ArmyBook.Data.DemonicAlreadyReplaced = !ArmyBook.Data.DemonicAlreadyReplaced;

            foreach (int i in new List<int> { 1, 2 })
            {
                foreach (Unit entry in Data.Units.Values)
                    entry.Type = ChangeUnitType(entry.Type);

                foreach (Unit entry in ArmyBook.Data.Units.Values)
                    entry.Type = ChangeUnitType(entry.Type);
            }
        }
    }
}
