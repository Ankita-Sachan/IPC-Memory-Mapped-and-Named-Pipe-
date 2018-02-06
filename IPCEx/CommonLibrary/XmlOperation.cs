using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    public static class XmlOperation
    {
        static string _path = AppDomain.CurrentDomain.BaseDirectory + "machinestate.xml";

        public static void SetXml()
        {
            if (!System.IO.File.Exists(_path))
            {
                using (XmlWriter writer = XmlWriter.Create(_path, new XmlWriterSettings() { Indent = true }))
                {
                    writer.WriteStartElement("configuration");
                    writer.WriteStartElement("AutolLogin");
                    writer.WriteElementString("CurrentStatus", "");
                    writer.WriteElementString("PreviousStatus", "");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }
        }
        public static void UpdateXml(string msg)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_path);
            XmlNodeList nodes = doc.SelectNodes("/configuration/AutolLogin");

            if (!nodes[0].ChildNodes[0].InnerText.Equals(msg))
            {
                nodes[0].ChildNodes[1].InnerText = nodes[0].ChildNodes[0].InnerText;
                nodes[0].ChildNodes[0].InnerText = msg.ToString();
            }
            doc.Save(_path);
        }
    }
}
