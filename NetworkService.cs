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
using System;

namespace MyCustom.NetworkService
{
    public class NetworkService : MonoBehaviour
    {
        [Tooltip("Using RaiseEvent to send data to all players")]
        public static void TriggerToAll(Action _func, params object[] _raiseEventData)
        {
            if (_raiseEventData == null || _raiseEventData.Length == 1)
            {
                Debug.LogWarning("No data provided for the event.");
            }

            _func.Invoke();
            PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Tcp;
            RaiseEventOptions _raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)_raiseEventData[0], _raiseEventData[1], _raiseEventOptions, SendOptions.SendReliable);
        }
        public static void TriggerUdpToAll(Action _func, params object[] _raiseEventData)
        {
            _func.Invoke();
            PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Udp;
            RaiseEventOptions _raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)_raiseEventData[0], _raiseEventData[1], _raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}