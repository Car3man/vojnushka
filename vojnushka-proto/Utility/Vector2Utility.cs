using System.Numerics;
using VojnushkaProto.Common;

namespace VojnushkaProto.Utility
{
    public static class Vector2Utility
    {
        public static Vector2 ToVector2(this Vector2ProtoMsg vector2Proto)
        {
            return new Vector2(
                vector2Proto.X,
                vector2Proto.Y
            );
        }
        
        public static Vector2ProtoMsg ToProtoVector2(this Vector3 vector2)
        {
            return new Vector2ProtoMsg
            {
                X = vector2.X,
                Y = vector2.Y
            };
        }
    }
}