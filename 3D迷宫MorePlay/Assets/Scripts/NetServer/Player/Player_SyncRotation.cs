using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Player_SyncRotation : NetworkBehaviour
{
    [SyncVar (hook = "OnPlayerRotSynced")]
    private float syncPlayerRotation;       //Y轴
    [SyncVar (hook = "OnCamRotSynced")]
    private float syncCamRotation;          //X轴

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform camTransform;
  
    private float lerpRotate = 20f;
    private float lastPlayerRot;
    private float lastCamRot;
    private float threshold = 1f;

    private List<float> syncPlayerRotList = new List<float>();
    private List<float> synCamRotList = new List<float>();
    private float closeEnough = 0.4f;
    [SerializeField] 
    private bool useHistorialInterpolation = false;               //使用历史

    void Start()
    {

    }
    void Update()
    {
        LerpTotations();
    }
    void FixedUpdate()
    {
        TransmitRotations();
    }
    void LerpTotations()
    {
        if(!isLocalPlayer)
        {
            if (useHistorialInterpolation)
            {
                HistoricalInterpolation();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }
    void HistoricalInterpolation()
    {
        if (syncPlayerRotList.Count > 0)
        {
            LerpPlayerRotation(syncPlayerRotList[0]);

            if (Mathf.Abs(playerTransform.localEulerAngles.y - syncPlayerRotList[0]) < closeEnough)
            {
                syncPlayerRotList.RemoveAt(0);
            }
            //Debug.Log(syncPlayerRotList.Count.ToString() + "syncPlayerRotListCount");
        }

        if (synCamRotList.Count > 0)
        {
            LerpCamRot(synCamRotList[0]);
            if (Mathf.Abs(camTransform.localEulerAngles.x - synCamRotList[0]) < closeEnough)
            {
                synCamRotList.RemoveAt(0);
            }
            //Debug.Log(synCamRotList.Count.ToString() + "syncCamRotListCount");
        }
    }
    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncPlayerRotation);
        LerpCamRot(syncCamRotation);
    }
    void LerpPlayerRotation(float rotAngle)
    {
        Vector3 playerNowRot = new Vector3(0, rotAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNowRot), lerpRotate * Time.deltaTime);
    }
    void LerpCamRot(float rotAngle)
    {
        //localRotation子物体在父物体的相对位移
        Vector3 camNowRot = new Vector3(rotAngle, 0, 0);
        camTransform.localRotation = Quaternion.Lerp(camTransform.localRotation, Quaternion.Euler(camNowRot), lerpRotate * Time.deltaTime);
    }

    [Command]       //命令需要调用
    void CmdProvideRotationsToServer(float playerRot, float camRot)
    {
        syncPlayerRotation = playerRot;
        syncCamRotation = camRot;
    }

    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            //if(Quaternion.Angle(playerTransform.rotation, lastPlayerRot) > threshold || Quaternion.Angle(camTransform.rotation, lastCamRot) > threshold)
            if(CheckifBeyondhold(playerTransform.localEulerAngles.y, lastPlayerRot) || CheckifBeyondhold(camTransform.localEulerAngles.x, lastCamRot))
            {               
                lastPlayerRot = playerTransform.localEulerAngles.y;
                lastCamRot = camTransform.localEulerAngles.x;
                CmdProvideRotationsToServer(lastPlayerRot, lastCamRot);
            }
        }
    }
    bool CheckifBeyondhold(float rot1, float rot2)
    {
        if (Mathf.Abs(rot1 - rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    [Client]
    void OnPlayerRotSynced(float latestPlayerRot)
    {
        syncPlayerRotation = latestPlayerRot;
        syncPlayerRotList.Add(syncPlayerRotation);
    }
    [Client]
    void OnCamRotSynced(float latestCamRot)
    {
        syncCamRotation = latestCamRot;
        synCamRotList.Add(syncCamRotation);
    }
}
