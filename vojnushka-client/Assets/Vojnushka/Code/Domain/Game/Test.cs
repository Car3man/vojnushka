using UnityEngine;
using Vojnushka.Infrastructure;
using Vojnushka.Network;
using VojnushkaProto.Avatar;
using VojnushkaProto.Core;
using VojnushkaProto.PingPong;
using VojnushkaProto.SessionSnapshot;
using VojnushkaProto.Utility;

namespace Vojnushka.Game
{
    public class Test : MonoBehaviour, System.IDisposable
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        private INetworkClient _networkClient;
        private AvatarsSnapshotProtoMsg _avatarsSnapshot;

        [Inject]
        public void Construct(INetworkClient networkClient)
        {
            _networkClient = networkClient;
            _networkClient.OnMessage += OnMessage;
        }
        
        public void Dispose()
        {
            _networkClient.OnMessage -= OnMessage;
        }

        private void OnMessage(ServerProtoMsg message)
        {
            if (message.Type == ServerProtoMsgType.SessionSnapshot)
            {
                var sessionSnapshot = SessionSnapshotProtoMsg.Parser.ParseFrom(message.Data);
                if (sessionSnapshot.Avatars != null)
                {
                    _avatarsSnapshot = sessionSnapshot.Avatars;
                }
            }
        }

        private void Update()
        {
            if (_avatarsSnapshot == null)
            {
                return;
            }

            float horizontalAxis = Input.GetAxis("Horizontal");
            if (_avatarsSnapshot.Id.Count > 0 && Mathf.Abs(horizontalAxis) > 0.1f)
            {
                var position = _avatarsSnapshot.Positions[0];
                position.X += Input.GetAxis("Horizontal") * Time.deltaTime;
                
                var syncMessage = new AvatarSyncProtoMsg
                {
                    Id = _avatarsSnapshot.Id[0],
                    Position = position,
                    Rotation = _avatarsSnapshot.Rotations[0]
                };

                _networkClient.Send(new ServerProtoMsg
                {
                    Type = ServerProtoMsgType.AvatarSync,
                    Data = MessageUtility.MessageToByteString(syncMessage)
                });
            }

            for (var i = 0; i < _avatarsSnapshot.Id.Count; i++)
            {
                var position = _avatarsSnapshot.Positions[i];
                var rotation = _avatarsSnapshot.Rotations[i];
                var matrix = Matrix4x4.TRS(
                    new Vector3(position.X, position.Y, position.Z),
                    Quaternion.Euler(new Vector3(rotation.X, rotation.Y, rotation.Z)),
                    Vector3.one
                );
                Graphics.DrawMesh(mesh, matrix, material, 0, Camera.main);
            }
        }
    }
}