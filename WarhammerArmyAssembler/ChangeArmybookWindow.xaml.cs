using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WarhammerArmyAssembler
{
    public partial class ChangeArmybookWindow : Window
    {
        public static bool sortedByEditions = false;

        private Point? PointesTumblerOffset;
        private double PointesTumblerRotate = 0;

        public ChangeArmybookWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e) =>
            Environment.Exit(0);

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

        private void pointesTumbler_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            PointesTumblerOffset = e.GetPosition(tumbler);
        }

        private void pointesTumbler_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            double x = e.GetPosition(sender as UIElement).X - PointesTumblerOffset.Value.X;

            PointesTumblerRotate += 0.1 * (x > 0 ? 1 : -1);

            if (PointesTumblerRotate < -170)
            {
                PointesTumblerRotate = -170;
            }
            else if (PointesTumblerRotate > 170)
            {
                PointesTumblerRotate = 170;
            }
            else
            {
                tumbler.RenderTransform = new RotateTransform(PointesTumblerRotate, 75, 75);
            }

            int points = Interface.Services.PointsCalculator(PointesTumblerRotate);
            listArmybookPoints.Text = points.ToString();
        }

        private void buttonArmybook_Click(object sender, RoutedEventArgs e) =>
            StartArmybookOption(listArmybookPoints.Text);

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (Interface.Changes.changeArmybook.Visibility == Visibility.Visible)
                this.DragMove();
        }

        private void closeArmybook_Click(object sender, MouseButtonEventArgs e) =>
            Environment.Exit(0);

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

        private void ChangeTumblerAngle(int points) =>
            tumbler.RenderTransform = new RotateTransform(Interface.Services.AngleCalculator(points), 75, 75);

        private void listArmybookPoints_KeyUp(object sender, KeyEventArgs e)
        {
            bool success = int.TryParse(listArmybookPoints.Text, out int points);

            if (success)
                ChangeTumblerAngle(points);
        }

        private void mark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string points = (sender as Label).Name.Replace("mark", String.Empty);
            listArmybookPoints.Text = points;
            ChangeTumblerAngle(int.Parse(points));
        }
    }
}
