using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler
{
    class ArmyMod
    {
        public static int GetNextIndex()
        {
            return Army.MaxIDindex += 1;
        }

        public static int AddUnitByID(int id)
        {
            Unit unit = ArmyBook.Units[id].Clone();

            unit.ArmyID = GetNextIndex();

            Army.Units.Add(unit.ArmyID, unit);

            if (!String.IsNullOrEmpty(unit.MountInit))
            {
                foreach (KeyValuePair<int, Unit> mount in ArmyBook.Mounts)
                    if (mount.Value.Name == unit.MountInit)
                    {
                        Unit newMount = mount.Value.Clone();

                        int newMountID = GetNextIndex();
                        Army.Units[unit.ArmyID].MountOn = newMountID;
                        newMount.ArmyID = newMountID;
                        Army.Units.Add(newMountID, newMount);
                    }
            }

            return unit.ArmyID;
        }

        public static void AddMountByID(int id, int unit)
        {
            Unit mount = ArmyBook.Mounts[id].Clone();

            int newID = GetNextIndex();
            Army.Units[unit].MountOn = newID;
            Army.Units.Add(newID, mount);
        }

        public static void DeleteAllUnits()
        {
            Army.Units.Clear();
        }

        public static void DeleteUnitByID(int id)
        {
            int? removeUnitAlso = null;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                if (entry.Value.MountOn == id)
                {
                    foreach (Option option in entry.Value.Options)
                        if (option.Name == Army.Units[id].Name)
                            option.Realised = false;

                    entry.Value.MountOn = 0;

                    if (!String.IsNullOrEmpty(entry.Value.MountInit))
                        removeUnitAlso = entry.Key;
                }

            foreach (Option option in Army.Units[id].Options)
                if (option.IsMagicItem())
                    InterfaceMod.SetArtefactAlreadyUsed(option.ID, false);

            if (removeUnitAlso != null)
                Army.Units.Remove((int)removeUnitAlso);

            Army.Units.Remove(id);

            ChangeGeneralIfNeed();
        }

        public static void ChangeGeneralIfNeed()
        {
            int maxLeadership = 0;
            int maxLeadershipOwner = -1;

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                if (entry.Value.ArmyGeneral)
                    entry.Value.ArmyGeneral = false;

                int unitLeadership = entry.Value.Leadership;

                foreach (Option option in entry.Value.Options)
                    if (option.IsActual() && (option.AddToLeadership > 0))
                        unitLeadership += option.AddToLeadership;

                bool newChallenger = !entry.Value.NotALeader && (unitLeadership > maxLeadership);

                if (entry.Value.IsHero() && (newChallenger || entry.Value.MustBeGeneral))
                {
                    maxLeadership = unitLeadership;
                    maxLeadershipOwner = entry.Key;
                }

                if (entry.Value.MustBeGeneral)
                    break;
            }

            if (maxLeadershipOwner < 0)
                return;

            Army.Units[maxLeadershipOwner].ArmyGeneral = true;

            bool newGeneralIsDemon = (Army.Units[maxLeadershipOwner].Group == "Demonic");

            if (ArmyBook.DemonicMortal && newGeneralIsDemon && !ArmyBook.DemonicAlreadyReplaced)
                ChangeCoreSpecialUnits();
            else if (ArmyBook.DemonicMortal && !newGeneralIsDemon && ArmyBook.DemonicAlreadyReplaced)
                ChangeCoreSpecialUnits();

            InterfaceReload.ReloadArmyData();
        }

        private static Unit.UnitType ChangeUnitType(Unit.UnitType unitType)
        {
            if (unitType == Unit.UnitType.Core)
                unitType = Unit.UnitType.ToSpecial;

            else if (unitType == Unit.UnitType.Special)
                unitType = Unit.UnitType.ToCore;

            else if (unitType == Unit.UnitType.ToCore)
                unitType = Unit.UnitType.Core;

            else if (unitType == Unit.UnitType.ToSpecial)
                unitType = Unit.UnitType.Special;

            return unitType;
        }

        private static void ChangeCoreSpecialUnits()
        {
            ArmyBook.DemonicAlreadyReplaced = !ArmyBook.DemonicAlreadyReplaced;

            foreach (int i in new List<int> { 1, 2 })
            {
                foreach (KeyValuePair<int, Unit> entry in Army.Units)
                    entry.Value.Type = ChangeUnitType(entry.Value.Type);

                foreach (KeyValuePair<int, Unit> entry in ArmyBook.Units)
                    entry.Value.Type = ChangeUnitType(entry.Value.Type);
            }
        }
    }
}
