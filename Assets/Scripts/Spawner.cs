using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<ScriptableSpawner> spawnCue;
    
    private int _curSpawnCue = 0;
    public float spawnWaitTimer = 0;

    void Start()
    {
        spawnWaitTimer = spawnCue[0].addToWaitTimer;
    }

    void Update()
    {
        Debug.Log("SpawnwaitTimer: " + spawnWaitTimer);
        
        if (spawnWaitTimer > 0)
        {
            spawnWaitTimer -= Time.deltaTime;
            return;
        }

        if (_curSpawnCue < spawnCue.Count)
        {
            NextSpawnCue();
        }
    }

    public void NextSpawnCue()
    {
        spawnCue[_curSpawnCue].StartSpawn();
        StartCoroutine(spawnCue[_curSpawnCue].Spawn());
        _curSpawnCue++;
        if (_curSpawnCue < spawnCue.Count)
        {
            spawnWaitTimer = 0;
            spawnWaitTimer += spawnCue[_curSpawnCue].GetTime();
            spawnWaitTimer += spawnCue[_curSpawnCue].addToWaitTimer;
        }
    }
}


