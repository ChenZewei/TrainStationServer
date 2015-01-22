using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    class InterfaceB:Interface
    {
        public XmlDocument AlarmResSubscribe(XmlDocument Doc)
        {
            XmlDocument Response = new XmlDocument();
            XmlElement root,responseRoot;
            XmlNodeList nodeList;
            XmlNode node;
            string muId, muName;
            int action;
            //string[] idURL;
            //string[] typeURL;
            List<string> idURL = new List<string>();
            List<string> typeURL = new List<string>();
            root = Doc.DocumentElement;
            
            node = root.SelectSingleNode("/request/parameters/muId");
            muId = node.InnerText;
            node = root.SelectSingleNode("/request/parameters/muName");
            muName = node.InnerText;
            node = root.SelectSingleNode("/request/parameters/action");
            action = Int16.Parse(node.InnerText);

            nodeList = root.SelectNodes("/request/parameters/group/URL");
            foreach (XmlNode tempNode in nodeList)
            {
                idURL.Add(tempNode.SelectSingleNode("id").InnerText);
                typeURL.Add(tempNode.SelectSingleNode("type").InnerText);
            }

            XmlDeclaration dec = Response.CreateXmlDeclaration("1.0", "GB2312", "yes");
            Response.AppendChild(dec);

            responseRoot = Response.CreateElement("response");
            responseRoot.SetAttribute("command", "AlarmResSubscribe");
            Response.AppendChild(responseRoot);
            XmlNode responseNode = Response.SelectSingleNode("response");
            XmlElement result = Response.CreateElement("result");
            result.SetAttribute("code", "0");
            result.InnerText = "success";
            responseNode.AppendChild(result);
            XmlElement para = Response.CreateElement("parameters");
            XmlElement responsemuId = Response.CreateElement("muId");
            responsemuId.InnerText = muId;
            para.AppendChild(responsemuId);
            responseNode.AppendChild(para);

            Response.Save("D://test1.xml");
            XmlCreator test = new XmlCreator();
            XmlDocument testdoc = test.XmlCreate();
            test.ElementAdd(testdoc,null,"response");
            test.SetNodeAttribute(testdoc, "response", 0, "command", "AlarmResSubscribe");
            test.ElementAdd(testdoc, "response", "result");
            test.SetNodeAttribute(testdoc, "result", 0, "code", "0");
            test.SetNodeInnerText(testdoc, "result", 0, "success");
            test.ElementAdd(testdoc, "response", "parameters");
            test.ElementAdd(testdoc, "parameters", "muId");
            test.SetNodeInnerText(testdoc, "muId", 0, muId);
            testdoc.Save("D://test2.xml");
            return Response;
        }

        XmlDocument ControlFileBack(XmlDocument Doc)
        {
            XmlDocument Response = new XmlDocument();

            return Response;
        }
    }
}
