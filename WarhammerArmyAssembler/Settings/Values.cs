using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler.Settings
{
    class Values
    {
        private static Dictionary<string, string> Settings { get; set; }

        public Values() =>
            Clean();

        public static string Get(string name) =>
            Settings.ContainsKey(name) ? Settings[name] : Default.Get(name);

        public static bool IsTrue(string name)
        {
            string value = Get(name);
            return !(String.IsNullOrWhiteSpace(value) || (value == "False"));
        }

        public static string Set(string name, string value) =>
            Settings[name] = value;

        public static Dictionary<string, string> All() =>
            new Dictionary<string, string>(Settings);

        public static void Clean() =>
            Settings = new Dictionary<string, string>();
    }
}
