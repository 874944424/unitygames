using UnityEngine;
using System.Collections;

public class LaserDust : MonoBehaviour
{
    private float startTime;
    private float life = 0.5f;
    private float lifeVariation = 1.0f;
    private float endTime;
    private float length;
    private float scale;
    private float maxAlpha;

    private Camera _camera;

    void Start()
    {
        startTime = Time.time;
        life = life + lifeVariation * Random.value;
        endTime = Time.time + life;
        length = Random.Range(6, 8);
        scale = Random.Range(0.11f, 0.14f);
        Color laserColor = new Color(0, 0, 0);
        this.GetComponent<Renderer>().material.SetColor("_TintColor", laserColor);
        maxAlpha = Random.Range(0.1f, 0.3f);

        if ((_camera = Camera.main) == null)
        {
            _camera = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(gameObject);
        }
        float age = Time.time - startTime;
        float progress = age / life;
        float curveProgress = -4 * Mathf.Pow(progress, 2) + progress * 4;
        float parentAlpha = 1.0f;
        if (transform.parent != null)
        {
            parentAlpha = transform.parent.GetComponent<laserLine>().GetCurveProgress();
        }
        Color laserColor = new Color(curveProgress * maxAlpha * parentAlpha, 0, 0);
        this.GetComponent<Renderer>().material.SetColor("_TintColor", laserColor);
        transform.LookAt(_camera.transform.position);
        transform.localScale = Vector3.one * (scale + curveProgress * scale * 0.2f);
    }
}
