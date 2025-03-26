using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class InventoryController : MonoBehaviour
{
    public Inventory inventory { get; private set; }
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        UpdateGridOnMouse();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Check if mouse is inside any grid.
            //Debug.Log($"checking if {inventory.gridOnMouse != null} or " +
            //    $"{!inventory.itemManager.ReachedBoundary(inventory.GetSlotAtMouseCoords(), inventory.gridOnMouse)}" +
            //    $"with {inventory.GetSlotAtMouseCoords()} and {inventory.gridOnMouse}");
            if (inventory.gridOnMouse != null && !inventory.itemManager.ReachedBoundary(inventory.GetSlotAtMouseCoords(), inventory.gridOnMouse))
            {
                if (inventory.selectedItem)
                {
                    Item oldSelectedItem = inventory.selectedItem;
                    Item overlapItem = inventory.GetItemAtMouseCoords();

                    if (overlapItem != null)
                    {
                        inventory.SwapItem(overlapItem, oldSelectedItem);
                    }
                    else
                    {
                        inventory.MoveItem(oldSelectedItem);
                    }
                }
                else
                {
                    SelectItemWithMouse();
                }
            }
        }

        // Remove an item from the inventory
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RemoveItemWithMouse();
        }

        // Generates a random item in the inventory
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.AddItem(inventory.itemsData[UnityEngine.Random.Range(0, inventory.itemsData.Length)]);
            Debug.Log($"{inventory.grids.Length} {inventory.grids}");
        }

        if (inventory.selectedItem != null)
        {
            MoveSelectedItemToMouse();

            if (Input.GetKeyDown(KeyCode.R))
            {
                inventory.selectedItem.Rotate();
            }
        }
    }

    private void UpdateGridOnMouse()
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Raycast and check for inventory grids
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            var grid = result.gameObject.GetComponent<InventoryGrid>();
            if (grid != null)
            {
                inventory.gridOnMouse = grid;
                return;
            }
        }

        // If no grid is found, set gridOnMouse to null
        inventory.gridOnMouse = null;
    }

    /// <summary>
    /// Select a new item in the inventory.
    /// </summary>
    private void SelectItemWithMouse()
    {
        Item item = inventory.GetItemAtMouseCoords();

        if (item != null)
        {
            inventory.SelectItem(item);
        }
    }

    /// <summary>
    /// Removes the item from the inventory that the mouse is hovering over.
    /// </summary>
    private void RemoveItemWithMouse()
    {
        Item item = inventory.GetItemAtMouseCoords();

        if (item != null)
        {
            inventory.RemoveItem(item);
        }
    }

    /// <summary>
    /// Moves the currently selected object to the mouse.
    /// </summary>
    private void MoveSelectedItemToMouse()
    {
        inventory.selectedItem.rectTransform.position = new Vector3(
                Input.mousePosition.x
                    + ((inventory.selectedItem.correctedSize.width * InventorySettings.slotSize.x) / 2)
                    - InventorySettings.slotSize.x / 2,
                Input.mousePosition.y
                    - ((inventory.selectedItem.correctedSize.height * InventorySettings.slotSize.y) / 2)
                    + InventorySettings.slotSize.y / 2,
                Input.mousePosition.z
            );
    }
}