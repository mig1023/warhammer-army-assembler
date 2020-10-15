using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class Mod
    {
        public delegate void ShowSomething();

        public static void SetArmyGridAltColor(Brush color)
        {
            Interface.Changes.main.ArmyGrid.AlternatingRowBackground = color;
        }

        public static void SetArtefactAlreadyUsed(int id, bool value)
        {
            foreach (Object group in Interface.Changes.main.ArmyList.Items)
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
            Interface.Changes.unitTestIsOpen = false;
            Interface.Changes.main.UpdateLayout();
            Interface.Changes.main.mainGrid.Width = Interface.Changes.main.ActualWidth;
        }

        public static void View(Canvas canvasToShow = null, bool top = false, bool left = false, bool right = false)
        {
            if (top)
            {
                string canvasName = (canvasToShow == null ? String.Empty : canvasToShow.Name);

                List<Canvas> canvases = new List<Canvas>
                {
                    Interface.Changes.main.mainMenu,
                    Interface.Changes.main.errorDetail,
                };

                foreach (Canvas canvas in canvases)
                    canvas.Visibility = (canvasName == canvas.Name ? Visibility.Visible : Visibility.Hidden);
            }

            if (left)
            {
                Interface.Changes.main.armybookDetail.Visibility = Visibility.Visible;
                Interface.Changes.main.armyUnitTestScroll.Visibility = Visibility.Hidden;
            }

            if (right)
            {
                Interface.Changes.unitTestIsOpen = true;
                Interface.Changes.main.armybookDetail.Visibility = Visibility.Hidden;
                Interface.Changes.main.armyUnitTestScroll.Visibility = Visibility.Visible;
            }
        }
    }
}
