using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        private Client udpClient;

        public UDP(Client _udpClient)
        {
            udpClient = _udpClient;
            endPoint = new IPEndPoint(IPAddress.Parse(udpClient.ip), udpClient.port);
        }

        //Client connects to Server
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            };
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    udpClient.Disconnect();
                    return;
                }
                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();
                    HandleData(_packet);
                };
            }
            catch
            {

                Disconnect();
            }
        }

        //Server connects to Client
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
            udpClient.SendServerUDPTest();
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(udpClient.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }

            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }
        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    udpClient.packetHandlers[_packetId](udpClient.myId, _packet);
                }
            });
        }
        private void Disconnect()
        {
            udpClient.Disconnect();

            endPoint = null;
            socket = null;
        }
    }
}
