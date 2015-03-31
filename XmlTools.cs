using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    class XmlTools
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

        public void ElementAdd(XmlDocument doc, string fatherNode, string nodeName, int fatherIndex)
        {
            if (fatherNode != null)
            {
                XmlNodeList list = doc.SelectNodes("//" + fatherNode);
                XmlNode father = list.Item(fatherIndex);
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

        public List<string> GetInnerTextList(XmlDocument doc, string fatherNode, string nodeName)
        {
            List<string> list = new List<string>();
            XmlNodeList fatherNodeList = doc.SelectNodes("//" + fatherNode);
            XmlNodeList nodeList;
            foreach (XmlNode tempFatherNode in fatherNodeList)
            {
                nodeList = tempFatherNode.SelectNodes("//" + nodeName);
                foreach (XmlNode tempNode in nodeList)
                {
                    list.Add(tempNode.InnerText);
                }
            }
            return list;
        }

        public List<string> GetInnerTextListByPath(XmlDocument doc, string path)
        {
            List<string> list = new List<string>();
            XmlNodeList nodeList = doc.SelectNodes(path);
            foreach (XmlNode tempNode in nodeList)
            {
                list.Add(tempNode.InnerText);
            }
            return list;
        }

        public string[] GetAttribute(XmlDocument doc, string nodeName, string[] attributeName)
        {
            string[] result = new string[attributeName.Length];
            int i = 0;
            XmlNode node = doc.SelectSingleNode("//" + nodeName);
            XmlNodeList nodeList = doc.GetElementsByTagName(nodeName);
            //foreach(XmlElement temp in nodeList)
            //{
            //    result[i++] = temp.GetAttribute(attributeName[i]);
            //}//原
            for (; i < attributeName.Length; i++)
            {
                result[i] = node.Attributes[i].Value.ToString();
            }
            
            return result;
        }
    }
}
