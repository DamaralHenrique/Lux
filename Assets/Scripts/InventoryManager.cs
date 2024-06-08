using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<string> inventory = new List<string>();

    public void PrintInventory()
    {
        if (inventory.Count > 0)
        {
            string formattedString = "[" + string.Join(", ", inventory) + "]";
            Debug.Log("Inventory Items: " + formattedString);
        }
        else
        {
            Debug.Log("Inventory is empty.");
        }
    }

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log("Added " + itemName + " to inventory.");
    }
}
