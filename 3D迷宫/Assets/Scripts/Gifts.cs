using UnityEngine;
using System.Collections;

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
    private OwnerPlayer ownerpalyer;
    private gift gift_this;
    public GameObject[] giftprefabs;      //物品属性

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
            ownerpalyer = other.GetComponent<OwnerPlayer>();
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

        ownerpalyer.blood.bloodmax_before = ownerpalyer.max_hp;
        Debug.Log("Gift bold");
        Destroy(this.gameObject);
    }

    void AddAttack()
    {
        ownerpalyer.weapons_attack += attack;
        Debug.Log("Gift attac");
        Destroy(this.gameObject);
    }

    void AddAomor()
    {
        ownerpalyer.armor += armor;
        Debug.Log("Gift armor");
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
            Debug.Log("Gift way");
            Destroy(this.gameObject);
        }
    }
}
