using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

public class UnityPingTest : MonoBehaviour
{
    public bool start = false;
    public bool checkForPings = false;
    public float searchTimeout=10.0f;
    public string ipBase;
    private float startTime;
    private float timeSearching;
    private bool[] startedPing;
    private int test = 0;
    Ping[] pinger;

    // Start is called before the first frame update
    private void Start()
    {
        startedPing = new bool[255];
        startedPing = Enumerable.Repeat(false, 255).ToArray();
        pinger = new Ping[255];
        startTime = Time.time;
        string localIp = GetLocalIPAddress();
        int ipBaseLength = localIp.LastIndexOf(".") +1;
        ipBase = localIp.Substring(0, ipBaseLength);
        Debug.Log(ipBase);
    }
    void StartProgram(int i)
    {
        string ip = ipBase + (i+1).ToString();
        pinger[i] = new Ping(ip);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (start)
        {
            start = false;
            Debug.Log("Start to ping!");
            for (int i = 0; i < 255; i++)
            {
                startedPing[i] = true;
                StartProgram(i);
            }
            Debug.Log("All Pings have started!");
            checkForPings = true;
            startTime = Time.time;
            test = 0;
        }

        if (checkForPings)
        {
            timeSearching = Time.time - startTime;
            for (int i = 0; i < 255; i++)
            {
                if (startedPing[i])
                {
                    if (pinger[i].isDone)
                    {
                        Debug.Log($"Ip {pinger[i].ip.ToString()} was found!");
                        Debug.Log($"It took {pinger[i].time} ms to reach target!");

                        startedPing[i] = false;
                    }
                }
            }
            if (timeSearching > searchTimeout)
            {
                checkForPings = false;
            }
        }


    }
    public string GetLocalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            { return ip.ToString(); }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
