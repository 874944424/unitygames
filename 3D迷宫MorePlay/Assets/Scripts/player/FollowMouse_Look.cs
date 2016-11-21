using UnityEngine;
using System.Collections;

public class FollowMouse_Look : MonoBehaviour
{
    private Transform camera_tran;  //相机的位置
    private float camera_x;

    private Vector3 eulerAngles;

    void Start()
    {
        camera_tran = GameObject.FindGameObjectWithTag(Tags.playercamera).transform;
        eulerAngles = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        camera_x = camera_tran.localRotation.x * 100 + 5 * (camera_x / 25);
        this.transform.localEulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z + -camera_x);
    }

}
