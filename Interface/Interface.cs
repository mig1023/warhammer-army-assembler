using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.Interface
{
    class Interface
    {
        public static MainWindow main = null;

        public static void LoadArmyList()
        {
            MenuItem UnitType = new MenuItem() { Title = "Core", Points = String.Empty };

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.ArmyBook.Units)
            {
                UnitType.Items.Add(new MenuItem() { ID = entry.Key, Title = entry.Value.Name, Points = string.Format(" ({0} pts)", entry.Value.Points) });
            }

            main.ArmyList.Items.Add(UnitType);
        }
    }
}
