using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Player player;

    int maxSpawns = 30;
    public LayerMask groundMask;
    List<GameObject> enemiesSpawned = new List<GameObject>();
    Vector3 spawnPos = Vector3.zero;

    public int spawnCount = 4;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (enemiesSpawned.Count < maxSpawns)
            {
                
                for(int i = 0; i < Random.Range(1, spawnCount + 1); i++)
                {
                    spawnPos = GetSpawnPos();
                    spawnPos = GetGroundPos(spawnPos);
                    GameObject enemy = Instantiate(zombiePrefab);

                    enemy.transform.position = spawnPos;
                    enemiesSpawned.Add(enemy);
                }
                

                yield return new WaitForSeconds(10f);
            }
        }
    }
    Vector3 GetSpawnPos()
    {
        return new Vector3(player.transform.position.x + Random.Range(-30f, 30f), player.transform.position.y + 100f, player.transform.position.z + Random.Range(-30f, 30f));
    }
    Vector3 GetGroundPos(Vector3 pos)
    {
        RaycastHit hit;
        Physics.Raycast(pos, Vector3.down, out hit, 300f, groundMask);

        return hit.point + new Vector3(0f, 3f, 0f);
    }
}