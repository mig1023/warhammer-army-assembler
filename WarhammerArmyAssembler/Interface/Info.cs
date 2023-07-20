using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarhammerArmyAssembler.Interface
{
    class Info
    {
        public static string ArmyPoints()
        {
            Dictionary<Unit.UnitType, double> units = Army.Checks.UnitsPointsPercent();

            double armyCurrentPoint = Army.Params.GetArmyPoints();
            double availablePoints = Army.Params.GetArmyMaxPoints() - armyCurrentPoint;
            int currentPercent = Services.CalcPercent(armyCurrentPoint, Army.Params.GetArmyMaxPoints());
            int availablePercent = Services.CalcPercent(availablePoints, Army.Params.GetArmyMaxPoints());
            int max = Army.Params.GetArmyMaxPoints();

            string pointsMsg = $"All points:\t\t{max} pts\n\nAlready used:\t\t{armyCurrentPoint} pts" +
                $" / {currentPercent}%\n\nAvailable:\t\t{availablePoints} pts / {availablePercent}%\n\n\n\n";

            foreach (KeyValuePair<Unit.UnitType, double> entry in Army.Checks.UnitsMaxPointsPercent())
            {
                int percent = Services.CalcPercent(units[entry.Key], Army.Data.MaxPoints);
                string minMax = (entry.Key == Unit.UnitType.Core ? "min" : "max");
                int maxPoints = (int)(Army.Data.MaxPoints * entry.Value);

                pointsMsg += $"{entry.Key}:\t{units[entry.Key],10} pts " +
                    $"/ {percent}%\t( {minMax} {maxPoints} pts " +
                    $"/ {entry.Value * 100}% )\n{PercentLine(percent)}\n\n";
            }

            return pointsMsg;
        }

        private static string PercentLine(int pecrent)
        {
            int current = (int)Math.Ceiling((double)pecrent / 4.4);
            StringBuilder line = new StringBuilder(new string('▯', 23));

            for (int i = 0; i < current; i++)
                line[i] = '▮';

            return line.ToString();
        }

        public static string ArmyUnits() =>
            $"CORE UNITS:\n{UnitsByType(Unit.UnitType.Core)}\n\n" +
            $"SPECIAL UNITS:\n{UnitsByType(Unit.UnitType.Special)}\n\n" +
            $"RARE UNITS:\n{UnitsByType(Unit.UnitType.Rare)}";

        public static string ArmyHeroes() =>
            $"LORDS:\n{UnitsByType(Unit.UnitType.Lord)}\n\n" +
            $"HEROES:\n{UnitsByType(Unit.UnitType.Hero)}";

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
                
            if (!String.IsNullOrEmpty(ArmyBook.Data.MagicOptions))
                footer = String.Format("\n\n\nAnother magic options:\n{0}", ArmyBook.Data.MagicOptions);

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

        private static int Floor(double level, int difficulty) =>
            (int)Math.Floor(level / difficulty);

        private static string UnitsByType(Unit.UnitType u) =>
            Army.Params.GetUnitsListByType(u);

        private static int UnitsByBase(Army.Params.BasesTypes u) =>
            Army.Params.GetUnitsNumberByBase(u);
    }
}
