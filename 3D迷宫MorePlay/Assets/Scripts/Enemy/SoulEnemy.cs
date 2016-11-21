using UnityEngine;
using System.Collections;

public class SoulEnemy : Enemy
{
    public int soul_attack = 1;                     //怪物的伤害
    public float soul_attack_distance = 2f;
    public int soul_hp = 100;
    public float soul_speed = 1f;

    public string attack_name = "BossAttack01";     //攻击动画状态名称判断是否可以移动
    public float attack_time = 0.2f;                //攻击间隔时间
    public float attacktimer = 0f;                  //计时器

    public override void Start()
    {
        attack_distance = soul_attack_distance;
        attack = soul_attack;
        speed = soul_speed;
        hp = soul_hp;
        base.Start();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //攻击计时冷却时间
        if (IsCanAttack && attacktimer >= 0f)
        {
            MakeDamage();
            attacktimer -= Time.deltaTime;
        }
        else
        {
            attacktimer = attack_time;
        }
        //判断不为攻击状态着可以移动
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(attack_name) && !anim.GetBool(AnimValueNames.attack))
        {
            IsCanWalk = true;
        }
        else
        {
            IsCanWalk = false;
        }
    }

    //Soul攻击时进行攻击动画的播放并在动画过程中造成伤害
    protected override void MakeDamage()
    {
        base.MakeDamage();
        anim.SetTrigger(AnimValueNames.attack);     
    }
    //伤害计算EVENT
    public void DamageCaculate()
    {
        if(player != null && Vector3.Distance(player.transform.position, transform.position) <= attack_distance*1.5f)
            player.GetComponent<SoildAnimation>().PlayerOnHurt(attack);
    }
    //停止攻击动画播放EVENT
    public virtual void stopAttack()
    {
        anim.SetBool(AnimValueNames.attack, false); 
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
