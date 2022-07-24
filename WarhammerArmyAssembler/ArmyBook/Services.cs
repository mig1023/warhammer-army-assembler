using System;
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

        public static bool GetCommonXmlOption(string optionName, out string option)
        {
            if (Constants.CommonXmlOption.ContainsKey(optionName))
            {
                option = Constants.CommonXmlOption[optionName];
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
    }
}
