using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{
    public class Server : MonoBehaviour
    {
        public int numberOfClients = 1;
        public string username;
        public int Port;

        public Dictionary<int, Client> clients;

        private ServerClientManager serverManager;
        private TcpListener tcpListener;
        private UdpClient udpListener;

        public void StartServer( ServerClientManager manager)
        {
            serverManager = manager;
            username = serverManager.username;
            Port = serverManager.port;

            clients = new Dictionary<int, Client>();

            Debug.Log("Server is starting...");
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            Debug.Log($"Server started on Port {Port}.");

        }

        private void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            ThreadManager.ExecuteOnMainThread(() => AddClient(_client));

        }

        private void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId < 2)
                    {
                        return;
                    }
                    if (numberOfClients >= _clientId)
                    {
                        if (clients[_clientId].udp.endPoint == null)
                        {
                            clients[_clientId].udp.Connect(_clientEndPoint);
                            return;
                        }

                        if (clients[_clientId].udp.endPoint.Equals(_clientEndPoint))
                        {
                            clients[_clientId].udp.HandleData(_packet);
                        }
                    }
                }

            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving UDP data: {_ex}");
            }
        }

        public void SendTCPDataToAll(Packet _packet)
        {
            if (numberOfClients > 1)
            {

                Client _client;
                foreach (KeyValuePair<int, Client> kvp in clients)
                {
                    _client = kvp.Value;
                    _client.SendTCPData(_packet);
                }
            } else
            {
                Debug.Log("No Clients are connected to the server.");
            }
        }

        public void SendUDPDataToAll(Packet _packet)
        {
            Client _client;
            foreach (KeyValuePair<int, Client> kvp in clients)
            {
                _client = kvp.Value;
                _client.SendUDPData(_packet);
            }
        }

        public void AddClient( TcpClient _client)
        {
            numberOfClients += 1;

            GameObject newClientObject = new GameObject($"Client {numberOfClients}");
            newClientObject.transform.SetParent(this.transform);

            Client newClient = serverManager.InitializeClientComponent(newClientObject, numberOfClients);
            clients.Add(numberOfClients, newClient);
            newClient.Connect(_client);
        }
    }
}
