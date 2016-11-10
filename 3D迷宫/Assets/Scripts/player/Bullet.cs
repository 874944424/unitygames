using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 2000;
    public GameObject[] bulletHoles;        //子弹打中墙或地面贴图
    private GameObject bulletHole_parent;   //贴图父物体方便管理
    public float damage = 10f;

    void Start()
    {
        bulletHole_parent = GameObject.Find("bulletHole_parent");
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
            //如果射线检测到的什么物体
            OnHitCollider(hitinfo);
        }
    }

    void OnDestory()
    {
        Destroy(this.gameObject, 1f);
    }

    void OnHitCollider(RaycastHit hit)
    {
        if (!hit.collider.CompareTag(Tags.ground) && !hit.collider.CompareTag(Tags.wall))
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
        go.transform.parent = bulletHole_parent.transform;

        Destroy(this.gameObject);
    }
}
