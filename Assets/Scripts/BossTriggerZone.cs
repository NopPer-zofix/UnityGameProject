using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossController bossController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossController != null)
            {
                bossController.ActivateBoss();
                
                Destroy(gameObject); 
            }
        }
    }
}