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

        private void Window_Closed(object sender, EventArgs e) => Environment.Exit(0);

        private void changeArmybook_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            armybookCanvas.Height = e.NewSize.Height;
            armybookCanvas.Width = e.NewSize.Width;

            menuArmybookScroll.Height = e.NewSize.Height - 14;

            gridCloseArmybook.Height = e.NewSize.Height;
            gridCloseArmybook.Width = 30;

            armybookList.Width = armybookCanvas.Width - 330;

            armybookListScroll.SetValue(Canvas.LeftProperty, menuArmybookPlace.Width + 41);

            armybookListScroll.Height = armybookCanvas.Height;
            armybookListScroll.Width = armybookCanvas.Width - 360;
        }

        private void prev_Click(object sender, RoutedEventArgs e) => Interface.Changes.PreviewArmyList(prev: true);

        private void next_Click(object sender, RoutedEventArgs e) => Interface.Changes.PreviewArmyList(next: true);

        private void StartArmybook(int points)
        {
            Interface.Reload.LoadArmySize(points, armyAdditionalName.Text);

            Interface.Changes.main.armyVersionLabel_PositionCorrect();

            this.Hide();

            Interface.Changes.main.Show();
        }

        private void StartArmybookOption(string armySize)
        {
            bool successParse = int.TryParse(armySize, out int size);

            if (successParse)
                StartArmybook(size);
            else
                MessageBox.Show("Wrong army points!");
        }

        public void buttonPoints_Click(object sender, RoutedEventArgs e) => StartArmybookOption((sender as Label).Content.ToString().Split()[0]);

        private void buttonArmybook_Click(object sender, RoutedEventArgs e) => StartArmybookOption(listArmybookPoints.Text);

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void closeArmybook_Click(object sender, MouseButtonEventArgs e) => Environment.Exit(0);

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Interface.Changes.PreviewArmyList(prev: true);
            else
                Interface.Changes.PreviewArmyList(next: true);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                Interface.Changes.PreviewArmyList(prev: true);
            else if (e.Key == Key.Right)
                Interface.Changes.PreviewArmyList(next: true);
        }

        private void showArmyAdditionalName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            showArmyAdditionalName.Visibility = Visibility.Hidden;
            armyAdditionalName.Visibility = Visibility.Visible;
        }
    }
}
