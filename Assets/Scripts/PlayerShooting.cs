using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // A child object of the player where bullets spawn

    void Update()
    {
        // 1. Aiming: Rotate player/weapon towards the mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (Vector2)(mousePos - transform.position);
        
        // Calculate the angle to look at the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 2. Shooting: Detect Left Click
        if (Input.GetButtonDown("Fire1")) // Fire1 is Left Click by default
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    
}