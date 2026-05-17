using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Enum to keep track of which weapon is selected
    public enum WeaponType { Pistol, Shotgun, Rifle }

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Pistol;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform gunTransform;

    [Header("Weapon Animators")]
    public Animator pistolAnimator;
    public Animator shotgunAnimator;
    public Animator rifleAnimator;

    [Header("Weapon Audio Sources")]
    public AudioSource pistolAudioSource;
    public AudioSource shotgunAudioSource;
    public AudioSource rifleAudioSource;

    void Update()
    {
        if (gunTransform != null)
        {
            RotateGun();
        }

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
        // Execute animation and sound based on the active weapon type
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                if (pistolAnimator != null) pistolAnimator.SetTrigger("Shoot");
                if (pistolAudioSource != null && pistolAudioSource.clip != null) pistolAudioSource.PlayOneShot(pistolAudioSource.clip);
                break;

            case WeaponType.Shotgun:
                if (shotgunAnimator != null) shotgunAnimator.SetTrigger("Shoot");
                if (shotgunAudioSource != null && shotgunAudioSource.clip != null) shotgunAudioSource.PlayOneShot(shotgunAudioSource.clip);
                break;

            case WeaponType.Rifle:
                if (rifleAnimator != null) rifleAnimator.SetTrigger("Shoot");
                if (rifleAudioSource != null && rifleAudioSource.clip != null) rifleAudioSource.PlayOneShot(rifleAudioSource.clip);
                break;
        }

        // Create the bullet
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}