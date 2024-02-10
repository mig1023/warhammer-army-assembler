using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace WarhammerArmyAssembler.Test
{
    class Data
    {
        public enum TestTypes { FullTest, StatisticTest, BattleRoyale }; 

        public static Unit CurrentUnit = null;
        public static Unit CurrentUnitMount = null;
        public static Unit CurrentEnemy = null;
        public static Unit CurrentEnemyMount = null;

        public static List<Interface.Text> TextConsole = new List<Interface.Text>();

        public static Brush Text = Brushes.Black;
        public static Brush SupplText = Brushes.Gray;
        public static Brush GoodText = Brushes.Green;
        public static Brush BadText = Brushes.Red;

        public static Random Rand = new Random();


        public static void PrepareUnit(Unit unit)
        {
            Data.CurrentUnit = unit
                .Clone()
                .GetOptionRules(hasMods: out _, directModification: true)
                .GetUnitMultiplier();

            if (unit.MountOn > 0)
            {
                int size = Army.Data.Units[unit.ArmyID].Chariot > 0 ?
                    Army.Data.Units[unit.ArmyID].Chariot : Data.CurrentUnit.Size;

                Data.CurrentUnitMount = Army.Data.Units[unit.MountOn]
                    .Clone()
                    .GetOptionRules(hasMods: out _, directModification: true)
                    .GetUnitMultiplier(size);
            }
            else
            {
                Data.CurrentUnitMount = null;
            }
        }

        public static void PrepareEnemy(string enemyName)
        {
            Data.CurrentEnemy = Enemy
                .ByName(enemyName)
                .Clone()
                .GetOptionRules(hasMods: out _, directModification: true)
                .GetUnitMultiplier();

            Data.CurrentEnemyMount = CurrentEnemy
                .Mount?
                .Clone()
                .GetOptionRules(hasMods: out _, directModification: true)
                .GetUnitMultiplier();
        }

        public static void Console(Brush color, string line) =>
            Interface.Test.LineToConsole(line, color);
    }
}
