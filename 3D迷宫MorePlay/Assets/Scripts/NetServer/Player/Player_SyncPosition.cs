using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

//[NetworkSettings (channel = 0, sendInterval =0.033f)]
public class Player_SyncPosition : NetworkBehaviour {
    [SyncVar (hook = "SyncPositionValue")]
    private Vector3 syncPos;
    [SerializeField]
    Transform myTransform;
    [SerializeField]
    private float lerpRote;
    private float normalLerpRote = 15;
    private float fasterLerpRote = 25;

    private Vector3 lastPos;            //优化检测移动
    private float threshold = 0.5f;     //最小移动变化响应距离

    private NetworkClient nClient;      //延迟查看点
    private int latency;

    private Text latencyText;           //显示

    private List<Vector3> syncPostList = new List<Vector3>();
    [SerializeField]
    private bool useHistorialLerping = false;               //使用历史
    private float closeEnough = 0.11f;

    void Start()
    {
        nClient = GameObject.Find("NetWorkManager").GetComponent<NetworkManager>().client;
        latencyText = GameObject.Find("Latency_Text").GetComponent<Text>();
        lerpRote = normalLerpRote;
    }
    void Update()
    {
        LerpPosition();
        ShowLatency();
    }

	void FixedUpdate ()
    {
        TransmitPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (useHistorialLerping)
            {
                HistorialLerping();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
        //Debug.Log("Command called!");
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }

    void ShowLatency()  //显示延迟
    {
        if (isLocalPlayer)
        {
            latency = nClient.GetRTT();
            latencyText.text = latency.ToString();
        }
    }

    void OrdinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRote);
    }

    void HistorialLerping()
    {//在有延迟下，的记录历史移动节点后的移动
        if (syncPostList.Count > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPostList[0], Time.deltaTime * lerpRote);

            if (Vector3.Distance(myTransform.position, syncPostList[0]) < closeEnough)
            {
                syncPostList.RemoveAt(0);
            }

            if (syncPostList.Count > 10)
            {
                lerpRote = fasterLerpRote;
            }
            else
            {
                lerpRote = normalLerpRote;
            }

            //Debug.Log(syncPostList.Count.ToString());
        }
    }

    [Client]
    void SyncPositionValue(Vector3 latesPos)
    {
        syncPos = latesPos;
        syncPostList.Add(syncPos);
    }
}
