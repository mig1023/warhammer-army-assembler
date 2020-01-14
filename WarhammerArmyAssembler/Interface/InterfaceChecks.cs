﻿using System;
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

        public static bool EnoughUnitPointsForAddArtefact(int artefactID, int unitID)
        {
            int unitAllMagicPoints = Army.Units[unitID].MagicItems;

            foreach (Option option in Army.Units[unitID].Options)
                if ((option.IsOption() && option.Realised) || option.IsMagicItem())
                    unitAllMagicPoints += option.MagicItems;

            bool enoughUnitPoints = ((ArmyBook.Artefact[artefactID].Points + UnitMagicPointsAlreadyUsed(unitID)) <= unitAllMagicPoints);
            bool enoughOptionsPoints = ArmyBook.Artefact[artefactID].IsUsableByUnit(Army.Units[unitID]);

            return (enoughUnitPoints && enoughOptionsPoints);
        }

        public static double UnitMagicPointsAlreadyUsed(int unitID)
        {
            double pointsAlreayUsed = 0;

            foreach (Option option in Army.Units[unitID].Options)
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
