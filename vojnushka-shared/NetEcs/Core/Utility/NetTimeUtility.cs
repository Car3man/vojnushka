using System;
using Arch.Core;

namespace VojnushkaShared.NetEcs.Core
{
    public static class NetTimeUtility
    {
        private static readonly QueryDescription NetTimeQuery = new QueryDescription()
            .WithAll<NetTime>();
        
        public static int GetNetTick(this World world)
        {
            var tick = 0;
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                tick = netTime.Tick;
            });
            return tick;
        }
        
        public static void SetNetTick(this World world, int tick)
        {
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                netTime.Tick = tick;
            });
        }
        
        public static DateTime GetNetLastTickTime(this World world)
        {
            var lastTickTime = DateTime.MinValue;
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                lastTickTime = netTime.LastTickTime;
            });
            return lastTickTime;
        }
        
        public static void SetNetLastTickTime(this World world, DateTime lastTickTime)
        {
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                netTime.LastTickTime = lastTickTime;
            });
        }
        
        public static int GetNetLastTickPing(this World world)
        {
            var lastTickPing = 0;
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                lastTickPing = netTime.LastTickPing;
            });
            return lastTickPing;
        }
        
        public static void SetNetLastTickPing(this World world, int lastTickPing)
        {
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                netTime.LastTickPing = lastTickPing;
            });
        }
        
        public static int GetNetPing(this World world)
        {
            var ping = 0;
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                ping = netTime.Ping;
            });
            return ping;
        }

        public static void SetNetPing(this World world, int ping)
        {
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                netTime.Ping = ping;
            });
        }
    }
}