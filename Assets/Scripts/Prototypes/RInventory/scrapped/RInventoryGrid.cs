using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RInventoryGrid : MonoBehaviour
{
    public List<RInventoryItem> items;
    public int gridWidth;
    public int gridHeight;
    private bool[,] grid;

    public RInventoryGrid(int width, int height)
    {
        this.gridWidth = width;
        this.gridHeight = height;
        grid = new bool[width, height];
        items = new List<RInventoryItem>();
    }

    public bool AddItem(RInventoryItem item, Vector2Int position)
    {
        if (DoesItemFit(item, position))
        {
            Vector2Int itemSize = item.GetSize();
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    grid[position.x + x, position.y + y] = true;
                }
            }
            item.position = position;
            items.Add(item);
            return true;
        }
        return false;
    }

    public bool RemoveItem(RInventoryItem item)
    {
        if (items.Contains(item))
        {
            Vector2Int itemSize = item.GetSize();
            Vector2Int position = item.position;

            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    grid[position.x + x, position.y + y] = false;
                }
            }
            items.Remove(item);
            return true;
        }
        return false;
    }

    public bool AddItemToFirstAvail(RInventoryItem newItem)
    {
        Vector2Int itemSize = newItem.GetSize();

        for (int x = 0; x <= gridWidth - itemSize.x; x++)
        {
            for (int y = 0; y <= gridHeight - itemSize.y; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (DoesItemFit(newItem, position))
                {
                    return AddItem(newItem, position);
                }
            }
        }
        return false;
    }

    private bool DoesItemFit(RInventoryItem item, Vector2Int position)
    {
        Vector2Int itemSize = item.GetSize();

        // basic error handling
        if (position.x + itemSize.x > gridWidth || position.y + itemSize.y > gridHeight)
        {
            return false;
        }

        // if overlap
        for (int x = 0; x < itemSize.x; x++)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                if (grid[position.x+x, position.y + y])
                {
                    return false; 
                }
            }
        }

        return true;
    }

    public List<RInventoryItem> GetInventory()
    {
        return items;
    }

    //public void PrintInventory()
    //{
    //    foreach (RInventoryItem item in items)
    //    {
    //        Debug.Log(item.itemName + ": " + item.quantity + " (" + item.GetSize().x + "x" + item.GetSize().y + ") at position (" +
    //            item.position.x + ", " + item.position.y + ")");
    //    }
    //}
}
