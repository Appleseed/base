namespace Appleseed.Services.Core.Serialization
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSerialization
    {
        public static string Serialize<T>(T result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result", "Cannot serialize null value");
            }

            var serializer = new XmlSerializer(result.GetType());
            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, result);
                    return sww.ToString();
                }
            }
        }

        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException("xml", "Cannot deserialize null value");
            }

            var deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                object obj = deserializer.Deserialize(reader);
                return (T)obj;
            }
        }
    }
}
