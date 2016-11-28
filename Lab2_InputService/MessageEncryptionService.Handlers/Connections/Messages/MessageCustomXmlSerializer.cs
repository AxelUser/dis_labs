using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessageEncryptionService.Handlers.Connections.Messages
{
    /// <summary>
    /// Custom serializer to xml.
    /// </summary>
    public class MessageCustomXmlConverter
    {
        private static int formatVersion = 1;
        /// <summary>
        /// Serialize message to xml.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <returns>Xml-string.</returns>
        public static string ToXml(MessageModel message)
        {          
            var root = new XElement("Message");
            root.SetAttributeValue("version", formatVersion);
            if (message.TicketId != null)
            {
                root.Add(new XElement("TicketId", message.TicketId));
            }
            root.Add(new XElement("MessageType", message.MessageType));
            if(message is ReplyModel)
            {
                root.Add(new XElement("ReplyType", ((ReplyModel)message).ReplyType));
            }
            root.Add(new XElement("Sender", message.SenderId),
                new XElement("IsBodyEncrypted", message.IsBodyEncrypted),
                new XElement("DESKey", message.DESKey),
                new XElement("DESIV", message.DESIV),
                new XElement("Body", message.Body));
            XDocument xDoc = new XDocument();
            xDoc.Add(root);
            return xDoc.ToString();
        }

        /// <summary>
        /// Deserialize message from xml.
        /// </summary>
        /// <param name="xml">Serialized message.</param>
        /// <returns>Deserialized message object.</returns>
        public static MessageModel ToModel(string xml)
        {
            XDocument xDoc = XDocument.Parse(xml);
            var docRoot = xDoc.Root;
            MessageModel message;
            MessageTypes type = (MessageTypes) Enum.Parse(typeof(MessageTypes), docRoot.Elements().Single(e => e.Name == "MessageType").Value);
            if (type == MessageTypes.Reply)
            {                
                message = new ReplyModel(type);
                MessageTypes replyType = (MessageTypes)Enum
                    .Parse(typeof(MessageTypes), docRoot
                    .Elements().Single(e => e.Name == "ReplyType").Value);
                ((ReplyModel)message).ReplyType = replyType;
            }
            else
            {
                message = new MessageModel(type);
            }
            message.TicketId = Guid.Parse(docRoot.Elements().SingleOrDefault(e => e.Name == "TicketId").Value);
            message.SenderId = Guid.Parse(docRoot.Elements().Single(e => e.Name == "Sender").Value);
            message.IsBodyEncrypted = Boolean.Parse(docRoot.Elements().Single(e => e.Name == "IsBodyEncrypted").Value);
            message.DESKey = docRoot.Elements().Single(e => e.Name == "DESKey").Value;
            message.DESIV = docRoot.Elements().Single(e => e.Name == "DESIV").Value;
            message.Body = docRoot.Elements().Single(e => e.Name == "Body").Value;

            return message;
        }
    }
}
