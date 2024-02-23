﻿using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Settings
{
    class Default
    {
        public static List<Setting> listOfSettings = new List<Setting>
        {
            new Setting
            {
                ID = "CheckNumberOfCore",
                Name = "Check the number of Core units",
                Group = "ARMY CHECKS",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
            new Setting
            {
                ID = "CheckNumberOfCharacter",
                Name = "Check the number of characters",
                Group = "ARMY CHECKS",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
            new Setting
            {
                ID = "CheckNumberOfUnits",
                Name = "Check the number of heroes and units by types",
                Group = "ARMY CHECKS",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
            new Setting
            {
                ID = "DogsOfWarEnabled",
                Name = "Enable Dogs of War",
                Group = "DOGS OF WAR",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
            new Setting
            {
                ID = "DogsOfWarType",
                Name = "Unit type of Dogs of War",
                Group = "DOGS OF WAR",
                Type = Setting.Types.combobox,
                Options = "Core, Special, Rare",
                Default = "Rare"
            },
        };

        public static string Get(string name) =>
            listOfSettings.Where(x => x.ID == name).FirstOrDefault().Default;

        public static List<Setting> List() =>
            new List<Setting>(listOfSettings);
    }
}
