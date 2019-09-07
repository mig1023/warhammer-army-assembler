using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WarhammerArmyAssembler.Units;

namespace WarhammerArmyAssembler.Interface
{
    class Interface
    {
        public static MainWindow main = null;

        public static void LoadArmyList()
        {
            MenuItem UnitType = new MenuItem() { Title = "Основные", Points = String.Empty };

            foreach (KeyValuePair<string, Unit> entry in ArmyBook.ArmyBook.Units)
                UnitType.Items.Add(new MenuItem() { ID = entry.Key, Title = entry.Value.Name, Points = string.Format(" ({0} pts)", entry.Value.Points) });

            main.ArmyList.Items.Add(UnitType);
        }

        public static void ArmyGridDrop(object sender, DragEventArgs e)
        {
            string id = (string)e.Data.GetData(DataFormats.Text);

            Unit unit = ArmyBook.ArmyBook.Units[id];

            Army.Army.Units.Add(unit.ID, unit);

            DataGrid l = sender as DataGrid;
            l.Items.Add(new ArmyListViewItem
            {
                Name = unit.Name,
                Movement = unit.Movement,
                WeaponSkill = unit.WeaponSkill,
                BallisticSkill = unit.BallisticSkill,
                Strength = unit.Strength,
                Toughness = unit.Toughness,
                Wounds = unit.Wounds,
                Initiative = unit.Initiative,
                Attacks = unit.Attacks,
                Leadership = unit.Leadership,
                Points = Army.Army.GetUnitPoints(unit.ID),
                Size = unit.Size,
                SpecialRules = Army.Army.GetSpecialRules(unit.ID),
            });
        }
    }
}
