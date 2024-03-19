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
                },
                new Setting
                {
                    ID = "CheckNumberOfCharacter",
                    Name = "Check the number of characters",
                },
                new Setting
                {
                    ID = "CheckNumberOfUnits",
                    Name = "Check the number of heroes and units by types",
                },
                new Setting
                {
                    ID = "CheckOfPoints",
                    Name = "Check the strict sufficiency of points",
                },
                new Setting
                {
                    ID = "CheckOfRatio",
                    Name = "Check the ratio of unit types",
                },
                new Setting
                {
                    ID = "CheckOfDublication",
                    Name = "Check for invalid duplication of units",
                },
                new Setting
                {
                    ID = "CheckOfSingletons",
                    Name = "Check the uniqueness of units",
                },
            },
            ["DOGS OF WAR"] = new List<Setting>
            {
                new Setting
                {
                    ID = "DogsOfWarEnabled",
                    Name = "Enable units in other armies",
                },
                new Setting
                {
                    ID = "DogsOfWarCharacter",
                    Name = "Enable characters in other armies",
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
                },
                new Setting
                {
                    ID = "ExportPDFSpecialRules",
                    Name = "Unit/Characters Special rules",
                },
                new Setting
                {
                    ID = "ExportPDFModifiedParams",
                    Name = "Modified params",
                },
                new Setting
                {
                    ID = "ExportPDFFooter",
                    Name = "Statistic footer",
                },
            },
            ["EXPORT TO TXT"] = new List<Setting>
            {
                new Setting
                {
                    ID = "ExportTXTInline",
                    Name = "Everything in one line",
                },
                new Setting
                {
                    ID = "ExportTXTWizardLevel",
                    Name = "Wizard's levels",
                    Default = "False",
                },
                new Setting
                {
                    ID = "ExportTXTSpecialRules",
                    Name = "Unit/Characters Special rules",
                    Default = "False",
                },
                new Setting
                {
                    ID = "ExportTXTModifiedParams",
                    Name = "Modified params",
                    Default = "False",
                },
            }
        };

        public static string Get(string name) =>
            listOfSettings.SelectMany(y => y.Value).Where(x => x.ID == name).FirstOrDefault().Default;

        public static Dictionary<string, List<Setting>> All() =>
            new Dictionary<string, List<Setting>>(listOfSettings);
    }
}
