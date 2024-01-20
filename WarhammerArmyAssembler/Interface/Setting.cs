using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WarhammerArmyAssembler.Interface
{
    internal class Setting
    {
        private static List<Control> controls = new List<Control>();

        private static bool IsSettingCheckboxTrue(string name,
            Dictionary<string, string> settings, string defaultValue)
        {
            if (settings.ContainsKey(name))
                return settings[name] == "True";

            return defaultValue == "True";
        }

        private static void CleanSettings()
        {
            foreach (Control control in controls)
                Changes.settingsWindow.SettingsPanel.Children.Remove(control);

            controls.Clear();
        }

        private static void Setting_CheckedChange(string name, bool isChecked) =>
            Settings.Values.Set(name, isChecked ? "True" : "False");

        public static void ShowSettingsWindow()
        {
            CleanSettings();

            Dictionary<string, string> settings = Settings.Values.All();

            foreach (Settings.Setting setting in Settings.Default.List())
            {
                CheckBox checkbox = new CheckBox
                {
                    Name = setting.ID,
                    Content = setting.Name,
                    IsChecked = IsSettingCheckboxTrue(setting.ID, settings, setting.Default),
                };

                checkbox.Checked += (sender, args) => Setting_CheckedChange(setting.ID, true);
                checkbox.Unchecked += (sender, args) => Setting_CheckedChange(setting.ID, false);

                controls.Add(checkbox);
                Changes.settingsWindow.SettingsPanel.Children.Add(checkbox);
            }

            Changes.settingsWindow.CloseSettings.Background = ArmyBook.Data.FrontColor;
            Changes.settingsWindow.Show();

            Changes.Move(Changes.MovingType.ToMain, menu: true);
        }
    }
}
