using Unity.VisualScripting;
using UnityEngine;

public class PomagranateCollectorScript : MonoBehaviour
{
    public InventoryController inventoryController;
    public Sprite sprite;



    private bool isPlayerNearby = false;


    void Update()
    {
                if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                Slot slot = Instantiate(inventoryController.slotPrefab, inventoryController.inventoryArea.transform).GetComponent<Slot>();
                GameObject item = Instantiate(inventoryController.itemPrefabs[8], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
                Destroy(gameObject);
        }
           
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
