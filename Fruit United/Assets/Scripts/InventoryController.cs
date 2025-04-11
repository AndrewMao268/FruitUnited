using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryArea;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    public Slot[] inventorySlots;
    void Start()
    {

        inventorySlots = new Slot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryArea.transform).GetComponent<Slot>();
            inventorySlots[i] = slot;
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    
}
