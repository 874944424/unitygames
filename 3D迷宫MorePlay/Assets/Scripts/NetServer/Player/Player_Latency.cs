using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player_Latency : NetworkBehaviour
{

    private NetworkClient nClient;      //延迟查看点
    private int latency;
    private Text latencyText;           //显示

    public override void OnStartLocalPlayer()
    {
        nClient = GameObject.Find("NetWorkManager").GetComponent<NetworkManager>().client;
        latencyText = GameObject.Find("Latency_Text").GetComponent<Text>();
    }

    void Update()
    {
        ShowLatency();
    }

    void ShowLatency()  //显示延迟
    {
        if (isLocalPlayer)
        {
            latency = nClient.GetRTT();
            latencyText.text = latency.ToString();
        }
    }
}
