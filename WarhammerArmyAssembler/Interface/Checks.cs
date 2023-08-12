using System;

namespace WarhammerArmyAssembler.Interface
{
    class Checks
    {
        public static bool EnoughPointsForAddUnit(int id)
        {
            Unit unit = ArmyBook.Data.Units[id];
            double points = unit.StaticPoints + (unit.Size * unit.Points);
            int maxPoints = Army.Params.GetArmyMaxPoints();
            double armyPoint = Army.Params.GetArmyPoints();

            return points <= (maxPoints - armyPoint);
        }

        public static bool EnoughUnitPointsForAddOption(double points) =>
            points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughPointsForAddArtefact(int id, double pointsPenalty)
        {
            double artefactPoints = ArmyBook.Data.Artefact[id].Points - pointsPenalty;
            double armyPoints = Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints();

            return artefactPoints <= armyPoints;
        }

        public static bool EnoughUnitPointsForAddArtefact(int artefactID,
            Unit unit, bool addOption = true, double pointsPenalty = 0)
        {
            if (!ArmyBook.Data.Artefact.ContainsKey(artefactID))
                return true;

            bool isPowers = ArmyBook.Data.Artefact[artefactID].IsPowers();

            double newArtefactPoints = ArmyBook.Data.Artefact[artefactID].Points;

            bool enoughUnitPoints;

            if (isPowers && (unit.MagicPowersCount > 0))
            {
                double addPoints = addOption ? unit.MagicPowersCountAlreadyUsed() : 0;
                enoughUnitPoints = addPoints < unit.GetMagicPowersCount();
            }
            else if (isPowers)
            {
                double magicPoints = unit.MagicPowersPointsAlreadyUsed();
                double alreadyUsedPoints = addOption ? magicPoints + newArtefactPoints : 0;
                enoughUnitPoints = alreadyUsedPoints < unit.GetUnitMagicPowersPoints();
            }
            else if (unit.MagicItemCount > 0)
            {
                enoughUnitPoints = unit.MagicItemSlotsAlreadyUsed() < unit.MagicItemCount;
            }
            else
            {
                double alreadyUsedPoints = (addOption ? unit.MagicPointsAlreadyUsed() : 0);
                double points = newArtefactPoints - pointsPenalty + alreadyUsedPoints;
                enoughUnitPoints = points <= unit.GetUnitMagicPoints();
            }

            bool enoughOptionsPoints = ArmyBook.Data.Artefact[artefactID].IsUsableByUnit(unit, addOption);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            Unit unit = Army.Data.Units[id];
            double newPrice = unit.StaticPoints + (newSize * unit.Points);
            double currentPrice = unit.StaticPoints + (unit.Size * unit.Points);
            double actualPoints = Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints();

            return (newPrice - currentPrice) <= actualPoints;
        }
    }
}
