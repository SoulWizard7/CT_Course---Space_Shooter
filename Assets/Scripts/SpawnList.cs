using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnList : MonoBehaviour
{

    public float addToWaitTimer;

    public virtual void StartSpawn()
    {
        Debug.Log("base function : StartSpawn");
    }
}
