using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using MessagePack;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Snapshot.Utility;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetSnapshotSendSystem : BaseSystem<World, float>
    {
        private readonly INetListener _netListener;
        
        private readonly QueryDescription _netObjectQuery = new QueryDescription()
            .WithAll<NetObject>();
        
        public NetSnapshotSendSystem(World world, INetListener netListener) : base(world)
        {
            _netListener = netListener;
        }

        public override void Update(in float deltaTime)
        {
            var currentTick = World.GetLocalTick();
            var snapshotData = new SnapshotData
            {
                Tick = currentTick,
                Objects = new List<SnapshotObjectData>(),
                DependentOnTick = currentTick
            };
            
            World.Query(in _netObjectQuery, (in Entity entity, ref NetObject netObject) =>
            {
                var snapshotObjectData = new SnapshotObjectData
                {
                    Id = netObject.Id,
                    Components = new List<SnapshotComponentData>()
                };
                
                var components = entity.GetAllComponents();
                foreach (var component in components)
                {
                    if (component is not IPackableComponent packableComponent)
                    {
                        continue;
                    }

                    var componentValues = new List<SnapshotComponentValueData>();
                    CollectComponentValues(packableComponent, componentValues);
                    
                    snapshotObjectData.Components.Add(new SnapshotComponentData
                    {
                        Type = component.GetType(),
                        Values = componentValues
                    });
                }
                
                snapshotData.Objects.Add(snapshotObjectData);
            });

            var message = new MessageData
            {
                Type = MessageType.Snapshot,
                RawBytes = MessagePackSerializer.Serialize(snapshotData)
            };
            _netListener.Broadcast(MessagePackSerializer.Serialize(message));
        }

        private void CollectComponentValues(IPackableComponent component, List<SnapshotComponentValueData> values)
        {
            var componentFields = component.GetType().GetFields()
                .Where(field => field.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0)
                .ToArray();

            foreach (var componentField in componentFields)
            {
                var keyAttribute = (KeyAttribute)componentField.GetCustomAttribute(typeof(KeyAttribute));
                var keyId = (int)keyAttribute.IntKey!;
                
                values.Add(new SnapshotComponentValueData
                {
                    KeyId = keyId,
                    RawBytes = MessagePackSerializer.Serialize(componentField.GetValue(component))
                });
            }
        }
    }
}