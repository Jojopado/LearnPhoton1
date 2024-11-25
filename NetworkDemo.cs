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
