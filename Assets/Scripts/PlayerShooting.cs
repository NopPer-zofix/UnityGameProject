using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public Transform gunTransform; // Drag your Pistol child object here!

    void Update()
    {
        // Check if gunTransform is assigned to avoid errors
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
        // 1. Get Mouse Position in World Space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Keep it in 2D space

        // 2. Calculate Direction and Angle from the Gun to the Mouse
        Vector2 direction = (Vector2)(mousePos - gunTransform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 3. Apply Rotation ONLY to the Gun
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);

        float flipY = (angle > 90 || angle < -90) ? -1f : 1f;
    
        // Set this to whatever the 'normal' size of your gun should be
        gunTransform.localScale = new Vector3(1f, flipY, 1f);


        // 4. Flip the Gun on the Y-axis so it isn't upside down when aiming left
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

        // Visual Debug: Draws a line in the Scene View to show where you are aiming
        Debug.DrawLine(gunTransform.position, mousePos, Color.red);
    }
    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    

}