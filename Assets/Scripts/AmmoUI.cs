using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI ammoText; // e.g. "12 / 30"

    public void UpdateAmmo(int current, int max, string weapon = "")
    {
        if (ammoText != null)
            ammoText.text = $"{current} / {max}";
    }
}