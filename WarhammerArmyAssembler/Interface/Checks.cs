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

            double alreadyUsedPonts = (addOption ? unit.MagicPointsAlreadyUsed() : 0);

            bool enoughUnitPoints = ((ArmyBook.Data.Artefact[artefactID].Points + alreadyUsedPonts) <= unit.GetUnitMagicPoints());

            if (unit.MagicItemCount > 0)
                enoughUnitPoints = (unit.MagicPointsAlreadyUsed() < unit.GetUnitMagicPoints());

            bool enoughOptionsPoints = ArmyBook.Data.Artefact[artefactID].IsUsableByUnit(unit);

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
