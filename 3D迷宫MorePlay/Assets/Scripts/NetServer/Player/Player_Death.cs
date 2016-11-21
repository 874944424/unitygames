using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player_Death : NetworkBehaviour
{
    private PlayerHeath healthScript;
    private Image crossHarImage;

    public override void PreStartClient()
    {
        healthScript = GetComponent<PlayerHeath>();
        healthScript.EventDie += DisablePlayer;
    }

    public override void OnStartLocalPlayer()
    {
        crossHarImage = GameObject.Find("CrossHarImage").GetComponent<Image>();
    }

    public override void OnNetworkDestroy()
    {
        healthScript.EventDie -= DisablePlayer;
        base.OnNetworkDestroy();
    }


    void DisablePlayer()
    {
        UnityEngine.Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerShoot>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in renderers)
        {
            ren.enabled = false;
        }

        healthScript.isDead = true;

        if (isLocalPlayer)
        {
            crossHarImage.enabled = false;
            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            GameObject.Find("GameManager").GetComponent<GameManager_References>().repawnButton.SetActive(true);           
        }
    }
}
