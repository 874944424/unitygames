using UnityEngine;
using System.Collections;

public class MoveSelf : MonoBehaviour
{
    public Vector3 pos_moveto;
    void Start()
    {
        pos_moveto = new Vector3(0, 300) + transform.position;
    }
    void FixedUpdate()
    {
        StartCoroutine(MoveUp());
    }

    IEnumerator MoveUp()
    {
        Vector3 pos =Vector3.Lerp(transform.position, pos_moveto, Time.deltaTime);
        transform.position = pos;
        yield return new WaitForSeconds(1f);
    }
}
