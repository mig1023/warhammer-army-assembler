using System;
using System.Collections.Generic;

namespace WarhammerArmyAssembler.Settings
{
    class Values
    {
        private static Dictionary<string, string> Settings { get; set; }

        public Values() =>
            Clean();

        public static string GetLine(string name)
        {
            if (Settings.ContainsKey(name))
                return Settings[name];
            else
                return String.Empty;
        }

        public static bool Get(string name)
        {
            string value = GetLine(name);
            return !(String.IsNullOrWhiteSpace(value) || (value == "False"));
        }

        public static string Set(string name, string value) =>
            Settings[name] = value;

        public static Dictionary<string, string> All()
        {
            if (Settings != null)
                return new Dictionary<string, string>(Settings);
            else
                return new Dictionary<string, string>();
        }

        public static void Clean() =>
            Settings = new Dictionary<string, string>();
    }
}
