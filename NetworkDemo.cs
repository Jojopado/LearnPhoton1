using UnityEngine;


public class NetworkDemo : MonoBehaviour
{
    public struct PlayerInfo
    {
        public int userID;
        public float power;
        public float speed;
    }
    private void OnEnable()
    {
        //PhotonNetwork.NetworkingClient.EventReceived += OnEvent; 官網會這樣綁，OnEvent是自己的function
        Service.BindEvent<int>(NetworkEvent.event1, OnEvent1);
        Service.BindEvent<int, string, float>(NetworkEvent.TestTcp, ReceiveTcp);
        Service.BindEvent<PlayerInfo>(NetworkEvent.TestUdp, ReceiveUdp);

    }
    private void OnDisable()
    {
        Service.UnBindEvent<int, string, float>(NetworkEvent.TestTcp, ReceiveTcp);
        Service.UnBindEvent<PlayerInfo>(NetworkEvent.TestUdp, ReceiveUdp);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int i = Random.Range(1, 100);
            string s = $"key{Random.Range(1, 500)}"
            float f = Random.Range(-1, -1000);
            NetworkService.TriggerToAll(NetworkEvent.TestTcp, i , s, f);
            /* 會這樣Invoke
            private void SendMoveUnitsToTargetPositionEvent()
            {
                object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
                PhotonNetwork.RaiseEvent(MoveUnitsToTargetPositionEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }
            */
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            PlayerInfo info = new();
            info.userID = Service.MyUserID();
            info.power  = 5;
            info.speed = 20;
            NetworkService.TriggerUdpToAll(NetworkEvent.TestUdp, info);
        }
    }
    void OnEvent1(int data) { Debug.Log(data)}
    void ReceiveTcp(int data1, int data2, float data3) { Debug.Log($"RecieveTcp: int ={data1}, string ={data2}, float = {data3}")}
    void ReceiveUdp(PlayerInfo info) { Debug.Log($"RecieveUdp: userId = {info.userID}, power = {info.power}, speed = {info.speed}")}

}
