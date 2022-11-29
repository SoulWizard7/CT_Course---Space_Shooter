using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector2 _movement = new Vector2(0,0);
    private Vector3 _startPos;
    private Vector3 _curPos;

    public GameObject projectile;
    public float fireRate = 0.5f;
    private float _fireRateTimer;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        Movement();
        Shooting();
    }

    private void Shooting()
    {
        if (_fireRateTimer > 0)
        {
            _fireRateTimer -= Time.deltaTime;
            return;
        }
        
        if (Input.GetButton("Fire1"))
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            _fireRateTimer = fireRate;
        }
    }
    
    private void Movement()
    {
        _curPos = transform.position;
        
        _movement.x = Input.GetAxis("Horizontal") * speed;
        _movement.x /= 100;
        _movement.y = Input.GetAxis("Vertical") * speed;
        _movement.y /= 100;
        
        transform.Translate(_movement.x, 0, _movement.y);
        
        if (_curPos.x < -35.1f)
        {
            transform.position = new Vector3(-35, _startPos.y, _curPos.z);
        }
        if (_curPos.x > 35.1f)
        {
            transform.position = new Vector3(35, _startPos.y, _curPos.z);
        }
        if (_curPos.z < -18.1f)
        {
            transform.position = new Vector3(_curPos.x, _startPos.y, -18);
        }
        if (_curPos.z > .1f)
        {
            transform.position = new Vector3(_curPos.x, _startPos.y, 0);
        }
    }
}
