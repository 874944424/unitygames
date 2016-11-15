using UnityEngine;
using System.Collections;

public class ZombEnemy : Enemy
{
    public int zomb_attack = 1;                     //怪物的伤害
    public float zomb_attack_distance = 3f;
    public int zomb_hp = 500;
    public float zomb_speed = 1f;

    public float attack_time = 1f;                //攻击间隔时间
    public float attacktimer = 0f;                  //计时器

    public override void Start()
    {
        attack_distance = zomb_attack_distance;
        attack = zomb_attack;
        speed = zomb_speed;
        attacktimer = attack_time;
        hp = zomb_hp;
        base.Start();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //攻击计时冷却时间
        if (IsCanAttack && attacktimer <= 0f)
        {
            MakeDamegeAttack();
            attacktimer = attack_time;
        }
        else
        {
            attacktimer -= Time.deltaTime;
            IsCanWalk = true;
        }
    }

    protected void MakeDamegeAttack()
    {      
        if(player != null && Vector3.Distance(player.transform.position, transform.position) <= attack_distance)
            player.GetComponent<PlayerAnimation>().PlayerOnHurt(attack);
    }

    public override void OnHurt(int damage)
    {
        //计算后传入实际伤害 （减去护甲的值）
        base.OnHurt(damage);
    }

    public virtual void OnDeading()
    {
        Destroy(this, 1f);
        this.GetComponent<CapsuleCollider>().enabled = false;
    }
}
