using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                            ArmyBook.Data.Artefact[artefact.ID].ArtefactAlreadyUsed = value;
                        }
                    }
            }
        }

        public static void UnitTestClose()
        {
            Interface.unitTestIsOpen = false;
            Interface.main.UpdateLayout();
            Interface.main.mainGrid.Width = Interface.main.ActualWidth;
        }

        public static void View(Canvas canvasToShow, bool left = false, bool right = false)
        {
            string canvasName = (canvasToShow == null ? String.Empty : canvasToShow.Name);

            List<Canvas> canvases = new List<Canvas>
            {
                Interface.main.mainMenu,
                Interface.main.errorDetail,
            };

            foreach (Canvas canvas in canvases)
                canvas.Visibility = (canvasName == canvas.Name ? Visibility.Visible : Visibility.Hidden);

            if (left)
            {
                Interface.main.armybookDetail.Visibility = Visibility.Visible;
                Interface.main.armyUnitTestScroll.Visibility = Visibility.Hidden;
            }

            if (right)
            {
                Interface.unitTestIsOpen = true;
                Interface.main.armybookDetail.Visibility = Visibility.Hidden;
                Interface.main.armyUnitTestScroll.Visibility = Visibility.Visible;
            }
        }
    }
}
