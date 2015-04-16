using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrainStationServer
{
    class ResponseList
    {
        public int Cseq;
        public XmlDocument Doc;

        public ResponseList(int cseq, XmlDocument doc)
        {
            Cseq = cseq;
            Doc = doc;
        }
    }
}
