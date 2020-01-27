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

            menuArmybookScroll.Height = e.NewSize.Height - 70;

            startHelpInfo.Height = armybookCanvas.Height;
            startHelpInfo.Width = armybookCanvas.Width - 320;
            startHelpInfo.Margin = new Thickness(320, 0, 0, 0);
            startHelpMainText.Width = startHelpInfo.Width - 100;
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(prev: true);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Interface.PreviewArmyList(next: true);
        }

        private void buttonArmybook_Click(object sender, RoutedEventArgs e)
        {
            Interface.startArmybookMenu = false;

            //InterfaceReload.LoadArmySize(InterfaceOther.IntParse(listArmybookPoints.Text));
            //ArmyBookLoad.LoadArmy(Interface.CurrentSelectedArmy);

            //InterfaceReload.LoadArmyList();
            //InterfaceReload.ReloadArmyData();

            //Interface.Move(Interface.MovingType.ToMain);
        }
    }
}
