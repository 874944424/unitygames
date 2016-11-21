using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Zomb_ID : NetworkBehaviour
{
    [SyncVar]
    public string zombieID;
    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        SetIdentity();
    }

    void SetIdentity()
    {
        if (myTransform.name == "" || myTransform.name == "Zomb(Clone)" || myTransform.name == "Zombunny(Clone)" || myTransform.name == "Zombear(Clone)")
        {
            myTransform.name = zombieID;   
        }
    }
}
