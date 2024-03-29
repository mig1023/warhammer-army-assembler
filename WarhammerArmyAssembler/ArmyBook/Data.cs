﻿using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace WarhammerArmyAssembler.ArmyBook
{
    public class Data
    {
        public static Dictionary<string, Dictionary<string, int>> Magic =
            new Dictionary<string, Dictionary<string, int>>();

        public static Dictionary<string, Dictionary<string, int>> Dispell =
            new Dictionary<string, Dictionary<string, int>>();

        public static string AddStyle = String.Empty;
        public static string DropStyle = String.Empty;
        public static string ItemsStyle = String.Empty;
        public static string PowersStyle = String.Empty;

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
