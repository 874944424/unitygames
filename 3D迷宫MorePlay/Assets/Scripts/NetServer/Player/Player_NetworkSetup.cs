using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

    //序列化场
    [SerializeField] Camera FPSCharacter_camera;
    [SerializeField] AudioListener audiolistener;

    public NetworkAnimator net_anim;

    public override void OnStartLocalPlayer()
    {
        GameObject.Find("SceneCamera").SetActive(false);
        //GetComponent<CharacterController>().enabled = true;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        FPSCharacter_camera.enabled = true;
        audiolistener.enabled = true;

        Renderer[] rens = GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in rens)
        {
            ren.enabled = false;
        }
        net_anim.SetParameterAutoSend(0, true);
    }

    public override void PreStartClient()
    {
        net_anim.SetParameterAutoSend(0, true);
    }
}
