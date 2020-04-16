using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace WarhammerArmyAssembler
{
    class InterfaceOther
    {
        public static Random Rand = new Random();

        public static int IntParse(string line)
        {
            bool success = Int32.TryParse(line, out int value);

            return (success ? value : 0);
        }

        public static Brush BrushFromXml(XmlNode path)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(path.InnerText);
        }

        public static int CalcPercent(double x, double y)
        {
            return (int)System.Math.Round((x * 100) / y);
        }

        public static string[] WordSplit(string caption, int partLength = 35)
        {
            if (caption.Length <= partLength)
                return new string[] { caption };

            string[] words = caption.Split(' ');

            List<string> parts = new List<string>();

            string part = string.Empty;

            int partCounter = 0;

            foreach (string word in words)
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
