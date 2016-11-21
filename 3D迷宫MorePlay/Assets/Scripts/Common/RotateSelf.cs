using UnityEngine;
using System.Collections;

public class RotateSelf : MonoBehaviour
{

	void Start () {
        
    }
	
	void Update () {
        transform.Rotate(-60 * Time.deltaTime, -30 * Time.deltaTime, -70 * Time.deltaTime, Space.Self);
    }
}
