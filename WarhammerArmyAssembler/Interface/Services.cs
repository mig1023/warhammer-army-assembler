﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml;

namespace WarhammerArmyAssembler.Interface
{
    class Services
    {
        public static Random Rand = new Random();

        public static readonly int SPINNER_TOP_MARGIN = 50;
        public static readonly int SPINNER_LEFT_MARGIN = 40;

        public static readonly double DICE_SIZE = 6;
        public static readonly double DICE_HALF = 3.5;

        public static int IntParse(string line)
        {
            bool success = Int32.TryParse(line, out int value);

            return (success ? value : 0);
        }

        public static Brush BrushFromXml(XmlNode path) => (SolidColorBrush)new BrushConverter().ConvertFromString(path.InnerText);

        public static int CalcPercent(double x, double y) => (int)Math.Round((x * 100) / y);

        public static string[] WordSplit(string caption, int partLength = 35)
        {
            if (caption.Length <= partLength)
                return new string[] { caption };

            List<string> parts = new List<string>();
            string part = string.Empty;
            int partCounter = 0;

            foreach (string word in caption.Split(' '))
            {
                if ((part.Length + word.Length) < partLength)
                    part += (String.IsNullOrEmpty(part) ? word : " " + word);
                else
                {
                    parts.Add(part);
                    part = word;
                    partCounter += 1;
                }
            }

            parts.Add(part);

            return parts.ToArray();
        }
    }
}
