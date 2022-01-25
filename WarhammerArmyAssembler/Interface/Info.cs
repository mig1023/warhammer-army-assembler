﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Interface
{
    class Info
    {
        public static string ArmyPoints()
        {
            Dictionary<Unit.UnitType, double> units = Army.Checks.UnitsPointsPercent();

            double armyCurrentPoint = Army.Params.GetArmyPoints();
            double availablePoints = (Army.Params.GetArmyMaxPoints() - armyCurrentPoint);

            string headLine = "All points:\t\t{0} pts\n\nAlready used:\t\t{1} pts / {2}%\n\nAvailable:\t\t{3} pts / {4}%";
            int currentPercent = Services.CalcPercent(armyCurrentPoint, Army.Params.GetArmyMaxPoints());
            int availablePercent = Services.CalcPercent(availablePoints, Army.Params.GetArmyMaxPoints());

            string pointsMsg = String.Format(headLine + "\n\n\n\n", Army.Params.GetArmyMaxPoints(),
                armyCurrentPoint, currentPercent, availablePoints, availablePercent);

            foreach (KeyValuePair<Unit.UnitType, double> entry in Army.Checks.UnitsMaxPointsPercent())
            {
                int percent = Interface.Services.CalcPercent(units[entry.Key], Army.Data.MaxPoints);
                string minMax = (entry.Key == Unit.UnitType.Core ? "min" : "max");
                int maxPoints = (int)(Army.Data.MaxPoints * entry.Value);

                pointsMsg += String.Format("{0}:\t{1,10} pts / {2}%\t( {3} {4} pts / {5}% )\n\n",
                    entry.Key, units[entry.Key], percent, minMax, maxPoints, entry.Value * 100);
            }

            return pointsMsg;
        }

        public static string ArmyUnits() => String.Format("CORE UNITS:\n{0}\n\nSPECIAL UNITS:\n{1}\n\nRARE UNITS:\n{2}",
            UnitsByType(Unit.UnitType.Core), UnitsByType(Unit.UnitType.Special), UnitsByType(Unit.UnitType.Rare));

        public static string ArmyHeroes() => String.Format("LORDS:\n{0}\n\nHEROES:\n{1}",
            UnitsByType(Unit.UnitType.Lord), UnitsByType(Unit.UnitType.Hero));

        public static string ArmyModels()
        {
            int normal = UnitsByBase(Army.Params.BasesTypes.normal);
            int cavalry = UnitsByBase(Army.Params.BasesTypes.cavalry);
            int large = UnitsByBase(Army.Params.BasesTypes.large);
            int chariot = UnitsByBase(Army.Params.BasesTypes.chariot);

            return String.Format("Normal base:\t{0}\n\nCavalry base:\t{1}\n\nLarge base:\t{2}\n\nChariots:\t\t{3}",
                normal, cavalry, large, chariot);
        }

        public static string ArmyCast()
        {
            if (ArmyBook.Data.Magic.Count == 0)
                return "There is no traditional spell's magic model.";

            int max = ArmyBook.Data.Magic.Max(x => x.Key.Length);
            var spellList = ArmyBook.Data.Magic.OrderBy(x => x.Value).Select(x => SpellFormat(x.Key, x.Value, max));
            return String.Join("\n\n", spellList);
        }

        private static string SpellFormat(string name, int difficulty, int max)
        {
            int maxTabCount = (int)Math.Floor((double)max / 4); 
            int current = (int)Math.Floor((double)name.Length / 4);
            int diff = (maxTabCount - current);
            name += new String('\t', (diff == 0 ? 1 : diff) + 1);

            return String.Format("{0}{1}+", name, difficulty);
        }

        private static string UnitsByType(Unit.UnitType u) => Army.Params.GetUnitsListByType(u);

        private static int UnitsByBase(Army.Params.BasesTypes u) => Army.Params.GetUnitsNumberByBase(u);
    }
}
