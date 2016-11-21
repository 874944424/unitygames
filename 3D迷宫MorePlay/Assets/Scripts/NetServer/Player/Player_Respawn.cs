using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player_Respawn : NetworkBehaviour
{
    private Image crossHairImage;
    private PlayerHeath healthScript;
    private GameObject repawnButton;

    public override void PreStartClient()
    {
        healthScript = GetComponent<PlayerHeath>();
        healthScript.EventRespawn += EnablePlayer;
    }

    public override void OnStartLocalPlayer()
    {
        crossHairImage = GameObject.Find("CrossHarImage").GetComponent<Image>();
        SetRespawnButton();
    }

    public override void OnNetworkDestroy()
    {
        healthScript.EventRespawn -= EnablePlayer;
        base.OnNetworkDestroy();
    }

    void SetRespawnButton()
    {
        if (isLocalPlayer)
        {
            repawnButton = GameObject.Find("Respawn_Button");
            repawnButton.GetComponent<Button>().onClick.AddListener(CommandRespawn);
            repawnButton.SetActive(false);
        }
    }

    void EnablePlayer()
    {
        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerShoot>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Player_Death>().enabled = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in renderers)
        {
            ren.enabled = true;
        }

        if (isLocalPlayer)
        {
            crossHairImage.enabled = true;
            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;

        }
    }

    void CommandRespawn()
    {
        CmdRespawnServer();
        repawnButton.SetActive(false);
    }

    [Command]
    void CmdRespawnServer()
    {
        healthScript.ResetHealth();
    }
}
