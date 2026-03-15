using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace eShopLegacy.Utilities
{
    public class Serializing
    {
        public Stream SerializeBinary(object input)
        {
            var stream = new MemoryStream();
            var json = JsonSerializer.Serialize(input);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public object DeserializeBinary(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            // Read all bytes from the stream
using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize(json, typeof(object));
        }
    }
}
