using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Interface
{
    class Mod
    {
        public delegate void ShowSomething();

        public static void SetArmyGridAltColor(Brush color) => Changes.main.ArmyGrid.AlternatingRowBackground = color;

        public static void SetArtefactAlreadyUsed(int id, bool value)
        {
            foreach (Object group in Changes.main.ArmyList.Items)
            {
                if (!(group is Option))
                    continue;

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
            Changes.unitTestIsOpen = false;
            Changes.main.UpdateLayout();
            Changes.main.mainGrid.Width = Changes.main.ActualWidth;
        }

        public static void View(Canvas canvasToShow = null, bool top = false, bool left = false, bool right = false)
        {
            if (top)
            {
                string canvasName = (canvasToShow == null ? String.Empty : canvasToShow.Name);

                List<Canvas> canvases = new List<Canvas>
                {
                    Changes.main.mainMenu,
                    Changes.main.errorDetail,
                };

                foreach (Canvas canvas in canvases)
                    canvas.Visibility = (canvasName == canvas.Name ? Visibility.Visible : Visibility.Hidden);
            }

            if (left)
            {
                Changes.main.armybookDetail.Visibility = Visibility.Visible;
                Changes.main.armyUnitTestScroll.Visibility = Visibility.Hidden;
            }

            if (right)
            {
                Changes.unitTestIsOpen = true;
                Changes.main.armybookDetail.Visibility = Visibility.Hidden;
                Changes.main.armyUnitTestScroll.Visibility = Visibility.Visible;
            }
        }
    }
}
