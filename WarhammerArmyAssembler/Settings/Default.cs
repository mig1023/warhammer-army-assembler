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
                    Name = "Enable units in other armies",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "DogsOfWarCharacter",
                    Name = "Enable characters in other armies",
                    Type = Setting.Types.checkbox,
                    Default = "False",
                },
                new Setting
                {
                    ID = "DogsOfWarType",
                    Name = "Unit type of Dogs of War",
                    Type = Setting.Types.combobox,
                    Options = "Core, Special, Rare",
                    Default = "Rare"
                },
            },
            ["EXPORT TO PDF"] = new List<Setting>
            {
                new Setting
                {
                    ID = "ExportPDFWizardLevel",
                    Name = "Wizard's levels",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "ExportPDFSpecialRules",
                    Name = "Unit/Characters Special rules",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "ExportPDFModifiedParams",
                    Name = "Modified params",
                    Type = Setting.Types.checkbox,
                },
                new Setting
                {
                    ID = "ExportPDFFooter",
                    Name = "Statistic footer",
                    Type = Setting.Types.checkbox,
                },
            }
        };

        public static string Get(string name) =>
            listOfSettings.SelectMany(y => y.Value).Where(x => x.ID == name).FirstOrDefault().Default;

        public static Dictionary<string, List<Setting>> All() =>
            new Dictionary<string, List<Setting>>(listOfSettings);
    }
}
