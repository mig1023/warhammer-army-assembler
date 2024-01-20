using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            Settings.Values.Set(checkbox.Name, "True");
        }

        private void Setting_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            Settings.Values.Set(checkbox.Name, "False");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            Settings.ConfigFile.Save();
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) =>
            this.Close();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (this.Visibility == Visibility.Visible)
                this.DragMove();
        }
    }
}
