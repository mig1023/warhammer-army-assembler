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

            checkbox.Checked += (sender, args) => 
                Setting_CheckedChange(setting.ID, true);

            checkbox.Unchecked += (sender, args) =>
                Setting_CheckedChange(setting.ID, false);

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

            return comboBox;
        }

        private static void SettingText_Change(string name, TextBox textBox) =>
            Settings.Values.Set(name, textBox.Text);

        private static UIElement CreateInput(Settings.Setting setting,
            Dictionary<string, string> settings)
        {
            TextBox textBox = new TextBox
            {
                Name = setting.ID,
                Text = ChosenElement(setting.ID, settings, setting.Default),
                Height = 24,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5, 0, 0, 0)
            };

            textBox.TextChanged += (sender, args) => SettingText_Change(setting.ID, textBox);

            return textBox;
        }

        private static UIElement WithBorder(string name, UIElement element)
        {
            Label label = new Label
            {
                Content = $"{name}:",
            };

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            panel.Children.Add(label);
            panel.Children.Add(element);

            return panel;
        }

        private static UIElement GroupHeader(string name) => new Border
        {
            Padding = new Thickness(25,25,0,0),
            Child = new Label
            {
                Content = name,
                FontSize = 20,
                FontWeight = FontWeights.UltraLight,
                Foreground = ArmyBook.Data.FrontColor,
            }
        }; 

        public static void ShowSettingsWindow()
        {
            CleanSettings();

            Dictionary<string, string> values = Settings.Values.All();
            Dictionary<string, List<Settings.Setting>> settings = Settings.Default.All();
            UIElement last = null;

            foreach (string group in settings.Keys)
            {
                UIElement header = GroupHeader(group);
                controls.Add(header);
                DockPanel.SetDock(header, Dock.Top);
                Changes.settingsWindow.SettingsPanel.Children.Add(header);

                foreach (Settings.Setting setting in settings[group])
                {
                    UIElement control = null;

                    if (setting.Type == Settings.Setting.Types.checkbox)
                    {
                        control = CreateCheckBox(setting, values);
                    }
                    else if (setting.Type == Settings.Setting.Types.combobox)
                    {
                        control = WithBorder(setting.Name, CreateCombobox(setting, values));
                    }
                    else if (setting.Type == Settings.Setting.Types.input)
                    {
                        control = WithBorder(setting.Name, CreateInput(setting, values));
                    }

                    Border border = new Border
                    {
                        Padding = new Thickness(25, 10, 50, 0),
                        Child = control,
                    };

                    controls.Add(border);
                    DockPanel.SetDock(border, Dock.Top);
                    Changes.settingsWindow.SettingsPanel.Children.Add(border);

                    last = border;
                }
            }

            (last as Border).Padding = new Thickness(25, 10, 50, 25);

            Changes.settingsWindow.CloseSettings.Background = ArmyBook.Data.FrontColor;
            Changes.settingsWindow.Show();

            Changes.Move(Changes.MovingType.ToMain, menu: true);
        }
    }
}
