using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Animation : NetworkBehaviour
{
    private float animSpeed;
    public Animator anim;

    void Start()
    {
     
    }

	void FixedUpdate ()
    {
        if (isLocalPlayer)
        {
            float vertical = Input.GetAxis("Vertical");
            animSpeed = Mathf.Abs(vertical);
            anim.SetFloat("Speed", animSpeed);
        }
    }
}
