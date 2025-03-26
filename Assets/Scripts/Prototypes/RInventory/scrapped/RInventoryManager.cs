using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RInventoryManager : MonoBehaviour
{
    public RInventoryGrid inventoryGrid;
    // add equippable items here

    private int GRID_WIDTH = 16;
    private int GRID_HEIGHT = 8;

    void Start()
    {
        inventoryGrid = new RInventoryGrid(GRID_WIDTH, GRID_HEIGHT);
    }

    public void AddItemToInventory(RInventoryItem item)
    {
        if (!inventoryGrid.AddItemToFirstAvail(item))
        {
            Debug.Log("Inventory is full from InventoryManager");
        }
    }

    public void RemoveItemFromInventory(RInventoryItem item)
    {
        inventoryGrid.RemoveItem(item);
    }

    public void MoveItemInInventory(RInventoryItem item, Vector2Int newPosition)
    {
        if (!inventoryGrid.AddItem(item, newPosition))
        {
            Debug.Log("Cannot move item to the new position: " + newPosition);
            inventoryGrid.AddItemToFirstAvail(item); // on fail then revert to free position?
        }
    }

    public void RotateItemInInventory(RInventoryItem item)
    {
        item.isRotated = !item.isRotated;
        Vector2Int originalPosition = item.position;
        inventoryGrid.RemoveItem(item);
        if (!inventoryGrid.AddItem(item, originalPosition))
        {
            Debug.Log("Cannot rotate item at the current position: " + originalPosition);
            item.isRotated = !item.isRotated; // Revert rotation if it doesn't fit
            inventoryGrid.AddItem(item, originalPosition);
        }
    }
}
