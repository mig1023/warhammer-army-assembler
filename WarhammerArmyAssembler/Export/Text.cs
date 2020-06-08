using System;
using System.Collections.Generic;
using System.IO;

namespace WarhammerArmyAssembler.Export
{
    class Text
    {
        public static string SaveArmy()
        {
            string fileName = Export.Other.GetFileName("txt");

            Add(fileName, Export.Other.AllArmyName());
            Add(fileName, Export.Other.AllArmyPointsAndEdition());
            Add(fileName);

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
                foreach (Unit unit in unitType.Items)
                {
                    if (unit.Type == Unit.UnitType.Mount)
                        continue;

                    string equipmentLine = unit.GetEquipmentLine();

                    Add(fileName, String.Format("{0}{1}{2}{3}{4}",
                        Export.Other.UnitSizeIfNeed(unit), unit.Name, Export.Other.UnitPointsLine(unit), 
                        (String.IsNullOrEmpty(equipmentLine) ? String.Empty : ": "),
                        equipmentLine)
                    );
                }

            Add(fileName);

            Add(fileName,
                String.Format(
                    "Points: {0} / Models: {1} / Cast: {2} / Dispell: {3}",
                    Army.Params.GetArmyPoints(), Army.Params.GetArmySize(), Army.Params.GetArmyCast(), Army.Params.GetArmyDispell()
                )
            );

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
