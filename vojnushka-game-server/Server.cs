using VojnushkaGameServer.Domain;

namespace VojnushkaGameServer;

internal class Server : IDisposable
{
    private readonly ServerWorld _serverWorld;
    
    private bool _stop;

    private float _tickTime;
    private float _lastTickTime;
    private float _tickDeltaTime;

    private const int TickRate = 12; // Default is 24
    private const int TickDelayMs = (int)(1f / TickRate * 1000);

    public Server(ServerWorld serverWorld)
    {
        _serverWorld = serverWorld;
    }
    
    public async Task Run()
    {
        await _serverWorld.InitializeAndStart();
        
        while (!_stop)
        {
            _tickDeltaTime = _tickTime - _lastTickTime;
            _lastTickTime = _tickTime;
            
            _serverWorld.Update(_tickDeltaTime);
            
            await Task.Delay(TickDelayMs);
            _tickTime += TickDelayMs / 1000f;
        }

        _serverWorld.Dispose();
    }
    
    public void Dispose()
    {
        _stop = true;
        _serverWorld.Dispose();
    }
}