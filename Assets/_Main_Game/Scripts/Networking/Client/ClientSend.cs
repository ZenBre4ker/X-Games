using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Networking
{
    public partial class Client : MonoBehaviour
    {
        public void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            tcp.SendData(_packet);
        }
        public void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            udp.SendData(_packet);
        }

        //Server-side
        public void SendServerWelcome()
        {
            string _msg = "Welcome to the Server!";

            using (Packet _packet = new Packet((int)PacketsNum.welcome))
            {
                _packet.Write(myId);
                _packet.Write(_msg);

                SendTCPData(_packet);
            }
        }
        public void SendServerUDPTest()
        {
            using (Packet _packet = new Packet((int)PacketsNum.udpTest))
            {
                _packet.Write("A test packet for UDP.");

                SendUDPData(_packet);
            }
        }
        //Client-side
        public void SendWelcomeReceived()
        {
            using (Packet _newPacket = new Packet((int)PacketsNum.welcome))
            {
                _newPacket.Write(myId);
                _newPacket.Write(username);

                SendTCPData(_newPacket);
            }
            udp.Connect(((IPEndPoint)tcp.socket.Client.LocalEndPoint).Port);
        }

        public void SendUDPTestReceived()
        {
            using (Packet _newPacket = new Packet((int)PacketsNum.udpTest))
            {
                _newPacket.Write("Received a UDP packet.");

                SendUDPData(_newPacket);
            }
        }
    }
}
