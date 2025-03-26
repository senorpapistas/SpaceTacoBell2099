using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// slots in the hotbar
public class RInventorySlot : MonoBehaviour
{
    public RInventoryItem currentItem;
    public Vector2Int position;

    public void SetItem(RInventoryItem item)
    {
        currentItem = item;
    }

    public void ClearSlot()
    {
        currentItem = null;
    }

}

