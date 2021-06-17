using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Networking;

public class ServerClientHandling : MonoBehaviour
{
    public static ServerClientHandling instance;

    private int[] ids;
    private float[,] coords;
    private Transform[] transforms;

    private int myId;

    public GameObject prefabPlayer;

    private ServerClientManager NetworkManager;
    private Discoverer NetworkDiscoverer;
    private ThreadManager threadManager;

    public bool isServer = false;
    public bool managerInitialized = false;
    public bool connectionInitialized = false;
    public bool startDiscovering = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        threadManager = gameObject.AddComponent<ThreadManager>();
    }
    void Start()
    {
        ids = new int[2];
        transforms = new Transform[2];
        coords = new float[2, 3];
        for (int i = 0; i < 2; i++)
        {
            GameObject test = GameObject.Instantiate(prefabPlayer);
            transforms[i] = test.transform;
            ids[i] = i;
        }
        NetworkManager = gameObject.AddComponent<ServerClientManager>();

        NetworkDiscoverer = gameObject.AddComponent<Discoverer>();
        NetworkDiscoverer.peerJoined = ip => peerJoined(ip);
        NetworkDiscoverer.peerLeft = ip => peerLeft(ip);
        startDiscovering = true;

    }
    void FixedUpdate()
    {
        if (startDiscovering)
        {
            startDiscovering = false;
            NetworkDiscoverer.StartDiscovering();

        }
        if (isServer)
        {
            for(int i = 0; i < 2; i++){
                coords[i, 0] = transforms[i].localPosition.x;
                coords[i, 1] = transforms[i].localPosition.y;
                coords[i, 2] = transforms[i].localPosition.z;
            }
            ServerSendTransformTest(ids, coords);
        } else
        {
            if (connectionInitialized)
            {
                int i = myId - 1;
                if (i < ids.Length)
                {
                    coords[i, 0] = transforms[i].localPosition.x;
                    coords[i, 1] = transforms[i].localPosition.y;
                    coords[i, 2] = transforms[i].localPosition.z;
                    ClientSendTransformTest(coords);
                }
            } else if(managerInitialized)
            {
                ClientSendInitialize();
            }
        }
    }
    public void StartServer()
    {
        NetworkManager.StartNetworking(true);
        NetworkManager.packetHandlers.Add((int)PacketsNum.transformInitialize, ServerReadInitialize);
        NetworkManager.packetHandlers.Add((int)PacketsNum.transformTest, ServerReadTransformTest);
        isServer = true;
    }

    public void ConnectToServer()
    {
        NetworkManager.StartNetworking(false);
        NetworkManager.packetHandlers.Add((int)PacketsNum.transformInitialize, ClientReadInitialize);
        NetworkManager.packetHandlers.Add((int)PacketsNum.transformTest, ClientReadTransformTest);
        NetworkManager.packetHandlers.Add((int)PacketsNum.playerDisconnected, playerDisconnected);
        managerInitialized = true;
    }

    public void SetTransform(int _id, float x, float y,float z)
    {
        transforms[_id].localPosition=new Vector3(x, y, z);
    }

    //Server Logic
    private void ServerSendInitialize(int _myId)
    {
        using (Packet _packet = new Packet((int)PacketsNum.transformInitialize))
        {
            NetworkManager.myServer.clients[_myId].SendTCPData(_packet);
        }
    }
    private void ServerSendTransformTest(int[] ids, float[,] coords)
    {
        using (Packet _packet = new Packet((int)PacketsNum.transformTest))
        {
            for (int i = 0; i < 2; i++)
            {
                _packet.Write(ids[i]);
                for (int j = 0; j < 3; j++)
                {
                    _packet.Write(coords[i, j]);
                }
            }
            NetworkManager.myServer.SendTCPDataToAll(_packet);
        }

    }
    private void ServerReadInitialize(int _myId, Packet _packet)
    {
        ServerSendInitialize(_myId);
    }
    private void ServerReadTransformTest(int _myId, Packet _packet)
    {
        int i = _myId - 1;
        if (i < ids.Length)
        {
            float _x = _packet.ReadFloat();
            float _y = _packet.ReadFloat();
            float _z = _packet.ReadFloat();
            SetTransform(i, _x, _y, _z);
        }
    }

    //Client Logic 
    private void ClientSendInitialize()
    {
        using (Packet _packet = new Packet((int) PacketsNum.transformInitialize))
        {
            NetworkManager.myClient.SendTCPData(_packet);
        }
    }
    private void ClientSendTransformTest(float[,] coords)
    {
        using (Packet _packet = new Packet((int) PacketsNum.transformTest))
        {
            int i = myId - 1;
                for (int j = 0; j< 3; j++)
                {
                    _packet.Write(coords[i, j]);
                }
            
            NetworkManager.myClient.SendTCPData(_packet);
        }

    }
    private void ClientReadInitialize(int _myId, Packet _packet)
    {
        myId = _myId;
        connectionInitialized = true;
    }
    private void ClientReadTransformTest(int _myId, Packet _packet)
    {
        int id = _myId - 1;
        for (int i = 0; i < 2; i++)
        {
            int _id = _packet.ReadInt();
            float _x = _packet.ReadFloat();
            float _y = _packet.ReadFloat();
            float _z = _packet.ReadFloat();
            if (_id != id)
            {
                SetTransform(_id, _x, _y, _z);
            }
        }
    }
    private void peerJoined(string ipAddress)
    {
        Debug.Log($"Player with Address {ipAddress} is Online.");
    }
    private void peerLeft(string ipAddress)
    {
        Debug.Log($"Player with Address {ipAddress} is Offline.");
    }
    private void playerDisconnected(int _myId, Packet _packet)
    {
        int _disconnectedPlayerId = _packet.ReadInt();
    }


}
