using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WarhammerArmyAssembler.Export
{
    class Text
    {
        public static string SaveArmy()
        {
            string fileName = File.GetName("txt");

            Add(fileName, Lines.AllArmyName());
            Add(fileName, Lines.AllArmyPointsAndEdition());
            Add(fileName);

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
            {
                List<Unit> units = unitType.Items
                    .Where(x => x.Type != Unit.UnitType.Mount)
                    .ToList();

                foreach (Unit unit in units)
                {
                    string sizeUnit = Lines.UnitSizeIfNeed(unit);
                    string nameUnit = Lines.GetUnitName(unit);
                    string pointsUnit = Lines.UnitPointsLine(unit);
                    string equipment = Line(unit.GetEquipmentLine()); 
                        
                    Add(fileName, $"{sizeUnit}{nameUnit}{pointsUnit}{Line(equipment, end: true)}");
                }
            }

            Add(fileName);

            double points = Army.Params.GetArmyPoints();
            int size = Army.Params.GetArmySize();
            int cast = Army.Params.GetArmyCast();
            int dispell = Army.Params.GetArmyDispell();

            Add(fileName, $"Points: {points} / Models: {size} / Cast: {cast} / Dispell: {dispell}");

            System.Diagnostics.Process.Start(fileName);

            return String.Empty;
        }

        private static string Line(string line, bool end = false)
        {
            if (String.IsNullOrEmpty(line))
            {
                return line;
            }
            else if (Settings.Values.IsTrue("ExportTXTInline"))
            {
                return end ? $": {line}" : $"{line};";
            }
            else
            {
                return end ? $"\n{line}" : $"\t{line}";
            }
        }

        private static void Add(string fileName, string line = "")
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
                sw.WriteLine(line);
        }
    }
}
