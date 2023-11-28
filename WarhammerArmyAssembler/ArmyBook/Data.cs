using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace WarhammerArmyAssembler.ArmyBook
{
    public class Data
    {
        public static string MagicLoreName = String.Empty;
        public static string MagicOptions = String.Empty;
        public static Dictionary<string, int> Magic = new Dictionary<string, int>();

        public static string EnemyMagicLoreName = String.Empty;
        public static string EnemyMagicName = String.Empty;
        public static Dictionary<string, int> Dispell = new Dictionary<string, int>();

        public static string AddStyle = String.Empty;
        public static string DropStyle = String.Empty;
        public static string MagicItemsStyle = String.Empty;
        public static string MagicPowersStyle = String.Empty;

        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
        public static Dictionary<int, Unit> Mounts = new Dictionary<int, Unit>();
        public static Dictionary<int, Option> Artefact = new Dictionary<int, Option>();

        public static int MaxIDindex = 0;

        public static Brush FrontColor = null;
        public static Brush BackColor = null;
        public static Brush GridColor = null;
        public static Brush TooltipColor = null;
        public static string Upgraded = String.Empty;

        public static bool DemonicMortal = false;
        public static bool DemonicAlreadyReplaced = false;
        public static bool NoDogsOfWar = false;
    }
}
