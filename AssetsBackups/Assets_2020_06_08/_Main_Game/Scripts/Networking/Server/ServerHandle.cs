using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oldNetworking
{
    public class ServerHandle
    {
        public static void welcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Debug.Log($"Player \"{_username}\" (ID: {_fromClient} has assumed the wrong Client ID ({_clientIdCheck})!");
            }
        }
        public static void UDPTestReceived(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Debug.Log($"Received packet via UDP. Contains Message: {_msg}");
        }
    }
}
