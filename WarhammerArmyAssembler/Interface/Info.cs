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

            return $"Normal base:\t{normal}\n\nCavalry base:\t{cavalry}\n\n" +
                $"Large base:\t{large}\n\nChariots:\t\t{chariot}";
        }

        private static string SpellDispell(int value) =>
            CastingProbability(value, Army.Params.GetArmyDispell());

        public static string ArmyDispell()
        {
            if (ArmyBook.Data.Dispell.Count == 0)
                return String.Empty;

            string dispells = String.Empty;
            int count = 1;

            foreach (string dispell in ArmyBook.Data.Dispell.Keys)
            {
                List<string> dispellLine = dispell
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList();

                string lore = dispellLine[1];
                string enemy = dispellLine[0];

                var spellList = ArmyBook.Data.Dispell[dispell].
                    OrderBy(x => x.Value).
                    Select(x => $"~ {x.Value}+\t{x.Key} ({SpellDispell(x.Value)} dispells)");

                dispells += SpellLine(spellList, count, $"{lore.ToUpper()}\nby {enemy}",
                    ArmyBook.Data.Dispell.Count);

                count += 1;
            }
            
            int dispScrolls = Army.Params.GetArmyDispellScroll();
            string footer = dispScrolls > 0 ? $"\n+ {dispScrolls} Dispell Scrolls\n\n\n" : String.Empty;

            return $"{dispells}{footer}";
        }

        private static string SpellCast(int value) =>
            CastingProbability(value, Army.Params.GetArmyCast());

        public static string ArmyCast()
        {
            if ((ArmyBook.Data.Magic.Count == 0) || (ArmyBook.Data.Magic.First().Value.Count == 0))
                return "There is no traditional spell's magic model.";

            string spells = String.Empty;
            int count = 1;

            foreach (string magic in ArmyBook.Data.Magic.Keys)
            {
                string loreName = magic.ToUpper();

                var spellList = ArmyBook.Data.Magic[magic].
                    OrderBy(x => x.Value).
                    Select(x => $"{x.Value}+\t{x.Key} ({SpellCast(x.Value)} spells)");

                spells += SpellLine(spellList, count,
                    loreName, ArmyBook.Data.Magic.Count);

                count += 1;
            }

            return spells;
        }

        private static string SpellLine(IEnumerable<string> spellList,
            int count, string loreName, int magicsCount)
        {
            bool largeMagic = magicsCount > 1;
            string newLines = largeMagic ? "\n" : "\n\n";

            string spell = String.Join(newLines, spellList);
            string separator = count > 1 ? "\n\n" : String.Empty;

            return $"{separator}{loreName}\n{newLines}{spell}{newLines}";
        }

        private static string CastingProbability(int difficulty, int cast)
        {
            double possible = cast * Services.DICE_SIZE;

            if (possible < difficulty)
            {
                return "no";
            }
            else
            {
                int spellsMin = Floor((cast * Services.DICE_HALF), difficulty);
                int spellsMax = Floor(possible, difficulty);

                if (spellsMax > cast)
                {
                    spellsMax = cast;
                }

                if ((spellsMax > spellsMin) && (spellsMin > 0))
                {
                    return $"~{spellsMin}-{spellsMax}";
                }
                else
                {
                    return $"~{spellsMax}";
                }
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
