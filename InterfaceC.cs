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

        public static bool IsRequest(XmlDocument doc)
        {
            XmlElement root;
            XmlNodeList nodeList;

            //FileStream sendbuf = new FileStream("D://recieve.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //sendbuf.Close();
            //sendbuf = new FileStream("D://recieve.txt", FileMode.Append, FileAccess.Write);
            //sendbuf.Write(Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml), 0, Encoding.GetEncoding("GB2312").GetBytes(doc.OuterXml).Length);
            //sendbuf.Close();

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
            nodeList = doc.GetElementsByTagName("request");
            if (nodeList.Count > 0)
            {
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
                    case "ResChange":
                        response = ResChange(doc);
                        break;
                    default:
                        response = new XmlDocument();
                        break;
                }
            }

            return response;
        }

        public static byte[] Request(byte[] recv, int i)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            XmlDocument response = new XmlDocument();
            try
            {
                sip = new SIPTools(recv, i);
                doc = SIPTools.XmlExtract(recv, i);
                if (doc == null)
                    return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(response));
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
                        response = SaRegister(doc);
                        break;
                    case "SaKeepAlive":
                        response = SaKeepAlive(doc);
                        break;
                    case "ResReport":
                        response = ResReport(doc);
                        break;
                    case "ResChange":
                        response = ResChange(doc);
                        break;
                    default:
                        response = new XmlDocument();
                        break;
                }
            }

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(response));
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
                    case "QueryAlarmRes":
                        result = new string[3];
                        result = QueryAlarmResResponse(doc);
                        break;
                    case "ControlPTZ":
                        result = null;
                        break;
                    default:
                        result = null;
                        break;
                }
            }
            return result;
        }

        public static XmlDocument Translate(XmlDocument doc)
        {
            XmlDocument request = new XmlDocument();
            XmlNode node;
            try
            {
                node = doc.SelectSingleNode("//*");
                switch (node.Name)
                {
                    case "PTZControl"://收到的信息为PTZControl
                        request = ControlPTZTranslate(doc);
                        break;
                    default:
                        break;
                }
                
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            return request;
        }

        #region Down 2 Up

        #region SaRegister
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
            Response.Save("D://SaRegister-request.xml");

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
            Response.Save("D://SaRegister-request.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }
        #endregion

        #region SaKeepAlive
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
            //Response.Save("D://SaKeepAlive-request.xml");

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
            Response.Save("D://SaKeepAlive-request.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }
        #endregion

        #region ResReport
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

            database.Insert("ivms_resources", columes, resId, name, location, purpose);

            //for (int j = 0; j < resId.Count; j++)
            //{
            //    num = 2;
            //    values[0] = resId[j];
            //    values[1] = name[j];
            //    if (location.Count >= 1)
            //    {
            //        values[2] = location[j];
            //        num++;
            //    }
            //    if (purpose.Count >= 1)
            //    {
            //        values[3] = purpose[j];
            //        num++;
            //    }
            //    database.Insert("ivms_resources", columes, values, num);
            //}

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResReport-request.xml");

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

            database.Insert("ivms_resources", columes, resId, name, location, purpose);

            //for (int j = 0; j < resId.Count; j++)
            //{
            //    values[0] = resId[j];
            //    values[1] = name[j];
            //    if (location.Count >= 1)
            //    {
            //        values[2] = location[j];
            //        num++;
            //    }
            //    if (purpose.Count >= 1)
            //    {
            //        values[3] = purpose[j];
            //        num++;
            //    }
            //    database.Insert("ivms_resources", columes, values, num);
            //}

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResReport-request.xml");

            return Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(Response));
        }
        #endregion
        
        #region ResChange
        public static XmlDocument ResChange(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, cmd;
            string[] columes = { "name", "location", "custom" };
            string[] values = new string[3];
            string[] locColumes = { "id" };
            string[] locValues = new string[1];
            int num = 2;
            List<string> resId;
            List<string> name;
            List<string> location;
            List<string> purpose;
            List<string> infomation;

            saId = XmlOp.GetInnerText(Doc, "saId");
            cmd = XmlOp.GetInnerText(Doc, "cmd");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            name = XmlOp.GetInnerTextList(Doc, "name");
            location = XmlOp.GetInnerTextList(Doc, "location");
            purpose = XmlOp.GetInnerTextList(Doc, "purpose");
            infomation = XmlOp.GetInnerTextList(Doc, "infomation");

            for (int j = 0; j < resId.Count; j++)
            {
                num = 1;
                locValues[0] = resId[j];
                values[0] = name[j];
                if (location.Count >= 1)
                {
                    values[1] = location[j];
                    num++;
                }
                if (purpose.Count >= 1)
                {
                    values[2] = purpose[j];
                    num++;
                }
                database.Update("ivms_resources", columes, values, locColumes, locValues, num, locValues.Length);
            }

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResChange");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            Response.Save("D://ResChange-response.xml");

            return Response;
        }
        #endregion

        #region ReportCamResState
        public static XmlDocument ReportCamResState(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId;
            List<string> resId;
            List<string> state;

            saId = XmlOp.GetInnerText(Doc, "saId");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            state = XmlOp.GetInnerTextList(Doc, "state");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ReportCamResState");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            for (int i = 0; i < 3;i++ )
            {
                XmlOp.ElementAdd(Response, "parameters", "URL");
                XmlOp.ElementAdd(Response, "URL", "resId", i);
                XmlOp.SetNodeInnerText(Response, "resId", i, "test");
            }
            
            Response.Save("D://response-ReportCamResState.xml");

            return Response;
        }
        #endregion

        #region UserResReport
        public static XmlDocument UserResReport(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saName, totalPkt, pktNum;
            List<string> id;
            List<string> name;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saName = XmlOp.GetInnerText(Doc, "saName");
            totalPkt = XmlOp.GetInnerText(Doc, "totalPkt");
            pktNum = XmlOp.GetInnerText(Doc, "pktNum");
            id = XmlOp.GetInnerTextList(Doc, "ip");
            name = XmlOp.GetInnerTextList(Doc, "name");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "UserResReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");

            Response.Save("D://response-UserResReport.xml");

            return Response;
        }
        #endregion

        #region UserResChange
        public static XmlDocument UserResChange(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, totalPkt, pktNum, cmd;
            List<string> id;
            List<string> name;

            saId = XmlOp.GetInnerText(Doc, "saId");
            totalPkt = XmlOp.GetInnerText(Doc, "totalPkt");
            pktNum = XmlOp.GetInnerText(Doc, "pktNum");
            cmd = XmlOp.GetInnerText(Doc, "cmd");
            id = XmlOp.GetInnerTextList(Doc, "ip");
            name = XmlOp.GetInnerTextList(Doc, "name");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "UserResChange");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");

            Response.Save("D://response-UserResChange.xml");

            return Response;
        }
        #endregion

        #region AlarmResListReport
        public static XmlDocument AlarmResListReport(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saName, totalPkt, pktNum;
            List<string> id;
            List<string> name;
            List<string> discription;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saName = XmlOp.GetInnerText(Doc, "saName");
            totalPkt = XmlOp.GetInnerText(Doc, "totalPkt");
            pktNum = XmlOp.GetInnerText(Doc, "pktNum");
            id = XmlOp.GetInnerTextList(Doc, "ip");
            name = XmlOp.GetInnerTextList(Doc, "name");
            discription = XmlOp.GetInnerTextList(Doc, "discription");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "AlarmResListReport");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saId");
            XmlOp.SetNodeInnerText(Response, "saId", 0, "saId");

            Response.Save("D://response-AlarmResListReport.xml");

            return Response;
        }
        #endregion

        #region AlarmResListChange
        public static XmlDocument AlarmResListChange(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saName, num, totalPkt, pktNum, cmd;
            List<string> id;
            List<string> name;
            List<string> type;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saName = XmlOp.GetInnerText(Doc, "saName");
            num = XmlOp.GetInnerText(Doc, "num");
            totalPkt = XmlOp.GetInnerText(Doc, "totalPkt");
            pktNum = XmlOp.GetInnerText(Doc, "pktNum");
            cmd = XmlOp.GetInnerText(Doc, "cmd");
            id = XmlOp.GetInnerTextList(Doc, "ip");
            name = XmlOp.GetInnerTextList(Doc, "name");
            type = XmlOp.GetInnerTextList(Doc, "type");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "AlarmResListChange");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saId");
            XmlOp.SetNodeInnerText(Response, "saId", 0, "saId");

            Response.Save("D://response-AlarmResListChange.xml");

            return Response;
        }
        #endregion

        #region StartMediaState
        public static XmlDocument StartMediaState(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userName, userLevel, mediaType, linkMode, targetIpAddr, targetPort, flag;
            List<string> id;
            List<string> name;
            List<string> type;

            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userName = XmlOp.GetInnerText(Doc, "userName");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            mediaType = XmlOp.GetInnerText(Doc, "mediaType");
            linkMode = XmlOp.GetInnerText(Doc, "linkMode");
            targetIpAddr = XmlOp.GetInnerText(Doc, "targetIpAddr");
            targetPort = XmlOp.GetInnerText(Doc, "targetPort");
            flag = XmlOp.GetInnerText(Doc, "flag");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartMediaState");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "sessionId");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "tcpIp");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "tcpPort");

            Response.Save("D://response-StartMediaState.xml");

            return Response;
        }
        #endregion

        #region InfoOrder
        public static XmlDocument InfoOrder(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId, userId, userLevel;
            List<string> id;
            List<string> name;
            List<string> type;

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "InfoOrder");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "sessionId");

            Response.Save("D://response-InfoOrder.xml");

            return Response;
        }
        #endregion

        #region ReportAlarmRes
        public static XmlDocument ReportAlarmRes(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string saId, saName;
            List<string> id;
            List<string> type;
            List<string> startTime;
            List<string> endTime;
            List<string> targetInfo;
            List<string> level;
            List<string> state;
            List<string> description;
            List<string> alarmHisRecord;
            List<string> resId;
            List<string> time;

            saId = XmlOp.GetInnerText(Doc, "saId");
            saName = XmlOp.GetInnerText(Doc, "saName");
            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");
            startTime = XmlOp.GetInnerTextList(Doc, "startTime");
            endTime = XmlOp.GetInnerTextList(Doc, "endTime");
            targetInfo = XmlOp.GetInnerTextList(Doc, "targetInfo");
            level = XmlOp.GetInnerTextList(Doc, "level");
            state = XmlOp.GetInnerTextList(Doc, "state");
            description = XmlOp.GetInnerTextList(Doc, "description");
            alarmHisRecord = XmlOp.GetInnerTextList(Doc, "alarmHisRecord");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            time = XmlOp.GetInnerTextList(Doc, "time");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ReportAlarmRes");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "saId");
            XmlOp.SetNodeInnerText(Response, "saId", 0, "saId");
            Response.Save("D://response-ReportAlarmRes.xml");

            return Response;
        }
        #endregion

        #endregion

        #region Up 2 Down

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
        public static XmlDocument StartMediaReq(string resId, string userId, string userLevel, string mediaType, string linkMode, string targetIpAddr, string targetPort, string flag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "request", 0, "command", "StartMediaReq");
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
            return Request;
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
        public static XmlDocument ControlPTZ(string resId, string userId, string userLevel, string cmd, string param, string speed)
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

            return Request;
        }
        public static XmlDocument ControlPTZ(string userLevel, string cmd, string param)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();
            //*测试*直接对resId，userId，speed赋值
            string resId = "1111111", userId = "22222222", speed = "7777";
            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "request", 0, "command", "ControlPTZ");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "resId");//test
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");//test
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

            return Request;
        }

        public static XmlDocument ControlPTZTranslate(XmlDocument doc)
        {
            XmlTools XmlOp = new XmlTools();
            string[] parameters;
            //string[] paraNames = { "resId", "userId", "userLevel", "cmd", "param", "speed" };//原
            string[] paraNames = { "level", "cmd", "parameter"};
            parameters = XmlOp.GetAttribute(doc, "PTZControl", paraNames);
            //return ControlPTZ(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);//原
            return ControlPTZ(parameters[0], parameters[1], parameters[2]);
        }
        #endregion

        #region StopMediaReq
        //public static byte[] StopMediaReq(string sessionId, string resId, string stopFlag)
        //{
        //    XmlTools XmlOp = new XmlTools();
        //    XmlDocument Request = XmlOp.XmlCreate();

        //    XmlOp.ElementAdd(Request, null, "request");
        //    XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StopMediaReq");
        //    XmlOp.ElementAdd(Request, "request", "parameters");
        //    XmlOp.ElementAdd(Request, "parameters", "sessionId");
        //    XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
        //    XmlOp.ElementAdd(Request, "parameters", "resId");
        //    XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
        //    XmlOp.ElementAdd(Request, "parameters", "stopFlag");
        //    XmlOp.SetNodeInnerText(Request, "stopFlag", 0, stopFlag);
        //    Request.Save("D://StopMediaReq-response.xml");

        //    return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        //}
        public static XmlDocument StopMediaReq(string sessionId, string resId, string stopFlag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();
            
            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "request", 0, "command", "StopMediaReq");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "stopFlag");
            XmlOp.SetNodeInnerText(Request, "stopFlag", 0, stopFlag);
            Request.Save("D://StopMediaReq-response.xml");

            return Request;
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
        //public static byte[] StartPlayBack(string resId, string userId, string userLevel, string startTime, string endTime, int linkMode, string targetIpAddr, string targetPort, int flag, int locationFlag)//原
        public static XmlDocument StartPlayBack(string resId, string userId, string userLevel, string startTime, string endTime, int linkMode, string targetIpAddr, string targetPort, int flag, int locationFlag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "request", 0, "command", "StartPlayBack");
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
            XmlOp.ElementAdd(Request, "parameters", "linkMode");
            XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode.ToString());
            XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
            XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
            XmlOp.ElementAdd(Request, "parameters", "targetPort");
            XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
            XmlOp.ElementAdd(Request, "parameters", "flag");
            XmlOp.SetNodeInnerText(Request, "flag", 0, flag.ToString());
            XmlOp.ElementAdd(Request, "parameters", "locationFlag");
            XmlOp.SetNodeInnerText(Request, "locationFlag", 0, locationFlag.ToString());
            Request.Save("D://StartPlayBack-response.xml");

            //return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));//原
            return Request;

        }

        //public static XmlDocument StartPlayBack(XmlDocument doc)
        //{
        //    XmlTools XmlOp = new XmlTools();
        //    XmlDocument Request = XmlOp.XmlCreate();
        //    string resId, userId, userLevel, startTime, endTime, targetIpAddr, targetPort;
        //    int flag, locationFlag, linkMode;


        //    //saId = XmlOp.GetInnerText(Doc, "saId");
        //    //resId = XmlOp.GetInnerTextList(Doc, "resId");
        //    //state = XmlOp.GetInnerTextList(Doc, "state");

        //    XmlOp.ElementAdd(Request, null, "request");
        //    XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartPlayBack");
        //    XmlOp.ElementAdd(Request, "request", "parameters");
        //    XmlOp.ElementAdd(Request, "parameters", "resId");
        //    XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
        //    XmlOp.ElementAdd(Request, "parameters", "userId");
        //    XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
        //    XmlOp.ElementAdd(Request, "parameters", "userLevel");
        //    XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
        //    XmlOp.ElementAdd(Request, "parameters", "startTime");
        //    XmlOp.SetNodeInnerText(Request, "startTime", 0, startTime);
        //    XmlOp.ElementAdd(Request, "parameters", "endTime");
        //    XmlOp.SetNodeInnerText(Request, "endTime", 0, endTime);
        //    XmlOp.ElementAdd(Request, "parameters", "linkMode");
        //    XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode.ToString());
        //    XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
        //    XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
        //    XmlOp.ElementAdd(Request, "parameters", "targetPort");
        //    XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
        //    XmlOp.ElementAdd(Request, "parameters", "flag");
        //    XmlOp.SetNodeInnerText(Request, "flag", 0, flag.ToString());
        //    XmlOp.ElementAdd(Request, "parameters", "locationFlag");
        //    XmlOp.SetNodeInnerText(Request, "locationFlag", 0, locationFlag.ToString());
        //    Request.Save("D://StartPlayBack-response.xml");

        //    return Request;
        //}

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

        #region StartHisLoad
        //public static byte[] StartHisLoad(string resId, string userId, string userLevel, string startTime, string endTime, int linkMode, string targetIpAddr, string targetPort, int flag, int locationFlag)
        //{
        //    XmlTools XmlOp = new XmlTools();
        //    XmlDocument Request = XmlOp.XmlCreate();

        //    XmlOp.ElementAdd(Request, null, "request");
        //    XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartPlayBack");
        //    XmlOp.ElementAdd(Request, "request", "parameters");
        //    XmlOp.ElementAdd(Request, "parameters", "resId");
        //    XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
        //    XmlOp.ElementAdd(Request, "parameters", "userId");
        //    XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
        //    XmlOp.ElementAdd(Request, "parameters", "userLevel");
        //    XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
        //    XmlOp.ElementAdd(Request, "parameters", "startTime");
        //    XmlOp.SetNodeInnerText(Request, "startTime", 0, startTime);
        //    XmlOp.ElementAdd(Request, "parameters", "endTime");
        //    XmlOp.SetNodeInnerText(Request, "endTime", 0, endTime);
        //    XmlOp.ElementAdd(Request, "parameters", "linkMode");
        //    XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode.ToString());
        //    XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
        //    XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
        //    XmlOp.ElementAdd(Request, "parameters", "targetPort");
        //    XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
        //    XmlOp.ElementAdd(Request, "parameters", "flag");
        //    XmlOp.SetNodeInnerText(Request, "flag", 0, flag.ToString());
        //    XmlOp.ElementAdd(Request, "parameters", "locationFlag");
        //    XmlOp.SetNodeInnerText(Request, "locationFlag", 0, locationFlag.ToString());
        //    Request.Save("D://StartHisLoad-response.xml");

        //    return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        //}
        public static XmlDocument StartHisLoad(string resId, string userId, string userLevel, string startTime, string endTime, int linkMode, string targetIpAddr, string targetPort, int flag, int locationFlag)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "request", 0, "command", "StartHisLoad");
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
            XmlOp.ElementAdd(Request, "parameters", "linkMode");
            XmlOp.SetNodeInnerText(Request, "linkMode", 0, linkMode.ToString());
            XmlOp.ElementAdd(Request, "parameters", "targetIpAddr");
            XmlOp.SetNodeInnerText(Request, "targetIpAddr", 0, targetIpAddr);
            XmlOp.ElementAdd(Request, "parameters", "targetPort");
            XmlOp.SetNodeInnerText(Request, "targetPort", 0, targetPort);
            XmlOp.ElementAdd(Request, "parameters", "flag");
            XmlOp.SetNodeInnerText(Request, "flag", 0, flag.ToString());
            XmlOp.ElementAdd(Request, "parameters", "locationFlag");
            XmlOp.SetNodeInnerText(Request, "locationFlag", 0, locationFlag.ToString());
            Request.Save("D://StartHisLoad-response.xml");

            return Request;
        }

        public static string[] StartHisLoadResponse(XmlDocument Doc)
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

        #region HisLoadInfo
        public static byte[] HisLoadInfo(string sessionId, string resId, string userId, string userLevel)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "StartPlayBack");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            Request.Save("D://HisLoadInfo-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] HisLoadInfoResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string sessionId;
            string[] result = new string[1];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");

            result[0] = sessionId;

            return result;
        }
        #endregion

        #region INFO
        public static byte[] INFO(string sessionId, string resId, string userId, string userLevel)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "INFO");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            Request.Save("D://INFO-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] INFOResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string sessionId;
            string[] result = new string[1];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");

            result[0] = sessionId;

            return result;
        }
        #endregion

        #region HisInfo
        public static byte[] HisInfo(string sessionId, string resId, string userId, string userLevel)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "HisInfo");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "userId");
            XmlOp.SetNodeInnerText(Request, "userId", 0, userId);
            XmlOp.ElementAdd(Request, "parameters", "userLevel");
            XmlOp.SetNodeInnerText(Request, "userLevel", 0, userLevel);
            Request.Save("D://HisInfo-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] HisInfoResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string sessionId;
            string[] result = new string[1];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");

            result[0] = sessionId;

            return result;
        }
        #endregion

        #region ControlFileBack
        public static byte[] ControlFileBack(string sessionId, string resId, string cmd, int param)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ControlFileBack");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Request, "sessionId", 0, sessionId);
            XmlOp.ElementAdd(Request, "parameters", "resId");
            XmlOp.SetNodeInnerText(Request, "resId", 0, resId);
            XmlOp.ElementAdd(Request, "parameters", "cmd");
            XmlOp.SetNodeInnerText(Request, "cmd", 0, cmd);
            XmlOp.ElementAdd(Request, "parameters", "param");
            XmlOp.SetNodeInnerText(Request, "param", 0, param.ToString());
            Request.Save("D://ControlFileBack-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] ControlFileBackResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string sessionId;
            string[] result = new string[1];

            sessionId = XmlOp.GetInnerText(Doc, "sessionId");

            result[0] = sessionId;

            return result;
        }
        #endregion

        #region ReqCamResState
        public static byte[] ReqCamResState(string saId, string[] resId, int resNum)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ReqCamResState");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < resNum;i++ )
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "resId", i);
                XmlOp.SetNodeInnerText(Request, "resId", i, resId[i]);
            }
            Request.Save("D://ReqCamResState-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] ReqCamResStateResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            List<string> resId = new List<string>();
            List<string> state = new List<string>();
            string[] result = new string[1];

            resId = XmlOp.GetInnerTextList(Doc, "resId");
            state = XmlOp.GetInnerTextList(Doc, "state");

            return result;
        }
        #endregion

        #region GetUserCurState
        public static byte[] GetUserCurState(string saId, string curUserId)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "GetUserCurState");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "curUserId");
            XmlOp.SetNodeInnerText(Request, "curUserId", 0, curUserId);
            Request.Save("D://GetUserCurState-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] GetUserCurStateResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string saId, curUserId, userIp, userState;
            List<string> camId = new List<string>();
            List<string> camName = new List<string>();
            string[] result = new string[4];

            saId = XmlOp.GetInnerText(Doc, "saId");
            curUserId = XmlOp.GetInnerText(Doc, "curUserId");
            userIp = XmlOp.GetInnerText(Doc, "userIp");
            userState = XmlOp.GetInnerText(Doc, "userState");

            camId = XmlOp.GetInnerTextList(Doc, "camId");
            camName = XmlOp.GetInnerTextList(Doc, "camName");

            return result;
        }
        #endregion

        #region SetUserCamManage
        public static byte[] SetUserCamManage(string cuId, string cuLevel, int action, string startTime, string endTime, string schduleCreatTime, string[] cameId, int cameIdNum, string[] id, int idNum)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "SetUserCamManage");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "cuId");
            XmlOp.SetNodeInnerText(Request, "cuId", 0, cuId);
            XmlOp.ElementAdd(Request, "parameters", "cuLevel");
            XmlOp.SetNodeInnerText(Request, "cuLevel", 0, cuLevel);
            XmlOp.ElementAdd(Request, "parameters", "action");
            XmlOp.SetNodeInnerText(Request, "action", 0, action.ToString());
            XmlOp.ElementAdd(Request, "parameters", "startTime");
            XmlOp.SetNodeInnerText(Request, "startTime", 0, startTime);
            XmlOp.ElementAdd(Request, "parameters", "endTime");
            XmlOp.SetNodeInnerText(Request, "endTime", 0, endTime);
            XmlOp.ElementAdd(Request, "parameters", "schduleCreatTime");
            XmlOp.SetNodeInnerText(Request, "schduleCreatTime", 0, schduleCreatTime);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < cameIdNum;i++ )
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "cameId", i);
                XmlOp.SetNodeInnerText(Request, "cameId", i, cameId[i]);
            }
            XmlOp.ElementAdd(Request, "parameters", "whiteUser");
            for (int i = 0; i < cameIdNum; i++)
            {
                XmlOp.ElementAdd(Request, "whiteUser", "URL");
                XmlOp.ElementAdd(Request, "URL", "id", i);
                XmlOp.SetNodeInnerText(Request, "id", i, id[i]);
            }

            Request.Save("D://SetUserCamManage-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] SetUserCamManageResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string[] result = null;                   

            return result;
        }
        #endregion

        #region AlarmResSubscribe
        public static byte[] AlarmResSubscribe(string saId, string saName, int action, string[] id, string[] type, int num)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "AlarmResSubscribe");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "saName");
            XmlOp.SetNodeInnerText(Request, "saName", 0, saName);
            XmlOp.ElementAdd(Request, "parameters", "action");
            XmlOp.SetNodeInnerText(Request, "action", 0, action.ToString());
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < num; i++)
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "id", i);
                XmlOp.SetNodeInnerText(Request, "id", i, id[i]);
                XmlOp.ElementAdd(Request, "URL", "type", i);
                XmlOp.SetNodeInnerText(Request, "type", i, type[i]);
            }

            Request.Save("D://AlarmResSubscribe-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] AlarmResSubscribeResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string saId;
            string[] result = new string[1];

            saId = XmlOp.GetInnerText(Doc, "saId");

            result[0] = saId;

            return result;
        }
        #endregion

        #region QueryAlarmRes
        public static byte[] QueryAlarmRes(string saId, string saName, string[] id, string[] type, int num)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "QueryAlarmRes");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "saName");
            XmlOp.SetNodeInnerText(Request, "saName", 0, saName);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < num; i++)
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "id", i);
                XmlOp.SetNodeInnerText(Request, "id", i, id[i]);
                XmlOp.ElementAdd(Request, "URL", "type", i);
                XmlOp.SetNodeInnerText(Request, "type", i, type[i]);
            }

            Request.Save("D://QueryAlarmRes-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] QueryAlarmResResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            List<string> id = new List<string>();
            List<string> type = new List<string>();
            List<string> time = new List<string>();
            List<string> state = new List<string>();
            List<string> alarmHisRecord = new List<string>();
            List<string> resId = new List<string>();
            List<string> time2 = new List<string>();
            string[] result = null;

            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");
            time = XmlOp.GetInnerTextListByPath(Doc, "/response/parameters/group/URL/time");
            state = XmlOp.GetInnerTextList(Doc, "state");
            alarmHisRecord = XmlOp.GetInnerTextList(Doc, "alarmHisRecord");
            resId = XmlOp.GetInnerTextList(Doc, "resId");
            time2 = XmlOp.GetInnerTextListByPath(Doc, "/response/parameters/group/URL/url/time");


            return result;
        }
        #endregion

        #region ReportAlarmInfo
        public static byte[] ReportAlarmInfo(string muId, string muName, string[] id, string[] type, string[] startTime, string[] endTime, int num)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ReportAlarmInfo");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "muId");
            XmlOp.SetNodeInnerText(Request, "muId", 0, muId);
            XmlOp.ElementAdd(Request, "parameters", "muName");
            XmlOp.SetNodeInnerText(Request, "muName", 0, muName);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < num; i++)
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "id", i);
                XmlOp.SetNodeInnerText(Request, "id", i, id[i]);
                XmlOp.ElementAdd(Request, "URL", "type", i);
                XmlOp.SetNodeInnerText(Request, "type", i, type[i]);
                XmlOp.ElementAdd(Request, "URL", "startTime", i);
                XmlOp.SetNodeInnerText(Request, "startTime", i, startTime[i]);
                XmlOp.ElementAdd(Request, "URL", "endTime", i);
                XmlOp.SetNodeInnerText(Request, "endTime", i, endTime[i]);
            }

            Request.Save("D://ReportAlarmInfo-response.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] ReportAlarmInfoResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string muId;
            List<string> id = new List<string>();
            List<string> type = new List<string>();
            List<string> startTime = new List<string>();
            List<string> endTime = new List<string>();
            List<string> message = new List<string>();
            string[] result = null;

            muId = XmlOp.GetInnerText(Doc, "muId");
            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");
            startTime = XmlOp.GetInnerTextList(Doc, "startTime");
            endTime = XmlOp.GetInnerTextList(Doc, "endTime");
            message = XmlOp.GetInnerTextList(Doc, "message");

            return result;
        }
        #endregion

        #region ResTransOrder
        public static byte[] ResTransOrder(string saId, string totalPacketNum, string curPacketNum, string[] resId, string[] name, string[] location, string[] purpose, string[] infomation, int num)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ResTransOrder");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "totalPacketNum");
            XmlOp.SetNodeInnerText(Request, "totalPacketNum", 0, totalPacketNum);
            XmlOp.ElementAdd(Request, "parameters", "curPacketNum");
            XmlOp.SetNodeInnerText(Request, "curPacketNum", 0, curPacketNum);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < num; i++)
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "resId", i);
                XmlOp.SetNodeInnerText(Request, "resId", i, resId[i]);
                XmlOp.ElementAdd(Request, "URL", "name", i);
                XmlOp.SetNodeInnerText(Request, "name", i, name[i]);
                XmlOp.ElementAdd(Request, "URL", "location", i);
                XmlOp.SetNodeInnerText(Request, "location", i, location[i]);
                XmlOp.ElementAdd(Request, "URL", "purpose", i);
                XmlOp.SetNodeInnerText(Request, "purpose", i, purpose[i]);
                XmlOp.ElementAdd(Request, "URL", "infomation", i);
                XmlOp.SetNodeInnerText(Request, "infomation", i, infomation[i]);
            }

            Request.Save("D://request-ResTransOrder.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] ResTransOrderResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string saId;
            string[] result = null;

            saId = XmlOp.GetInnerText(Doc, "saId");

            return result;
        }
        #endregion

        #region ResChangeOrder
        public static byte[] ResChangeOrder(string saId, string cmd, string[] resId, string[] name, string[] location, string[] purpose, string[] infomation, int num)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Request = XmlOp.XmlCreate();

            XmlOp.ElementAdd(Request, null, "request");
            XmlOp.SetNodeAttribute(Request, "response", 0, "command", "ResChangeOrder");
            XmlOp.ElementAdd(Request, "request", "parameters");
            XmlOp.ElementAdd(Request, "parameters", "saId");
            XmlOp.SetNodeInnerText(Request, "saId", 0, saId);
            XmlOp.ElementAdd(Request, "parameters", "cmd");
            XmlOp.SetNodeInnerText(Request, "cmd", 0, cmd);
            XmlOp.ElementAdd(Request, "parameters", "group");
            for (int i = 0; i < num; i++)
            {
                XmlOp.ElementAdd(Request, "group", "URL");
                XmlOp.ElementAdd(Request, "URL", "resId", i);
                XmlOp.SetNodeInnerText(Request, "resId", i, resId[i]);
                XmlOp.ElementAdd(Request, "URL", "name", i);
                XmlOp.SetNodeInnerText(Request, "name", i, name[i]);
                XmlOp.ElementAdd(Request, "URL", "location", i);
                XmlOp.SetNodeInnerText(Request, "location", i, location[i]);
                XmlOp.ElementAdd(Request, "URL", "purpose", i);
                XmlOp.SetNodeInnerText(Request, "purpose", i, purpose[i]);
                XmlOp.ElementAdd(Request, "URL", "infomation", i);
                XmlOp.SetNodeInnerText(Request, "infomation", i, infomation[i]);
            }

            Request.Save("D://request-ResChangeOrder.xml");

            return Encoding.GetEncoding("GB2312 ").GetBytes(sip.SIPRequest(Request));
        }

        public static string[] ResChangeOrderResponse(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            string[] result = null;

            return result;
        }
        #endregion

        #endregion
    }
}
