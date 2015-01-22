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
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId, muName;
            int action;
            List<string> id = new List<string>();
            List<string> type = new List<string>();

            muId = XmlOp.GetInnerText(Doc, "muId");
            muName = XmlOp.GetInnerText(Doc, "muName");
            action = Int16.Parse(XmlOp.GetInnerText(Doc, "action"));
            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");

            
            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "AlarmResSubscribe");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "muId");
            XmlOp.SetNodeInnerText(Response, "muId", 0, muId);
            Response.Save("D://AlarmResSubscribe-response.xml");

            return Response;
        }

        public XmlDocument ControlFileBack(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId, cmd;
            int param;
            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            cmd = XmlOp.GetInnerText(Doc, "cmd");
            param = Int16.Parse(XmlOp.GetInnerText(Doc, "param"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ControlFileBack");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, sessionId);
            Response.Save("D://ControlFileBack-response.xml");

            return Response;
        }

        public XmlDocument ControlPTZ(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, cmd, param;
            int speed;
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            cmd = XmlOp.GetInnerText(Doc, "cmd");
            param = XmlOp.GetInnerText(Doc, "param");
            speed = Int16.Parse(XmlOp.GetInnerText(Doc, "speed"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ControlPTZ");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "resId");
            XmlOp.SetNodeInnerText(Response, "resId", 0, resId);
            Response.Save("D://ControlPTZ-response.xml");

            return Response;
        }

        public XmlDocument GetUserCurState(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId, curUserId;
            muId = XmlOp.GetInnerText(Doc, "muId");
            curUserId = XmlOp.GetInnerText(Doc, "curUserId");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ControlPTZ");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "muId");
            XmlOp.SetNodeInnerText(Response, "muId", 0, muId);
            XmlOp.ElementAdd(Response, "parameters", "curUserId");
            XmlOp.SetNodeInnerText(Response, "curUserId", 0, curUserId);
            XmlOp.ElementAdd(Response, "parameters", "userIp");
            XmlOp.SetNodeInnerText(Response, "userIp", 0, "192.168.1.101");
            XmlOp.ElementAdd(Response, "parameters", "userState");
            XmlOp.SetNodeInnerText(Response, "userState", 0, "0");
            XmlOp.ElementAdd(Response, "parameters", "group");
            for (int i = 0; i < 5; i++ )
            {
                XmlOp.ElementAdd(Response, "group", "URL");
                XmlOp.ElementAdd(Response, "URL", "id", i);
                XmlOp.SetNodeInnerText(Response, "id", i, i.ToString());
                XmlOp.ElementAdd(Response, "URL", "name", i);
                XmlOp.SetNodeInnerText(Response, "name", i, "name" + i.ToString());

            }
            Response.Save("D://GetUserCurState-response.xml");

            return Response;
        }

        public XmlDocument HisInfo(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId, userId, userLevel;
            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "HisInfo");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, sessionId);
            Response.Save("D://HisInfo-response.xml");

            return Response;
        }

        public XmlDocument HisLoadInfo(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId, userId, userLevel;
            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "HisLoadInfo");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, sessionId);
            Response.Save("D://HisLoadInfo-response.xml");

            return Response;
        }

        public XmlDocument INFO(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId, userId, userLevel;
            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "INFO");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, sessionId);
            Response.Save("D://INFO-response.xml");

            return Response;
        }

        public XmlDocument QueryAlarmRes(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId, muName;
            List<string> id = new List<string>();
            List<string> type = new List<string>();
            muId = XmlOp.GetInnerText(Doc, "muId");
            muName = XmlOp.GetInnerText(Doc, "muName");
            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "QueryAlarmRes");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "group");
            for (int i = 0; i < 5; i++ )
            {
                XmlOp.ElementAdd(Response, "group", "URL");
                XmlOp.ElementAdd(Response, "URL", "id", i);
                XmlOp.SetNodeInnerText(Response, "id", i, "00000" + i);
                XmlOp.ElementAdd(Response, "URL", "type", i);
                XmlOp.SetNodeInnerText(Response, "type", i, "00000" + i);
                XmlOp.ElementAdd(Response, "URL", "time", i);
                XmlOp.SetNodeInnerText(Response, "URL/time", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
                XmlOp.ElementAdd(Response, "URL", "state", i);
                XmlOp.SetNodeInnerText(Response, "state", i, "0");
                XmlOp.ElementAdd(Response, "URL", "alarmHisRecord", i);
                XmlOp.SetNodeInnerText(Response, "alarmHisRecord", i, "0");
                XmlOp.ElementAdd(Response, "URL", "url", i);
                XmlOp.ElementAdd(Response, "url", "resId", i);
                XmlOp.SetNodeInnerText(Response, "url/resId", i, "000000");
                XmlOp.ElementAdd(Response, "url", "time", i);
                XmlOp.SetNodeInnerText(Response, "url/time", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
            }
            Response.Save("D://QueryAlarmRes-response.xml");

            return Response;
        }

        public XmlDocument QueryHistoryFiles(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, cuId, fromDate, toDate;
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            cuId = XmlOp.GetInnerText(Doc, "cuId");
            fromDate = XmlOp.GetInnerText(Doc, "fromDate");
            toDate = XmlOp.GetInnerText(Doc, "toDate");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "QueryHistoryFiles");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "resId");
            XmlOp.SetNodeInnerText(Response, "resId", 0, resId);
            XmlOp.ElementAdd(Response, "parameters", "cuId");
            XmlOp.SetNodeInnerText(Response, "cuId", 0, cuId);
            XmlOp.ElementAdd(Response, "parameters", "totalNumber");
            XmlOp.SetNodeInnerText(Response, "totalNumber", 0, (100).ToString());
            XmlOp.ElementAdd(Response, "parameters", "curNumber");
            XmlOp.SetNodeInnerText(Response, "curNumber", 0, (50).ToString());
            XmlOp.ElementAdd(Response, "parameters", "group");
            for (int i = 0; i < 5; i++ )
            {
                XmlOp.ElementAdd(Response, "group", "URL");
                XmlOp.ElementAdd(Response, "URL", "startTime",i);
                XmlOp.SetNodeInnerText(Response, "startTime", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
                XmlOp.ElementAdd(Response, "URL", "endTime", i);
                XmlOp.SetNodeInnerText(Response, "endTime", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
                XmlOp.ElementAdd(Response, "URL", "size", i);
                XmlOp.SetNodeInnerText(Response, "size", i, (100*i).ToString());
            }
            Response.Save("D://QueryHistoryFiles-response.xml");

            return Response;
        }

        public XmlDocument ReportAlarmInfo(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId, muName;
            List<string> id, type, startTime, endTime;
            muId = XmlOp.GetInnerText(Doc, "muId");
            muName = XmlOp.GetInnerText(Doc, "muName");
            id = XmlOp.GetInnerTextList(Doc, "id");
            type = XmlOp.GetInnerTextList(Doc, "type");
            startTime = XmlOp.GetInnerTextList(Doc, "startTime");
            endTime = XmlOp.GetInnerTextList(Doc, "endTime");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ReportAlarmInfo");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "muId");
            XmlOp.SetNodeInnerText(Response, "muId", 0, muId);
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "group");
            for (int i = 0; i < 5; i++)
            {
                XmlOp.ElementAdd(Response, "group", "URL");
                XmlOp.ElementAdd(Response, "URL", "id", i);
                XmlOp.SetNodeInnerText(Response, "id", i, "00000" + i);
                XmlOp.ElementAdd(Response, "URL", "type", i);
                XmlOp.SetNodeInnerText(Response, "type", i, "00000" + i);
                XmlOp.ElementAdd(Response, "URL", "startTime", i);
                XmlOp.SetNodeInnerText(Response, "startTime", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
                XmlOp.ElementAdd(Response, "URL", "endTime", i);
                XmlOp.SetNodeInnerText(Response, "endTime", i, DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"));
                XmlOp.ElementAdd(Response, "URL", "message", i);
                XmlOp.SetNodeInnerText(Response, "message", i, "No message");
            }
            Response.Save("D://ReportAlarmInfo-response.xml");

            return Response;
        }

        public XmlDocument ReqCamResState(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId;
            List<string> resId;
            muId = XmlOp.GetInnerText(Doc, "muId");
            resId = XmlOp.GetInnerTextList(Doc, "resId");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ReqCamResState");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "group");
            for (int i = 0; i < resId.Count; i++)
            {
                XmlOp.ElementAdd(Response, "group", "URL");
                XmlOp.ElementAdd(Response, "URL", "resId", i);
                XmlOp.SetNodeInnerText(Response, "resId", i, resId[i]);
                XmlOp.ElementAdd(Response, "URL", "state", i);
                XmlOp.SetNodeInnerText(Response, "state", i, "online");
            }
            Response.Save("D://ReqCamResState-response.xml");

            return Response;
        }

        public XmlDocument ResChangeOrder(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId;
            List<string> resId;
            muId = XmlOp.GetInnerText(Doc, "muId");
            resId = XmlOp.GetInnerTextList(Doc, "resId");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResChangeOrder");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "muId");
            XmlOp.SetNodeInnerText(Response, "muId", 0, muId);
            Response.Save("D://ResChangeOrder-response.xml");

            return Response;
        }

        public XmlDocument ResTransOrder(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId;
            List<string> resId;
            muId = XmlOp.GetInnerText(Doc, "muId");
            resId = XmlOp.GetInnerTextList(Doc, "resId");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "ResTransOrder");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "muId");
            XmlOp.SetNodeInnerText(Response, "muId", 0, muId);
            Response.Save("D://ResTransOrder-response.xml");

            return Response;
        }

        public XmlDocument SetUserCamManage(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string cuId, cuLevel, startTime, endTime, schduleCreatTime;
            int action;
            List<string> camId,id;
            cuId = XmlOp.GetInnerText(Doc, "cuId");
            cuLevel = XmlOp.GetInnerText(Doc, "cuLevel");
            action = Int16.Parse(XmlOp.GetInnerText(Doc, "action"));
            startTime = XmlOp.GetInnerText(Doc, "startTime");
            endTime = XmlOp.GetInnerText(Doc, "endTime");
            schduleCreatTime = XmlOp.GetInnerText(Doc, "schduleCreatTime");
            camId = XmlOp.GetInnerTextList(Doc, "camId");
            id = XmlOp.GetInnerTextList(Doc, "id");

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "SetUserCamManage");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "cuId");
            XmlOp.SetNodeInnerText(Response, "cuId", 0, cuId);
            Response.Save("D://SetUserCamManage-response.xml");

            return Response;
        }

        public XmlDocument StartHisLoad(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, startTime, endTime, targetIpAddr, targetPort;
            int linkMode, flag, locationFlag;
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            startTime = XmlOp.GetInnerText(Doc, "startTime");
            endTime = XmlOp.GetInnerText(Doc, "endTime");
            linkMode = Int16.Parse(XmlOp.GetInnerText(Doc, "linkMode"));
            targetIpAddr = XmlOp.GetInnerText(Doc, "targetIpAddr");
            targetPort = XmlOp.GetInnerText(Doc, "targetPort");
            flag = Int16.Parse(XmlOp.GetInnerText(Doc, "flag"));
            locationFlag = Int16.Parse(XmlOp.GetInnerText(Doc, "locationFlag"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartHisLoad");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "000000");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "192.168.1.101");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "10000");
            Response.Save("D://StartHisLoad-response.xml");

            return Response;
        }

        public XmlDocument StartMediaReq(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, targetIpAddr, targetPort;
            int mediaType, linkMode, flag;
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            mediaType = Int16.Parse(XmlOp.GetInnerText(Doc, "mediaType"));
            linkMode = Int16.Parse(XmlOp.GetInnerText(Doc, "linkMode"));
            targetIpAddr = XmlOp.GetInnerText(Doc, "targetIpAddr");
            targetPort = XmlOp.GetInnerText(Doc, "targetPort");
            flag = Int16.Parse(XmlOp.GetInnerText(Doc, "flag"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartMediaReq");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "000000");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "192.168.1.101");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "10000");
            Response.Save("D://StartMediaReq-response.xml");

            return Response;
        }

        public XmlDocument StartPlayBack(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string resId, userId, userLevel, startTime, endTime, targetIpAddr, targetPort;
            int linkMode, flag, locationFlag;
            resId = XmlOp.GetInnerText(Doc, "resId");
            userId = XmlOp.GetInnerText(Doc, "userId");
            userLevel = XmlOp.GetInnerText(Doc, "userLevel");
            startTime = XmlOp.GetInnerText(Doc, "startTime");
            endTime = XmlOp.GetInnerText(Doc, "endTime");
            linkMode = Int16.Parse(XmlOp.GetInnerText(Doc, "linkMode"));
            targetIpAddr = XmlOp.GetInnerText(Doc, "targetIpAddr");
            targetPort = XmlOp.GetInnerText(Doc, "targetPort");
            flag = Int16.Parse(XmlOp.GetInnerText(Doc, "flag"));
            locationFlag = Int16.Parse(XmlOp.GetInnerText(Doc, "locationFlag"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StartPlayBack");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, "000000");
            XmlOp.ElementAdd(Response, "parameters", "tcpIp");
            XmlOp.SetNodeInnerText(Response, "tcpIp", 0, "192.168.1.101");
            XmlOp.ElementAdd(Response, "parameters", "tcpPort");
            XmlOp.SetNodeInnerText(Response, "tcpPort", 0, "10000");
            Response.Save("D://StartPlayBack-response.xml");

            return Response;
        }

        public XmlDocument StopMediaReq(XmlDocument Doc)
        {
            XmlTools XmlOp = new XmlTools();
            XmlDocument Response = XmlOp.XmlCreate();
            string sessionId, resId;
            int stopFlag;
            sessionId = XmlOp.GetInnerText(Doc, "sessionId");
            resId = XmlOp.GetInnerText(Doc, "resId");
            stopFlag = Int16.Parse(XmlOp.GetInnerText(Doc, "stopFlag"));

            XmlOp.ElementAdd(Response, null, "response");
            XmlOp.SetNodeAttribute(Response, "response", 0, "command", "StopMediaReq");
            XmlOp.ElementAdd(Response, "response", "result");
            XmlOp.SetNodeAttribute(Response, "result", 0, "code", "0");
            XmlOp.SetNodeInnerText(Response, "result", 0, "success");
            XmlOp.ElementAdd(Response, "response", "parameters");
            XmlOp.ElementAdd(Response, "parameters", "sessionId");
            XmlOp.SetNodeInnerText(Response, "sessionId", 0, sessionId);
            Response.Save("D://StopMediaReq-response.xml");

            return Response;
        }
    }
}
