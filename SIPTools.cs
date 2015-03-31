using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    class SIPTools
    {
        private string To, From, CSeq;
        public string Id;
        public SIPTools()
        {
            Id = "XX";
            To = "XX";
            From = "XX";
            CSeq = "XX INVITE";
        }

        public SIPTools(byte[] buffer, int bufferlen)
        {
            Id = GetSIPInfo(buffer, bufferlen, "sip")/*.Substring(0,16)*/;
            To = GetSIPInfo(buffer, bufferlen, "From");
            From = GetSIPInfo(buffer, bufferlen, "To");
            CSeq = GetSIPInfo(buffer, bufferlen, "CSeq");
        }
        public SIPTools(string to, string from, string cseq)
        {
            To = to;
            From = from;
            CSeq = cseq;
        }

        public string SIPRequest(XmlDocument doc)//为请求消息加上Sip头
        {
            string sendBuffer = "";
            sendBuffer += "SIP/2.0 200 OK\r\n";//原
            //sendBuffer += "INVIT sip:XX SIP/2.0\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:XX\r\n";
            sendBuffer += "CSeq:" + CSeq + "\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }
        public string SIPRequest(XmlDocument doc,string To,string From, string CSeq)
        {
            string sendBuffer = "";
            sendBuffer += "SIP/2.0 200 OK\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:XX\r\n";
            sendBuffer += "CSeq:" + CSeq + "\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }
        public string SIPResponse(XmlDocument doc)//为响应消息加上Sip头
        {
            string sendBuffer = "";
            sendBuffer += "SIP/2.0 200 OK\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:XX\r\n";
            sendBuffer += "CSeq:" + CSeq + "\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }

        public static XmlDocument XmlExtract(byte[] buffer, int bufferlen)//Xml提取
        {
            XmlDocument xmlDoc;
            int i = 0, index = 0;
            byte[] bufferline;
            byte[] length;
            string strBuffer;
            bufferline = new byte[100];
            length = new byte[10];
            if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes("Content-Length"))) != -1)
            {
                index++;
                while ((index + i) < bufferlen)
                {
                    length[i] = buffer[index + i];
                    i++;
                    if ((buffer[index + i] == '\r') && (buffer[index + i + 1] == '\n'))
                        break;
                }
            }
            if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes("\r\n\r\n"))) != -1)
            {
                xmlDoc = new XmlDocument();
                strBuffer = "";
                for (int j = 1;j<bufferlen ;j++ )
                {
                    if (buffer[bufferlen - j] != '0' && buffer[bufferlen - j] != '\0')
                    {
                        strBuffer = Encoding.GetEncoding("GB2312").GetString(buffer, index, (bufferlen - index - j+1));
                        break;
                    }
                }

                xmlDoc.LoadXml(strBuffer);
                return xmlDoc;
            }
            return null;
        }

        public static string GetSIPInfo(byte[] buffer, int bufferlen ,string infoType)
        {
            int length = 0,index = 0;
            string info = null;
            byte[] infoByte = new byte[1024];
            if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes(infoType))) != -1)
            {
                index++;
                while ((index + length) < bufferlen)
                {
                    infoByte[length] = buffer[index + length];
                    length++;
                    if (((buffer[index + length] == '\r') && (buffer[index + length + 1] == '\n')))
                        break;
                }
            }
            else
                return "";
            info = Encoding.UTF8.GetString(infoByte, 0, length);
            return info;
        }

        private static int IndexOf(byte[] srcBytes, byte[] searchBytes)//搜索byte数组，返回-1即未找到，返回值不为-1则为搜索字串后一个字节的序号
        {
            if (srcBytes == null) 
                return -1;
            if (searchBytes == null)
                return -1;
            if (srcBytes.Length == 0) 
                return -1;
            if (searchBytes.Length == 0) 
                return -1;
            if (srcBytes.Length < searchBytes.Length) 
                return -1;
            for (int i = 0; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] == searchBytes[0])
                {
                    if (searchBytes.Length == 1) { return i; }
                    bool flag = true;
                    for (int j = 1; j < searchBytes.Length; j++)
                    {
                        if (srcBytes[i + j] != searchBytes[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        return i + searchBytes.Length;
                }
            }
            return -1;
        }
    }
}
