using System;
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

        public static string ArmyDispell()
        {
            if (ArmyBook.Data.Dispell.Count == 0)
                return String.Empty;

            var spellList = ArmyBook.Data.Dispell.
                OrderBy(x => x.Value).
                Select(x => String.Format("~ {0}+\t{1} ({2} dispells)",
                    x.Value, x.Key, CastingProbability(x.Value, Army.Params.GetArmyDispell())));

            string lore = ArmyBook.Data.EnemyMagicLoreName;
            string enemy = ArmyBook.Data.EnemyMagicName;

            int dispScrolls = Army.Params.GetArmyDispellScroll();
            string footer = (dispScrolls > 0 ? String.Format("\n\n\n+ {0} Dispell Scrolls", dispScrolls) : String.Empty);

            return String.Format("ENEMY MAGIC:\n\n{0}\nby {1}\n\n\n{2}{3}", lore, enemy, String.Join("\n\n", spellList), footer);
        }

        public static string ArmyCast()
        {
            if (ArmyBook.Data.Magic.Count == 0)
                return "There is no traditional spell's magic model.";

            var spellList = ArmyBook.Data.Magic.
                OrderBy(x => x.Value).
                Select(x => String.Format("{0}+\t{1} ({2} spells)",
                x.Value, x.Key, CastingProbability(x.Value, Army.Params.GetArmyCast())));

            string loreName = ArmyBook.Data.MagicLoreName.ToUpper();
            string footer = String.Empty;
                
            if (!String.IsNullOrEmpty(ArmyBook.Data.MagicAlternative))
                footer = String.Format("\n\n\nAlternative:\n{0}", ArmyBook.Data.MagicAlternative);

            return String.Format("{0}\n\n\n{1}{2}", loreName, String.Join("\n\n", spellList), footer);
        }

        private static string CastingProbability(int difficulty, int cast)
        {
            double possible = cast * Services.DICE_SIZE;

            if (possible < difficulty)
                return "no";
            else
            {
                int spellsMin = Floor((cast * Services.DICE_HALF), difficulty);
                int spellsMax = Floor(possible, difficulty);

                if (spellsMax > cast)
                    spellsMax = cast;

                if ((spellsMax > spellsMin) && (spellsMin > 0))
                    return String.Format("~{0}-{1}", spellsMin, spellsMax);
                else
                    return String.Format("~{0}", spellsMax);
            }
        }

        private static int Floor(double level, int difficulty) => (int)Math.Floor(level / difficulty);

        private static string UnitsByType(Unit.UnitType u) => Army.Params.GetUnitsListByType(u);

        private static int UnitsByBase(Army.Params.BasesTypes u) => Army.Params.GetUnitsNumberByBase(u);
    }
}
