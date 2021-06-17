using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Networking
{
    public partial class Client : MonoBehaviour
    {
        //Server-side
        public void receiveClientWelcome(Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Debug.Log($"{ip} connected succesfully and is now player {myId}.");
            if (myId != _clientIdCheck)
            {
                Debug.Log($"Player \"{_username}\" (ID: {myId} has assumed the wrong Client ID ({_clientIdCheck})!");
                Debug.Log($"His sent username is {_username}");
            }
            else
            {
                username = _username;
            }
        }
        public void receiveClientUDPTest(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Debug.Log($"Received packet via UDP of Client {myId}. Contains Message: {_msg}");
        }

        //Client-side
        public void receiveServerWelcome(Packet _packet)
        {
            int _myNewId = _packet.ReadInt();
            string _msg = _packet.ReadString();

            Debug.Log($"Message from Server: {_msg}");
            myId = _myNewId;
            ClientManager.myNetworkInfo.myId = myId;

            //Send Welcome package back and connect via Udp.
            SendWelcomeReceived();
        }

        public void receiveServerUDPTest(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Debug.Log($"Received packet via UDP. Contains Message: {_msg}");

            //Send UDP Test package back
            SendUDPTestReceived();
        }
    }
}
