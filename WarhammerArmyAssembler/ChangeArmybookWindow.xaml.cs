using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarhammerArmyAssembler
{
    public partial class ChangeArmybookWindow : Window
    {
        public static bool sortedByEditions = false;

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
            menuArmybookPlace.Height = menuArmybookScroll.Height;

            gridCloseArmybook.Height = e.NewSize.Height;
            gridCloseArmybook.Width = 30;

            armybookList.Width = armybookCanvas.Width - 330;

            armybookListScroll.SetValue(Canvas.LeftProperty, menuArmybookPlace.Width + 41);

            armybookListScroll.Height = armybookCanvas.Height;
            armybookListScroll.Width = armybookCanvas.Width - 360;
        }

        private void prev_Click(object sender, RoutedEventArgs e) =>
            Interface.Changes.PreviewArmyList(prev: true, reset: true);

        private void next_Click(object sender, RoutedEventArgs e) =>
            Interface.Changes.PreviewArmyList(next: true, reset: true);

        private void StartArmybook(int points)
        {
            Interface.Reload.LoadArmySize(points, armyAdditionalName.Text);
            Interface.Changes.main.armyEditionLabel_PositionCorrect();

            this.Hide();

            Interface.Changes.main.Show();
        }

        private void StartArmybookOption(string armySize)
        {
            if (int.TryParse(armySize, out int size))
                StartArmybook(size);
            else
                MessageBox.Show("Wrong army points!");
        }

        public void buttonPoints_Click(object sender, RoutedEventArgs e) =>
            StartArmybookOption((sender as Label).Content.ToString().Split()[0]);

        private void buttonArmybook_Click(object sender, RoutedEventArgs e) =>
            StartArmybookOption(listArmybookPoints.Text);

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (Interface.Changes.changeArmybook.Visibility == Visibility.Visible)
                this.DragMove();
        }

        private void closeArmybook_Click(object sender, MouseButtonEventArgs e) => Environment.Exit(0);

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

        private void randomArmy_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.RandomArmy();

        private void resetSelection_MouseDown(object sender, MouseButtonEventArgs e) =>
            Interface.Changes.SetArmySelected(String.Empty);

        private void sortedBy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sortedByEditions = !sortedByEditions;

            sortedBy.Content = "Sorted by army ";

            if (sortedByEditions)
            {
                sortedBy.Content += "edition";
                sortedBy.Margin = Interface.Changes.Thick(sortedBy, left: -3);
            }
            else
            {
                sortedBy.Content += "name";
                sortedBy.Margin = Interface.Changes.Thick(sortedBy, left: 0);
            }

            Interface.Changes.LoadAllArmy(ArmyBook.XmlBook.FindAllXmlFiles(AppDomain.CurrentDomain.BaseDirectory), reload: true);
            Interface.Changes.PreviewArmyList();
        }

        private void armySize_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (listArmybookPoints == null)
                return;

            Dictionary<int, int> armybookPoints = new Dictionary<int, int>
            {
                [1] = 200,
                [2] = 500,
                [3] = 600,
                [4] = 750,
                [5] = 1000,
                [6] = 1250,
                [7] = 1500,
                [8] = 1750,
                [9] = 1850,
                [10] = 2000,
                [11] = 2250,
                [12] = 2400,
                [13] = 2500,
                [14] = 2700,
                [15] = 3000,
                [16] = 0,
            };

            listArmybookPoints.Text = armybookPoints[(int)e.NewValue].ToString();
        }
    }
}
