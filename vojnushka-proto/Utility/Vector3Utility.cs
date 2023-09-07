using System.Numerics;
using VojnushkaProto.Common;

namespace VojnushkaProto.Utility
{
    public static class Vector3Utility
    {
        public static Vector3 ToVector3(this Vector3ProtoMsg vector3Proto)
        {
            return new Vector3(
                vector3Proto.X,
                vector3Proto.Y,
                vector3Proto.Z
            );
        }
        
        public static Vector3ProtoMsg ToProtoVector3(this Vector3 vector3)
        {
            return new Vector3ProtoMsg
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
        }
    }
}