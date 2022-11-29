using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarParticles : MonoBehaviour
{
    private ParticleSystem _ps;
    ParticleSystem.MainModule main;
    public float startSpeed = 10f;
    public float normalSpeed;
    private PlayerMovement _playerMovement; 

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        main = _ps.main;
        normalSpeed = main.simulationSpeed;

        _playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        main.simulationSpeed = startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        startSpeed -= Time.deltaTime * 50;

        if (main.simulationSpeed > normalSpeed)
        {
            main.simulationSpeed = startSpeed;
        }
        else
        {
            main.simulationSpeed = normalSpeed;
            _playerMovement.enabled = true;
            Destroy(this);
        }
    }
}
