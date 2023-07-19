using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Text.RegularExpressions;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Services
    {
        public static XmlNode AddFrenzyAttack(XmlDocument xml)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, "AdditionalAttackByFrenzy", String.Empty);
            XmlNode nodeParam = xml.CreateNode(XmlNodeType.Element, "AddToAttacks", String.Empty);
            nodeParam.InnerText = "1";
            nodeName.AppendChild(nodeParam);

            return nodeName;
        }

        public static XmlNode CreateRuleOnlyOption(XmlDocument xml, string rule)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, rule, String.Empty);
            XmlNode nodeParam = xml.CreateNode(XmlNodeType.Element, "OnlyRuleOption", String.Empty);
            nodeParam.InnerText = "true";
            nodeName.AppendChild(nodeParam);

            return nodeName;
        }

        public static bool GetCommonXmlOption(string optionName, out string option,
            Dictionary<string, string> commonXmlOption)
        {
            Dictionary<string, string> xmlOption = commonXmlOption ?? Constants.CommonXmlOption;

            if (xmlOption.ContainsKey(optionName))
            {
                option = xmlOption[optionName];
                return true;
            }
            else
            {
                option = string.Empty;
                return false;
            }
        }

        public static XmlNode Intro(XmlDocument xmlFile, string name) =>
            xmlFile.SelectSingleNode($"ArmyBook/Introduction/{name}");

        public static XmlNode StyleColor(XmlDocument xmlFile, string name) =>
            xmlFile.SelectSingleNode($"ArmyBook/Introduction/Styles/Colors/{name}");

        public static BitmapImage GetUnitImage(string path)
        {
            BitmapImage image = null;

            try
            {
                image = new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }

            return image;
        }

        public static string ExistsInOnly(string onlyline, string sublines)
        {
            if (String.IsNullOrEmpty(onlyline) || String.IsNullOrEmpty(sublines))
                return String.Empty;

            foreach (string ifOnly in onlyline.Split(',').Select(x => x.Trim()))
                foreach (string subline in sublines.Split(',').Select(x => x.Trim()))
                    if (!String.IsNullOrEmpty(subline) && (ifOnly == subline))
                        return ifOnly;

            return String.Empty;
        }

        private static string CaptalLetter(string lower)
        {
            char[] upper = lower.ToCharArray();
            upper[0] = char.ToUpper(upper[0]);
            return new String(upper);
        }

        public static string CamelNameSplit(string name)
        {
            List<string> multiWords = Regex.Split(name, @"(?<!^)(?=[A-Z])").Select(x => x.ToLower()).ToList();
            return multiWords.Count < 2 ? name : CaptalLetter(String.Join(" ", multiWords));
        }
    }
}
