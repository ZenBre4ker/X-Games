﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oldNetworking
{
    public class ClientSend : MonoBehaviour
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.tcp.SendData(_packet);

        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.udp.SendData(_packet);
        }
        #region Packets
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)PacketsNum.welcome))
            {
                _packet.Write(Client.instance.myId);
                _packet.Write("Insert Username here.");

                SendTCPData(_packet);
            }
        }

        public static void UDPTestReceived()
        {
            using (Packet _packet = new Packet((int)PacketsNum.udpTest))
            {
                _packet.Write("Received a UDP packet.");

                SendUDPData(_packet);
            }
        }
        #endregion
    }
}