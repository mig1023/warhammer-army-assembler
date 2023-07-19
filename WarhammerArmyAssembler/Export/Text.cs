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
            string fileName = Services.GetFileName("txt");

            Add(fileName, Services.AllArmyName());
            Add(fileName, Services.AllArmyPointsAndEdition());
            Add(fileName);

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
                foreach (Unit unit in unitType.Items.Where(x => x.Type != Unit.UnitType.Mount))
                {
                    string equipmentLine = unit.GetEquipmentLine();
                    string sizeUnit = Services.UnitSizeIfNeed(unit);
                    string nameUnit = Services.GetUnitName(unit);
                    string pointsUnit = Services.UnitPointsLine(unit);
                    string equipment = String.IsNullOrEmpty(equipmentLine) ? String.Empty : ": ";

                    Add(fileName, $"{sizeUnit}{nameUnit}{pointsUnit}{equipment}{equipmentLine}");
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

        public static void Add(string fileName, string line = "")
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
                sw.WriteLine(line);
        }
    }
}
