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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            Settings.Storage.Save();
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TestScroll.Width = e.NewSize.Width - 29;
        }
    }
}
