namespace VojnushkaShared.NetEcs.Core
{
    public interface IPackableComponent
    {
        byte[] PackTo();
        void ParseFrom(byte[] rawBytes);
    }
}