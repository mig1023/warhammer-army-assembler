using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler.Settings
{
    class Values
    {
        private static Dictionary<string, string> Settings { get; set; }

        public Values() =>
            Clean();

        public static string Get(string name)
        {
            if (Settings.ContainsKey(name))
                return Settings[name];
            else
                return String.Empty;
        }

        public static string Set(string name, string value) =>
            Settings[name] = value;

        public static Dictionary<string, string> All() =>
            new Dictionary<string, string>(Settings);

        public static void Clean() =>
            Settings = new Dictionary<string, string>();
    }
}
