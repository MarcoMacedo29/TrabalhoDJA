using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    public Button profileInventoryButton;
    public Button profilePlayerButton;
    public InventoryUI inventory;
    public ProfilePlayerStatus playerstatus;

    public void Start()
    {
        profileInventoryButton.onClick.AddListener(OpenInventory);
        profilePlayerButton.onClick.AddListener(OpenPlayerStatus);
    }

    public void OpenInventory()
    {
        inventory.ToggleInventory(); 
    }

    public void OpenPlayerStatus()
    {
        playerstatus.TogglePlayerStatus();
    }
}
