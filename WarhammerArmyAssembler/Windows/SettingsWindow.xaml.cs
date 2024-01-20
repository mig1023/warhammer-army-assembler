using System.Windows;
using System.Windows.Controls;

namespace WarhammerArmyAssembler
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Setting_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            MessageBox.Show("+" + checkbox.Name);

            Settings.Values.Set(checkbox.Name, "True");
        }

        private void Setting_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            MessageBox.Show("-" + checkbox.Name);

            Settings.Values.Set(checkbox.Name, "False");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            Settings.ConfigFile.Save();
        }
    }
}
