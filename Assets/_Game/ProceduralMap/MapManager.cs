using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public List<GameObject> SpawnableObjectPrefabs;
    private List<GameObject> _spawnedObjects = new List<GameObject>();
    
    public float SpawnInterval = 1f;
    private float _spawnTimer = 0f;
    
    private Vector3 _spawnPositionOffset = new Vector3(0, 0, 0);


    private void Awake()
    {
        _spawnTimer = SpawnInterval;
        //make the objects spawn out of camera view
        _spawnPositionOffset = new Vector3(10, 5, 0);
    }


    private void FixedUpdate()
    {
        TimedSpawnObjects();
    }

    private void TimedSpawnObjects()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            _spawnTimer = SpawnInterval;
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
         int randomIndex = Random.Range(0, SpawnableObjectPrefabs.Count);
         GameObject randomPrefab = SpawnableObjectPrefabs[randomIndex];
         
         Instantiate(randomPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        Camera mainCamera = Camera.main;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;

        float leftEdge = cameraPosition.x - cameraWidth / 2;
        float rightEdge = cameraPosition.x + cameraWidth / 2;
        float topEdge = cameraPosition.y + cameraHeight / 2;
        float bottomEdge = cameraPosition.y - cameraHeight / 2;

        float xOffset = 0;
        float yOffset = 0;

        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: // Left edge
                xOffset = leftEdge - 1;
                yOffset = Random.Range(bottomEdge, topEdge);
                break;
            case 1: // Right edge
                xOffset = rightEdge + 1;
                yOffset = Random.Range(bottomEdge, topEdge);
                break;
            case 2: // Top edge
                xOffset = Random.Range(leftEdge, rightEdge);
                yOffset = topEdge + 1;
                break;
            case 3: // Bottom edge
                xOffset = Random.Range(leftEdge, rightEdge);
                yOffset = bottomEdge - 1;
                break;
        }

        return new Vector3(xOffset, yOffset, 0);
    }
    


}
