using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{
    public partial class Client : MonoBehaviour
    {
        public int dataBufferSize = 4096; 
        public Dictionary<int, PacketHandler> packetHandlers;
        public ServerClientManager ClientManager;

        public string username = "ClientSven";
        public string ip = "192.168.178.55"; //"127.0.0.1";
        public int port = 26950;
        public int myId = 0;
        public TCP tcp;
        public UDP udp;

        private bool isConnected = false;

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void ConnectToIp(string _ip)
        {
            ip = _ip;
            tcp = new TCP(this);
            udp = new UDP(this);
            isConnected = true;
            tcp.Connect();
        }
        public void Connect(TcpClient _socket)
        {
            tcp = new TCP(this);
            udp = new UDP(this);
            isConnected = true;
            tcp.Connect(_socket);
        }
        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                if (tcp.socket != null)
                {
                    tcp.socket.Close();
                }
                if (udp.socket != null)
                {
                    udp.socket.Close();
                }

                ClientManager.DisconnectClient(myId);

                Debug.Log($"{ip} has disconnected.");
            }
        }
        
    }
}
