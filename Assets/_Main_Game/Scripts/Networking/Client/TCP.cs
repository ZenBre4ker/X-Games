using UnityEngine;
using System;
using System.Net.Sockets;

namespace Networking
{
    public class TCP
    {
        public TcpClient socket;
        public bool sendWelcome = false;

        private Client tcpClient;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(Client _tcpClient)
        {
            tcpClient = _tcpClient;

            receivedData = new Packet();
            receiveBuffer = new byte[tcpClient.dataBufferSize];
        }

        //If Client connects to the Server
        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = tcpClient.dataBufferSize,
                SendBufferSize = tcpClient.dataBufferSize
            };

            socket.BeginConnect(tcpClient.ip, tcpClient.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, tcpClient.dataBufferSize, ReceiveCallback, null);
        }

        //If Server connects to Clients
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = tcpClient.dataBufferSize;
            socket.SendBufferSize = tcpClient.dataBufferSize;

            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, tcpClient.dataBufferSize, ReceiveCallback, null);

            tcpClient.ip = socket.Client.RemoteEndPoint.ToString();
            tcpClient.SendServerWelcome();
        }
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {tcpClient.myId} server via TCP: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    tcpClient.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, tcpClient.dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP Data from player {tcpClient.myId} with {_ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }

            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Debug.Log($"Packet Id {_packetId} was received");
                        if (tcpClient.packetHandlers.ContainsKey(_packetId))
                        {
                            tcpClient.packetHandlers[_packetId](tcpClient.myId, _packet);
                        } else
                        {

                        }
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }

                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }
        private void Disconnect()
        {
            tcpClient.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
}
