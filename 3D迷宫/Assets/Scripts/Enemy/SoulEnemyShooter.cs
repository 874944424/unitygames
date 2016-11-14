using UnityEngine;
using System.Collections;

public class SoulEnemyShooter : SoulEnemy
{
    public GameObject enemy_bulletpre;
    public GameObject origin_gameobject;     //子弹生成点  
    public GameObject bullet_parent;

    public GameObject[] OnhurtChangeMaterials;

    public override void Start()
    {
        base.Start();
        bullet_parent = GameObject.Find("bulletHole_parent");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void DamageCaculateshooter()
    {
        //发射子弹
        //子弹检测打中则受到伤害
        GameObject enemy_bullet = Instantiate(enemy_bulletpre, origin_gameobject.transform.position, Quaternion.identity) as GameObject;
        enemy_bullet.transform.parent = bullet_parent.transform;
        Vector3 attack_pos = new Vector3(player.transform.position.x, enemy_bullet.transform.position.y, player.transform.position.z);
        enemy_bullet.transform.LookAt(attack_pos);
    }

    public override void OnHurt(int damage)
    {
        //计算后传入实际伤害 （减去护甲的值）
        base.OnHurt(damage);
        for (int i = 0; i < OnhurtChangeMaterials.Length; i++)
        {
            OnhurtChangeMaterials[i].GetComponent<Renderer>().material = on_hurtmaterial;
        }
        Invoke("ComeBackOriginMaterial", 0.2f);
    }

    public override void ComeBackOriginMaterial()
    {
        for (int i = 0; i < OnhurtChangeMaterials.Length; i++)
        {
            OnhurtChangeMaterials[i].GetComponent<Renderer>().material = origin_material_enemy;
        }
        base.ComeBackOriginMaterial();
    }
}
