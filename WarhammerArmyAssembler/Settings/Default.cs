using System.Collections.Generic;
using System.Linq;

namespace WarhammerArmyAssembler.Settings
{
    class Default
    {
        private static Dictionary<string, List<Setting>> listOfSettings = new Dictionary<string, List<Setting>>
        {
            ["ARMY CHECKS"] = new List<Setting>
            {
                new Setting
                {
                    ID = "CheckNumberOfCore",
                    Name = "Check the number of Core units",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckNumberOfCharacter",
                    Name = "Check the number of characters",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckNumberOfUnits",
                    Name = "Check the number of heroes and units by types",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckOfPoints",
                    Name = "Check the strict sufficiency of points",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckOfRatio",
                    Name = "Check the ratio of unit types",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckOfDublication",
                    Name = "Check for invalid duplication of units",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "CheckOfSingletons",
                    Name = "Check the uniqueness of units",
                    Type = Setting.Types.checkbox,
                },
            },
            ["DOGS OF WAR"] = new List<Setting>
            {
                new Setting
                {
                    ID = "DogsOfWarEnabled",
                    Name = "Enable Dogs of War",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "DogsOfWarType",
                    Name = "Unit type of Dogs of War",
                    Type = Setting.Types.combobox,
                    Options = "Core, Special, Rare",
                    Default = "Rare"
                },
            }
        };

        public static string Get(string name) =>
            listOfSettings.SelectMany(y => y.Value).Where(x => x.ID == name).FirstOrDefault().Default;

        public static Dictionary<string, List<Setting>> All() =>
            new Dictionary<string, List<Setting>>(listOfSettings);
    }
}
