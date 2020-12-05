using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Interface
{
    class Checks
    {
        public static bool EnoughPointsForAddUnit(int id)
        {
            return (ArmyBook.Data.Units[id].Size * ArmyBook.Data.Units[id].Points) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddOption(double points)
        {
            return points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }

        public static bool EnoughPointsForAddArtefact(int id, double pointsPenalty)
        {
            return (ArmyBook.Data.Artefact[id].Points - pointsPenalty) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, Unit unit, bool addOption = true, double pointsPenalty = 0)
        {
            if (!ArmyBook.Data.Artefact.ContainsKey(artefactID))
                return true;

            bool isPowers = ArmyBook.Data.Artefact[artefactID].IsPowers();

            double alreadyUsedPoints;

            if (!addOption)
                alreadyUsedPoints = 0;
            else if (isPowers && (unit.MagicPowersCount > 0))
                alreadyUsedPoints = unit.MagicPowersCountAlreadyUsed();
            else if (isPowers)
                alreadyUsedPoints = unit.MagicPowersPointsAlreadyUsed();
            else
                alreadyUsedPoints = unit.MagicPointsAlreadyUsed();

            double magicPoints;

            if (isPowers && (unit.MagicPowersCount > 0))
                magicPoints = unit.GetMagicPowersCount();
            else if (isPowers)
                magicPoints = unit.GetUnitMagicPowersPoints();
            else
                magicPoints = unit.GetUnitMagicPoints();

            bool enoughUnitPoints = (((ArmyBook.Data.Artefact[artefactID].Points - pointsPenalty) + alreadyUsedPoints) <= magicPoints);

            if ((isPowers && (unit.MagicPowersCount > 0)) || (!isPowers && (unit.MagicItemCount > 0)))
                enoughUnitPoints = (unit.MagicPointsAlreadyUsed() < unit.GetUnitMagicPoints());

            bool enoughOptionsPoints = ArmyBook.Data.Artefact[artefactID].IsUsableByUnit(unit, addOption);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            double newPrice = newSize * Army.Data.Units[id].Points;
            double currentPrice = Army.Data.Units[id].Size * Army.Data.Units[id].Points;

            return (newPrice - currentPrice) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }
    }
}
