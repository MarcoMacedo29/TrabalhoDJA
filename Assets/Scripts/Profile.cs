using UnityEngine;
using UnityEngine.EventSystems;

public class Profile : MonoBehaviour
{
    public GameObject PlayerStatusCanvas;
    public GameObject inventoryCanvas;

    private void Start()
    {
        if (PlayerStatusCanvas != null)
            PlayerStatusCanvas.SetActive(false);

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
            {
                CloseAllCanvases();
            }
        }
    }

    public void ToggleInventory()
    {
        bool isInventoryOpen = inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(!isInventoryOpen);

        if (!isInventoryOpen && PlayerStatusCanvas.activeSelf)
        {
            PlayerStatusCanvas.SetActive(false);
        }
    }

    public void TogglePlayerStatus()
    {
        bool isPlayerStatusOpen = PlayerStatusCanvas.activeSelf;
        PlayerStatusCanvas.SetActive(!isPlayerStatusOpen);

        if (!isPlayerStatusOpen && inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CloseAllCanvases()
    {
        inventoryCanvas.SetActive(false);
        PlayerStatusCanvas.SetActive(false);
    }
}
