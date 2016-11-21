using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_ID : NetworkBehaviour {

    [SyncVar]
    private string playerUniqueIndentity;
    private NetworkInstanceId playerNetID;
    private Transform myTransform;

	public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
	}

    void Awake()
    {
        myTransform = transform;
    }

	void Update ()
    {
        if (myTransform.name == "" || myTransform.name == "NetPlayer(Clone)")
        {
            SetIdentity();
        }
	}

    [Client]
    void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            myTransform.name = playerUniqueIndentity;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueName = "Player" + playerNetID.ToString();
        return uniqueName;
    }

    [Command]
    void CmdTellServerMyIdentity(string name)
    {
        playerUniqueIndentity = name;
    }
}
