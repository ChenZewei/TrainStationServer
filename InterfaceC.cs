using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;

namespace TrainStationServer
{
    class InterfaceC
    {
        static DataBase database;
        static SIPTools sip;
        public InterfaceC()
        {
            database = new DataBase();
            sip = new SIPTools();
        }

        public InterfaceC(DataBase Database)
        {
            database = Database;
            sip = new SIPTools();
        }

        public static XmlDocument Response(XmlDocument doc)
        {
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            XmlDocument response = new XmlDocument();
            root = doc.DocumentElement;
            nodeList = root.SelectNodes("/request/@command");
            node = nodeList.Item(0);
            switch (node.InnerText)
            {
                case "SaRegister":
                    response = SaRegister(doc);
                    break;
                case "SaKeepAlive":
                    response = SaKeepAlive(doc);
                    break;
                case "ResReport":
                    response = ResReport(doc);
                    break;
                default:
                    response = new XmlDocument();
                    break;
            }
            return response;
        }

        public static byte[] Response(byte[] recv,int i)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                doc = SIPTools.XmlExtract(recv, i);
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            XmlDocument response = new XmlDocument();
            root = doc.DocumentElement;
            nodeList = root.SelectNodes("/request/@command");
            node = nodeList.Item(0);
            switch (node.InnerText)
            {
                case "SaRegister":
                    response = SaRegister(doc);
                    break;
                case "SaKeepAlive":
                    response = SaKeepAlive(doc);
                    break;
                case "ResReport":
                    response = ResReport(doc);
                    break;
                default:
                    response = new XmlDocument();
                    break;
            }
            return Encoding.UTF8.GetBytes(sip.SIPResponse(response));
        }

        public static XmlDocument SaRegister(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saIp, saSipPort, saName, saPassword, manufactureId, manufactureName, productiveVersion, softwareVersion;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saIp = XmlOp.GetInnerText(Doc, "saIp");
            saSipPort = XmlOp.GetInnerText(Doc, "saSipPort");
            saName = XmlOp.GetInnerText(Doc, "saName");
            saPassword = XmlOp.GetInnerText(Doc, "saPassword");
            manufactureId = XmlOp.GetInnerText(Doc, "manufactureId");
            manufactureName = XmlOp.GetInnerText(Doc, "manufactureName");
            productiveVersion = XmlOp.GetInnerText(Doc, "productiveVersion");
            softwareVersion = XmlOp.GetInnerText(Doc, "softwareVersion");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaRegister");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "60");
            Response.Save("D://SaRegister-response.xml");

            return Response;
        }

        public static byte[] SaRegister(byte[] recv, int i)
        {
            XmlDocument Doc = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                Doc = SIPTools.XmlExtract(recv, i);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saIp, saSipPort, saName, saPassword, manufactureId, manufactureName, productiveVersion, softwareVersion;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saIp = XmlOp.GetInnerText(Doc, "saIp");
            saSipPort = XmlOp.GetInnerText(Doc, "saSipPort");
            saName = XmlOp.GetInnerText(Doc, "saName");
            saPassword = XmlOp.GetInnerText(Doc, "saPassword");
            manufactureId = XmlOp.GetInnerText(Doc, "manufactureId");
            manufactureName = XmlOp.GetInnerText(Doc, "manufactureName");
            productiveVersion = XmlOp.GetInnerText(Doc, "productiveVersion");
            softwareVersion = XmlOp.GetInnerText(Doc, "softwareVersion");
            
            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaRegister");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "60");
            Response.Save("D://SaRegister-response.xml");

            return Encoding.UTF8.GetBytes(sip.SIPResponse(Response));
        }

        public static XmlDocument SaKeepAlive(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string SaKeepAlive;

            SaKeepAlive = XmlOp.GetInnerText(Doc, "SaKeepAlive");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaKeepAlive");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "60");
            Response.Save("D://SaKeepAlive-response.xml");

            return Response;
        }

        public static byte[] SaKeepAlive(byte[] recv, int i)
        {
            XmlDocument Doc = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                Doc = SIPTools.XmlExtract(recv, i);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string SaKeepAlive;

            SaKeepAlive = XmlOp.GetInnerText(Doc, "SaKeepAlive");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaKeepAlive");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "60");
            Response.Save("D://SaKeepAlive-response.xml");

            return Encoding.UTF8.GetBytes(sip.SIPResponse(Response));
        }

        public static XmlDocument ResReport(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, totalPacketNum, curPacketNum;
            string[] columes = { "id", "name", "location", "custom"};
            string[] values = new string[4]; 
            List<string> resId;
            List<string> name;
            List<string> location;
            List<string> purpose;
            List<string> infomation;

            saId = XmlOp.GetInnerText(Doc, "saId");
            totalPacketNum = XmlOp.GetInnerText(Doc, "totalPacketNum");
            curPacketNum = XmlOp.GetInnerText(Doc, "curPacketNum");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            name = XmlOp.GetInnerTextList(Doc, "name");
            location = XmlOp.GetInnerTextList(Doc, "location");
            purpose = XmlOp.GetInnerTextList(Doc, "purpose");
            infomation = XmlOp.GetInnerTextList(Doc, "infomation");

            for (int j = 0; j < resId.Count; j++)
            {
                values[0] = resId[j];
                values[1] = name[j];
                values[2] = location[j];
                values[3] = infomation[j];
                database.Insert("ivms_resources", columes, values);
            }

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResReport-response.xml");

            return Response;
        }

