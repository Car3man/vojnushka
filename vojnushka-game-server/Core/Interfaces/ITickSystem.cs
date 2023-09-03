using Arch.Core;

namespace VojnushkaGameServer.Core;

public interface ITickSystem : ISystem
{
    void OnTick(World world, float deltaTime);
}