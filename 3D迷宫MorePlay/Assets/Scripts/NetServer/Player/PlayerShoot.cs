using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class PlayerShoot : NetworkBehaviour
{
    private int damage = 25;
    private float range = 200;
    [SerializeField]
    private Transform camTransform;
    private RaycastHit hit;

    public GameObject bulletpre;            //子弹的模型
    public AudioClip fire_audioclip;        //开火的声音

    void Update()
    {
        CheckShooting();
    }

    void CheckShooting()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Shootbullet();
            Shoot();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Shootbullet()
    {
        GameObject go_bullet = GameObject.Instantiate(bulletpre, transform.position, Quaternion.identity) as GameObject;
        transform.GetComponent<AudioSource>().PlayOneShot(fire_audioclip);

        //得到当前物体的目标
        Vector3 point_center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        point_center += Camera.main.transform.forward * 1000;
        go_bullet.transform.LookAt(point_center);
    }

    void Shoot()
    {
        if (Physics.Raycast(camTransform.TransformPoint(0, 0, 0.5f), camTransform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.tag);

            if (hit.transform.tag == "Player")
            {
                string uldentity = hit.transform.name;
                CmdTellServerWithWhoWasShot(uldentity, damage);
            }

            else if (hit.transform.tag == "Zombie")
            {
                string uidentity = hit.transform.name;
                CmdTellServerWhichZombWasShot(uidentity, damage);
            }
        }
    }

    [Command]
    void CmdTellServerWhichZombWasShot(string uniqueID, int dmg)
    {
        GameObject go = GameObject.Find(uniqueID);
        go.GetComponent<ZombHealth>().DeductHealth(dmg);
    }
    [Command]
    void CmdTellServerWithWhoWasShot(string uniqueID, int dmg)
    {
        GameObject go = GameObject.Find(uniqueID);
        //Apply damage to that player
        go.GetComponent<PlayerHeath>().DeductHealth(dmg);
    }


}
