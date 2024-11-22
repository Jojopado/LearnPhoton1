using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkSyncBody : MonoBehaviourPunCallbacks
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    Transform headRig;
    Transform leftHandRig;
    Transform rightHandRig;

    public List<Renderer> hidingRenderers = new List<Renderer>();

    public const byte UpdatePosCode = 0;


    void Start()
    {
        foreach (Renderer renderer in hidingRenderers)
        {
            renderer.enabled = false;
        }

        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
    }

    void Update()
    {
        OnGettingBodyPartsData();
    }

    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnStartUpdateBodyPartsEventRecevied;
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnStartUpdateBodyPartsEventRecevied;
    }

    void OnStartUpdateBodyPartsEventRecevied(EventData eventData)
    {
        if (eventData.Code == UpdatePosCode)
        {
            object[] InputData = (object[])eventData.CustomData;

            head.transform.position = (Vector3)InputData[0];
            head.transform.rotation = (Quaternion)InputData[1];
            leftHand.transform.position = (Vector3)InputData[2];
            leftHand.transform.rotation = (Quaternion)InputData[3];
            rightHand.transform.position = (Vector3)InputData[4];
            rightHand.transform.rotation = (Quaternion)InputData[5];
        }
    }

    void OnGettingBodyPartsData()
    {
        object[] OutputData = new object[]
        {
            headRig.transform.position,
            headRig.rotation,
            leftHandRig.transform.position,
            leftHandRig.rotation,
            rightHandRig.transform.position,
            rightHandRig.rotation
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(UpdatePosCode, OutputData, raiseEventOptions, SendOptions.SendReliable);
    }

    void SetInitialPlayerPosition()
    {
        Vector3 spawnPosition = new Vector3(PhotonNetwork.LocalPlayer.ActorNumber * 2.0f, 0, 1);
        transform.position = spawnPosition;
    }
}
