using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WarhammerArmyAssembler
{
    public partial class ChangeArmybookWindow : Window
    {
        public ChangeArmybookWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void changeArmybook_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            armybookCanvas.Height = e.NewSize.Height;
            armybookCanvas.Width = e.NewSize.Width;

            menuArmybookScroll.Height = e.NewSize.Height - 14;

            gridCloseArmybook.Height = e.NewSize.Height - 10;
            gridCloseArmybook.Width = 30;

            startHelpInfo.Height = armybookCanvas.Height;
            startHelpInfo.Width = armybookCanvas.Width - 360;
            startHelpInfo.Margin = new Thickness(360, 0, 0, 0);
            startHelpMainText.Width = startHelpInfo.Width - 100;

            imageArmybookBack.Height = startHelpInfo.Height;
            imageArmybookBack.Width = startHelpInfo.Width;
            imageArmybookBack.Margin = startHelpInfo.Margin;
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(prev: true);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(next: true);
        }

        private void StartArmybook(int points)
        {
            InterfaceReload.LoadArmySize(points);

            this.Hide();

            Interface.main.Show();
        }

        public void buttonPoints_Click(object sender, RoutedEventArgs e)
        {
            StartArmybook(InterfaceOther.IntParse((sender as Label).Content.ToString().Split()[0]));
        }

        private void buttonArmybook_Click(object sender, RoutedEventArgs e)
        {
            StartArmybook(InterfaceOther.IntParse(listArmybookPoints.Text));
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void closeArmybook_Click(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
