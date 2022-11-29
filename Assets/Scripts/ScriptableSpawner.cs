using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableSpawner", order = 1)]
public class ScriptableSpawner : ScriptableObject
{
    [System.Serializable]
    public struct Spawning
    {
        public GameObject EnemyType;
        public List<float> positions;
        public float waitTime;
    }
    
    public float addToWaitTimer;
    [SerializeField] List<Spawning> Spawlings;

    
    public virtual void StartSpawn()
    {
        
    }
    
    public virtual float GetTime()
    {
        float time = 0;
        for (int i = 0; i < Spawlings.Count; i++)
        {
            time += Spawlings[i].waitTime;
        }

        return time;
    }
    
    public virtual IEnumerator Spawn()
    {
        for (int i = 0; i < Spawlings.Count; i++)
        {
            for (int j = 0; j < Spawlings[i].positions.Count; j++)
            {
                Instantiate(Spawlings[i].EnemyType, new Vector3(Spawlings[i].positions[j], 25, 22), Quaternion.identity);
            }
            yield return new WaitForSeconds(Spawlings[i].waitTime);
        }
        yield break;
    }
}


