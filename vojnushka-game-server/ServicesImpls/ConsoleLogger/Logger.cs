using VojnushkaShared.Logger;

namespace VojnushkaGameServer.ConsoleLogger;

internal class Logger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }

    public void Log(string tag, string message)
    {
        Console.WriteLine($"[{tag}]: {message}");
    }
}