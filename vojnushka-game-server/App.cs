// ReSharper disable NotAccessedField.Local

using VojnushkaGameServer.Infrastructure;

namespace VojnushkaGameServer;

internal class App
{
    public static async Task Main()
    {
        await using var context = new ServerContext();
        await context
            .CreateServer()
            .Run();
    }
}