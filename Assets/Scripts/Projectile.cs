using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed;
    public int damageAmount = 1;

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        transform.Translate(0, 0, projectileSpeed / 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        Debug.Log("Hit");
        //other.GetComponentInParent<AlienShip>().TakeDamage(damageAmount);
        Destroy(gameObject);
    }
}
