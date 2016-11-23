using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MessageEncryptionService.Handlers.Data
{
    public class DataTransformHandler
    {
        public TaskInfoViewModel FromXML(string xml)
        {
            var formatter = new XmlSerializer(typeof(TaskInfoViewModel));
            using(StringReader sr = new StringReader(xml))
            {
                return (TaskInfoViewModel)formatter.Deserialize(sr);
            }
        }

        public string ToXML(TaskInfoViewModel model)
        {
            var formatter = new XmlSerializer(typeof(TaskInfoViewModel));
            using (StringWriter sw = new StringWriter())
            {
                formatter.Serialize(sw, model);
                return sw.ToString();
            }
        }
    }
}
