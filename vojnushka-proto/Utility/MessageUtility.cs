using System.IO;
using Google.Protobuf;

namespace VojnushkaProto.Utility
{
    public static class MessageUtility
    {
        public static ByteString MessageToByteString(IMessage message)
        {
            using var memStream = new MemoryStream();
            message.WriteTo(memStream);
            return ByteString.CopyFrom(memStream.ToArray());
        }
    }
}