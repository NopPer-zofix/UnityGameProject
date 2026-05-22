using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI ammoText;

    public void UpdateAmmo(int current)
    {
        if (ammoText != null)
            ammoText.text = $"{current}";
    }
}