using System;
using UnityEngine;

// Credit: https://github.com/DavidSouzaLD/InventoryTetris-Unity/tree/main

public static class InventorySettings
{
    public static readonly Vector2Int slotSize = new(96, 96);
    public static readonly float slotScale = 1f;
    public static readonly float rotationAnimationSpeed = 30f;
}

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    public ItemData[] itemsData;
    public Item itemPrefab;
    public InventoryGrid gridOnMouse { get; set; }
    public InventoryGrid[] grids { get; private set; }
    public Item selectedItem { get; private set; }
    public ItemManager itemManager { get; private set; }

    private void Awake()
    {
        grids = GameObject.FindObjectsOfType<InventoryGrid>();
        itemManager = new ItemManager(this);
    }

    public void SelectItem(Item item)
    {
        itemManager.SelectItem(item);
    }

    public void DeselectItem()
    {
        itemManager.DeselectItem();
    }

    public void AddItem(ItemData itemData)
    {
        itemManager.AddItem(itemData);
    }

    public void RemoveItem(Item item)
    {
        itemManager.RemoveItem(item);
    }

    public void MoveItem(Item item, bool deselectItemInEnd = true)
    {
        itemManager.MoveItem(item, deselectItemInEnd);
    }

    public void SwapItem(Item overlapItem, Item oldSelectedItem)
    {
        itemManager.SwapItem(overlapItem, oldSelectedItem);
    }

    public Vector2Int GetSlotAtMouseCoords()
    {
        return itemManager.GetSlotAtMouseCoords();
    }

    public Item GetItemAtMouseCoords()
    {
        return itemManager.GetItemAtMouseCoords();
    }

    public Item GetItemFromSlotPosition(Vector2Int slotPosition)
    {
        return itemManager.GetItemFromSlotPosition(slotPosition);
    }

    internal void SetSelectedItem(Item item)
    {
        selectedItem = item;
    }
}

public class ItemManager
{
    private readonly Inventory inventory;

