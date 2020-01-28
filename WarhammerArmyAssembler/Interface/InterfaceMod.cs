using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WarhammerArmyAssembler
{
    class InterfaceMod
    {
        public delegate void ShowSomething();

        public static void SetArmyGridAltColor(Brush color)
        {
            Interface.main.ArmyGrid.AlternatingRowBackground = color;
        }

        public static void SetArtefactAlreadyUsed(int id, bool value)
        {
            foreach (Object group in Interface.main.ArmyList.Items)
            {
                if (group is Option)
                    foreach (Object item in (group as Option).Items)
                    {
                        Option artefact = item as Option;

                        if (artefact.ID == id)
                        {
                            artefact.ArtefactAlreadyUsed = value;
                            ArmyBook.Artefact[artefact.ID].ArtefactAlreadyUsed = value;
                        }
                    }
            }
        }

        public static void ShowError()
        {
            Interface.main.errorDetail.Visibility = Visibility.Visible;
        }

        public static void ShowMainMenu()
        {
            Interface.main.mainMenu.Visibility = Visibility.Visible;
        }

        public static void ShowArmybookMenu()
        {
            Interface.main.armybookDetailScrollHead.Visibility = Visibility.Visible;
            //Interface.main.menuArmybookScroll.Visibility = Visibility.Visible;
        }

        public static void ShowArmyDetailMenu()
        {
            Interface.main.armybookDetailScrollHead.Visibility = Visibility.Visible;
            Interface.main.armybookDetailScroll.Visibility = Visibility.Visible;
        }

        public static void HideAllAndShow(ShowSomething showSomething)
        {
            //Interface.main.armybookDetailScroll.Visibility = Visibility.Hidden;
            ////Interface.main.menuArmybookScroll.Visibility = Visibility.Hidden;
            //Interface.main.armybookDetailScrollHead.Visibility = Visibility.Hidden;
            Interface.main.mainMenu.Visibility = Visibility.Hidden;
            //Interface.main.errorDetail.Visibility = Visibility.Hidden;

            showSomething?.Invoke();
        }
    }
}
