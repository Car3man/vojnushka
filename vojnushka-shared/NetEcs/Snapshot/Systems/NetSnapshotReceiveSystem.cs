using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.Core.Utils;
using Arch.System;
using MessagePack;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetSnapshotReceiveSystem : BaseSystem<World, float>
    {
        private readonly INetClient _netClient;

        private SnapshotData? _snapshotQueue;
        private SnapshotData? _lastSnapshot;
        
        private readonly QueryDescription _netObjectQuery = new QueryDescription()
            .WithAll<NetObject>();

        public NetSnapshotReceiveSystem(World world, INetClient netClient) : base(world)
        {
            _netClient = netClient;
            _netClient.OnMessage += OnServerMessage;
        }

        public override void Dispose()
        {
            _netClient.OnMessage -= OnServerMessage;
        }

        public override void Update(in float deltaTime)
        {
            
            
            if (!_snapshotQueue.HasValue)
            {
                return;
            }

            var snapshotData = _snapshotQueue.Value;
            
            World.SetNetTick(snapshotData.Tick);
            World.SetNetLastTickPing((int)(snapshotData.Time - World.GetNetLastTickTime()).TotalMilliseconds);
            World.SetNetLastTickTime(DateTime.UtcNow);
            
            UpdateOrDestroyNetObjects(snapshotData, out var missingObjects);
            CreateMissingObjects(snapshotData, missingObjects);
            CreateSnapshotTrail(snapshotData);
            
            _lastSnapshot = snapshotData;
            _snapshotQueue = null;
        }

        private void OnServerMessage(byte[] data)
        {
            if (!TryGetSnapshotMessage(data, out var message))
            {
                return;
            }

            var currentTick = World.GetNetTick();
            var snapshotData = MessagePackSerializer.Deserialize<SnapshotData>(message.RawBytes);
            if (currentTick > snapshotData.Tick)
            {
                return;
            }

            _snapshotQueue = snapshotData;
        }

        private void CreateSnapshotTrail(SnapshotData snapshotData)
        {
            var entity = this.World.Create();
            entity.Add(new NetSnapshotTrail
            {
                PrevSnapshot = _lastSnapshot,
                CurrSnapshot = snapshotData
            });
        }

        private void CreateNetObjectCreatedEvent(EntityReference reference)
        {
            var entity = this.World.Create();
            entity.Add(new NetObjectCreated(reference));
        }

        private void CreateNetObjectDestroyedEvent(EntityReference reference)
        {
            var entity = this.World.Create();
            entity.Add(new NetObjectDestroyed(reference));
        }

        private void UpdateOrDestroyNetObjects(SnapshotData snapshotData, out HashSet<int> outMissingObjects)
        {
            var missingObjects = new HashSet<int>();
            
            foreach (var snapshotObject in snapshotData.Objects)
            {
                missingObjects.Add(snapshotObject.Id);
            }

            var objectsToDestroy = new List<EntityReference>();
            
            World.Query(in _netObjectQuery, (in Entity entity, ref NetObject netObject) =>
            {
                var netObjectId = netObject.Id;
                missingObjects.Remove(netObjectId);
                
                var objectInSnapshot = snapshotData.Objects.FirstOrDefault(x => x.Id == netObjectId);
                if (objectInSnapshot != null)
                {
                    UpdateOrDestroyComponents(entity, objectInSnapshot.Components, out var missingComponents);
                    CreateMissingComponents(entity, objectInSnapshot.Components, missingComponents);
                }
                else
                {
                    var entityReference = entity.Reference();
                    objectsToDestroy.Add(entityReference);
                    CreateNetObjectDestroyedEvent(entityReference);
                }
            });
            
            foreach (var objectToDestroy in objectsToDestroy)
            {
                World.Destroy(objectToDestroy);
            }

            outMissingObjects = missingObjects;
        }

        private void CreateMissingObjects(SnapshotData snapshotData, HashSet<int> missingObjects)
        {
            foreach (var missingObjectId in missingObjects)
            {
                var objectInSnapshot = snapshotData.Objects.First(x => x.Id == missingObjectId);
                var entity = this.World.Create();
                CreateMissingComponents(entity, objectInSnapshot.Components,
                    objectInSnapshot.Components.Select(x => x.Type!).ToHashSet());
                CreateNetObjectCreatedEvent(entity.Reference());
            }
        }

        private void UpdateOrDestroyComponents(Entity entity, List<SnapshotComponentData> components,
            out HashSet<Type> outMissingComponents)
        {
            var missingComponents = new HashSet<Type>();
            
            foreach (var component in components)
            {
                if (component.Type != null)
                {
                    missingComponents.Add(component.Type);
                }
            }

            var componentsToRemove = new List<ComponentType>();

            var entityComponents = entity.GetAllComponents();
            foreach (var entityComponent in entityComponents)
            {
                var componentType = entityComponent.GetType();
                missingComponents.Remove(componentType);
                
                var componentInSnapshot = components.FirstOrDefault(x => x.Type == componentType);
                if (componentInSnapshot != null)
                {
                    UpdateComponentValues(entityComponent, componentInSnapshot.Values);
                    entity.Set(entityComponent);
                }
                else
                {
                    componentsToRemove.Add(componentType);
                }
            }
            
            entity.RemoveRange(componentsToRemove);

            outMissingComponents = missingComponents;
        }

        private void CreateMissingComponents(Entity entity, List<SnapshotComponentData> components,
            HashSet<Type> missingComponents)
        {
            foreach (var missingComponent in missingComponents)
            {
                var componentInSnapshot = components.First(x => x.Type == missingComponent);
                var component = Activator.CreateInstance(componentInSnapshot.Type!);
                UpdateComponentValues(component, componentInSnapshot.Values);
                entity.Add(component);
            }
        }

        private void UpdateComponentValues(object component, List<SnapshotComponentValueData> values)
        {
            var componentFields = component.GetType().GetFields()
                .Where(field => field.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0)
                .ToDictionary(
                    field => ((KeyAttribute)field.GetCustomAttribute(typeof(KeyAttribute))).IntKey,
                    field => field
                );
            
            foreach (var componentValue in values)
            {
                var componentField = componentFields[componentValue.KeyId];
                componentField.SetValue(component, MessagePackSerializer.Deserialize(componentField.FieldType, componentValue.RawBytes));
            }
        }

        private bool TryGetSnapshotMessage(byte[] data, out MessageData message)
        {
            var parsedMessage = MessagePackSerializer.Deserialize<MessageData>(data);
            if (parsedMessage.Type != MessageType.Snapshot)
            {
                message = default;
                return false;
            }

            message = parsedMessage;
            return true;
        }
    }
}