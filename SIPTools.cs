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
        public int cseq,ncseq;
        public string Id;
        public string CallId;
        public SIPTools()
        {
            Id = "XX";
            To = "XX";
            From = "XX";
            CSeq = "XX INVITE";
            cseq = 0;
        }

        public SIPTools(byte[] buffer, int bufferlen)
        {
            Id = GetSIPInfo(buffer, bufferlen, "sip");
            To = GetSIPInfo(buffer, bufferlen, "From");
            From = GetSIPInfo(buffer, bufferlen, "To");
            CSeq = GetSIPInfo(buffer, bufferlen, "CSeq");
            CallId = GetSIPInfo(buffer, bufferlen, "Call-ID");
            cseq = 0;
            ncseq = 0;
        }

        public SIPTools(string buffer)
        {
            Id = GetSIPInfo(buffer, "sip: ");
            To = GetSIPInfo(buffer, "From: ");
            From = GetSIPInfo(buffer, "To: ");
            CSeq = GetSIPInfo(buffer, "CSeq: ");
            CallId = GetSIPInfo(buffer, "Call-ID: ");
            cseq = 0;
            ncseq = 0;
        }

        public SIPTools(string to, string from, string cseq)
        {
            To = to;
            From = from;
            CSeq = cseq;
            this.cseq = 0;
            this.ncseq = 0;
        }
        

        public void Refresh(byte[] recv,int i)
        {
            CSeq = GetSIPInfo(recv, i, "CSeq");
            CallId = GetSIPInfo(recv, i, "Call-ID");
        }

        public void Refresh(string buffer)
        {
            Id = GetSIPInfo(buffer, "sip: ");
            To = GetSIPInfo(buffer, "From: ");
            From = GetSIPInfo(buffer, "To: ");
            CSeq = GetSIPInfo(buffer, "CSeq: ");
            CallId = GetSIPInfo(buffer, "Call-ID: ");
        }

        public string SIPRequest(XmlDocument doc,string To,string From, string CSeq)
        {
            string sendBuffer = "";
            sendBuffer += "SIP/2.0 200 OK\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:6100002007000001\r\n";
            sendBuffer += "CSeq:" + CSeq + "\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }

        public string SIPRequest(XmlDocument doc)
        {
            string sendBuffer = "";
            //sendBuffer += "INVITE sip:6100002007000001 SIP/2.0\r\n";
            sendBuffer += "SIP/2.0 200 OK\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:" + CallId + "\r\n";
            sendBuffer += "CSeq:" + (cseq++).ToString() + " INVITE\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }
        public string SIPResponse(XmlDocument doc)
        {
            string sendBuffer = "";
            sendBuffer += "SIP/2.0 200 OK\r\n";
            sendBuffer += "Via:SIP/2.0/TCP XX\r\n";
            sendBuffer += "To:" + To + "\r\n";
            sendBuffer += "From:" + From + "\r\n";
            sendBuffer += "Call-ID:" + CallId + "\r\n";
            sendBuffer += "CSeq:" + CSeq + "\r\n";
            sendBuffer += "Content-Type:RVSS/xml\r\n";
            sendBuffer += "Content-Length:" + doc.OuterXml.Length.ToString() + "\r\n\r\n";
            sendBuffer += doc.OuterXml;
            return sendBuffer;
        }

        public static XmlDocument XmlExtract(byte[] buffer, int bufferlen)//Xml提取
        {
            XmlDocument xmlDoc;
            int i = 0, index = 0, end = bufferlen;
            string bufstr = Encoding.ASCII.GetString(buffer);
            string strBuffer;
            string mSip;
            byte[] mXmlBuffer;
            int sip_end = bufstr.IndexOf("\r\n\r\n");
            if (sip_end > 0)
            {
                int sip_length = sip_end + 4;
                mSip = Encoding.ASCII.GetString(buffer, 0, sip_length);
                int Content_Length_i = bufstr.IndexOf("Content-Length:");
                if (Content_Length_i < 0)
                {
                    throw new Exception("SIP头错误，没有Content-Length:XX");
                }
                int xml_length;
                int data_length_start = Content_Length_i + "Content-Length:".Length;
                int data_length_end = bufstr.IndexOf("\r\n", data_length_start);
                string data_length_str = bufstr.Substring(data_length_start, data_length_end - data_length_start);
                if (!int.TryParse(data_length_str, out xml_length))
                {
                    throw new Exception("SIP头错误，Content-Length:XX,不能解析");
                }
                mXmlBuffer = new byte[xml_length];
            }
            try
            {
                if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes("\r\n\r\n"))) != -1)
                {
                    xmlDoc = new XmlDocument();
                    strBuffer = "";
                    for (int k = bufferlen-1; k >= 0; k-- )
                    {
                        if(buffer[k] == '>')
                        {
                            end = k;
                            break;
                        }
                    }
                    strBuffer = Encoding.GetEncoding("GB2312").GetString(buffer, index, (end - index+1));
                        //Console.WriteLine("strbuffer:" + strBuffer);
                    xmlDoc.LoadXml(strBuffer);
                    return xmlDoc;
                }
            }
            catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine("ArgumentOutOfRangeException:");
                Console.WriteLine(e.Message);
            }
            catch (XmlException e)
            {
                Console.WriteLine("XmlException:");
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public static byte[] PckExtract(ref byte[] buffer, int recvlen)
        {
            int i, j = 0;
            string bufstr = Encoding.ASCII.GetString(buffer, 0, recvlen);
            int head = bufstr.IndexOf("INVITE sip:");
            if (head == -1)
                return null;
            int end = bufstr.IndexOf("INVITE sip:", "INVITE sip:".Length);
            if (end == -1)
                end = recvlen;
            string mSip;
            byte[] mXmlBuffer;
            int sip_end = bufstr.IndexOf("\r\n\r\n");
            if (sip_end > 0)
            {
                int sip_length = sip_end + 4;
                mSip = Encoding.ASCII.GetString(buffer, 0, sip_length);
                int Content_Length_i = bufstr.IndexOf("Content-Length:");
                if (Content_Length_i < 0)
                {
                    throw new Exception("SIP头错误，没有Content-Length:XX");
                }
                int xml_length;
                int data_length_start = Content_Length_i + "Content-Length:".Length;
                int data_length_end = bufstr.IndexOf("\r\n", data_length_start);
                string data_length_str = bufstr.Substring(data_length_start, data_length_end - data_length_start);
                if (!int.TryParse(data_length_str, out xml_length))
                {
                    throw new Exception("SIP头错误，Content-Length:XX,不能解析");
                }
                mXmlBuffer = new byte[xml_length];
                if((recvlen - sip_length) < xml_length)
                {
                    return null;
                }
                if((recvlen - sip_length) == xml_length)
                {
                    string temp = bufstr.Substring(0, sip_length + xml_length);
                    string remainstr = bufstr.Substring(sip_length + xml_length, recvlen - (sip_length + xml_length));
                    buffer = Encoding.ASCII.GetBytes(remainstr);
                    return Encoding.ASCII.GetBytes(temp);
                    //Array.Copy(buffer, sip_length, mXmlBuffer, 0, xml_length);
                    //mSip = null;
                    //return mXmlBuffer;
                }

            }
            return null;
            //int head = bufstr.IndexOf("INVITE sip:");
            //if (head == -1)
            //    return null;
            //int end = bufstr.IndexOf("INVITE sip:", "INVITE sip:".Length);
            //if (end == -1)
            //    end = recvlen;

            //string temp = bufstr.Substring(head, end - head);
            //string remainstr = bufstr.Substring(end, recvlen - end);
            //buffer = Encoding.ASCII.GetBytes(remainstr);
            //return Encoding.ASCII.GetBytes(temp);
        }

        //public static byte[] PckExtract(byte[] buffer, int bflen, int recvlen)
        //{
        //    int i,j = 0;
        //    string bufferstr = Encoding.ASCII.GetString(buffer, 0, recvlen);
        //    int head = BeginOf(buffer, Encoding.ASCII.GetBytes("INVITE sip"), 0);
        //    if (head == -1)
        //        return null;
        //    int end = BeginOf(buffer, Encoding.ASCII.GetBytes("INVITE sip"), head + 10);
        //    //if (end == -1 && bflen != 8000)
        //    //    end = recvlen;
        //    if (end == -1)
        //        end = recvlen;
        //        //return null;
        //    string temp = Encoding.ASCII.GetString(buffer, head, end - head);
        //    for (i = end; i < 10000; i++)
        //    {
        //        buffer[j++] = buffer[i];
        //        buffer[i] = 0;
        //    }

        //    return Encoding.ASCII.GetBytes(temp);
        //}

        public static bool Extraction(ref SipSocket s, ref byte[] buffer, ref int bufferlen, out string sip, out string xml)
        {
            string mSip;
            int mXmlOffset;
            byte[] mXmlBuffer;
            string sip_tmp = Encoding.ASCII.GetString(buffer);
            int sip_end = sip_tmp.IndexOf("\r\n\r\n");
            if (sip_end > 0)
            {
                int sip_length = sip_end + 4;
                mSip = Encoding.ASCII.GetString(buffer, 0, sip_length);
                int Content_Length_i = sip_tmp.IndexOf("Content-Length:");
                if (Content_Length_i < 0)
                {
                    throw new Exception("SIP头错误，没有Content-Length:XX");
                }
                int xml_length;
                int data_length_start = Content_Length_i + "Content-Length:".Length;
                int data_length_end = sip_tmp.IndexOf("\r\n", data_length_start);
                string data_length_str = sip_tmp.Substring(data_length_start, data_length_end - data_length_start);
                if (!int.TryParse(data_length_str, out xml_length))
                {
                    throw new Exception("SIP头错误，Content-Length:XX,不能解析");
                }
                //Offset = sip_tmp.IndexOf("INVITE sip");
                //if (Offset == -1)
                //    Offset = bufferlen;
                if (bufferlen - sip_length < xml_length) //只读取了部分xml
                {
                    //if (bufferlen > sip_length)
                    //{
                    //    mXmlOffset = bufferlen - sip_length;
                    //    s.lastRecv = new byte[mXmlOffset];
                    //    Array.Copy(buffer, sip_length, s.lastRecv, 0, mXmlOffset);
                    //    buffer = Encoding.ASCII.GetBytes("");
                    //    bufferlen = 0; 
                    //}
                    //else
                    //{
                    mXmlOffset = bufferlen;
                    s.lastRecv = new byte[mXmlOffset];
                    Array.Copy(buffer, 0, s.lastRecv, 0, bufferlen);
                    buffer = Encoding.ASCII.GetBytes("");
                    bufferlen = 0; 
                    //}
                }
                else if (bufferlen - sip_length == xml_length)//刚好读完了所有xml
                {
                    mXmlBuffer = new byte[xml_length];
                    Array.Copy(buffer, sip_length, mXmlBuffer, 0, xml_length);
                    mXmlOffset = xml_length;
                    sip = mSip;
                    xml = Encoding.GetEncoding("GB2312").GetString(mXmlBuffer);
                    mSip = null;
                    mXmlBuffer = null;
                    mXmlOffset = 0;
                    buffer = Encoding.ASCII.GetBytes("");
                    bufferlen = 0; 
                    return true;
                }
                else//还读取了后续消息
                {
                    mXmlBuffer = new byte[xml_length];
                    Array.Copy(buffer, sip_length, mXmlBuffer, 0, xml_length);
                    sip = mSip;
                    xml = Encoding.GetEncoding("GB2312").GetString(mXmlBuffer);
                    string remainstr = sip_tmp.Substring(sip_length + xml_length, bufferlen - (sip_length + xml_length));
                    buffer = Encoding.ASCII.GetBytes(remainstr);
                    bufferlen = buffer.Length;
                    mSip = null;
                    mXmlBuffer = null;
                    mXmlOffset = 0;
                    return true;
                }
            }
            else
            {
                mXmlOffset = bufferlen;
                s.lastRecv = new byte[mXmlOffset];
                Array.Copy(buffer, 0, s.lastRecv, 0, bufferlen);
                buffer = Encoding.ASCII.GetBytes("");
                bufferlen = 0; 
            }
            sip = null;
            xml = null;
            return false;
        }

        public static string GetSIPInfo(byte[] buffer, int bufferlen ,string infoType)
        {
            int length = 0,index = 0;
            string info = null;
            byte[] infoByte = new byte[1024];
            if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes(infoType))) != -1)
            {
                index++;
                while ((index + length + 1) < bufferlen)
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

        public static string GetSIPInfo(string buffer, string infoType)
        {
            int length = 0, index = 0, end = 0;
            string info = null;
            byte[] infoByte = new byte[1024];
            if ((index = buffer.IndexOf(infoType)) != -1)
            {
                index += infoType.Length;
                end = buffer.IndexOf("\r\n", index);
                info = buffer.Substring(index, end - index);
            }
            else
                return "";
            return info;
        }

        public static int getCSeq(byte[] recv)
        {
            int index, n = 0;
            byte[] result = new byte[10];
            if ((index = IndexOf(recv, Encoding.ASCII.GetBytes("CSeq:"))) != -1)
            {
                while (recv[index] == ' ')
                    index++;
                for(int i = index; ;i++)
                {
                    if (recv[i] == ' ' || recv[i] < '0' || recv[i] > '9')
                    {
                        break;
                    }
                    result[n++] = recv[i];
                }
                return int.Parse(Encoding.ASCII.GetString(result));
            }
            return -1;
        }

        private static int BeginOf(byte[] srcBytes, byte[] searchBytes,int begin)//搜索byte数组，返回-1即未找到，返回值不为-1则为搜索字串后一个字节的序号
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
            for (int i = begin; i < srcBytes.Length - searchBytes.Length; i++)
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
                        return i;
                }
            }
            return -1;
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
