﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml;

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
            xmlFile.SelectSingleNode(String.Format("ArmyBook/Introduction/{0}", name));

        public static XmlNode StyleColor(XmlDocument xmlFile, string name) =>
            xmlFile.SelectSingleNode(String.Format("ArmyBook/Introduction/Styles/Colors/{0}", name));

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
            foreach (string ifOnly in onlyline.Split(',').Select(x => x.Trim()))
                foreach (string subline in sublines.Split(',').Select(x => x.Trim()))
                    if (!String.IsNullOrEmpty(subline) && (ifOnly == subline))
                        return ifOnly;

            return String.Empty;
        }
    }
}
