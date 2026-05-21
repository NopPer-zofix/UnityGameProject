using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public enum WeaponType { Pistol, Shotgun, Rifle }

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Pistol;

    [Header("UI")]
    public AmmoUI ammoUI;

    [Header("References")]
    public GameObject PistolBulletPrefab;
    public GameObject ShotgunBulletPrefab;
    public GameObject RifleBulletPrefab;
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

    [Header("Ammo Settings (Max Capacity)")]
    public int pistolMaxAmmo = 7;
    public int shotgunMaxAmmo = 2;
    public int rifleMaxAmmo = 30;

    [Header("Current Ammo Tracking")]
    public int currentPistolAmmo;
    public int currentShotgunAmmo;
    public int currentRifleAmmo;

    private float nextPistolFireTime = 0f;
    private Transform activeFirePoint;

    public bool CanShoot { get; set; } = true;

    void Start()
    {
        // Fill up all ammo pools right at the start of the game
        currentPistolAmmo = pistolMaxAmmo;
        currentShotgunAmmo = shotgunMaxAmmo;
        currentRifleAmmo = rifleMaxAmmo;

        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        if (PauseMenu.IsPaused) return;

        if (!CanShoot) return;
        
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

    // --- Centralized UI update: reads current weapon state and pushes it to HUD ---
    void RefreshAmmoUI()
    {
        if (ammoUI == null) return;

        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                ammoUI.UpdateAmmo(currentPistolAmmo);
                break;
            case WeaponType.Shotgun:
                ammoUI.UpdateAmmo(currentShotgunAmmo);
                break;
            case WeaponType.Rifle:
                ammoUI.UpdateAmmo(currentRifleAmmo);
                break;
        }
    }

        // --- UPDATED: Handles swapping models seamlessly and resetting counters ---
    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        // Turning a model ON automatically disables the others, making previous weapons disappear
        if (pistolModel != null) pistolModel.SetActive(newWeapon == WeaponType.Pistol);
        if (shotgunModel != null) shotgunModel.SetActive(newWeapon == WeaponType.Shotgun);
        if (rifleModel != null) rifleModel.SetActive(newWeapon == WeaponType.Rifle);

        // Assign correct structural settings and fully REFRESH weapon ammo capacities upon pickup
        switch (newWeapon)
        {
            case WeaponType.Pistol:
                activeFirePoint = pistolFirePoint;
                currentPistolAmmo = pistolMaxAmmo; // Refresh counter
                break;

            case WeaponType.Shotgun:
                activeFirePoint = shotgunFirePoint;
                currentShotgunAmmo = shotgunMaxAmmo; // Refresh counter
                isShotgunReady = true;               // Auto-pump the clean shell
                break;

            case WeaponType.Rifle:
                activeFirePoint = rifleFirePoint;
                currentRifleAmmo = rifleMaxAmmo; // Refresh counter
                break;
        }
        
        Debug.Log($"Equipped: {newWeapon}. Ammo Counter Refreshed!");
        RefreshAmmoUI(); // Sync HUD immediately on weapon switch
    }

    void Shoot()
    {
        if (!HasAmmoToShoot())
        {
            Debug.Log($"{currentWeapon} is completely out of ammo!");
            return;
        }

        if (currentWeapon == WeaponType.Pistol && Time.time < nextPistolFireTime)
        {
            return; 
        }

        if (currentWeapon == WeaponType.Shotgun && !isShotgunReady)
        {
            Debug.Log("Shotgun needs to be reloaded! Press R.");
            return; 
        }

        int currentDamage = GetCurrentWeaponDamage();

        if (currentWeapon == WeaponType.Rifle)
        {
            StartCoroutine(RifleDoubleShotSequence(currentDamage));
            return;
        }

        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                if (pistolAnimator != null) pistolAnimator.SetTrigger("Shoot");
                if (pistolAudioSource != null && pistolAudioSource.clip != null) pistolAudioSource.PlayOneShot(pistolAudioSource.clip);
                
                currentPistolAmmo--; 
                nextPistolFireTime = Time.time + 1f;
                RefreshAmmoUI();
                break;

            case WeaponType.Shotgun:
                if (shotgunAnimator != null) shotgunAnimator.SetTrigger("Shoot");
                if (shotgunAudioSource != null && shotgunAudioSource.clip != null) shotgunAudioSource.PlayOneShot(shotgunAudioSource.clip);
                
                currentShotgunAmmo--; 
                isShotgunReady = false; 
                RefreshAmmoUI();
                break;
        }

        if (activeFirePoint != null)
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

    System.Collections.IEnumerator RifleDoubleShotSequence(int damageToSet)
    {
        // ======= SHOT 1 =======
        if (rifleAnimator != null) rifleAnimator.SetTrigger("Shoot");
        if (rifleAudioSource != null && rifleAudioSource.clip != null) rifleAudioSource.PlayOneShot(rifleAudioSource.clip);
        SpawnBullet(activeFirePoint.rotation, damageToSet, Vector3.zero);
        currentRifleAmmo--; 
        RefreshAmmoUI();

        yield return new WaitForSeconds(0.06f);

        // ======= SHOT 2 =======
        if (currentWeapon == WeaponType.Rifle && activeFirePoint != null && currentRifleAmmo > 0)
        {
            if (rifleAnimator != null) rifleAnimator.SetTrigger("Shoot");
            if (rifleAudioSource != null && rifleAudioSource.clip != null) rifleAudioSource.PlayOneShot(rifleAudioSource.clip);
            SpawnBullet(activeFirePoint.rotation, damageToSet, Vector3.zero);
            currentRifleAmmo--; 
            RefreshAmmoUI();
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

    bool HasAmmoToShoot()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol: return currentPistolAmmo > 0;
            case WeaponType.Shotgun: return currentShotgunAmmo > 0;
            case WeaponType.Rifle: return currentRifleAmmo > 0;
            default: return false;
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
        GameObject selectedPrefab = null;

        switch (currentWeapon)
        {
            case WeaponType.Pistol: selectedPrefab = PistolBulletPrefab; break;
            case WeaponType.Shotgun: selectedPrefab = ShotgunBulletPrefab; break;
            case WeaponType.Rifle: selectedPrefab = RifleBulletPrefab; break;
        }

        if (selectedPrefab == null || activeFirePoint == null) return;

        GameObject bulletInstance = Instantiate(selectedPrefab, activeFirePoint.position + positionOffset, rotation);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damageToSet);
        }
    }
}
