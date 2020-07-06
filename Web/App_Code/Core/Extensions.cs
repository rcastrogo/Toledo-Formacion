using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Toledo.Core
{
    public static class Extensions
    {
        public static string ToJsonString<T>(this T value) where T : class
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(value.GetType());
                serializer.WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

}