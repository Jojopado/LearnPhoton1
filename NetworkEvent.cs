using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace MyCustom.NetworkEvent
{
    //接收
    public class NetworkEvent : MonoBehaviour
    {
        public static void event1()
        {
        }
        public static void TestTCP()
        {
            Debug.Log("Hey TCP is here TestTCP");
        }
        public static void TestUDP()
        {

        }
    }
}