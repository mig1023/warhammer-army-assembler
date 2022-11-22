﻿using System;

namespace WarhammerArmyAssembler.Interface
{
    class Checks
    {
        public static bool EnoughPointsForAddUnit(int id)
        {
            Unit unit = ArmyBook.Data.Units[id];
            double points = unit.StaticPoints + (unit.Size * unit.Points);

            return points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddOption(double points) =>
            points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughPointsForAddArtefact(int id, double pointsPenalty) => 
            (ArmyBook.Data.Artefact[id].Points - pointsPenalty) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, Unit unit, bool addOption = true, double pointsPenalty = 0)
        {
            if (!ArmyBook.Data.Artefact.ContainsKey(artefactID))
                return true;

            bool isPowers = ArmyBook.Data.Artefact[artefactID].IsPowers();

            double newArtefactPoints = ArmyBook.Data.Artefact[artefactID].Points;

            bool enoughUnitPoints;

            if (isPowers && (unit.MagicPowersCount > 0))
            {
                enoughUnitPoints = (addOption ? unit.MagicPowersCountAlreadyUsed() : 0) < unit.GetMagicPowersCount();
            }
            else if (isPowers)
            {
                double alreadyUsedPoints = (addOption ? unit.MagicPowersPointsAlreadyUsed() + newArtefactPoints : 0);
                enoughUnitPoints = alreadyUsedPoints < unit.GetUnitMagicPowersPoints();
            }
            else if (unit.MagicItemCount > 0)
            {
                enoughUnitPoints = unit.MagicItemSlotsAlreadyUsed() < unit.MagicItemCount;
            }
            else
            {
                double alreadyUsedPoints = (addOption ? unit.MagicPointsAlreadyUsed() : 0);
                enoughUnitPoints = (newArtefactPoints - pointsPenalty + alreadyUsedPoints) <= unit.GetUnitMagicPoints();
            }

            bool enoughOptionsPoints = ArmyBook.Data.Artefact[artefactID].IsUsableByUnit(unit, addOption);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            Unit unit = Army.Data.Units[id];
            double newPrice = unit.StaticPoints + (newSize * unit.Points);
            double currentPrice = unit.StaticPoints + (unit.Size * unit.Points);

            return (newPrice - currentPrice) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }
    }
}
