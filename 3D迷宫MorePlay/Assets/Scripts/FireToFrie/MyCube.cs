using UnityEngine;
using System.Collections;

public class MyCube : MonoBehaviour {

    public float speed = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h * speed * Time.deltaTime, 0, v * speed * Time.deltaTime));
	}
}
