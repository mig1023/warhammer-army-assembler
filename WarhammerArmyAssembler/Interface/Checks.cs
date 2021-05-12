using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Interface
{
    class Checks
    {
        public static bool EnoughPointsForAddUnit(int id) =>
            (ArmyBook.Data.Units[id].Size * ArmyBook.Data.Units[id].Points) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughUnitPointsForAddOption(double points) => points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughPointsForAddArtefact(int id, double pointsPenalty) => 
            (ArmyBook.Data.Artefact[id].Points - pointsPenalty) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, Unit unit, bool addOption = true, double pointsPenalty = 0)
        {
            if (!ArmyBook.Data.Artefact.ContainsKey(artefactID))
                return true;

            bool isPowers = ArmyBook.Data.Artefact[artefactID].IsPowers();

            bool enoughUnitPoints;

            if (isPowers && (unit.MagicPowersCount > 0))
                enoughUnitPoints = ((addOption ? unit.MagicPowersCountAlreadyUsed() : 0) < unit.GetMagicPowersCount());

            else if (isPowers && (unit.MagicPowersCount > 0))
                enoughUnitPoints = ((addOption ? unit.MagicPowersPointsAlreadyUsed() : 0) < unit.GetUnitMagicPowersPoints());

            else
            {
                double alreadyUsedPoints = (addOption ? unit.MagicPointsAlreadyUsed() : 0);
                double magicPoints = unit.GetUnitMagicPoints();

                enoughUnitPoints = (((ArmyBook.Data.Artefact[artefactID].Points - pointsPenalty) + alreadyUsedPoints) <= magicPoints);
            }

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
