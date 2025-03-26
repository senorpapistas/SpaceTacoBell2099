using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public RInventoryManager inventoryManager;
    public List<RInventorySlot> slots;

    void Start()
    {
        inventoryManager = FindObjectOfType<RInventoryManager>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (RInventorySlot slot in slots)
        {
            slot.ClearSlot();
        }

        foreach (RInventoryItem item in inventoryManager.inventoryGrid.GetInventory())
        {
            RInventorySlot slot = FindSlotForItem(item);
            if (slot != null)
            {
                slot.SetItem(item);
            }
        }
    }

    public void OnItemClicked(RInventoryItem item)
    {
        
    }

    public void OnItemDragged(RInventoryItem item, Vector2Int newPosition)
    {
        inventoryManager.MoveItemInInventory(item, newPosition);
        UpdateUI();
    }

    public void OnItemRotated(RInventoryItem item)
    {
        inventoryManager.RotateItemInInventory(item);
        UpdateUI();
    }

    private RInventorySlot FindSlotForItem(RInventoryItem item)
    {
        foreach (RInventorySlot slot in slots)
        {
            if (slot.position == item.position)
            {
                return slot;
            }
        }
        return null;
    }
}