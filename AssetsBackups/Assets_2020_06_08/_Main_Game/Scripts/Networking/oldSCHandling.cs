using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oldNetworking;
public class oldSCHandling : MonoBehaviour
{

    public static oldSCHandling instance;

    private int[] ids;
    private float[,] coords;
    private Transform[] transforms;

    private int myId;

    public GameObject prefabPlayer;

    private Client client;
    private Server server;
    private ThreadManager threadManager;

    public bool isServer = false;
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
    }
    void FixedUpdate()
    {
        if (isServer)
        {
            for (int i = 0; i < 2; i++)
            {
                coords[i, 0] = transforms[i].localPosition.x;
                coords[i, 1] = transforms[i].localPosition.y;
                coords[i, 2] = transforms[i].localPosition.z;
            }
            ServerSendTransformTest(ids, coords);
        }
    }
    public void StartServer()
    {
        server = gameObject.AddComponent<Server>();
        server.StartServer(50, 26950);
        isServer = true;
    }

    public void ConnectToServer()
    {
        client = gameObject.AddComponent<Client>();
        Client.packetHandlers.Add((int)PacketsNum.transformTest, ClientReadTransformTest);
        Client.packetHandlers.Add((int)PacketsNum.playerDisconnected, playerDisconnected);
        client.ConnectToServer();
    }

    public void SetTransform(int _id, float x, float y, float z)
    {
        transforms[_id].localPosition = new Vector3(x, y, z);
    }

    //Server Logic
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
            ServerSend.SendTCPDataToAll(_packet);
        }

    }

    //Client Logic
    private void ClientReadTransformTest(Packet _packet)
    {
        for (int i = 0; i < 2; i++)
        {
            int _id = _packet.ReadInt();
            float _x = _packet.ReadFloat();
            float _y = _packet.ReadFloat();
            float _z = _packet.ReadFloat();
            SetTransform(_id, _x, _y, _z);
        }
    }

    private void playerDisconnected(Packet _packet)
    {
        int _disconnectedPlayerId = _packet.ReadInt();
    }



}
