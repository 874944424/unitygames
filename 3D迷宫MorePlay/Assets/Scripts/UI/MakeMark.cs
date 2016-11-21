using UnityEngine;
using System.Collections;

public class MakeMark : MonoBehaviour
{
    public GameObject markPre;
    public GameObject markparent;
    private float length = 5f;  //射线长度距离

    void Start() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Check();
        }
    }

    //检测碰撞到物体没有
    void Check()
    {
        Camera camera_player = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>();
        Ray r = camera_player.ScreenPointToRay(Input.mousePosition); //屏幕坐标设为射线目标位置
        RaycastHit hit; //射线碰撞信息

        if (Physics.Raycast(r, out hit, length))
        {
            if (hit.collider.tag == Tags.wall)
            {
                //生成标记
                Vector3 pos = hit.point;
                GameObject mark_obj = Instantiate(markPre, pos, Quaternion.identity) as GameObject;
                mark_obj.transform.LookAt(hit.point - hit.normal);
                mark_obj.transform.Translate(Vector3.back * 0.01f);
                mark_obj.transform.parent = markparent.transform;
            }
        }
    }
}
