using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryCanvas;
    void Start()
    {
        if (inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
    }
}
