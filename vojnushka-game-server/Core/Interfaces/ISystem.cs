using Arch.Core;

namespace VojnushkaGameServer.Core;

public interface ISystem
{
    void OnStart(World world);
    void OnStop(World world);
}