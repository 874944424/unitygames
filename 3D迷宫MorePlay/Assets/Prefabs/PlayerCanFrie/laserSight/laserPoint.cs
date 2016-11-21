using UnityEngine;
using System.Collections;

public class laserPoint : MonoBehaviour
{
    public bool on;
    private Camera _camera;

    void Start()
    {
        if ((_camera = Camera.main) == null)
        {
            _camera = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>();
        }
    }

	void Update ()
    {
        if (on)
        {
            transform.LookAt(_camera.transform.position);
            transform.localScale = Vector3.one * Random.Range(0.3f, 0.5f);
            Color laserColor = Color.red;
            laserColor.a = Random.Range(0.2f, 1.0f);
            GetComponent<Renderer>().material.SetColor("_TintColor", laserColor);
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
