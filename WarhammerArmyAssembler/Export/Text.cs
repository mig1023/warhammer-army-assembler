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

                    Add(fileName, String.Format("{0}{1}{2}{3}{4}",
                        Services.UnitSizeIfNeed(unit), Services.GetUnitName(unit), Services.UnitPointsLine(unit), 
                        (String.IsNullOrEmpty(equipmentLine) ? String.Empty : ": "),
                        equipmentLine));
                }

            Add(fileName);

            double points = Army.Params.GetArmyPoints();
            int size = Army.Params.GetArmySize();
            int cast = Army.Params.GetArmyCast();
            int dispell = Army.Params.GetArmyDispell();

            Add(fileName, String.Format("Points: {0} / Models: {1} / Cast: {2} / Dispell: {3}",
                points, size, cast, dispell));

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
