using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Heart Images (assign 4 in Inspector)")]
    public Image[] hearts; // drag 4 heart Image objects here

    // Updates heart display based on current HP (0-100)
    public void UpdateHearts(int currentHP)
    {
        // Each heart = 25 HP
        // 100 HP = 4 full, 75 = 3 full, 50 = 2 full, 25 = 1 full, 0 = 0 full
        int fullCount = Mathf.CeilToInt(currentHP / 25f);

        for (int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = i < fullCount ? fullHeart : emptyHeart;
    }
}