using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class OwnerPlayer : MonoBehaviour
{
    public CSBulletGenertor bullerGenertor;
    public FollowMouseLook spine2;
    private Camera playercamera;
    private FirstPersonController firstpersoncontroller;
    private PlayerAnimation playeranimation;
    private AudioListener audiolistener;
    private NetworkView networkview;

    public float hp = 100; //角色血量

    void Awake()
    {

        playercamera = GetComponentInChildren<Camera>();
        firstpersoncontroller = this.GetComponent<FirstPersonController>();
        playeranimation = this.GetComponent<PlayerAnimation>();
        audiolistener = this.GetComponentInChildren<AudioListener>();
        networkview = this.GetComponent<NetworkView>();
    }

    void Update()
    {

    }

    //当这个角色不是在创建者的客户端运行的时候，要取消相应控制
    public void LoseControl()
    {
        playercamera.gameObject.SetActive(false);     //禁止视角移动
        firstpersoncontroller.enabled = false;  //禁止移动控制
        playeranimation.enabled = false;        //禁止动画控制
        bullerGenertor.enabled = false;         //禁用发射子弹
        spine2.enabled = false;
        audiolistener.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        this.hp -= damage;
    }

    public void SysncHp(float hp)
    {
        this.hp = hp;
    }

}
