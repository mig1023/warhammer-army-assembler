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

            int unitAllMagicPoints = unit.MagicItems;

            foreach (Option option in unit.Options)
                if ((option.IsOption() && option.Realised) || option.IsMagicItem())
                    unitAllMagicPoints += option.MagicItems;

            double alreadyUsedPonts = (addOption ? UnitMagicPointsAlreadyUsed(unit) : 0);

            bool enoughUnitPoints = ((ArmyBook.Artefact[artefactID].Points + alreadyUsedPonts) <= unitAllMagicPoints);
            bool enoughOptionsPoints = ArmyBook.Artefact[artefactID].IsUsableByUnit(unit);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static double UnitMagicPointsAlreadyUsed(Unit unit)
        {
            double pointsAlreayUsed = 0;

            foreach (Option option in unit.Options)
                if (option.IsMagicItem())
                    pointsAlreayUsed += option.Points;

            return pointsAlreayUsed;
        }

        public static bool EnoughPointsForEditUnit(int id, int newSize)
        {
            int newPrice = (newSize * Army.Units[id].Points);
            int currentPrice = (Army.Units[id].Size * Army.Units[id].Points);

            return (newPrice - currentPrice) <= (ArmyParams.GetArmyMaxPoints() - ArmyParams.GetArmyPoints());
        }
    }
}
