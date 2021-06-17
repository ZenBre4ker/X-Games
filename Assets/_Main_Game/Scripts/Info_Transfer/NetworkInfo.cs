using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class NetworkInfo
{
    //local information
    public int myId;
    public int port;
    public string connectToIp;

    //public string myIp;//TODO: delete, because myIp should be obsolete

    public List<string> myIps;
    public Dictionary<string, NetworkInfo> connectableIps;
    public bool isSingleplayer;

    //multiplayer information
    public string myName;
    public bool isHost;
    public int numberOfClients;

    public NetworkInfo()
    {
        myId = 1;
        port = 26950;
        connectToIp = "192.168.178.55";

        //myIp = "192.168.178.1";

        myIps = new List<string>();
        myIps.Add("192.168.178.1");
        connectableIps = new Dictionary<string, NetworkInfo>();
        isSingleplayer = true;

        myName = "NamelessYet";
        isHost = false;
        numberOfClients=0;
    }

    public NetworkInfo(NetworkInfo _networkInfo)
    {
        myIps = new List<string>();
        connectableIps = new Dictionary<string, NetworkInfo>();
        clone(_networkInfo);
    }

    public void clone(NetworkInfo _networkInfo)
    {
        myId = _networkInfo.myId;
        port = _networkInfo.port;
        connectToIp = _networkInfo.connectToIp;

        foreach (string str in _networkInfo.myIps)
        {
            if (!myIps.Contains(str))
            {
                myIps.Add(str);
            }
        }

        foreach (KeyValuePair<String, NetworkInfo> kvp in _networkInfo.connectableIps)
        {
            bool containsKey = connectableIps.ContainsKey(kvp.Key);
            if (containsKey)
            {
                connectableIps[kvp.Key].clone(kvp.Value);
            }
            else
            {
                connectableIps.Add(kvp.Key, kvp.Value);
            }
        }
        isSingleplayer = _networkInfo.isSingleplayer;

        myName = _networkInfo.myName;
        isHost = _networkInfo.isHost;
        numberOfClients = _networkInfo.numberOfClients;
    }

    public bool compareTo(NetworkInfo _networkInfo)
    {
        if (myId != _networkInfo.myId)
        {
            return false;
        }
        if (port != _networkInfo.port)
        {
            return false;
        }
        if (connectToIp != _networkInfo.connectToIp)
        {
            return false;
        }

        foreach (string str in _networkInfo.myIps)
        {
            if (!myIps.Contains(str))
            {
                return false;
            }
        }

        foreach (KeyValuePair<String, NetworkInfo> kvp in _networkInfo.connectableIps)
        {
            if (connectableIps.ContainsKey(kvp.Key))
            {
                if (!connectableIps[kvp.Key].compareTo(kvp.Value))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        if (isSingleplayer != _networkInfo.isSingleplayer)
        {
            return false;
        }

        if (!(String.Equals(myName,_networkInfo.myName)))
        {
            return false;
        }
        if (isHost != _networkInfo.isHost)
        {
            return false;
        }
        if (numberOfClients != _networkInfo.numberOfClients)
        {
            return false;
        }

        return true;
    }
    public void writePacket(Packet _packet)
    {
        _packet.Write(myName);
        _packet.Write(isHost);
        _packet.Write(numberOfClients);
    }
    public void readPacket(Packet _packet)
    {
        myName = _packet.ReadString();
        isHost = _packet.ReadBool();
        numberOfClients = _packet.ReadInt();
    }
}


