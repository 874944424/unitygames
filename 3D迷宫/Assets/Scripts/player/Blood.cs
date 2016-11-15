using UnityEngine;
using System.Collections;

public class Blood : MonoBehaviour
{
    [HideInInspector]
    public Blood _instance;

    public float blood_speed = 5.0f;
    public float bloodshadow_speed = 3.0f;

    public float bloodnow;
    public float bloodbefore;

    public float bloodmax_before;
    public float bloodmax_now;

    private GameObject blood;           //血条
    private GameObject shadow;          //血条阴影

    void Awake()
    {
        blood = transform.GetChild(1).gameObject;
        shadow = transform.GetChild(0).gameObject;
        _instance = this;
    }

    void Start()
    {
        bloodbefore = bloodmax_now;
        bloodmax_before = bloodmax_now;
    }

    void FixedUpdate()
    {
        if (bloodmax_before != bloodmax_now)
        {
            bloodmax_before = bloodmax_now;
            bloodnow = bloodmax_now;
        }
        StartCoroutine(Changeblood(bloodmax_before / bloodmax_now, blood_speed, this.gameObject));
        if (bloodbefore != bloodnow)
        {
            bloodbefore = bloodnow;
        }
        StartCoroutine(Changeblood(bloodnow / bloodmax_now, blood_speed, blood));
        StartCoroutine(Changeblood(bloodnow / bloodmax_now, bloodshadow_speed, shadow));
    }

    IEnumerator Changeblood(float px, float speed, GameObject obj)
    {
        //实现血条缩减
        if (px < 0)
            px = 0;
        if (obj.transform.localScale.x >= px + 0.005 || obj.transform.localScale.x <= px - 0.005)
        {
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, new Vector3(px, obj.transform.localScale.y, obj.transform.localScale.z), Time.deltaTime * speed);
        }
        yield return null;
    }
}
