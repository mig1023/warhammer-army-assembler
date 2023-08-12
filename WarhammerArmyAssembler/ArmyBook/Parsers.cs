using System;
using System.Collections.Generic;
using System.Xml;
using WarhammerArmyAssembler.Test;
using System.Text.RegularExpressions;
using static WarhammerArmyAssembler.Option;
using static WarhammerArmyAssembler.Unit;
using System.Linq;
using System.Reflection;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Parsers
    {
        public static int IntParse(XmlNode xmlNode, int? byDefault = null)
        {
            if (xmlNode == null)
                return byDefault ?? 0;

            return IntParse(xmlNode.InnerText, byDefault);
        }

        public static Profile ProfileParse(XmlNode xmlNode) =>
            new Profile { Value = IntParse(xmlNode), Null = (xmlNode == null) };

        public static int RunicParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return 0;

            string valueAttr = xmlNode.Attributes["Val"]?.InnerText ?? String.Empty;
            bool success = int.TryParse(valueAttr, out int value);

            return (success ? value : Constants.SingleRunicItem);
        }

        public static int IntParse(string xmlNode, int? byDefault = null)
        {
            if (xmlNode.StartsWith("D6"))
                xmlNode = "1" + xmlNode;

            if (xmlNode.EndsWith("D6"))
                xmlNode += "0";

            string text = xmlNode.Replace("D", String.Empty).Replace("+", String.Empty);

            bool success = int.TryParse(text, out int value);

            return (success ? value : (byDefault ?? 0));
        }

        public static Profile ProfileParse(string xmlNode) =>
            new Profile { Value = IntParse(xmlNode) };

        public static Profile IntNullableParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return new Profile { Value = 0, Null = true };

            bool success = int.TryParse(xmlNode.InnerText, out int value);

            return (success ? new Profile { Value = value } : new Profile { Value = 0, Null = true });
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

            return Regex.Unescape(xmlNode.InnerText);
        }

        public static List<string> SlotsParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            List<string> allSlots = new List<string>();

            foreach (XmlNode xmlSlot in xmlNode.SelectNodes("Slots"))
            {
                List<string> slots = xmlSlot.InnerText
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList();

                foreach (string slot in slots)
                    allSlots.Add(slot);
            }

            return allSlots;
        }

        public static Countable CountableParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            Countable countable = new Countable
            {
                Min = IntParse(xmlNode.Attributes["Min"]),
                Max = IntParse(xmlNode.Attributes["Max"]),
                Value = IntParse(xmlNode.Attributes["Value"]),
                Nullable = BoolParse(xmlNode["Nullable"]),
                ExportToUnitSize = BoolParse(xmlNode["ExportToUnitSize"]),
                ExportToWizardLevel = BoolParse(xmlNode["ExportToWizardLevel"]),
            };

            XmlNode max = xmlNode["MaxDependency"];

            if (max != null)
            {
                if (max.Attributes["Dependency"] != null)
                {
                    countable.Dependency = max.Attributes["Dependency"].InnerText;
                }

                if (max.Attributes["Ratio"] != null)
                {
                    bool success = Double.TryParse(max.Attributes["Ratio"].InnerText, out double value);
                    countable.Ratio = (success ? value : 0);
                }
            }

            return countable;
        }

        public static List<Param> ParamParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return null;

            List<Param> allParamTests = new List<Param>();

            foreach (XmlNode xmlParamTest in xmlNode.SelectNodes("Test"))
            {
                Param newParamTest = new Param { Type = xmlParamTest.Attributes["Profile"].Value };

                bool success = Enum.TryParse(xmlParamTest.Attributes["Bet"].Value, out Param.TestType bet);
                newParamTest.Bet = success ? bet : Param.TestType.Wound;

                success = Enum.TryParse(xmlParamTest.Attributes["Context"].Value, out Param.ContextType context);
                newParamTest.Context = success ? context : Param.ContextType.Round;

                XmlNode xmlRepeat = xmlParamTest.Attributes["Repeat"];
                if (xmlRepeat != null)
                {
                    success = Enum.TryParse(xmlRepeat.Value, out Param.RepeatType repeat);
                    newParamTest.Repeat = (success ? repeat : Param.RepeatType.Normal);
                }
                else
                {
                    newParamTest.Repeat = Param.RepeatType.Normal;
                }

                newParamTest.MountsOnly = xmlParamTest.Attributes["MountsOnly"] != null;

                newParamTest.UsedAlready = false;

                allParamTests.Add(newParamTest);
            }

            return allParamTests;
        }

        public static string[] AllStringParse(XmlNode xmlNode, string nodeName, bool comma = false)
        {
            if (xmlNode == null)
                return new string[] { };

            List<string> allString = new List<string>();

            foreach (XmlNode xmlSpecialRule in xmlNode.SelectNodes(nodeName))
            {
                if (comma)
                {
                    List<string> rules = xmlSpecialRule.InnerText
                        .Split(',')
                        .Select(x => x.Trim())
                        .ToList();

                    foreach (string rule in rules)
                        allString.Add(rule);
                }
                else
                {
                    allString.Add(xmlSpecialRule.InnerText);
                }
            }

            return allString.ToArray();
        }

        public static UnitType UnitTypeParse(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return UnitType.Core;

            bool success = Enum.TryParse(xmlNode.Name, out UnitType value);

            if (success && (value == UnitType.Crew))
            {
                return UnitType.Mount;
            }
            else if (success)
            {
                return value;
            }
            else
            {
                return UnitType.Core;
            }
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

            string type = xmlNode["Type"] == null ? xmlNode.Name : xmlNode["Type"].InnerText;

            bool success = Enum.TryParse(type, out OptionType value);

            if ((xmlNode.ParentNode?.Name == "Options") || (xmlNode.ParentNode?.Name == "Command"))
                return success ? value : Option.OptionType.Option;
            else
                return success && (value == OptionType.Armour) ? value : OptionType.Info;
                
        }

        public static bool BoolParse(XmlNode xmlNode) =>
            xmlNode != null;  
        
        public static void SizeParse(string line, out int min, out int max)
        {
            if (line.Contains("+"))
            {
                min = IntParse(line.Replace("+", String.Empty));
                max = 0;
            }
            else if (line.Contains("-"))
            {
                string[] sizes = line.Split('-');
                min = IntParse(sizes[0]);
                max = IntParse(sizes[1]);
            }
            else
            {
                min = 0;
                max = 0;
            }
        }

        private static object PropertyByType(object element, XmlNode value,
            string paramName, bool byAttr = false)
        {
            PropertyInfo param = element.GetType().GetProperty(paramName);

            if (param.PropertyType == typeof(Profile))
            {
                return ProfileParse(value);
            }
            else if (byAttr)
            {
                return null;
            }
            else if (param.PropertyType == typeof(bool))
            {
                return BoolParse(value);
            }
            else if (param.PropertyType == typeof(int?))
            {
                return IntNullableParse(value);
            }
            else if (param.PropertyType == typeof(int))
            {
                return IntParse(value);
            }
            else if (param.PropertyType == typeof(string))
            {
                return StringParse(value);
            }
            else
            {
                return null;
            }
        }

        private static XmlNode XmlValueSemiParser(XmlNode value, string name)
        {
            XmlNode xmlNode = value[name];

            if (xmlNode == null)
                return null;

            if (!String.IsNullOrEmpty(xmlNode.InnerText))
                return xmlNode;

            string valueAttr = xmlNode.Attributes["Val"]?.InnerText ?? String.Empty;

            if (!String.IsNullOrEmpty(valueAttr))
                xmlNode.InnerText = valueAttr;

            return xmlNode;
        }

        public static void SetProperty(object element, XmlNode value, string name, string byAttr = "")
        {
            XmlNode xmlNode = null;
            bool notByAttr = String.IsNullOrEmpty(byAttr);

            if (notByAttr)
            {
                xmlNode = XmlValueSemiParser(value, name);
            }
            else
            {
                xmlNode = (XmlNode)value.Attributes[byAttr];
            }

            object propetyValue = PropertyByType(element, xmlNode, name, !notByAttr);

            if (propetyValue != null)
            {
                element.GetType().GetProperty(name).SetValue(element, propetyValue);
            }
        }
    }
}
