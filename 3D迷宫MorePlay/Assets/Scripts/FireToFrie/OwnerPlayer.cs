using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class OwnerPlayer : MonoBehaviour
{

    public NetworkPlayer ownerPlayer; //表示这个soldier是由谁创建的

    public CSBulletGenertor bullerGenertor;
    public FollowMouseLook spine2;
    private Camera camera;
    private FirstPersonController firstpersoncontroller;
    private PlayerAnimation playeranimation;
    private AudioListener audiolistener;
    private NetworkView networkview;

    public float hp = 100; //角色血量

    void Awake()
    {

        camera = GetComponentInChildren<Camera>();
        firstpersoncontroller = this.GetComponent<FirstPersonController>();
        playeranimation = this.GetComponent<PlayerAnimation>();
        audiolistener = this.GetComponentInChildren<AudioListener>();
        networkview = this.GetComponent<NetworkView>();
    }

    void Update()
    {
        if (hp <= 0)
        {
            bool isWin = false;
            if (ownerPlayer != Network.player)
            {
                isWin = true;
            }
            GameController._instance.ShowGameOver(isWin);
        }

    }

    //当这个角色不是在创建者的客户端运行的时候，要取消相应控制
    public void LoseControl()
    {
        camera.gameObject.SetActive(false);     //禁止视角移动
        firstpersoncontroller.enabled = false;  //禁止移动控制
        playeranimation.enabled = false;        //禁止动画控制
        bullerGenertor.enabled = false;         //禁用发射子弹
        spine2.enabled = false;
        audiolistener.enabled = false;
    }

    [RPC]   //使用这个注释表示这个方法是一个远程调用的方法
    public void SetOwnerPlayer(NetworkPlayer player)
    {
        this.ownerPlayer = player;

        if (Network.player != player)
        {
            LoseControl();
        }
    }

    public void TakeDamage(float damage)
    {
        this.hp -= damage;
        networkview.RPC("SysncHp", RPCMode.AllBuffered, this.hp);
    }

    [RPC]
    public void SysncHp(float hp)
    {
        this.hp = hp;
    }

}
