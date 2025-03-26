using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RInventoryItem : MonoBehaviour
{
    public string itemName;
    //public int quantity;
    public Vector2Int size; // size of item in grid, denoted as WxH
    public bool isRotated = false; // if the item is rotated in grid, return as HxW
    public Vector2Int position;

    public RInventoryItem(string name, Vector2Int size, Vector2Int position)
    {
        this.itemName = name;
        //this.quantity = quantity;
        this.size = size;
        this.position = position;
    }

    public Vector2Int GetSize()
    {
        return isRotated ? new Vector2Int(size.y, size.x) : size;
    }
}
