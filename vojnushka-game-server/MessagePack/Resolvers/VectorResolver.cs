using System.Numerics;
using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
    public class VectorResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new VectorResolver();

        private VectorResolver()
        {
        }
        
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;
            
            static FormatterCache()
            {
                Formatter = (IMessagePackFormatter<T>)VectorResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class VectorResolverGetFormatterHelper
    {
        static readonly Dictionary<Type, object> FormatterMap = new()
        {
            {typeof(Vector3), new Vector3Formatter()}
        };

        internal static object GetFormatter(Type type)
        {
            if (FormatterMap.TryGetValue(type, out var formatter))
            {
                return formatter;
            }

            throw new NullReferenceException();
        }
    }
    
    class Vector3Formatter : IMessagePackFormatter<Vector3>
    {
        public void Serialize(ref MessagePackWriter writer, Vector3 value, MessagePackSerializerOptions options)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }

        Vector3 IMessagePackFormatter<Vector3>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }
    }
}