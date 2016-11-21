using UnityEngine;
using System.Collections;

public class FollowMouseLook : MonoBehaviour
{
    private Quaternion m_CameraTargetRot;
    private Transform camera_tran;
    private float camera_x;

    private Vector3 eulerAngles;

    void Start()
    {
        camera_tran = GameObject.Find("Main Camera").transform;
        eulerAngles = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        camera_x = camera_tran.localRotation.x * 100 + 5 * (camera_x/25);
        this.transform.localEulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z + -camera_x);
    }
}
