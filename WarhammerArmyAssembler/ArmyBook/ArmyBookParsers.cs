using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static WarhammerArmyAssembler.Option;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler
{
    class ArmyBookParsers
    {
        public static int IntParse(XmlNode xmlNode, int? byDefault = null)
        {
            if (xmlNode == null)
                return byDefault ?? 0;

            int value = 0;

            bool success = int.TryParse(xmlNode.InnerText, out value);

            return (success ? value : (byDefault ?? 0));
        }

        public static int? IntNullableParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            int value = 0;

            bool success = int.TryParse(xmlNode.InnerText, out value);

            return (success ? value : (int?)null);
        }

        public static double DoubleParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            double value = 0;

            bool success = Double.TryParse(xmlNode.InnerText, out value);

            return (success ? value : 0);
        }

        public static string StringParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return String.Empty;

            return xmlNode.InnerText.Replace("|", "\n");
        }

        public static string[] AllStringParse(XmlNode xmlNode, string nodeName)
        {
            if (xmlNode == null)
                return new string[] { };

            List<string> allString = new List<string>();

            foreach (XmlNode xmlSpecialRule in xmlNode.SelectNodes(nodeName))
                allString.Add(xmlSpecialRule.InnerText);

            return allString.ToArray();
        }

        public static UnitType UnitTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return UnitType.Core;

            UnitType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : UnitType.Core);
        }

        public static MagicItemsTypes MagicItemsTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return MagicItemsTypes.Hero;

            MagicItemsTypes value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : MagicItemsTypes.Hero);
        }

        public static OptionType OptionTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            OptionType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : Option.OptionType.Option);
        }

        public static OnlyForType OnlyForParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            OnlyForType value;

            bool success = Enum.TryParse(xmlNode.InnerText, out value);

            return (success ? value : Option.OnlyForType.All);
        }

        public static bool BoolParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return false;

            return (xmlNode.InnerText == "true" ? true : false);
        }
    }
}
