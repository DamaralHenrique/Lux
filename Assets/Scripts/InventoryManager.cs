using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<string> inventory = new List<string>();
    public Image blueTotem;
    public Image redTotem;
    public Image greenTotem;

    public void InitIventory()
    {
        blueTotem.enabled = false;
        redTotem.enabled = false;
        greenTotem.enabled = false;
    }

    public void UpdateLayout()
    {
        foreach(string item in inventory){
            if(item == "BlueTotem"){
                blueTotem.enabled = true;
            }else if(item == "RedTotem"){
                redTotem.enabled = true;
            }else if(item == "GreenTotem"){
                greenTotem.enabled = true;
            }
        }
    }

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
        UpdateLayout();
    }
}
