﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Text.RegularExpressions;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Services
    {
        public static void AppendXmlNode(XmlDocument xml, XmlNode nodeName, string name, string value)
        {
            XmlNode nodeParam = xml.CreateNode(XmlNodeType.Element, name, String.Empty);
            nodeParam.InnerText = value;
            nodeName.AppendChild(nodeParam);
        }

        public static XmlNode AddFrenzyAttack(XmlDocument xml)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, "AdditionalAttackByFrenzy", String.Empty);
            AppendXmlNode(xml, nodeName, "AddToAttacks", "1");
            AppendXmlNode(xml, nodeName, "TechnicalElement", "true");

            return nodeName;
        }

        public static XmlNode CreateRuleOnlyOption(XmlDocument xml, string rule)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, rule, String.Empty);
            AppendXmlNode(xml, nodeName, "OnlyRuleOption", "true");

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
            {
                foreach (string subline in sublines.Split(',').Select(x => x.Trim()))
                {
                    if (!String.IsNullOrEmpty(subline) && (ifOnly == subline))
                        return ifOnly;
                }
            }

            return String.Empty;
        }

        private static string CaptalLetter(string lower)
        {
            char[] upper = lower.ToCharArray();
            upper[0] = char.ToUpper(upper[0]);
            return new String(upper);
        }

        public static string CamelNameSplit(string name,
            bool pathetic = false)
        {
            List<string> multiWords = Regex
                .Split(name, @"(?<!^)(?=[A-Z])")
                .Select(x => x.ToLower())
                .ToList();

            if (multiWords.Count < 2)
            {
                return name;
            }
            else if (pathetic)
            {
                for (int i = 0; i < multiWords.Count; i++)
                    multiWords[i] = CaptalLetter(multiWords[i]);
            }
            else if (multiWords[1] == "of")
            {
                for (int i = 0; i < multiWords.Count; i++)
                {
                    if (i != 1)
                        multiWords[i] = CaptalLetter(multiWords[i]);
                }
            }
            else
            {
                multiWords[0] = CaptalLetter(multiWords[0]);
            }

            return String.Join(" ", multiWords);
        }

        public static string FindNameInAttributes(string name, string attributesLine)
        {
            List<string> attributes = attributesLine
                .Split(new string[] { ";", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (string attributeLine in attributes)
            {
                if (!attributeLine.Contains(":"))
                    continue;

                List<string> attribute = attributeLine
                    .Split(':')
                    .Select(x => x.Trim())
                    .ToList();

                if (attribute[0] == name)
                    return attribute[1];
            }

            return String.Empty;
        }
    }
}
