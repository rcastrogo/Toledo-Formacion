using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Toledo.Core
{
    public static class Extensions
    {
        public static string ToJsonString<T>(this T value, 
                                            EmitTypeInformation emitType = EmitTypeInformation.AsNeeded,
                                            bool useSimpleDictionaryFormat = false) where T : class
        {
            using (MemoryStream stream = new MemoryStream())
            {
               var settings = new DataContractJsonSerializerSettings();
                settings.EmitTypeInformation = EmitTypeInformation.Never;
                settings.UseSimpleDictionaryFormat = useSimpleDictionaryFormat;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(value.GetType(), settings);
                serializer.WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T FromJsonTo<T>(this string jsonString) where T : class {
          using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
          {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return Microsoft.VisualBasic.CompilerServices.Conversions.ToGenericParameter<T>(serializer.ReadObject(stream));
          }
        }

        public static string ToXml<T>(this T value) where T : class {
          using (Utf8StringWriter writer = new Utf8StringWriter())
          {
            XmlSerializer serializer = new XmlSerializer(value.GetType());
            serializer.Serialize((TextWriter)writer, value);
            return writer.ToString();
          }
        }

        public static string ToClearXml<T>(this T value) where T : class
        {
          return value.ToClearXml<T>(null);
        }

        public static string ToClearXml<T>(this T value, Encoding Encoding) where T : class
        {
          using (EncodedStringWriter writer = new EncodedStringWriter(Encoding))
          {
            XmlSerializer serializer = new XmlSerializer(value.GetType());
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            serializer.Serialize(writer, value, namespaces);
            return writer.ToString();
          }
        }

        private class EncodedStringWriter : StringWriter {

          private System.Text.Encoding _encoding;

          public EncodedStringWriter(System.Text.Encoding encoding)
          {
            _encoding = encoding;
          }

          public override System.Text.Encoding Encoding
          {
            get
            {
              return _encoding;
            }
          }
        }

        private class Utf8StringWriter : Extensions.EncodedStringWriter
        {
          public Utf8StringWriter() : base(Encoding.UTF8)
          {
          }
        }

    }

}