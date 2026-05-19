using UnityEngine;
using UnityEngine.UI;
 
/// <summary>
/// BossHealthUI — attach to a Canvas UI GameObject.
///
/// Setup:
///   1. Create a Canvas → add a Slider (rename to BossHealthBar).
///   2. Add three small Image objects as phase indicators.
///   3. Drag references into this script.
///   4. Attach to the Canvas and drag BossController in.
/// </summary>
public class BossHealthUI : MonoBehaviour
{
    public BossController boss;
    public Slider         healthBar;
 
    [Tooltip("Three small icons that dim as phases complete.")]
    public Image[] phaseIndicators;
 
    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        if (boss == null || healthBar == null) return;
 
        // Update health bar fill
        healthBar.value = (float)boss.CurrentHealth / boss.maxHealth;
 
        // Dim phase indicators as phases complete
        // Phase 1 done = below 50%, phase 2 done = below 20%
        float pct = healthBar.value;
        if (phaseIndicators.Length >= 1)
            phaseIndicators[0].color = pct <= 0.5f ? Color.gray : Color.white;
        if (phaseIndicators.Length >= 2)
            phaseIndicators[1].color = pct <= 0.2f ? Color.gray : Color.white;
    }
}