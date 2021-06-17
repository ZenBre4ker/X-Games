using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Networking
{
    public delegate void PacketHandler(int _fromClient, Packet _packet);

    public class ServerClientManager : MonoBehaviour
    {
        
        public Dictionary<int, PacketHandler> packetHandlers;
        public string username = "HostSven";
        public string ip = "192.168.178.55";//"192.168.178.55"
        public int port = 26950;

        public Server myServer;
        public Client myClient;

        private bool isHost = false;

        public void StartNetworking(bool _isHost)
        {
            isHost = _isHost;

            if(packetHandlers==null) packetHandlers = new Dictionary<int, PacketHandler>();
            InitializePacketHandlers();

            if (isHost)
            {
                myServer = gameObject.AddComponent<Server>();
                myServer.StartServer(this);
            } else
            {
                myClient = InitializeClientComponent(gameObject, 1);
                myClient.ConnectToIp(ip);
            }
        }

        private void InitializePacketHandlers()
        {
            packetHandlers.Add((int)PacketsNum.welcome, Welcome);
            packetHandlers.Add((int)PacketsNum.udpTest, UDPTest);

            Debug.Log("Starting-Packets Initialized.");
        }

        private void Welcome (int _myId, Packet _packet)
        {
            if (isHost)
            {
                myServer.clients[_myId].receiveClientWelcome(_packet);
            } else
            {
                myClient.receiveServerWelcome(_packet);
            }
        }

        private void UDPTest(int _myId, Packet _packet)
        {
            if (isHost)
            {
                myServer.clients[_myId].receiveClientUDPTest(_packet);
            }
            else
            {
                myClient.receiveServerUDPTest(_packet);
            }
        }
        public Client InitializeClientComponent(GameObject _parentObject, int _tempId)
        {
            Client newClient = _parentObject.AddComponent<Client>();
            newClient.myId = _tempId;
            newClient.packetHandlers = packetHandlers;
            newClient.ClientManager = this;
            return newClient;
        }
        public void DisconnectClient(int _myId)
        {

        }
    }
}