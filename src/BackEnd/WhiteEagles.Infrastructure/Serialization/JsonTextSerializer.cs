namespace WhiteEagles.Infrastructure.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public class JsonTextSerializer : ITextSerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonTextSerializer() : this(JsonSerializer.Create(
            new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.Auto
            }))
        {
        }

        public JsonTextSerializer(JsonSerializer serializer)
            => _serializer = serializer ??
                             throw new ArgumentNullException(nameof(serializer));

        public string Serialize<T>(T data)
        {
            using var writer = new StringWriter();
            var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            _serializer.Serialize(jsonWriter, data);

            return writer.ToString();
        }

        public T Deserialize<T>(string serialized)
        {
            if (serialized == null)
            {
                throw new ArgumentNullException(nameof(serialized));
            }

            using var reader = new StringReader(serialized);
            var jsonReader = new JsonTextReader(reader);

            return _serializer.Deserialize<T>(jsonReader);
        }
    }
}
