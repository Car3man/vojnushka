using Arch.Core;

namespace VojnushkaGameServer.Core;

public interface IWorldSystem
{
    void OnStart(World world);
    void OnTick(World world, float deltaTime);
    void OnStop(World world);
}