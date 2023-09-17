using System.Numerics;
using Arch.Core;
using Arch.System;
using VojnushkaShared.Net;

namespace VojnushkaGameServer.Domain.Player;

public class PlayerSpawnSystem : BaseSystem<World, float>
{
    private readonly INetListener _netListener;
    private readonly PlayerFactory _playerFactory;
    private readonly Queue<IPeer> _playersToInstantiate;

    public PlayerSpawnSystem(World world, INetListener netListener) : base(world)
    {
        _netListener = netListener;
        _playerFactory = new PlayerFactory(world);
        _playersToInstantiate = new Queue<IPeer>();
    }

    public override void Initialize()
    {
        _netListener.OnPeerConnect += OnPeerConnect;
    }

    private void OnPeerConnect(IPeer peer)
    {
        _playersToInstantiate.Enqueue(peer);
    }

    public override void Update(in float deltaTime)
    {
        foreach (var peer in _playersToInstantiate)
        {
            _playerFactory.Instantiate(peer, Vector3.Zero);
        }
    }
}