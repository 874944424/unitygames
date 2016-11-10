using UnityEngine;
using System.Collections;

public class CSFlash : MonoBehaviour
{
    public Material[] material_flash;
    private int index = 0;
    private MeshRenderer renderer_this;
    private float flashTime = 0.05f;    //闪光的时间
    private float flashTimer = 0;

    void Awake()
    {
        renderer_this = this.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        renderer_this.enabled = false;
    }

    void Update()
    {
        if (renderer_this.enabled)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer >= flashTime)
            {
                renderer_this.enabled = false;
            }
        }
    }

    public void Flash()
    {
        index++;
        index %= 4;
        renderer_this.enabled = true;
        renderer_this.material = material_flash[index];
        flashTimer = 0;
    }
}
