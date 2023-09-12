using UnityEngine;
using ILogger = VojnushkaShared.Logger.ILogger;

namespace Vojnushka.EditorLogger
{
    public class DebugLoggerAdapter : ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}