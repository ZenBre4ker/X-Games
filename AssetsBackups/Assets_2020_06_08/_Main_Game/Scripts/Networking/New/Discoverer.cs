using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEditor;

namespace Networking
{
    public class Discoverer : MonoBehaviour
    {
        public int maxAnswerTime = 5;
        public int updateTime = 1;
        public string username = "Sven_";
        public string myIp;
        public List<string> myIps;
        public bool startedDiscovering { get; private set; }
        public bool keepReceiving = false;
        public bool isHost = false;

        public object dictionaryLocker;
        public Dictionary<string, bool> connectableIps;
        public Action<string> peerJoined = null;
        public Action<string> peerLeft = null;

        private string MULTICAST_IP = "238.212.223.50";
        private int MULTICAST_PORT = 2015;

        private AndroidJavaObject multicastLock;
        private UdpClient _UdpClient;
        private Dictionary<string,IPEndPoint> ipList;
        private Dictionary<string, float> ipLastAnswer;
        private List<string> deleteIps;

        private float currentUpdateTime = 0;
        private float lastUpdateTime = 0;
        private float deltaUpdateTime = 0;
        private DateTime unityTimeStamp;
        private float unityFloatTime;
        // Start is called before the first frame update
        public void StartDiscovering()
        {
            //Make receiving messages on android available for Multicasting
#if UNITY_ANDROID && !UNITY_EDITOR
            getMulticastLock();
#endif
            unityTimeStamp = DateTime.Now;
            unityFloatTime = Time.time;

            connectableIps = new Dictionary<string, bool>();
            ipList = new Dictionary<string, IPEndPoint>();
            ipLastAnswer = new Dictionary<string, float>();
            deleteIps = new List<string>();
            dictionaryLocker = new object();

            _UdpClient = new UdpClient();
            _UdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, MULTICAST_PORT));
            _UdpClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_IP));
            startedDiscovering = true;
            keepReceiving = true;
            Task.Run(() => Receiver());
            
        }

        public void StopDiscovering()
        {
            keepReceiving = false;
            startedDiscovering = false;

#if UNITY_ANDROID && !UNITY_EDITOR
           multicastLock?.Call("release");
#endif

        }
        private void FixedUpdate()
        {
            if (startedDiscovering)
            {
                currentUpdateTime = Time.time;
                deltaUpdateTime = currentUpdateTime - lastUpdateTime;
                if (deltaUpdateTime >= updateTime)
                {
                    Sender();
                    lastUpdateTime = currentUpdateTime;
                }
            }
        }
        
        private void Sender()
        {
            Debug.Log("Send packet.");
            using (Packet _packet = new Packet())
            {
                _packet.Write(isHost);
                if (isHost)
                {
                    _packet.Write($"{username} is a Host");
                }
                else
                {
                    _packet.Write($"{username} wants to join");
                }
                _packet.WriteLength();
                byte[] iAmHere = _packet.ToArray();
                IPEndPoint mcastEndPoint = new IPEndPoint(IPAddress.Parse(MULTICAST_IP), MULTICAST_PORT);
                _UdpClient.Send(iAmHere, iAmHere.Length, mcastEndPoint);
            }

            lock (dictionaryLocker)
            {
                foreach (KeyValuePair<string, float> kvp in ipLastAnswer)
                {
                    lastUpdateTime = kvp.Value;
                    deltaUpdateTime = currentUpdateTime - lastUpdateTime;
                    if (deltaUpdateTime >= maxAnswerTime)
                    {
                        string ipAddressFrom = kvp.Key;
                        deleteIps.Add(ipAddressFrom);
                    }
                }
                if (deleteIps.Count > 0)
                {
                    foreach (string ipAddressFrom in deleteIps)
                    {
                        ipList.Remove(ipAddressFrom);
                        ipLastAnswer.Remove(ipAddressFrom);
                        connectableIps.Remove(ipAddressFrom);
                        peerLeft?.Invoke(ipAddressFrom);
                    }
                    deleteIps.Clear();
                }
            }
        }

        private void Receiver()
        {
            Debug.Log("Begin to receive Messages.");
            bool _keepReceiving = keepReceiving;
            IPEndPoint fromIP = new IPEndPoint(0, 0);
            IPEndPoint compareAgainst = new IPEndPoint(0, 0);

            float _currentUpdateTime;

            int packetLength = 0;
            while (_keepReceiving)
            {
                _currentUpdateTime = getUnityTime();
                byte[] answer = _UdpClient.Receive(ref fromIP);
                bool isAnswerHost = false ;
                using (Packet _packet = new Packet(answer))
                {
                    if (_packet.UnreadLength() >= 4)
                    {
                        packetLength = _packet.ReadInt();
                    }
                    if (packetLength > 0)
                    {
                        isAnswerHost = _packet.ReadBool();
                        string message = _packet.ReadString();
                    }
                }

                string ipAddressFrom = fromIP.Address.ToString();
                bool ownIpFound = false;
                foreach(string ip in myIps)
                {
                    if(String.Equals(ipAddressFrom, ip))
                    {
                        ownIpFound = true;
                        break;
                    }
                }
                if (!ownIpFound)
                {
                    //Debug.Log($"Received Message from {ipAddressFrom}");
                    lock (dictionaryLocker)
                    {
                        if (!ipList.TryGetValue(ipAddressFrom, out compareAgainst))
                        {
                            ipList.Add(ipAddressFrom, fromIP);
                            ipLastAnswer.Add(ipAddressFrom, _currentUpdateTime);
                            connectableIps.Add(ipAddressFrom, isAnswerHost);
                            ThreadManager.ExecuteOnMainThread(() =>
                            {
                                peerJoined?.Invoke(ipAddressFrom);
                            });
                        }
                        else
                        {
                            ipLastAnswer[ipAddressFrom] = _currentUpdateTime;
                        }
                    }
                }
            }
        }

        private float getUnityTime()
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan elapsedSpan = currentDateTime - unityTimeStamp;
            float currentUpdateTime = unityFloatTime + (float)elapsedSpan.TotalSeconds;
            return currentUpdateTime;
        }
        public bool getMulticastLock()
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                try
                {
                    using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        multicastLock = wifiManager.Call<AndroidJavaObject>("createMulticastLock", "lock");
                        multicastLock.Call("acquire");
                        bool isHeld = multicastLock.Call<bool>("isHeld");
                        return isHeld;
                    }
                }
                catch (Exception err)
                {
                    Debug.Log(err.ToString());
                }
            }
            return false;
        }
    }
}
