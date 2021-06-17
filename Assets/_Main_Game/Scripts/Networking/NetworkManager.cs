using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

using Networking;
public class NetworkManager : MonoBehaviour
{
    public NetworkInfo myNetworkInfo;

    public Dictionary<int, PacketHandler> packetHandlers; 
    public Action<NetworkInfo> peerJoined = null;
    public Action<NetworkInfo> peerLeft = null;

    private Discoverer myDiscoverer;
    private ServerClientManager myServerClientManager;

    private void Awake()
    {
        myNetworkInfo = new NetworkInfo();
        //myNetworkInfo.myIp = GetLocalIPAddress();
        myNetworkInfo.myIps = GetLocalIPAddresses();

        myDiscoverer = gameObject.AddComponent<Discoverer>();
        myDiscoverer.peerJoined = peerJoinedDiscovery;
        myDiscoverer.peerLeft = peerLeftDiscovery;
        myDiscoverer.myNetworkInfo = myNetworkInfo;

        myServerClientManager = gameObject.AddComponent<ServerClientManager>();
        packetHandlers = new Dictionary<int, PacketHandler>();
        myServerClientManager.packetHandlers = packetHandlers;
    }
    public void sendTCPPacket(Packet _packet)
    {
        if (myNetworkInfo.isHost)
        {
            myServerClientManager.myServer.SendTCPDataToAll(_packet);
        } else
        {
            myServerClientManager.myClient.SendTCPData(_packet);
        }
    }

    public void StartLobby()
    {
        myNetworkInfo.isHost = false;

        myDiscoverer.StartDiscovering();
    }

    public void joinLobbyHost(string ipAddress)
    {
        myNetworkInfo.isHost = false;
        myNetworkInfo.isSingleplayer = false;
        myNetworkInfo.connectToIp = ipAddress;

        myDiscoverer.StopDiscovering();

        myServerClientManager.StartNetworking(myNetworkInfo);
    }
    public void StartLobbyHost()
    {
        myNetworkInfo.isHost = true;
        myNetworkInfo.isSingleplayer = false;

        if (!myDiscoverer.startedDiscovering)
        {
            myDiscoverer.StartDiscovering();
        }
        myServerClientManager.StartNetworking(myNetworkInfo);

    }

    public void StartNetworkGame()
    {
        if (myDiscoverer.startedDiscovering)
        {
            myDiscoverer.StopDiscovering();
        }
    }

    public void EndLobby()
    {
        if (myDiscoverer.startedDiscovering)
        {
            myDiscoverer.StopDiscovering();
        }
    }
    public void EndJoinLobbyHost()
    {
        myNetworkInfo.isSingleplayer = true;

        myDiscoverer.StartDiscovering();
        //TODO: stop ServerClientManager Networking
    }
    public void EndLobbyHost()
    {
        myNetworkInfo.isHost = false;
        myNetworkInfo.isSingleplayer = true;

        myDiscoverer.StopDiscovering();
        //TODO: stop ServerClientManager Networking
    }

    private void peerJoinedDiscovery (NetworkInfo networkInfo)
    {
        peerJoined?.Invoke(networkInfo);
    }
    private void peerLeftDiscovery(NetworkInfo networkInfo)
    {
        peerLeft?.Invoke(networkInfo); 
    }
    private string GetLocalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            { return ip.ToString(); }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    private List<string> GetLocalIPAddresses()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        List<string> ips = new List<string>();
        int count = 0;
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                count++;
                ips.Add(ip.ToString());
            }
        }
        if (count > 0)
        {
            return ips;
        } else
        {
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
