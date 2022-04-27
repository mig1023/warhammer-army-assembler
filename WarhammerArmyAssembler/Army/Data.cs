﻿using System.Collections.Generic;

namespace WarhammerArmyAssembler.Army
{
    class Data
    {
        public static Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

        public static string Name { get; set; }
        public static string InternalName { get; set; }
        public static string AdditionalName { get; set; }
        public static int ArmyEdition { get; set; }
        public static string UnitsImagesDirectory { get; set; }

        public static int MaxPoints = ArmyBook.Constants.BaseArmySize;

        public static int MaxIDindex = 0;
    }
}
