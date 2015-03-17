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
        DataBase database;
        public InterfaceC()
        {
            database = new DataBase();
        }

        public InterfaceC(DataBase Database)
        {
            database = Database;
        }
        public XmlDocument SaRegister(XmlDocument Doc)
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

        public XmlDocument ResReport(XmlDocument Doc)
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

            for (int i = 0; i < resId.Count; i++ )
            {
                values[0] = resId[i];
                values[1] = name[i];
                values[2] = location[i];
                values[3] = infomation[i];
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

        public XmlDocument StartMediaReq(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, mediaType, linkMode, targetIpAddr, targetPort, flag;

            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            mediaType = XmlOp.GetInnerText(Doc, "mediaType");
            linkMode = XmlOp.GetInnerText(Doc, "linkMode");
            targetIpAddr = XmlOp.GetInnerText(Doc, "targetIpAddr");
            targetPort = XmlOp.GetInnerText(Doc, "targetPort");
            flag = XmlOp.GetInnerText(Doc, "flag");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartMediaReq");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "XXXX");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "XXXX");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "XXXX");
            Response.Save("D://StartMediaReq-response.xml");

            return Response;
        }

        private void DataOp()
        {

        }
    }
}