        public static byte[] ResReport(byte[] recv, int i)
        {
            XmlDocument Doc = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                Doc = SIPTools.XmlExtract(recv, i);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, totalPacketNum, curPacketNum;
            string[] columes = { "id", "name", "location", "custom" };
            string[] values = new string[4];
            List<string> resId;
            List<string> name;
            List<string> location;
            List<string> purpose;
            List<string> infomation;

            saId = XmlOp.GetInnerText(Doc, "saId");
            totalPacketNum = XmlOp.GetInnerText(Doc, "totalPacketNum");
            curPacketNum = XmlOp.GetInnerText(Doc, "curPacketNum");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            name = XmlOp.GetInnerTextList(Doc, "name");
            location = XmlOp.GetInnerTextList(Doc, "location");
            purpose = XmlOp.GetInnerTextList(Doc, "purpose");
            infomation = XmlOp.GetInnerTextList(Doc, "infomation");

            for (int j = 0; j < resId.Count; j++)
            {
                values[0] = resId[j];
                values[1] = name[j];
                values[2] = location[j];
                values[3] = infomation[j];
                database.Insert("ivms_resources", columes, values);
            }

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResReport-response.xml");

            return Encoding.UTF8.GetBytes(sip.SIPResponse(Response));
        }

        public static XmlDocument StartMediaReq(string tcpIp, string tcpPort, string resId, string userId, string userLevel, string mediaType, string linkMode, string targetIpAddr, string targetPort, string flag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartMediaReq");
            XmlOp.ElementAdd(Request, "response", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            XmlOp.ElementAdd(Request, "parameters", "mediaType");
            XmlOp.SetNodeInnerText(Request, "mediaType", 0, mediaType);
            XmlOp.ElementAdd(Request, "parameters", "linkMode");
            XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode);
            XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
            XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
            XmlOp.ElementAdd(Request, "parameters", targetPort);
            XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
            XmlOp.ElementAdd(Request, "parameters", "flag");
            XmlOp.SetNodeInnerText(Request, "flag", 0, flag);
            Request.Save("D://StartMediaReq-response.xml");

            return Request;
        }

        //public static byte[] StartMediaReq(string tcpIp, string tcpPort, string resId, string userId, string userLevel, string mediaType, string linkMode, string targetIpAddr, string targetPort, string flag)
        //{
        //    XmlTools XmlOp = new XmlTools();
        //    XmlDocument Request = XmlOp.XmlCreate();

        //    XmlOp.ElementAdd(Request, null, "request");
        //    XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartMediaReq");
        //    XmlOp.ElementAdd(Request, "response", "parameters");
        //    XmlOp.ElementAdd(Request, "parameters", "resId");
        //    XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
        //    XmlOp.ElementAdd(Request, "parameters", "userId");
        //    XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
        //    XmlOp.ElementAdd(Request, "parameters", "userLevel");
        //    XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
        //    XmlOp.ElementAdd(Request, "parameters", "mediaType");
        //    XmlOp.SetNodeInnerText(Request, "mediaType", 0, mediaType);
        //    XmlOp.ElementAdd(Request, "parameters", "linkMode");
        //    XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode);
        //    XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
        //    XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
        //    XmlOp.ElementAdd(Request, "parameters", targetPort);
        //    XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
        //    XmlOp.ElementAdd(Request, "parameters", "flag");
        //    XmlOp.SetNodeInnerText(Request, "flag", 0, flag);
        //    Request.Save("D://StartMediaReq-response.xml");

        //    return Encoding.UTF8.GetBytes(sip.SIPRequest(Request));
        //}

        public static string[] StartMediaResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string sessionId, tcpIp, tcpPort;
            string[] result = new string[3];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            tcpIp = XmlOp.GetInnerText(Doc, "tcpIp");
            tcpPort = XmlOp.GetInnerText(Doc, "tcpPort");

            result[0] = sessionId;
            result[1] = tcpIp;
            result[2] = tcpPort;

            return result;
        }

        public static string[] StartMediaResponse(byte[] recv, int i)
        {
            XmlDocument Doc = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                Doc = SIPTools.XmlExtract(recv, i);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            XmlTools XmlOp = new XmlTools();
            string sessionId, tcpIp, tcpPort;
            string[] result = new string[3];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            tcpIp = XmlOp.GetInnerText(Doc, "tcpIp");
            tcpPort = XmlOp.GetInnerText(Doc, "tcpPort");

            result[0] = sessionId;
            result[1] = tcpIp;
            result[2] = tcpPort;

            return result;
        }
    }
}
