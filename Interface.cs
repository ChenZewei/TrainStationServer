using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    interface Interface
    {
        XmlDocument AlarmResSubscribe(XmlDocument Doc);
        XmlDocument ControlFileBack(XmlDocument Doc);
        XmlDocument ControlPTZ(XmlDocument Doc);
        XmlDocument GetUserCurState(XmlDocument Doc);
        XmlDocument HisInfo(XmlDocument Doc);
        XmlDocument HisLoadInfo(XmlDocument Doc);/*
        XmlDocument INFO(XmlDocument Doc);
        XmlDocument QueryAlarmRes(XmlDocument Doc);
        XmlDocument QueryHistoryFiles(XmlDocument Doc);
        XmlDocument ReportAlarmInfo(XmlDocument Doc);
        XmlDocument ReqCamResState(XmlDocument Doc);
        XmlDocument ResChangeOrder(XmlDocument Doc);
        XmlDocument ResTransOrder(XmlDocument Doc);
        XmlDocument SetUserCamManage(XmlDocument Doc);
        XmlDocument StartHisLoad(XmlDocument Doc);
        XmlDocument StartMediaReq(XmlDocument Doc);
        XmlDocument StartPlayBack(XmlDocument Doc);
        XmlDocument StopMediaReq(XmlDocument Doc);*/
    }
}
