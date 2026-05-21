using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool HasKey { get; private set; } = false;

    public void PickUpKey()
    {
        HasKey = true;
        Debug.Log("[Inventory] Key acquired!");
    }

    public void UseKey()
    {
        HasKey = false;
        Debug.Log("[Inventory] Key used.");
    }
}