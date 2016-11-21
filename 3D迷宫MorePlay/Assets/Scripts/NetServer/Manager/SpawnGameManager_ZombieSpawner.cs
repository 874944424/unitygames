using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager_ZombieSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject[] zombiePrefabs;
    private GameObject[] zombieSpawn;

    private int counter;
    private int maxNumberOfZombies = 50;
    private float waitRote = 10;
    private bool isSpawnActivoted = true; 

    public override void OnStartServer()
    {
        zombieSpawn = GameObject.FindGameObjectsWithTag("ZombieSpawn");

        StartCoroutine(ZombieSpawner());
    }

    IEnumerator ZombieSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitRote);
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            if (zombies.Length < maxNumberOfZombies)
            {
                CommentSpawn();
            }
        }
    }

    void CommentSpawn()
    {
        if (isSpawnActivoted)
        {
            for (int i = 0; i < maxNumberOfZombies; i++)
            {
                int randomIndex = Random.Range(0, zombieSpawn.Length);
                SpawnZombies(zombieSpawn[randomIndex].transform.position);
            }
        }
    }

    void SpawnZombies(Vector3 spawnPos)
    {
        counter++;
        int index = Random.Range(0, zombiePrefabs.Length - 1);
        GameObject go = GameObject.Instantiate(zombiePrefabs[index], spawnPos, Quaternion.identity) as GameObject;
        go.GetComponent<Zomb_ID>().zombieID = "Zombie" + counter;
        NetworkServer.Spawn(go);
    }
}
