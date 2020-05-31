﻿using System;
using System.Collections.Generic;
using System.IO;

namespace WarhammerArmyAssembler
{
    class ExportTXT
    {
        public static string SaveArmy()
        {
            string fileName = ExportOther.GetFileName("txt");

            Add(fileName, String.Format("{0} // {1}", Army.ArmyName, ExportOther.GetArmyName()));
            Add(fileName, String.Format("{0} pts", Army.MaxPoints));
            Add(fileName);

            List<Unit> armyByCategories = ArmyParams.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
                foreach (Unit unit in unitType.Items)
                {
                    if (unit.Type == Unit.UnitType.Mount)
                        continue;

                    string equipmentLine = unit.GetEquipmentLine();

                    Add(fileName, String.Format("{0}{1} ({2} pts){3}{4}",
                        ExportOther.UnitSizeIfNeed(unit), unit.Name, unit.GetUnitPoints(), 
                        (String.IsNullOrEmpty(equipmentLine) ? String.Empty : ": "),
                        equipmentLine)
                    );
                }

            Add(fileName);

            Add(fileName,
                String.Format(
                    "Points: {0} / Models: {1} / Cast: {2} / Dispell: {3}",
                    ArmyParams.GetArmyPoints(), ArmyParams.GetArmySize(), ArmyParams.GetArmyCast(), ArmyParams.GetArmyDispell()
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
