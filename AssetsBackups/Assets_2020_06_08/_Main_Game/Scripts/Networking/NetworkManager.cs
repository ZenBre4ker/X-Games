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
    public bool isHost = false;
    public bool isSinglePlayer = true;
    public Dictionary<int, PacketHandler> packetHandlers; 
    public Action<string> peerJoined = null;
    public Action<string> peerLeft = null;

    public string myIp;
    public List<string> myIps;
    public string username="Sven";
    public Dictionary<string,bool> connectableIps;
    public int connectionChangeState=0; //if IpList gets refreshed and changed, connectionState gets increased

    private Discoverer myDiscoverer;
    private ServerClientManager myServerClientManager;

    private void Awake()
    {
        myIp = GetLocalIPAddress();
        myIps = GetLocalIPAddresses();

        myDiscoverer = gameObject.AddComponent<Discoverer>();
        myDiscoverer.peerJoined = peerJoinedDiscovery;
        myDiscoverer.peerLeft = peerLeftDiscovery;
        myDiscoverer.myIp = myIp;
        myDiscoverer.myIps = myIps;
        connectableIps = myDiscoverer.connectableIps;

        myServerClientManager = gameObject.AddComponent<ServerClientManager>();
        packetHandlers = new Dictionary<int, PacketHandler>();
        myServerClientManager.packetHandlers = packetHandlers;
    }
    public void sendTCPPacket(Packet _packet)
    {
        if (isHost)
        {
            myServerClientManager.myServer.SendTCPDataToAll(_packet);
        } else
        {
            myServerClientManager.myClient.SendTCPData(_packet);
        }
    }

    public void StartLobby()
    {
        isHost = false;
        myDiscoverer.isHost = isHost;
        myDiscoverer.StartDiscovering();
    }

    public void joinLobbyHost(string ipAddress)
    {
        isHost = false;
        isSinglePlayer = false;
        myDiscoverer.StopDiscovering();
        myServerClientManager.ip = ipAddress;
        myServerClientManager.StartNetworking(isHost);
    }
    public void StartLobbyHost()
    {
        isHost = true;
        isSinglePlayer = false;
        myDiscoverer.isHost = isHost;
        if (!myDiscoverer.startedDiscovering)
        {
            myDiscoverer.StartDiscovering();
        }
        myServerClientManager.StartNetworking(isHost);

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
        isSinglePlayer = true;
        myDiscoverer.StartDiscovering();
        //TODO: stop ServerClientManager Networking
    }
    public void EndLobbyHost()
    {
        isHost = false;
        isSinglePlayer = true;
        myDiscoverer.StopDiscovering();
        //TODO: stop ServerClientManager Networking
    }

    private void peerJoinedDiscovery (string ip)
    {
        connectionChangeState += 1;
        peerJoined?.Invoke(ip);
    }
    private void peerLeftDiscovery(string ip)
    {
        connectionChangeState += 1;
        peerLeft?.Invoke(ip); 
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
