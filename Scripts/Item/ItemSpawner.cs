using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        public ItemData itemData;
    }

    public List<SpawnableItem> spawnableItems = new List<SpawnableItem>();
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnInterval = 10f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        if (spawnableItems.Count == 0 || spawnPoints.Count == 0) return;

        List<float> weights = new List<float>();
        float totalWeight = 0f;

        foreach (var item in spawnableItems)
        {
            float weight = 1f / Mathf.Max(item.itemData.healAmount, 1f);
            weights.Add(weight);
            totalWeight += weight;
        }

        float rand = Random.Range(0, totalWeight);
        float sum = 0f;
        int chosenIndex = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            sum += weights[i];
            if (rand <= sum)
            {
                chosenIndex = i;
                break;
            }
        }

        List<Transform> availablePoints = new List<Transform>();
        foreach (var point in spawnPoints)
        {
            Collider[] overlaps = Physics.OverlapSphere(point.position, 0.5f); // 범위 내 체크
            bool hasItem = false;

            foreach (var hit in overlaps)
            {
                if (hit.GetComponent<Item>() != null)
                {
                    hasItem = true;
                    break;
                }
            }

            if (!hasItem) availablePoints.Add(point);
        }

        if (availablePoints.Count > 0)
        {
            Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            Instantiate(spawnableItems[chosenIndex].prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
