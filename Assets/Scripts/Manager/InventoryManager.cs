using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<SwordPart> inventory = new List<SwordPart>();
    public int maxSlots = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadInventory();
    }

    public void AddItem(SwordPart item)
    {
        if (item != null && inventory.Count < maxSlots)
        {
            inventory.Add(item);
            Debug.Log("Added item: " + item.partName);

            SaveInventory();

            InventoryUI ui = Object.FindFirstObjectByType<InventoryUI>();
            if (ui != null)
                ui.RefreshUI();
        }
        else
        {
            Debug.LogWarning("Inventory full or item is null");
        }
    }

    void SaveInventory()
    {
        List<string> names = new List<string>();
        foreach (SwordPart item in inventory)
        {
            names.Add(item.partName);
        }

        string saveString = string.Join(",", names.ToArray());
        PlayerPrefs.SetString("Inventory", saveString);
        PlayerPrefs.Save();
    }

    void LoadInventory()
    {
        inventory.Clear();

        if (PlayerPrefs.HasKey("Inventory"))
        {
            string saved = PlayerPrefs.GetString("Inventory");
            if (!string.IsNullOrEmpty(saved))
            {
                string[] names = saved.Split(',');

                foreach (string partName in names)
                {
                    SwordPart item = GetSwordPartByName(partName);
                    if (item != null)
                        inventory.Add(item);
                    else
                        Debug.LogWarning("SwordPart not found in Resources: " + partName);
                }
            }
        }

        // Refresh UI after loading inventory
        InventoryUI ui = Object.FindFirstObjectByType<InventoryUI>();
        if (ui != null)
            ui.RefreshUI();
    }

    SwordPart GetSwordPartByName(string partName)
    {
        SwordPart[] parts = Resources.LoadAll<SwordPart>("SwordParts");
        foreach (SwordPart part in parts)
        {
            if (part.partName == partName)
                return part;
        }
        return null;
    }

    public bool HasPartOfType(SwordPartType type)
    {
        return inventory.Exists(item => item.partType == type);
    }
}
