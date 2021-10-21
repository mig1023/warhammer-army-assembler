using System;
using System.Xml;

namespace WarhammerArmyAssembler.ArmyBook
{
    class Other
    {
        public static XmlNode AddFrenzyAttack(XmlDocument xml)
        {
            XmlNode nodeName = xml.CreateNode(XmlNodeType.Element, "AdditionalAttackByFrenzy", String.Empty);
            XmlNode nodeParam = xml.CreateNode(XmlNodeType.Element, "AddToAttacks", String.Empty);
            nodeParam.InnerText = "1";
            nodeName.AppendChild(nodeParam);

            return nodeName;
        }

        public static XmlNode Intro(XmlDocument xmlFile, string name) =>
            xmlFile.SelectSingleNode(String.Format("ArmyBook/Introduction/{0}", name));
    }
}
