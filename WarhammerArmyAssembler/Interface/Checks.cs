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

        public static bool EnoughPointsForAddArtefact(int id)
        {
            return ArmyBook.Data.Artefact[id].Points <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, Unit unit, bool addOption = true)
        {
            if (!ArmyBook.Data.Artefact.ContainsKey(artefactID))
                return true;

            bool isPowers = ArmyBook.Data.Artefact[artefactID].IsPowers();

            double alreadyUsedPonts;

            if (!addOption)
                alreadyUsedPonts = 0;
            else if (isPowers)
                alreadyUsedPonts = unit.MagicPowersPointsAlreadyUsed();
            else
                alreadyUsedPonts = unit.MagicPointsAlreadyUsed();

            double magicPoints = (isPowers ? unit.GetUnitMagicPowersPoints() : unit.GetUnitMagicPoints());

            bool enoughUnitPoints = ((ArmyBook.Data.Artefact[artefactID].Points + alreadyUsedPonts) <= magicPoints);

            if (!isPowers && (unit.MagicItemCount > 0))
                enoughUnitPoints = (unit.MagicPointsAlreadyUsed() < unit.GetUnitMagicPoints());

            bool enoughOptionsPoints = ArmyBook.Data.Artefact[artefactID].IsUsableByUnit(unit, addOption);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            int newPrice = (newSize * Army.Data.Units[id].Points);
            int currentPrice = (Army.Data.Units[id].Size * Army.Data.Units[id].Points);

            return (newPrice - currentPrice) <= (Army.Params.GetArmyMaxPoints() - Army.Params.GetArmyPoints());
        }
    }
}
