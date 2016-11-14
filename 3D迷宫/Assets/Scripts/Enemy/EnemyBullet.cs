using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
    public float life = 0.5f;              //子弹生存时间
    public float bulletSpeed = 1.0f;       //子弹速度
    public GameObject bulletHolePerfab;    //子弹模型
    private float destroyTime;             //死亡时间
      

    public float attack = 1f;              //子弹的伤害
    public SoulEnemyShooter shooter;

    void Start()
    {
        destroyTime = Time.time + life;

    }

    void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }

        Vector3 oriPos = transform.position;
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        Vector3 direction = transform.position - oriPos;
        float length = (transform.position - oriPos).magnitude;     //向量的大小

        RaycastHit hit;
        if (Physics.Raycast(oriPos, direction, out hit, length))
        {
            //如果射线检测到的什么物体
            OnHitCollider(hit);
        }
    }

    void OnHitCollider(RaycastHit hit)
    {
        Debug.Log(hit.transform.name);

        if (hit.collider.CompareTag(Tags.ground) || hit.collider.CompareTag(Tags.wall))
        {
            Vector3 pos = hit.point;

            GameObject go = GameObject.Instantiate(bulletHolePerfab, hit.point, Quaternion.identity) as GameObject;
            go.transform.LookAt(hit.point - hit.normal);
            go.transform.Translate(Vector3.back * 0.01f);
        }
        else if (hit.collider.CompareTag(Tags.palyer))
        {
            //TODO damage
            hit.transform.GetComponent<PlayerAnimation>().PlayerOnHurt(shooter.soul_attack);
        }

        Destroy(this.gameObject);
    }
}
