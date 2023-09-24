using MemoryPack;
using Scellecs.Morpeh;

namespace VojnushkaShared.Net
{
    [MemoryPackable]
    public partial struct NetObject : IComponent
    {
        public int Id { get; set; }
    }
}