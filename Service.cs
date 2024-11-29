
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace MyCustom.Service
{
     public class Service : MonoBehaviour
    {
        // Implementing the BindEvent method using RaiseEvent
        [Tooltip("Bind event to method")]
        public static void BindEvent<T>(byte eventCodeIn, Action<T> _myFunc)
        {
            // Bind event handler to Photon event reception
            PhotonNetwork.NetworkingClient.EventReceived += (eventData) =>
            {
                // Check if the received event matches the event code
                if (eventData.Code == eventCodeIn)
                {
                    Debug.Log($"Code same");
                    // Extract the custom data from eventData (your event payload)
                    object[] data = (object[])eventData.CustomData;
                    T eventDataPayload = (T)data[0]; // Assuming the data is wrapped in an object array
                    
                    // Call the callback with the extracted data
                    _myFunc.Invoke(eventDataPayload);
                }
            };
        }
    }

}