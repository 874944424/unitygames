using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 800;
    public GameObject[] bulletHoles;
    public float damage = 10f;

    void Start()
    {
        OnDestory();
    }

    void Update()
    {
        Vector3 oriPos = transform.position;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Vector3 direction = transform.position - oriPos;
        float length = (transform.position - oriPos).magnitude;     //向量的大小

        RaycastHit hitinfo;
        bool isCollider = Physics.Raycast(oriPos, direction, out hitinfo,length);
        if (isCollider)
        {
            //如果射线检测到的什么物体，应该做什么动作TODO
            OnHitCollider(hitinfo);
        }
    }

    void OnDestory()
    {
        Destroy(this.gameObject, 2f);
    }

    void OnHitCollider(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Player"))
        {
            OwnerPlayer player = hit.collider.GetComponent<OwnerPlayer>();
            if (player.hp > 0)
            {
                player.TakeDamage(this.damage);
            }
            //hit.collider.GetComponent<NetworkView>().RPC("TakeDamage", RPCMode.AllBuffered, damage);
        }
        if (!hit.collider.CompareTag("ground"))
        {
            return;
        }

        int index = Random.Range(0, 2);
        GameObject bulletHolePrefab = bulletHoles[index];

        Vector3 pos = hit.point;
        GameObject go = GameObject.Instantiate(bulletHolePrefab, hit.point, Quaternion.identity) as GameObject;
        //hit.normal; //可以得到碰撞点的垂线向量, 面的法线
        go.transform.LookAt(hit.point - hit.normal);
        go.transform.Translate(Vector3.back * 0.01f);

        Destroy(this.gameObject);
    }
}
