using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Zombie_MotionSync : NetworkBehaviour
{
    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private float syncRot;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Transform myTransform;
    private float lerpRote = 10;
    private float posThreshold = 0.1f;
    private float rotThreshold = 5;

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        TransmitMotion();
        LerpMotion();
    }

    void TransmitMotion()
    {
        if (!isServer)
        {
            return;
        }
        if (Vector3.Distance(myTransform.position, lastPos) > posThreshold ||
            Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
        {
            lastPos = myTransform.position;
            lastRot = myTransform.rotation;

            syncPos = myTransform.position;
            syncRot = myTransform.localEulerAngles.y;
        }
    }

    void LerpMotion()
    {
        if (isServer)
        {
            return;
        }
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRote);

        Vector3 newRot = new Vector3(0, syncRot, 0);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRote);
    }
}
