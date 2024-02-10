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
                ID = "CheckNumberOfCharacter",
                Name = "Check the number of characters",
                Group = "ARMY CHECKS",
                Type = Setting.Types.checkbox,
                Default = "True"
            },
        };

        public static List<Setting> List() =>
            new List<Setting>(listOfSettings);
    }
}
