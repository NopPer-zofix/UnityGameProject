using UnityEngine;

/// <summary>
/// EnemyHealthBar — attach to the enemy GameObject.
///
/// Displays a world-space health bar above the enemy that shrinks
/// as the enemy takes damage. Works with your existing EnemyHealth.cs.
///
/// Setup:
///   1. Create the health bar prefab (see instructions below).
///   2. Attach this script to the enemy.
///   3. Drag the health bar prefab into the "Health Bar Prefab" field.
///   4. Set "Bar Offset" to control how high above the enemy it appears.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The health bar prefab (a Canvas with Background + Fill child objects).")]
    public GameObject healthBarPrefab;

    [Header("Position")]
    [Tooltip("How far above the enemy the bar floats. Increase if it overlaps the sprite.")]
    public Vector3 barOffset = new Vector3(0f, 0.8f, 0f);

    [Header("Colors")]
    public Color colorFull    = Color.green;
    public Color colorMedium  = Color.yellow;   // below 60%
    public Color colorLow     = Color.red;      // below 30%

    // ── private ──────────────────────────────────────────────────────────────
    private EnemyHealth    enemyHealth;
    private Transform      barTransform;   // the instantiated bar root
    private Transform      fillTransform;  // the inner fill that we scale
    private SpriteRenderer fillRenderer;   // to tint the fill by health %

    private int   maxHealth;
    private float lastHealthRatio = 1f;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        maxHealth   = enemyHealth.health;

        if (healthBarPrefab == null)
        {
            Debug.LogWarning($"[EnemyHealthBar] No prefab assigned on {gameObject.name}.");
            return;
        }

        // Spawn the bar as a child so it moves with the enemy automatically
        GameObject bar = Instantiate(healthBarPrefab, transform);
        bar.transform.localPosition = barOffset;
        barTransform = bar.transform;

        // Expect children named "Background" and "Fill"
        fillTransform = bar.transform.Find("Fill");
        if (fillTransform != null)
            fillRenderer = fillTransform.GetComponent<SpriteRenderer>();

        UpdateBar(1f);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        if (enemyHealth == null || barTransform == null) return;

        float ratio = (float)enemyHealth.health / maxHealth;

        // Only redraw when health actually changed
        if (!Mathf.Approximately(ratio, lastHealthRatio))
        {
            UpdateBar(ratio);
            lastHealthRatio = ratio;
        }

        // Keep bar upright even if the enemy flips horizontally
        barTransform.rotation = Quaternion.identity;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void UpdateBar(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);

        // Scale the fill on the X axis — localScale.x goes from 1 (full) to 0 (empty)
        if (fillTransform != null)
        {
            Vector3 s = fillTransform.localScale;
            fillTransform.localScale = new Vector3(ratio, s.y, s.z);
        }

        // Tint the fill based on health percentage
        if (fillRenderer != null)
        {
            if      (ratio > 0.6f) fillRenderer.color = colorFull;
            else if (ratio > 0.3f) fillRenderer.color = colorMedium;
            else                   fillRenderer.color = colorLow;
        }
    }
}