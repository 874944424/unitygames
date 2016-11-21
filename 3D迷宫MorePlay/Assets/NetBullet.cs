using UnityEngine;
using System.Collections;

public class NetBullet : MonoBehaviour
{
    public float speed = 2000;

    void Start()
    {
        OnDestory();
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnDestory()
    {
        Destroy(this.gameObject, 1f);
    }

}
