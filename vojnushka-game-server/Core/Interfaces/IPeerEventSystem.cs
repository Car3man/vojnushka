using Arch.Core;

namespace VojnushkaGameServer.Core;

public interface IPeerEventSystem : ISystem
{
    void OnPeerConnect(World world, EntityReference entityRef);
    void OnPeerDisconnect(World world, EntityReference entityRef);
}