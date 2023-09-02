using VojnushkaGameServer.Logger;

namespace VojnushkaGameServer;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}