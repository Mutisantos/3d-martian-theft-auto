using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;
    public float SpawnInterval = 10.0f;
    public float MinSpawnTime = 5f;
    public float ReductionRate = 0.2f;

    private float currentDelay;

    private void Start()
    {
        currentDelay = SpawnInterval;
        SpawnPrefab();
    }


    private void Update()
    {
        currentDelay -= Time.deltaTime;
        if(currentDelay <= 0)
        {
            SpawnPrefab();
            SpawnInterval -= SpawnInterval * ReductionRate;
            if(SpawnInterval < MinSpawnTime)
            {
                SpawnInterval = MinSpawnTime;
            }
            currentDelay = SpawnInterval;
        }
    }

    private void SpawnPrefab()
    {
        Instantiate(PrefabToSpawn, transform.position, Quaternion.identity);
        SpawnInterval -= SpawnInterval * ReductionRate;
        if (SpawnInterval < 0.1f)
        {
            SpawnInterval = 0.1f;
        }
    }
}
