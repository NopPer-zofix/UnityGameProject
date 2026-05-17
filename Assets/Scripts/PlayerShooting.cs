using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public enum WeaponType { Pistol, Shotgun, Rifle }

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Pistol;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform gunTransform;

    [Header("Weapon Models")]
    public GameObject pistolModel;
    public GameObject shotgunModel;
    public GameObject rifleModel;

    [Header("Weapon Animators")]
    public Animator pistolAnimator;
    public Animator shotgunAnimator;
    public Animator rifleAnimator;

    [Header("Weapon Audio Sources")]
    public AudioSource pistolAudioSource;
    public AudioSource shotgunAudioSource;
    public AudioSource rifleAudioSource;

    [Header("Weapon Specific Fire Points")]
    public Transform pistolFirePoint;
    public Transform shotgunFirePoint;
    public Transform rifleFirePoint;

    [Header("Shotgun Settings")]
    public float spreadAngle = 15f; 

    // --- ADDED: Custom damage values per weapon ---
    [Header("Weapon Damage Profiles")]
    public int pistolDamage = 1;
    public int shotgunDamage = 1;
    public int rifleDamage = 2;

    private Transform activeFirePoint;

    void Start()
    {
        EquipWeapon(currentWeapon);
    }

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

    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        if (pistolModel != null) pistolModel.SetActive(newWeapon == WeaponType.Pistol);
        if (shotgunModel != null) shotgunModel.SetActive(newWeapon == WeaponType.Shotgun);
        if (rifleModel != null) rifleModel.SetActive(newWeapon == WeaponType.Rifle);

        switch (newWeapon)
        {
            case WeaponType.Pistol:
                activeFirePoint = pistolFirePoint;
                break;
            case WeaponType.Shotgun:
                activeFirePoint = shotgunFirePoint;
                break;
            case WeaponType.Rifle:
                activeFirePoint = rifleFirePoint;
                break;
        }
    }

    void Shoot()
    {
        // Get the damage value based on the current weapon selection
        int currentDamage = GetCurrentWeaponDamage();

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

        if (bulletPrefab != null && activeFirePoint != null)
        {
            if (currentWeapon == WeaponType.Shotgun)
            {
                // Bullet 1: Center
                SpawnBullet(activeFirePoint.rotation, currentDamage);

                // Bullet 2: Left
                Quaternion leftRotation = activeFirePoint.rotation * Quaternion.Euler(0, 0, spreadAngle);
                SpawnBullet(leftRotation, currentDamage);

                // Bullet 3: Right
                Quaternion rightRotation = activeFirePoint.rotation * Quaternion.Euler(0, 0, -spreadAngle);
                SpawnBullet(rightRotation, currentDamage);
            }
            else
            {
                // Pistol and Rifle single shot
                SpawnBullet(activeFirePoint.rotation, currentDamage);
            }
        }
    }

    // --- ADDED: Helper method to look up active damage values ---
    int GetCurrentWeaponDamage()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol: return pistolDamage;
            case WeaponType.Shotgun: return shotgunDamage;
            case WeaponType.Rifle: return rifleDamage;
            default: return 1;
        }
    }

    // --- ADDED: Helper method that handles spawning AND transferring the damage parameter ---
    void SpawnBullet(Quaternion rotation, int damageToSet)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, activeFirePoint.position, rotation);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damageToSet);
        }
    }
}