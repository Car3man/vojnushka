namespace Vojnushka.VectorConverters
{
    public static class Vector3Converter
    {
        public static UnityEngine.Vector3 GetUnityVector(this System.Numerics.Vector3 vector3)
        {
            return new UnityEngine.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static System.Numerics.Vector3 GetSystemVector(this UnityEngine.Vector3 vector3)
        {
            return new System.Numerics.Vector3(vector3.x, vector3.y, vector3.z);
        }
    }
}