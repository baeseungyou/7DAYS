using UnityEngine;
using System.Linq;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int zombiesPerNight = 5;
    public Transform[] spawnPoints;
    private float spawnInterval;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null. 좀비 생성 생략됨.");
            return;
        }

        spawnInterval = Mathf.Max(30f, 60f - (GameManager.Instance.nowDay - 1) * 5f);

        int totalZombies = zombiesPerNight;
        SpawnAllZombies(totalZombies);

        StartCoroutine(SpawnZombiesOverTime());
    }

    IEnumerator SpawnZombiesOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnAllZombies(zombiesPerNight);
        }
    }

    void SpawnAllZombies(int count)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("스폰 위치가 없습니다!");
            return;
        }

        Transform[] shuffledPoints = spawnPoints.OrderBy(x => Random.value).ToArray();
        int uniqueCount = Mathf.Min(count, shuffledPoints.Length);

        for (int i = 0; i < uniqueCount; i++)
        {
            Instantiate(zombiePrefab, shuffledPoints[i].position, Quaternion.identity);
        }

        for (int i = uniqueCount; i < count; i++)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(zombiePrefab, randomPoint.position, Quaternion.identity);
        }
    }
}
