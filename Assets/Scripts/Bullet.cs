using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
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

   private void OnTriggerEnter2D(Collider2D hitInfo)
{
    // If it hits an enemy, deal damage first
    if (hitInfo.gameObject.tag.StartsWith("zombie")) 
    {
        hitInfo.GetComponent<EnemyHealth>()?.TakeDamage(damage);
    }

    // Unless it's the Player, destroy the bullet on impact with anything
    if (!hitInfo.CompareTag("Player"))
    {
        Destroy(gameObject);
    }
}
}