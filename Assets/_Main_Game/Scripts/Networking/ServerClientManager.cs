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

        public Server myServer;
        public Client myClient;

        public NetworkInfo myNetworkInfo;


        public void StartNetworking(NetworkInfo _networkInfo)
        {
            myNetworkInfo = _networkInfo;

            if(packetHandlers==null) packetHandlers = new Dictionary<int, PacketHandler>();
            InitializePacketHandlers();

            if (myNetworkInfo.isHost)
            {
                myServer = gameObject.AddComponent<Server>();
                myNetworkInfo.myId = 1;
                myServer.StartServer(this);
            } else
            {
                myClient = InitializeClientComponent(gameObject, 1);
                myNetworkInfo.myId = 2;
                myClient.ConnectToIp(myNetworkInfo.connectToIp);
            }
        }

        private void InitializePacketHandlers()
        {
            packetHandlers.Add((int)PacketsNum.welcome, Welcome);
            packetHandlers.Add((int)PacketsNum.udpTest, UDPTest);

            //Debug.Log("Starting-Packets Initialized.");
        }

        private void Welcome (int _myId, Packet _packet)
        {
            if (myNetworkInfo.isHost)
            {
                myServer.clients[_myId].receiveClientWelcome(_packet);
            } else
            {
                myClient.receiveServerWelcome(_packet);
            }
        }

        private void UDPTest(int _myId, Packet _packet)
        {
            if (myNetworkInfo.isHost)
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