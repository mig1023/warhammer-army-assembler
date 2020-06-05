using System;
using System.Collections.Generic;
using System.Xml;
using WarhammerArmyAssembler.Test;
using static WarhammerArmyAssembler.Option;
using static WarhammerArmyAssembler.Unit;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Parsers
    {
        public static int IntParse(XmlNode xmlNode, int? byDefault = null)
        {
            if (xmlNode == null)
                return byDefault ?? 0;

            bool success = int.TryParse(xmlNode.InnerText, out int value);

            return (success ? value : (byDefault ?? 0));
        }

        public static int? IntNullableParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            bool success = int.TryParse(xmlNode.InnerText, out int value);

            return (success ? value : (int?)null);
        }

        public static double DoubleParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            bool success = Double.TryParse(xmlNode.InnerText, out double value);

            return (success ? value : 0);
        }

        public static string StringParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return String.Empty;

            return xmlNode.InnerText.Replace("|", "\n");
        }

        public static List<Param> ParamParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            List<Param> allParamTests = new List<Param>();

            foreach (XmlNode xmlParamTest in xmlNode.SelectNodes("Test"))
            {
                Param newParamTest = new Param
                {
                    Type = xmlParamTest.InnerText,
                };

                bool success = Enum.TryParse(xmlParamTest.Attributes["Bet"].Value, out Param.TestType bet);
                newParamTest.Bet = (success ? bet : Param.TestType.Wound);

                success = Enum.TryParse(xmlParamTest.Attributes["Context"].Value, out Param.ContextType context);
                newParamTest.Context = (success ? context : Param.ContextType.Round);

                XmlNode xmlRepeat = xmlParamTest.Attributes["Repeat"];
                if (xmlRepeat != null)
                {
                    success = Enum.TryParse(xmlRepeat.Value, out Param.RepeatType repeat);
                    newParamTest.Repeat = (success ? repeat : Param.RepeatType.Normal);
                }
                else
                    newParamTest.Repeat = Param.RepeatType.Normal;

                newParamTest.UsedAlready = false;

                allParamTests.Add(newParamTest);
            }

            return allParamTests;
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

            bool success = Enum.TryParse(xmlNode.InnerText, out UnitType value);

            return (success ? value : UnitType.Core);
        }

        public static MagicItemsTypes MagicItemsTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return MagicItemsTypes.Hero;

            bool success = Enum.TryParse(xmlNode.InnerText, out MagicItemsTypes value);

            return (success ? value : MagicItemsTypes.Hero);
        }

        public static OptionType OptionTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            bool success = Enum.TryParse(xmlNode.InnerText, out OptionType value);

            return (success ? value : Option.OptionType.Option);
        }

        public static OnlyForType OnlyForParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            bool success = Enum.TryParse(xmlNode.InnerText, out OnlyForType value);

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
