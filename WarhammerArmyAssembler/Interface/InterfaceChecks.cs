using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler
{
    class InterfaceChecks
    {
        public static bool EnoughPointsForAddUnit(int id)
        {
            return (ArmyBook.Units[id].Size * ArmyBook.Units[id].Points) <= (ArmyParams.GetArmyMaxPoints() - ArmyParams.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddOption(double points)
        {
            return points <= (ArmyParams.GetArmyMaxPoints() - ArmyParams.GetArmyPoints());
        }

        public static bool EnoughPointsForAddArtefact(int id)
        {
            return ArmyBook.Artefact[id].Points <= (ArmyParams.GetArmyMaxPoints() - ArmyParams.GetArmyPoints());
        }

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, Unit unit, bool addOption = true)
        {
            if (!ArmyBook.Artefact.ContainsKey(artefactID))
                return true;

            double alreadyUsedPonts = (addOption ? unit.MagicPointsAlreadyUsed() : 0);

            bool enoughUnitPoints = ((ArmyBook.Artefact[artefactID].Points + alreadyUsedPonts) <= unit.GetUnitMagicPoints());

            if (unit.MagicItemCount > 0)
                enoughUnitPoints = (unit.MagicPointsAlreadyUsed() < unit.GetUnitMagicPoints());

            bool enoughOptionsPoints = ArmyBook.Artefact[artefactID].IsUsableByUnit(unit);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            int newPrice = (newSize * Army.Units[id].Points);
            int currentPrice = (Army.Units[id].Size * Army.Units[id].Points);

            return (newPrice - currentPrice) <= (ArmyParams.GetArmyMaxPoints() - ArmyParams.GetArmyPoints());
        }
    }
}
