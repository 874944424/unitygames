using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum gift
{
    bold = 0,
    attack = 1,
    armor = 2,
    way = 3,
};

public class Gifts : MonoBehaviour
{
    /* 奖励物品类型 随机
     * 1、血量属性上限+5并加满血量
     * 2、攻击力加+10
     * 3、防御力加+20
     * 4、发现自动寻路
     */
    private int addbold = 10;
    private int attack = 10;
    private int armor = 20;

    private Gamemanager gamemanager;
    private SoildPlayer ownerpalyer;
    private gift gift_this;
    public GameObject[] effects;          //特效

    public Text gift_text;                //捡到道具在屏幕中心显示相关信息
    private float giftShowtime = 2.5f;

    private string armor_text = "防御力增加了！！！";
    private string attack_text = "攻击力增加了！！！";
    private string hp_text = "血量上限增加并回复!";
    private string way_text = "启动秘宝自动寻路（可通过 '~' 打开关闭线路）";

    void Start()
    {
        gamemanager = GameObject.Find("Gamemanager").GetComponent<Gamemanager>();
        RandGifts();
    }

    //随机选着奖励属性
    void RandGifts()
    {
        gift_this = (gift)Random.Range(0, 4);
    }

    //tirrger对象为player激活
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.palyer)
        {
            ownerpalyer = other.GetComponent<SoildPlayer>();
            GetGift();
        }
    }

    //传递触发对象根据不同属性发生不同反应
    void GetGift()
    {
        switch (gift_this)
        {
            case gift.bold:
                AddBold();
                break;
            case gift.attack:
                AddAttack();
                break;
            case gift.armor:
                AddAomor();
                break;
            case gift.way:
                FindWay();
                break;
        }
    }

    void AddBold()
    {
        ownerpalyer.max_hp += addbold;
        ownerpalyer.hp = ownerpalyer.max_hp;

        ownerpalyer.blood.bloodmax_now = ownerpalyer.max_hp;
        Instantiate(effects[2], transform.position, Quaternion.identity);
        SelfHideGiftText(hp_text);

        //Debug.Log("Gift bold");
        Destroy(this.gameObject);
    }

    void AddAttack()
    {
        ownerpalyer.weapons_attack += attack;
        ownerpalyer.attack_text.text = (ownerpalyer.weapons_attack + 10).ToString();
        Instantiate(effects[1], transform.position, Quaternion.identity);
        SelfHideGiftText(attack_text);

        //Debug.Log("Gift attack");
        Destroy(this.gameObject);
    }

    void AddAomor()
    {
        ownerpalyer.armor += armor;
        ownerpalyer.armor_text.text = (ownerpalyer.armor + 10).ToString();
        Instantiate(effects[0], transform.position, Quaternion.identity);
        SelfHideGiftText(armor_text);

        //Debug.Log("Gift armor");
        Destroy(this.gameObject);
    }

    void FindWay()
    {
        if (gamemanager._instance.isCanfindway == true)
        {
            gift_this = (gift)Random.Range(0, 3);
            GetGift();
        }
        else
        {
            gamemanager._instance.isCanfindway = true;
            SelfHideGiftText(way_text);

            //Debug.Log("Gift way");
            Destroy(this.gameObject);
        }
    }

    void SelfHideGiftText(string showtext)
    {
        gift_text = Instantiate(gift_text);
        gift_text.transform.position = new Vector3(512, 384, 0);
        gift_text.transform.parent = GameObject.Find("Canvas").transform;
        gift_text.text = showtext;
        Destroy(gift_text.gameObject, giftShowtime);
    }
}
