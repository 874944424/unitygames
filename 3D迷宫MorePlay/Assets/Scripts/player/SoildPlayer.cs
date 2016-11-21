using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class SoildPlayer : MonoBehaviour
{
    public BulletGenertor bullerGenertor;
    public FollowMouse_Look spine2;
    private Camera playercamera;
    private FirstPersonController firstpersoncontroller;
    private SoildAnimation playeranimation;
    private AudioListener audiolistener;

    public int hp = 100;               //角色血量
    public int max_hp = 100;           //角色最大血量
    public int damage = 10;              //玩家的攻击力
    public float armor = 4;                 //护甲值
    public float weapons_attack = 1;        //武器攻击值

    public Blood blood;                //血条控制
    public Text armor_text;             
    public Text attack_text;

    void Awake()
    {
        blood = GameObject.FindGameObjectWithTag(Tags.blood).GetComponent<Blood>();
        armor_text = GameObject.Find("Armor").GetComponent<Text>();
        attack_text = GameObject.Find("Attack").GetComponent<Text>();
        playercamera = GetComponentInChildren<Camera>();
        firstpersoncontroller = this.GetComponent<FirstPersonController>();
        playeranimation = this.GetComponent<SoildAnimation>();
        audiolistener = this.GetComponentInChildren<AudioListener>();
    }
    void Start()
    {
        blood.bloodmax_now = max_hp;
        blood.bloodnow = hp;
        armor_text.text = armor.ToString();
        attack_text.text = (damage + weapons_attack).ToString();
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
        // 公式：设防御力为D，攻击力为A
        //则实际伤害的效果为：A*1/(D/100 + 1)
        this.hp -= (int)(damage / (armor/100 + 1));
        blood.bloodnow = this.hp;
        if (hp <= 0)
        {
            TakeDeath();
        }
        //TODO加上屏幕晃动变红效果
    }

    public void SysncHp(int hp)
    {
        this.hp = hp;
    }
    //死亡设计
    public void TakeDeath()
    {
        transform.tag = Tags.dead;
        firstpersoncontroller.enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
        gameObject.AddComponent<BoxCollider>().size = new Vector3(0.3f, 0.3f, 0.3f);
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }
}
