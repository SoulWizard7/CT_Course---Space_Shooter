using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AuthoringAndMono
{
    public class GameValuesMono : MonoBehaviour
    {
        public static float timerSingleton;
        public static int sunHealth;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            //Debug.Log(timerSingleton);
            Debug.Log(sunHealth);
        }
    }
    
    
}