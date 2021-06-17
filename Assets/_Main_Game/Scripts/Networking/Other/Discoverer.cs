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
        public NetworkInfo myNetworkInfo;
        public bool startedDiscovering { get; private set; }
        public bool keepReceiving = false;

        public object dictionaryLocker;
        public Action<NetworkInfo> peerJoined = null;
        public Action<NetworkInfo> peerLeft = null;

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
            //Debug.Log("Send packet.");
            using (Packet _packet = new Packet())
            {
                myNetworkInfo.writePacket(_packet);
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
                    NetworkInfo _NetworkInfo = new NetworkInfo();
                    foreach (string ipAddressFrom in deleteIps)
                    {
                        _NetworkInfo.myIps[0] = ipAddressFrom;
                        ipList.Remove(ipAddressFrom);
                        ipLastAnswer.Remove(ipAddressFrom);
                        myNetworkInfo.connectableIps.Remove(ipAddressFrom);
                        peerLeft?.Invoke(_NetworkInfo);
                    }
                    deleteIps.Clear();
                }
            }
        }

        private void Receiver()
        {
            Debug.Log("Begin to receive Messages.");
            IPEndPoint fromIP = new IPEndPoint(0, 0);
            IPEndPoint compareAgainst = new IPEndPoint(0, 0);
            NetworkInfo _NetworkInfo = new NetworkInfo();

            float _currentUpdateTime;

            int packetLength = 0;
            while (keepReceiving)
            {
                _currentUpdateTime = getUnityTime();
                byte[] answer = _UdpClient.Receive(ref fromIP);
                //bool isAnswerHost = false ;
                using (Packet _packet = new Packet(answer))
                {
                    if (_packet.UnreadLength() >= 4)
                    {
                        packetLength = _packet.ReadInt();
                    }
                    if (packetLength > 0)
                    {
                        _NetworkInfo.readPacket(_packet);
                    }
                }

                string ipAddressFrom = fromIP.Address.ToString();
                _NetworkInfo.myIps[0] = ipAddressFrom;
                bool ownIpFound = false;
                foreach(string ip in myNetworkInfo.myIps)
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
                            myNetworkInfo.connectableIps.Add(ipAddressFrom, new NetworkInfo(_NetworkInfo));
                            ThreadManager.ExecuteOnMainThread(() =>
                            {
                                peerJoined?.Invoke(new NetworkInfo(_NetworkInfo));
                            });
                        }
                        else
                        {
                            ipLastAnswer[ipAddressFrom] = _currentUpdateTime;
                            if (!_NetworkInfo.compareTo(myNetworkInfo.connectableIps[ipAddressFrom]))
                            {
                                myNetworkInfo.connectableIps[ipAddressFrom].clone(_NetworkInfo);

                                //TODO: Change to smoother workings like: peerChangedStatus
                                ThreadManager.ExecuteOnMainThread(() =>
                                {
                                    //Debug.Log("Information changed!");
                                    peerLeft?.Invoke(_NetworkInfo);
                                    peerJoined?.Invoke(_NetworkInfo);
                                });
                            } else
                            {
                                //Debug.Log("Information is still the same!");
                            }
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
        private void OnApplicationQuit()
        {
            keepReceiving = false;
        }
    }
    
}
