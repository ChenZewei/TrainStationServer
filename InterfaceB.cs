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
            XmlCreator XmlOp = new XmlCreator();
            XmlDocument Response = XmlOp.XmlCreate();
            string muId, muName;
            int action;
            List<string> idURL = new List<string>();
            List<string> typeURL = new List<string>();

            muId = XmlOp.GetInnerText(Doc, "muId");
            muName = XmlOp.GetInnerText(Doc, "muName");
            action = Int16.Parse(XmlOp.GetInnerText(Doc, "action"));
            idURL = XmlOp.GetInnerTextList(Doc, "id");
            typeURL = XmlOp.GetInnerTextList(Doc, "type");

            
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
            XmlCreator XmlOp = new XmlCreator();
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
            XmlCreator XmlOp = new XmlCreator();
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
    }
}
