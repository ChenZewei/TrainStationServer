using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using System.IO;

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

        public static bool IsRequest(byte[] recv, int i)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root;
            XmlNodeList nodeList;
            try
            {
                sip = new SIPTools(recv, i);
                doc = SIPTools.XmlExtract(recv, i);
                if (doc == null)
                    return false;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }

            FileStream sendbuf = new FileStream("D://recieve.txt", FileMode.OpenOrCreate, FileAccess.Write);
            sendbuf.Close();
            sendbuf = new FileStream("D://recieve.txt", FileMode.Append, FileAccess.Write);
            sendbuf.Write(Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml), 0, Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml).Length);
            sendbuf.Close();

            root = doc.DocumentElement;
            nodeList = doc.GetElementsByTagName("request");
            if (nodeList.Count > 0)
                return true;
            else
                return false;
        }

        public static XmlDocument Request(XmlDocument doc)
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

        public static byte[] Request(byte[] recv, int i)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            XmlDocument request = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                doc = SIPTools.XmlExtract(recv, i);
                if (doc == null)
                    return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(request));
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
            }

            FileStream sendbuf = new FileStream("D://test.txt", FileMode.OpenOrCreate, FileAccess.Write);
            sendbuf.Close();
            sendbuf = new FileStream("D://test.txt", FileMode.Append, FileAccess.Write);
            sendbuf.Write(Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml), 0, Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml).Length);
            sendbuf.Close();
            
            root = doc.DocumentElement;
            nodeList = doc.GetElementsByTagName("request");
            if(nodeList.Count > 0)
            {
                nodeList = root.SelectNodes("/request/@command");
                node = nodeList.Item(0);
                switch (node.InnerText)
                {
                    case "SaRegister":
                        request = SaRegister(doc);
                        break;
                    case "SaKeepAlive":
                        request = SaKeepAlive(doc);
                        break;
                    case "ResReport":
                        request = ResReport(doc);
                        break;
                    default:
                        request = new XmlDocument();
                        break;
                }
            }

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(request));
        }

        public static string[] Response(byte[] recv, int i)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            string[] result = null;
            try
            {
                sip = new SIPTools(recv, i);
                doc = SIPTools.XmlExtract(recv, i);
                if (doc == null)
                    return null;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }

            FileStream sendbuf = new FileStream("D://response.txt", FileMode.OpenOrCreate, FileAccess.Write);
            sendbuf.Close();
            sendbuf = new FileStream("D://response.txt", FileMode.Append, FileAccess.Write);
            sendbuf.Write(Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml), 0, Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml).Length);
            sendbuf.Close();

            root = doc.DocumentElement;
            nodeList = doc.GetElementsByTagName("response");
            if (nodeList.Count > 0)
            {
                nodeList = root.SelectNodes("/response/@command");
                node = nodeList.Item(0);
                switch (node.InnerText)
                {
                    case "StartMediaReq":
                        result = new string[3];
                        result = StartMediaResponse(doc);
                        break;
                    default:
                        result = null;
                        break;
                }
            }
            return result;
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
            manufactureId = XmlOp.GetInnerText(Doc, "manufacturerId");
            manufactureName = XmlOp.GetInnerText(Doc, "manufacturerName");
            productiveVersion = XmlOp.GetInnerText(Doc, "productVersion");
            softwareVersion = XmlOp.GetInnerText(Doc, "softwareVersion");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaRegister");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "20");
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
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "20");
            Response.Save("D://SaRegister-response.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }

        public static XmlDocument SaKeepAlive(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string SaKeepAlive;

            SaKeepAlive = XmlOp.GetInnerText(Doc, "saKeepAlivePeriod");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaKeepAlive");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "10");
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

            SaKeepAlive = XmlOp.GetInnerText(Doc, "saKeepAlivePeriod");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SaKeepAlive");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saKeepAlivePeriod");
            XmlOp.SetNodeInnerText(Response, "saKeepAlivePeriod", 0, "10");
            Response.Save("D://SaKeepAlive-response.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }

        public static XmlDocument ResReport(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, totalPacketNum, curPacketNum;
            string[] columes = { "id", "name", "location", "custom"};
            string[] values = new string[4];
            int num = 2;
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
                num = 2;
                values[0] = resId[j];
                values[1] = name[j];
                if (location.Count >= 1)
                {
                    values[2] = location[j];
                    num++;
                }
                if (purpose.Count >= 1)
                {
                    values[3] = purpose[j];
                    num++;
                }
                database.Insert("ivms_resources", columes, values, num);
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
            int num = 2;
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
                if (location.Count >= 1)
                {
                    values[2] = location[j];
                    num++;
                }
                if (purpose.Count >= 1)
                {
                    values[3] = purpose[j];
                    num++;
                }
                database.Insert("ivms_resources", columes, values, num);
            }

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResReport-response.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }

        //public static XmlDocument StartMediaReq(string tcpIp, string tcpPort, string resId, string userId, string userLevel, string mediaType, string linkMode, string targetIpAddr, string targetPort, string flag)
        //{
        //    XmlTools XmlOp = new XmlTools();
        //    XmlDocument Request = XmlOp.XmlCreate();

        //    XmlOp.ElementAdd(Request, null, "request");
        //    XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartMediaReq");
        //    XmlOp.ElementAdd(Request, "request", "parameters");
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

        //    return Request;
        //}

        #region StartMediaReq
        public static byte[] StartMediaReq(string tcpIp, string tcpPort, string resId, string userId, string userLevel, string mediaType, string linkMode, string targetIpAddr, string targetPort, string flag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartMediaReq");
            XmlOp.ElementAdd(Request, "request", "parameters");
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
            XmlOp.ElementAdd(Request, "parameters", "targetPort");
            XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
            XmlOp.ElementAdd(Request, "parameters", "flag");
            XmlOp.SetNodeInnerText(Request, "flag", 0, flag);
            Request.Save("D://StartMediaReq-response.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPRequest(Request));
        }

        public static byte[] test(byte[] recv, int i)//Only for test
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


            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartMediaReq");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "0001");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "127.0.0.1");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "12001");
            Response.Save("D://StartMediaReq-response.xml");

            return Encoding.UTF8.GetBytes(sip.SIPResponse(Response));
        }

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
        #endregion

        #region ControlPTZ
        public static byte[] ControlPTZ(string resId, string userId, string userLevel, string cmd, string param, string speed)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ControlPTZ");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            XmlOp.ElementAdd(Request, "parameters", "cmd");
            XmlOp.SetNodeInnerText(Request, "cmd", 0, cmd);
            XmlOp.ElementAdd(Request, "parameters", "param");
            XmlOp.SetNodeInnerText(Request, "param", 0, param);
            XmlOp.ElementAdd(Request, "parameters", "speed");
            XmlOp.SetNodeInnerText(Request, "speed", 0, speed);
            Request.Save("D://ControlPTZ-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }
        #endregion

        #region StopMediaReq
        public static byte[] StopMediaReq(string sessionId, string resId, string stopFlag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StopMediaReq");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "stopFlag");
            XmlOp.SetNodeInnerText(Request, "stopFlag", 0, stopFlag);
            Request.Save("D://StopMediaReq-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }
        #endregion

        #region QueryHistoryFiles
        public static byte[] QueryHistoryFiles(string resId, string userId, string userLevel, string cuId, string fromDate, string toDate)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "QueryHistoryFiles");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            XmlOp.ElementAdd(Request, "parameters", "cuId");
            XmlOp.SetNodeInnerText(Request, "cuId", 0, cuId);
            XmlOp.ElementAdd(Request, "parameters", "fromDate");
            XmlOp.SetNodeInnerText(Request, "fromDate", 0, fromDate);
            XmlOp.ElementAdd(Request, "parameters", "toDate");
            XmlOp.SetNodeInnerText(Request, "toDate", 0, toDate);
            Request.Save("D://QueryHistoryFiles-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] QueryHistoryFilesResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string resId, cuId, totalNumber, currentNumber;
            string[] result = new string[4];
            List<string> startTime = new List<string>();
            List<string> endTime = new List<string>();
            List<string> size = new List<string>();

            resId = XmlOp.GetInnerText(Doc, "resId");
            cuId = XmlOp.GetInnerText(Doc, "cuId");
            totalNumber = XmlOp.GetInnerText(Doc, "totalNumber");
            currentNumber = XmlOp.GetInnerText(Doc, "currentNumber");

            startTime = XmlOp.GetInnerTextList(Doc, "startTime");
            endTime = XmlOp.GetInnerTextList(Doc, "endTime");
            size = XmlOp.GetInnerTextList(Doc, "size");

            result[0] = resId;
            result[1] = cuId;
            result[2] = totalNumber;
            result[3] = currentNumber;

            return result;
        }
        #endregion

        #region StartPlayBack
        public static byte[] StartPlayBack(string resId, string userId, string userLevel, string startTime, string endTime, int LinkMode, string targetIpAddr, string targetPort, int flag, int locationFlag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartPlayBack");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            XmlOp.ElementAdd(Request, "parameters", "startTime");
            XmlOp.SetNodeInnerText(Request, "startTime", 0, startTime);
            XmlOp.ElementAdd(Request, "parameters", "endTime");
            XmlOp.SetNodeInnerText(Request, "endTime", 0, endTime);
            XmlOp.ElementAdd(Request, "parameters", "LinkMode");
            XmlOp.SetNodeInnerText(Request, "LinkMode", 0, LinkMode.ToString());
            XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
            XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
            XmlOp.ElementAdd(Request, "parameters", "targetPort");
            XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
            XmlOp.ElementAdd(Request, "parameters", "flag");
            XmlOp.SetNodeInnerText(Request, "flag", 0, flag.ToString());
            XmlOp.ElementAdd(Request, "parameters", "locationFlag");
            XmlOp.SetNodeInnerText(Request, "locationFlag", 0, locationFlag.ToString());
            Request.Save("D://StartPlayBack-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] StartPlayBackResponse(XmlDocument Doc)
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
        #endregion


    }
}
