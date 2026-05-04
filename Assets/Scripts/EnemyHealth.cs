using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage(int amount)
    {
        health -= amount;
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die() {
    Debug.Log("Enemy is dead!");
    Destroy(gameObject); // This is the line that actually removes them
}
}