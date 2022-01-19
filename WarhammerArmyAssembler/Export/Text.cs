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
            string fileName = Export.Services.GetFileName("txt");

            Add(fileName, Export.Services.AllArmyName());
            Add(fileName, Export.Services.AllArmyPointsAndEdition());
            Add(fileName);

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
                foreach (Unit unit in unitType.Items.Where(x => x.Type != Unit.UnitType.Mount))
                {
                    string equipmentLine = unit.GetEquipmentLine();

                    Add(fileName, String.Format("{0}{1}{2}{3}{4}",
                        Export.Services.UnitSizeIfNeed(unit), unit.Name, Export.Services.UnitPointsLine(unit), 
                        (String.IsNullOrEmpty(equipmentLine) ? String.Empty : ": "),
                        equipmentLine));
                }

            Add(fileName);

            Add(fileName, String.Format("Points: {0} / Models: {1} / Cast: {2} / Dispell: {3}",
                Army.Params.GetArmyPoints(), Army.Params.GetArmySize(), Army.Params.GetArmyCast(), Army.Params.GetArmyDispell()));

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
