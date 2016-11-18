using UnityEngine;
using System.Collections;

public class FollowPlayerSetMinimap : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        StartCoroutine(Followed());
    }

    IEnumerator Followed()
    {
        while (true)
        {
            if (player != null)
            {
                transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                player = GameObject.FindGameObjectWithTag(Tags.palyer);

            }
        }
    } 
}
