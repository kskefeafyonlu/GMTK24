using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    private Wave _currentWave;
    public List<Wave> Waves;
    public float GraceTimeBetweenWaves = 5f;
    private float _timeUntilNextWave;

    private TextMeshProUGUI _headerText;

    private void Awake()
    {
        _headerText = GameObject.Find("HeaderText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _currentWave = Waves[0];
        StartCoroutine(HandleWaves());
    }

    private IEnumerator HandleWaves()
    {
        // Initial countdown before the first wave
        _timeUntilNextWave = GraceTimeBetweenWaves;
        while (_timeUntilNextWave > 0)
        {
            _headerText.text = $"First wave in {_timeUntilNextWave:0.0} seconds";
            yield return new WaitForSeconds(0.1f);
            _timeUntilNextWave -= 0.1f;
        }

        int waveIndex = 0;

        while (waveIndex < Waves.Count)
        {
            _currentWave = Waves[waveIndex];
            _headerText.text = ""; // Reset text when wave is actively spawning
            yield return StartCoroutine(SpawnEnemies());

            while (AreEnemiesRemaining())
            {
                _headerText.text = $"Enemies remaining: {GameObject.FindGameObjectsWithTag("Enemy").Length}";
                yield return new WaitForSeconds(1f);
            }

            _timeUntilNextWave = GraceTimeBetweenWaves;
            while (_timeUntilNextWave > 0)
            {
                _headerText.text = $"Next wave in {_timeUntilNextWave:0.0} seconds";
                yield return new WaitForSeconds(0.1f);
                _timeUntilNextWave -= 0.1f;
            }

            waveIndex++;
        }

        _headerText.text = "WAVES FINISHED";
    }

    private bool AreEnemiesRemaining()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Any();
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < _currentWave.enemyCount; i++)
        {
            GameObject randomEnemy =
                _currentWave.enemyPrefabs[UnityEngine.Random.Range(0, _currentWave.enemyPrefabs.Count)];
            Instantiate(randomEnemy, GetRandomSpawnPositionOutsideCameraBounds(), Quaternion.identity);
            yield return new WaitForSeconds(_currentWave.spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPositionOutsideCameraBounds()
    {
        Vector3 spawnPosition = Vector3.zero;
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        float spawnPadding = 1f;

        int side = UnityEngine.Random.Range(0, 4);

        switch (side)
        {
            case 0: //top
                spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-cameraWidth + spawnPadding, cameraWidth - spawnPadding),
                    cameraHeight + spawnPadding, 
                    0
                );
                break;
            case 1: //right
                spawnPosition = new Vector3(
                    cameraWidth + spawnPadding,
                    UnityEngine.Random.Range(-cameraHeight + spawnPadding, cameraHeight - spawnPadding), 
                    0
                );
                break;
            case 2: //bottom
                spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-cameraWidth + spawnPadding, cameraWidth - spawnPadding),
                    -cameraHeight - spawnPadding, 
                    0
                );
                break;
            case 3: //left
                spawnPosition = new Vector3(
                    -cameraWidth - spawnPadding,
                    UnityEngine.Random.Range(-cameraHeight + spawnPadding, cameraHeight - spawnPadding), 
                    0
                );
                break;
        }

        return transform.position + spawnPosition;
    }
}


[System.Serializable]
public class Wave
{
    public int enemyCount;
    public List<GameObject> enemyPrefabs;
    public float spawnInterval;

    public Wave(int enemyCount, List<GameObject> enemyPrefabs, float spawnInterval)
    {
        this.enemyCount = enemyCount;
        this.enemyPrefabs = enemyPrefabs;
        this.spawnInterval = spawnInterval;
    }
}