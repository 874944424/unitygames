using UnityEngine;
using System.Collections;

public class CSBulletGenertor : MonoBehaviour
{
    public int shootRante = 5; //每秒可以射击多少子弹
    public CSFlash flash;
    public GameObject bulletprafab; //子弹模型
    public GameObject bullet_parent;    //子弹父物体类方便管理
    public GameObject bulletdection;    //默认开火方向

    private float timer = 0;
    private bool isFiring = false; //是否正在开火

    public Camera soldierCamera;

    void Start()
    {
        bullet_parent = GameObject.Find("bullet_parent");
        soldierCamera = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
        }
        if (isFiring)
        {
            timer += Time.deltaTime;
            if (timer > 1f / shootRante)    //控制射击速率
            {
                Shoot();
                timer -= 1f / shootRante;
            }
        }
    }

    void Shoot()
    {
        flash.Flash();//闪光
        GameObject go_bullet = GameObject.Instantiate(bulletprafab, transform.position, Quaternion.identity) as GameObject;
        go_bullet.transform.parent = bullet_parent.transform;

        //得到当前物体的目标
        Vector3 point_center = soldierCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //跑动中不能瞄准
        GameObject player = GameObject.FindGameObjectWithTag(Tags.palyer);
        /*射向屏幕中心*/
        //Input.GetMouseButton(1)
        if (player.GetComponent<Animation>().IsPlaying(AnimsName.soldierIdle_animName))
        {
            //TODO如果在移动相机中心的 +（-0.2, 0, 0）的位置。
            point_center += soldierCamera.transform.forward * 1000;
            go_bullet.transform.LookAt(point_center);
        }
        else
        {
            go_bullet.transform.LookAt(bulletdection.transform.position);
        }
    }
}
