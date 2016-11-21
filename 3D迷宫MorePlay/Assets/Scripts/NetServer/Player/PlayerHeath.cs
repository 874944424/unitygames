using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHeath : NetworkBehaviour
{
    [SyncVar (hook = "OnhealthChanged")]
    private int health = 100;
    private Text health_text;
    private bool shootDie = false;
    public bool isDead = false;

    public delegate void DieDelegate();
    public event DieDelegate EventDie;

    public delegate void RespawnDelegate();
    public event RespawnDelegate EventRespawn;

    public override void OnStartLocalPlayer()
    {
        health_text = GameObject.Find("Health_Text").GetComponent<Text>();
        SetHealthText();
    }

    void Update()
    {
        CheckComdition();
    }

    void CheckComdition()
    {
        if (health <= 0 && !shootDie && !isDead)
        {
            shootDie = true;
        }
        if (health <= 0 && shootDie)
        {
            if (EventDie != null)
            {
                EventDie();
            }

            shootDie = false;
        }

        if (health > 0 && isDead)
        {
            if (EventRespawn != null)
            {
                EventRespawn();
            }
            isDead = false;
        }
    }

    void SetHealthText()
    {
        if (isLocalPlayer)
        {
            health_text.text = health.ToString();
        }
    }

    public void DeductHealth(int dmg)
    {
        health -= dmg;
    }

    void OnhealthChanged(int hlth)
    {
        health = hlth;
        SetHealthText();
    }

    public void ResetHealth()
    {
        health = 100;
    }
}