    public ItemManager(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void SelectItem(Item item)
    {
        ClearItemReferences(item);
        inventory.SetSelectedItem(item);
        inventory.selectedItem.rectTransform.SetParent(inventory.transform);
        inventory.selectedItem.rectTransform.SetAsLastSibling();
    }

    public void DeselectItem()
    {
        inventory.SetSelectedItem(null);
    }

    public void AddItem(ItemData itemData)
    {
        for (int g = 0; g < inventory.grids.Length; g++)
        {
            for (int y = 0; y < inventory.grids[g].gridSize.y; y++)
            {
                for (int x = 0; x < inventory.grids[g].gridSize.x; x++)
                {
                    Vector2Int slotPosition = new Vector2Int(x, y);

                    for (int r = 0; r < 2; r++)
                    {
                        if (r == 0)
                        {
                            if (!ExistsItem(slotPosition, inventory.grids[g], itemData.size.width, itemData.size.height))
                            {
                                CreateNewItem(itemData, slotPosition, inventory.grids[g]);
                                return;
                            }
                        }

                        if (r == 1)
                        {
                            if (!ExistsItem(slotPosition, inventory.grids[g], itemData.size.height, itemData.size.width))
                            {
                                CreateNewItem(itemData, slotPosition, inventory.grids[g], true);
                                return;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("(Inventory) Not enough slots found to add the item!");
    }

    public void RemoveItem(Item item)
    {
        if (item != null)
        {
            ClearItemReferences(item);
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }

    public void MoveItem(Item item, bool deselectItemInEnd = true)
    {
        Debug.Log("move item has been called");
        Vector2Int slotPosition = GetSlotAtMouseCoords();

        if (inventory.gridOnMouse == null)
        {
            Debug.Log("No grid under the mouse.");
            return;
        }

        if (ReachedBoundary(slotPosition, inventory.gridOnMouse, item.correctedSize.width, item.correctedSize.height))
        {
            Debug.Log("Bounds");
            return;
        }

        if (ExistsItem(slotPosition, inventory.gridOnMouse, item.correctedSize.width, item.correctedSize.height))
        {
            Debug.Log("Item");
            return;
        }

        // Clear item ref in the old grid
        ClearItemReferences(item);

        item.indexPosition = slotPosition;
        item.rectTransform.SetParent(inventory.gridOnMouse.rectTransform);

        for (int x = 0; x < item.correctedSize.width; x++)
        {
            for (int y = 0; y < item.correctedSize.height; y++)
            {
                int slotX = item.indexPosition.x + x;
                int slotY = item.indexPosition.y + y;

                inventory.gridOnMouse.items[slotX, slotY] = item;
            }
        }

        item.rectTransform.localPosition = IndexToInventoryPosition(item);
        item.inventoryGrid = inventory.gridOnMouse;

        if (deselectItemInEnd)
        {
            DeselectItem();
        }
    }

    public void SwapItem(Item overlapItem, Item oldSelectedItem)
    {
        if (ReachedBoundary(overlapItem.indexPosition, inventory.gridOnMouse, oldSelectedItem.correctedSize.width, oldSelectedItem.correctedSize.height))
        {
            return;
        }

        ClearItemReferences(overlapItem);

        if (ExistsItem(overlapItem.indexPosition, inventory.gridOnMouse, oldSelectedItem.correctedSize.width, oldSelectedItem.correctedSize.height))
        {
            RevertItemReferences(overlapItem);
            return;
        }

        SelectItem(overlapItem);
        MoveItem(oldSelectedItem, false);
    }

    public void ClearItemReferences(Item item)
    {
        for (int x = 0; x < item.correctedSize.width; x++)
        {
            for (int y = 0; y < item.correctedSize.height; y++)
            {
                int slotX = item.indexPosition.x + x;
                int slotY = item.indexPosition.y + y;

                item.inventoryGrid.items[slotX, slotY] = null;
            }
        }
    }

    public void RevertItemReferences(Item item)
    {
        for (int x = 0; x < item.correctedSize.width; x++)
        {
            for (int y = 0; y < item.correctedSize.height; y++)
            {
                int slotX = item.indexPosition.x + x;
                int slotY = item.indexPosition.y + y;

                item.inventoryGrid.items[slotX, slotY] = item;
            }
        }
    }

    public bool ExistsItem(Vector2Int slotPosition, InventoryGrid grid, int width = 1, int height = 1)
    {
        if (ReachedBoundary(slotPosition, grid, width, height))
        {
            Debug.Log("Bounds2");
            return true;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int slotX = slotPosition.x + x;
                int slotY = slotPosition.y + y;

                if (grid.items[slotX, slotY] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool ReachedBoundary(Vector2Int slotPosition, InventoryGrid gridReference, int width = 1, int height = 1)
    {
        if (slotPosition.x + width > gridReference.gridSize.x || slotPosition.x < 0)
        {
            return true;
        }

        if (slotPosition.y + height > gridReference.gridSize.y || slotPosition.y < 0)
        {
            return true;
        }

        return false;
    }

    public Vector3 IndexToInventoryPosition(Item item)
    {
        Vector3 inventorizedPosition =
            new()
            {
                x = item.indexPosition.x * InventorySettings.slotSize.x
                    + InventorySettings.slotSize.x * item.correctedSize.width / 2,
                y = -(item.indexPosition.y * InventorySettings.slotSize.y
                    + InventorySettings.slotSize.y * item.correctedSize.height / 2
                )
            };

        return inventorizedPosition;
    }

    public Vector2Int GetSlotAtMouseCoords()
    {
        if (inventory.gridOnMouse == null)
        {
            return Vector2Int.zero;
        }

        Vector2 gridPosition =
            new(
                Input.mousePosition.x - inventory.gridOnMouse.rectTransform.position.x,
                inventory.gridOnMouse.rectTransform.position.y - Input.mousePosition.y
            );

        Vector2Int slotPosition =
            new(
                (int)(gridPosition.x / (InventorySettings.slotSize.x * InventorySettings.slotScale)),
                (int)(gridPosition.y / (InventorySettings.slotSize.y * InventorySettings.slotScale))
            );

        return slotPosition;
    }

    public Item GetItemAtMouseCoords()
    {
        Vector2Int slotPosition = GetSlotAtMouseCoords();

        if (!ReachedBoundary(slotPosition, inventory.gridOnMouse))
        {
            return GetItemFromSlotPosition(slotPosition);
        }

        return null;
    }

    public Item GetItemFromSlotPosition(Vector2Int slotPosition)
    {
        return inventory.gridOnMouse.items[slotPosition.x, slotPosition.y];
    }

    private void CreateNewItem(ItemData itemData, Vector2Int slotPosition, InventoryGrid grid, bool rotate = false)
    {
        Item newItem = UnityEngine.Object.Instantiate(inventory.itemPrefab);
        newItem.rectTransform = newItem.GetComponent<RectTransform>();
        newItem.rectTransform.SetParent(grid.rectTransform, false);
        newItem.rectTransform.sizeDelta = new Vector2(
            itemData.size.width * InventorySettings.slotSize.x,
            itemData.size.height * InventorySettings.slotSize.y
        );

        newItem.rectTransform.localScale = new Vector3(1, 1, 1);

        newItem.indexPosition = slotPosition;
        newItem.inventory = inventory;
        newItem.itemRotation = new ItemRotation(newItem); // Ensure itemRotation is initialized

        if (rotate)
        {
            newItem.Rotate();
        }

        for (int xx = 0; xx < itemData.size.width; xx++)
        {
            for (int yy = 0; yy < itemData.size.height; yy++)
            {
                int slotX = slotPosition.x + xx;
                int slotY = slotPosition.y + yy;

                grid.items[slotX, slotY] = newItem;
                grid.items[slotX, slotY].data = itemData;
            }
        }

        newItem.rectTransform.localPosition = IndexToInventoryPosition(newItem);
        newItem.inventoryGrid = grid;
    }
}