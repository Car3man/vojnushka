﻿using Arch.Core;
using Arch.Core.Extensions;

namespace VojnushkaShared.NetEcs.Core
{
    public static class NetObjectUtility
    {
        private static readonly QueryDescription NetObjectQuery = new QueryDescription()
            .WithAll<NetObject>();

        public static Entity CreateNetObject(this World world)
        {
            return world.CreateOwnedNetObject(-1);
        }
        
        public static Entity CreateOwnedNetObject(this World world, int ownerId)
        {
            var entity = world.Create();
            entity.Add(new NetObject
            {
                Id = world.GetFreeObjectId(),
                OwnerId = ownerId
            });
            return entity;
        }
        
        public static int GetFreeObjectId(this World world)
        {
            var freeId = 0;
            var maxCurrId = 0;
            
            world.Query(in NetObjectQuery, (ref NetObject netObject) =>
            {
                if (netObject.Id > maxCurrId)
                {
                    maxCurrId = netObject.Id;
                }
                
                if (netObject.Id == freeId)
                {
                    freeId = maxCurrId + 1;
                    maxCurrId++;
                }
            });
            
            return freeId;
        }
    }
}