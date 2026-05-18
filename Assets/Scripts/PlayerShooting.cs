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

    [Header("Weapon Damage Profiles")]
    public int pistolDamage = 1;
    public int shotgunDamage = 1;
    public int rifleDamage = 2;

    [Header("Shotgun Pump Mechanic")]
    public bool isShotgunReady = true; 
    [Tooltip("Optional sound to play when pressing R to pump the shotgun")]
    public AudioClip shotgunPumpSound;

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

        if (currentWeapon == WeaponType.Shotgun && Input.GetKeyDown(KeyCode.R))
        {
            ReloadShotgun();
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
        if (currentWeapon == WeaponType.Shotgun && !isShotgunReady)
        {
            Debug.Log("Shotgun needs to be reloaded! Press R.");
            return; 
        }

        int currentDamage = GetCurrentWeaponDamage();

        // --- FIXED: Rifle calls a sequence routine instead of instant frame calculation ---
        if (currentWeapon == WeaponType.Rifle)
        {
            StartCoroutine(RifleDoubleShotSequence(currentDamage));
            return; // Exit out early so the code below doesn't run for the rifle
        }

        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                if (pistolAnimator != null) pistolAnimator.SetTrigger("Shoot");
                if (pistolAudioSource != null && pistolAudioSource.clip != null) pistolAudioSource.PlayOneShot(pistolAudioSource.clip);
                break;

            case WeaponType.Shotgun:
                if (shotgunAnimator != null) shotgunAnimator.SetTrigger("Shoot");
                if (shotgunAudioSource != null && shotgunAudioSource.clip != null) shotgunAudioSource.PlayOneShot(shotgunAudioSource.clip);
                
                isShotgunReady = false; 
                break;
        }

        if (bulletPrefab != null && activeFirePoint != null)
        {
            if (currentWeapon == WeaponType.Shotgun)
            {
                SpawnBullet(activeFirePoint.rotation, currentDamage);

                Quaternion leftRotation = activeFirePoint.rotation * Quaternion.Euler(0, 0, spreadAngle);
                SpawnBullet(leftRotation, currentDamage);

                Quaternion rightRotation = activeFirePoint.rotation * Quaternion.Euler(0, 0, -spreadAngle);
                SpawnBullet(rightRotation, currentDamage);
            }
            else
            {
                SpawnBullet(activeFirePoint.rotation, currentDamage);
            }
        }
    }

    // --- NEW: This runs the two shots one right after another with audio and animation fixed ---
    System.Collections.IEnumerator RifleDoubleShotSequence(int damageToSet)
    {
        if (bulletPrefab == null || activeFirePoint == null) yield break;

        // ======= SHOT 1 =======
        if (rifleAnimator != null) rifleAnimator.SetTrigger("Shoot");
        if (rifleAudioSource != null && rifleAudioSource.clip != null) rifleAudioSource.PlayOneShot(rifleAudioSource.clip);
        SpawnBullet(activeFirePoint.rotation, damageToSet, Vector3.zero);

        // Wait a tiny split second (0.06 seconds) so the audio and animations can reset
        yield return new WaitForSeconds(0.06f);

        // ======= SHOT 2 =======
        if (currentWeapon == WeaponType.Rifle && activeFirePoint != null)
        {
            if (rifleAnimator != null) rifleAnimator.SetTrigger("Shoot");
            if (rifleAudioSource != null && rifleAudioSource.clip != null) rifleAudioSource.PlayOneShot(rifleAudioSource.clip);
            SpawnBullet(activeFirePoint.rotation, damageToSet, Vector3.zero);
        }
    }

    void ReloadShotgun()
    {
        if (isShotgunReady) 
        {
            Debug.Log("Shotgun is already loaded.");
            return; 
        }

        isShotgunReady = true;
        Debug.Log("Shotgun reloaded and ready to fire!");

        if (shotgunAudioSource != null && shotgunPumpSound != null)
        {
            shotgunAudioSource.PlayOneShot(shotgunPumpSound);
        }
    }

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

    void SpawnBullet(Quaternion rotation, int damageToSet, Vector3 positionOffset = default(Vector3))
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, activeFirePoint.position + positionOffset, rotation);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damageToSet);
        }
    }
}