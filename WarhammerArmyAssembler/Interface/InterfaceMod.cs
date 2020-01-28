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
                            ArmyBook.Artefact[artefact.ID].ArtefactAlreadyUsed = value;
                        }
                    }
            }
        }

        public static void View(Canvas canvasToShow)
        {
            foreach (Canvas canvas in new List<Canvas> { Interface.main.mainMenu, Interface.main.errorDetail })
                canvas.Visibility = (canvasToShow.Name == canvas.Name ? Visibility.Visible : Visibility.Hidden);
        }
    }
}
