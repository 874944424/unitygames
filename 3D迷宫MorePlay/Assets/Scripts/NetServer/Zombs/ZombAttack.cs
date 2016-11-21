using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ZombAttack : NetworkBehaviour
{
    private float attackRate = 3;
    private float nextAttack;
    private int damage = 10;
    private float minDistance = 2;
    private float currentDistance;
    private Transform myTransform;
    private ZombTarget targetScript;

    [SerializeField]
    private Material zombieGreen;
    [SerializeField]
    private Material zombieRed;

    void Start()
    {
        myTransform = transform;
        targetScript = GetComponent<ZombTarget>();

        if (isServer)
        {
            StartCoroutine(Attack());
        }
    }

    void Update()
    {

    }

    void CheckIfTargetRange()
    {
        if (targetScript.targetTransform != null)
        {
            currentDistance = Vector3.Distance(targetScript.targetTransform.position, myTransform.position);
            if (currentDistance < minDistance && Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;

                targetScript.targetTransform.GetComponent<PlayerHeath>().DeductHealth(damage);
                StartCoroutine(ChangeZombieMat());    //For the host player
                RpcChangeZombieAppearance();
            }
        }
    }

    IEnumerator ChangeZombieMat()
    {
        GetComponentInChildren<Renderer>().material = zombieRed;
        yield return new WaitForSeconds(attackRate / 2);
        GetComponentInChildren<Renderer>().material = zombieGreen;
    }

    [ClientRpc]
    void RpcChangeZombieAppearance()
    {
        StartCoroutine(ChangeZombieMat());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckIfTargetRange();
        }
    }
}
