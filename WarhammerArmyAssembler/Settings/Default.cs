using System.Collections.Generic;

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
                ID = "DogsWarTest",
                Name = "Choose type of Dogs of war:",
                Group = "ARMY CHECKS",
                Type = Setting.Types.combobox,
                Options = "Core, Special, Rare",
                Default = "Rare"
            },
            new Setting
            {
                ID = "PathTest",
                Name = "Test path for anything....",
                Group = "SECTION 2",
                Type = Setting.Types.input,
                Default = "Path"
            },
                        new Setting
            {
                ID = "CheckNumberOfCore",
                Name = "Check the number of Core units",
                Group = "SECTION 2",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
            new Setting
            {
                ID = "DogsWarTest",
                Name = "Choose type of Dogs of war:",
               Group = "SECTION 2",
                Type = Setting.Types.combobox,
                Options = "Core, Special, Rare",
                Default = "Rare"
            },
            new Setting
            {
                ID = "PathTest",
                Name = "Test path for anything....",
                Group = "SECTION 2",
                Type = Setting.Types.input,
                Default = "Path"
            },
        };

        public static List<Setting> List() =>
            new List<Setting>(listOfSettings);
    }
}
