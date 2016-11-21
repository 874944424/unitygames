using UnityEngine;
using System.Collections;

public class BulletHole : MonoBehaviour
{
    public float speed = 0.5f;
    public float timer = 0;

    private MeshRenderer renderer_this;


    void Awake()
    {
        renderer_this = this.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            StartCoroutine(AutoToDisappear());
        }
        if (timer >= 5f)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator AutoToDisappear()
    {
        renderer_this.material.color = Color.Lerp(renderer_this.material.color, Color.clear, Time.deltaTime * speed);
        yield return null;
    }
}
