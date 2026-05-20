using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    
    // FIXED: Changed from float to int to match EnemyHealth
    public int damage = 1; 

    void Start()
    {
        // Destroy the bullet after 2 seconds so it doesn't fly forever
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // FIXED: Changed parameter type from float to int
    public void SetDamage(int newDamageValue)
    {
        damage = newDamageValue;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.tag.StartsWith("zombie")) 
        {
            hitInfo.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject); 
            return; 
        }

        string layerName = LayerMask.LayerToName(hitInfo.gameObject.layer);
        
        if (layerName == "Wall" || layerName == "Junk")
        {
            Destroy(gameObject);
        }
    }
}