using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> prefabs;
    public float spawnInterval = 1f; // Time between spawns in seconds
    public float minDistance = 3f; // Minimum distance between spawned objects
    public int maxAttempts = 10; // Maximum attempts to find a valid position

    private void Start()
    {
        StartCoroutine(SpawnPrefabsIndefinitely());
    }

    // Coroutine to spawn prefabs indefinitely at random positions outside camera bounds
    private IEnumerator SpawnPrefabsIndefinitely()
    {
        while (true)
        {
            foreach (var prefab in prefabs)
            {
                Vector3 randomPosition = Vector3.zero;
                bool positionFound = false;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    randomPosition = GetRandomSpawnPositionOutsideCameraBounds();

                    if (!IsPositionOccupied(randomPosition))
                    {
                        positionFound = true;
                        break;
                    }
                }

                if (positionFound)
                {
                    Instantiate(prefab, randomPosition, Quaternion.identity);
                }

                yield return new WaitForSeconds(spawnInterval); // Delay between spawns
            }
        }
    }

    // Method to get a random spawn position outside camera bounds
    private Vector3 GetRandomSpawnPositionOutsideCameraBounds()
    {
        Vector3 spawnPosition = Vector3.zero;
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        float spawnPadding = 1f;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // top
                spawnPosition = new Vector3(
                    Random.Range(-cameraWidth + spawnPadding, cameraWidth - spawnPadding),
                    cameraHeight + spawnPadding, 
                    0
                );
                break;
            case 1: // right
                spawnPosition = new Vector3(
                    cameraWidth + spawnPadding,
                    Random.Range(-cameraHeight + spawnPadding, cameraHeight - spawnPadding), 
                    0
                );
                break;
            case 2: // bottom
                spawnPosition = new Vector3(
                    Random.Range(-cameraWidth + spawnPadding, cameraWidth - spawnPadding),
                    -cameraHeight - spawnPadding, 
                    0
                );
                break;
            case 3: // left
                spawnPosition = new Vector3(
                    -cameraWidth - spawnPadding,
                    Random.Range(-cameraHeight + spawnPadding, cameraHeight - spawnPadding), 
                    0
                );
                break;
        }

        Vector3 offset = transform.position;
        offset.z = 0;
        return offset + spawnPosition;
    }

    // Method to check if a position is too close to existing objects
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, minDistance);
        return colliders.Length > 0;
    }
}