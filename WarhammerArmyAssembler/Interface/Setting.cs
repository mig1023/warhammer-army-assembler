using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WarhammerArmyAssembler.Interface
{
    internal class Setting
    {
        private static List<UIElement> controls = new List<UIElement>();

        private static bool IsSettingCheckboxTrue(string name,
            Dictionary<string, string> settings, string defaultValue)
        {
            if (settings.ContainsKey(name))
                return settings[name] == "True";

            return defaultValue == "True";
        }

        private static void CleanSettings()
        {
            foreach (UIElement control in controls)
                Changes.settingsWindow.SettingsPanel.Children.Remove(control);

            controls.Clear();
        }

        private static void Setting_CheckedChange(string name, bool isChecked) =>
            Settings.Values.Set(name, isChecked ? "True" : "False");

        private static UIElement CreateCheckBox(Settings.Setting setting,
            Dictionary<string, string> settings)
        {
            CheckBox checkbox = new CheckBox
            {
                Name = setting.ID,
                Content = setting.Name,
                IsChecked = IsSettingCheckboxTrue(setting.ID, settings, setting.Default),
            };

            checkbox.Checked += (sender, args) => Setting_CheckedChange(setting.ID, true);
            checkbox.Unchecked += (sender, args) => Setting_CheckedChange(setting.ID, false);

            return checkbox;
        }

        private static string ChosenElement(string name,
            Dictionary<string, string> settings, string defaultValue) =>
                settings.ContainsKey(name) ? settings[name] : defaultValue;

        private static void Setting_Change(string name, ComboBox comboBox) =>
            Settings.Values.Set(name, comboBox.SelectedValue.ToString());

        private static UIElement CreateCombobox(Settings.Setting setting,
            Dictionary<string, string> settings)
        {
            Label label = new Label
            {
                Content = setting.Name,
            };

            ComboBox comboBox = new ComboBox
            {
                Name = setting.ID,
                ItemsSource = setting.Options
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList(),
                SelectedValue = ChosenElement(setting.ID, settings, setting.Default),
            };

            comboBox.SelectionChanged += (sender, args) => Setting_Change(setting.ID, comboBox);

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            panel.Children.Add(label);
            panel.Children.Add(comboBox);

            return panel;
        }

        public static void ShowSettingsWindow()
        {
            CleanSettings();

            Dictionary<string, string> settings = Settings.Values.All();

            foreach (Settings.Setting setting in Settings.Default.List())
            {
                UIElement control = null;

                if (setting.Type == Settings.Setting.Types.checkbox)
                {
                    control = CreateCheckBox(setting, settings);
                }
                else if (setting.Type == Settings.Setting.Types.combobox)
                {
                    control = CreateCombobox(setting, settings);
                }
                // else

                Border border = new Border
                {
                    Padding = new Thickness(25,10,0,0),
                    Child = control,
                };

                controls.Add(border);
                Changes.settingsWindow.SettingsPanel.Children.Add(border);
            }

            Changes.settingsWindow.CloseSettings.Background = ArmyBook.Data.FrontColor;
            Changes.settingsWindow.Show();

            Changes.Move(Changes.MovingType.ToMain, menu: true);
        }
    }
}
