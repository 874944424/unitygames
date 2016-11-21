using UnityEngine;
using System.Collections;

public class CSBulletGenertor : MonoBehaviour
{
    public int shootRante = 10; //每秒可以射击多少子弹
    public CSFlash flash;
    public GameObject bulletprafab;

    private float timer = 0;
    private bool isFiring = false; //是否正在开火

    public Camera soldierCamera;

    void Start()
    {


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
        //得到当前物体的目标
        Vector3 point_center = soldierCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hitinfo;
        bool isCollider = Physics.Raycast(point_center, soldierCamera.transform.forward, out hitinfo);
        if (isCollider)
        {
            go_bullet.transform.LookAt(hitinfo.point);
        }
        else
        {
            point_center += soldierCamera.transform.forward * 1000;
            go_bullet.transform.LookAt(point_center);
        }
    }
}
