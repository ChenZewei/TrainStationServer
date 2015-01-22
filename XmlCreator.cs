using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    class XmlCreator
    {
        public XmlDocument XmlCreate()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", "yes");
            doc.AppendChild(dec);
            return doc;
        }

        public void ElementAdd(XmlDocument doc,string fatherNode, string nodeName)
        {
            if (fatherNode != null)
            {
                XmlNode father = doc.SelectSingleNode("//" + fatherNode);
                XmlElement element = doc.CreateElement(nodeName);
                father.AppendChild(element);
            }
            else
            {
                //XmlElement father = doc.DocumentElement;
                XmlElement element = doc.CreateElement(nodeName);
                doc.AppendChild(element);
            }
        }

        public void SetNodeAttribute(XmlDocument doc, string nodeName, int index,string attributeName, string attribute)
        {
            XmlNode node = doc.SelectSingleNode("//" + nodeName);
            XmlNodeList list = doc.GetElementsByTagName(nodeName);
            foreach (XmlElement temp in list)
            {
                temp.SetAttribute(attributeName, attribute);
            }
        }

        public void SetNodeInnerText(XmlDocument doc, string nodeName, int index, string text)
        {
            XmlNodeList list = doc.SelectNodes("//" + nodeName);
            XmlNode node = list.Item(index);
            node.InnerText = text;
        }

        public string GetInnerText(XmlDocument doc,string nodeName)
        {
            XmlNode node;
            node = doc.SelectSingleNode("//" + nodeName);
            return node.InnerText;
        }

        public List<string> GetInnerTextList(XmlDocument doc, string nodeName)
        {
            List<string> list = new List<string>();
            XmlNodeList nodeList = doc.SelectNodes("//" + nodeName);
            foreach (XmlNode tempNode in nodeList)
            {
                list.Add(tempNode.InnerText);
            }
            return list;
        }
    }
}
