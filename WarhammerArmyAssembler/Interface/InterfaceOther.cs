﻿using System;
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
        public static int IntParse(string line)
        {
            int value = 0;

            bool success = Int32.TryParse(line, out value);

            return (success ? value : 0);
        }

        public static Brush BrushFromXml(XmlNode path)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(path.InnerText);
        }
    }
}
