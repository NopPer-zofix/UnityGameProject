using UnityEngine;
public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform gunTransform;
    public Animator pistolAnimator; // NEW: Drag the Pistol object here!
    void Update()
    {
        if (gunTransform != null)
        {
            RotateGun();
        }
        // Shooting: Detect Left Click
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    void RotateGun()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = (Vector2)(mousePos - gunTransform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        // Flip logic simplified
        Vector3 gunScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            gunScale.y = -1f;
        }
        else
        {
            gunScale.y = 1f;
        }
        gunTransform.localScale = gunScale;
        Debug.DrawLine(gunTransform.position, mousePos, Color.red);
    }
    void Shoot()
    {
        // 1. Play the Pistol's Animation (Recoil + Flames)
        if (pistolAnimator != null)
        {
            pistolAnimator.SetTrigger("Shoot");
        }
        // 2. Create the bullet
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}