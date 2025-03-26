using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Item : MonoBehaviour
{
    public ItemData data;
    public Image icon;
    public Image background;
    public Vector2Int indexPosition { get; set; }
    public Inventory inventory { get; set; }
    public RectTransform rectTransform { get; set; }
    public InventoryGrid inventoryGrid { get; set; }
    public ItemRotation itemRotation { get; set; }

    private void Awake() // make sure these are initialized
    {
        rectTransform = GetComponent<RectTransform>();
        itemRotation = new ItemRotation(this);
    }

    private void Start()
    {
        icon.sprite = data.icon;
        background.color = data.backgroundColor;
        itemRotation = new ItemRotation(this);
    }

    private void LateUpdate()
    {
        itemRotation.UpdateRotateAnimation();
    }

    public void Rotate()
    {
        itemRotation.Rotate();
    }

    public void ResetRotate()
    {
        itemRotation.ResetRotate();
    }

    public SizeInt correctedSize => itemRotation.CorrectedSize;
}

public class ItemRotation
{
    private readonly Item item;
    private Vector3 rotateTarget;
    public bool IsRotated { get; private set; }
    public int RotateIndex { get; private set; }

    public ItemRotation(Item item)
    {
        this.item = item;
    }

    public SizeInt CorrectedSize => new(!IsRotated ? item.data.size.width : item.data.size.height, !IsRotated ? item.data.size.height : item.data.size.width);

    public void Rotate()
    {
        RotateIndex = (RotateIndex + 1) % 4;
        UpdateRotation();
    }

    public void ResetRotate()
    {
        RotateIndex = 0;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        switch (RotateIndex)
        {
            case 0:
                rotateTarget = new Vector3(0, 0, 0);
                IsRotated = false;
                break;
            case 1:
                rotateTarget = new Vector3(0, 0, -90);
                IsRotated = true;
                break;
            case 2:
                rotateTarget = new Vector3(0, 0, -180);
                IsRotated = false;
                break;
            case 3:
                rotateTarget = new Vector3(0, 0, -270);
                IsRotated = true;
                break;
        }
    }

    public void UpdateRotateAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(rotateTarget);
        if (item.rectTransform.localRotation != targetRotation)
        {
            item.rectTransform.localRotation = Quaternion.Slerp(
                item.rectTransform.localRotation,
                targetRotation,
                InventorySettings.rotationAnimationSpeed * Time.deltaTime
            );
        }
    }
}