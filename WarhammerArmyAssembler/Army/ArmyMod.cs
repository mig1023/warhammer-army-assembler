﻿using System;
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
            for (int i = (Army.Units.Count - 1); i >= 0; i--)
                DeleteUnitByID(Army.Units.ElementAt(i).Key, onlyDirectlyHim: true);
        }

        public static void DeleteUnitByID(int id, bool onlyDirectlyHim = false)
        {
            int? removeUnitAlso = null;

            if (!onlyDirectlyHim)
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
            InterfaceReload.ReloadArmyData();
        }
    }
}
