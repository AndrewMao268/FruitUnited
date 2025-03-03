using UnityEngine;

public class Sword : Item
{
    public Sprite woodenSword;
    public Sprite stoneSword;
    public Sprite ironSword;
    public Sprite diamondSword;

    void Start()
    {
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        OwnStart();
    }

    void Update()
    {
        Sprite correctSprite = woodenSword;
        switch (itemID)
        {
            case 0: correctSprite = woodenSword; break;
            case 1: correctSprite = stoneSword; break;
            case 2: correctSprite = ironSword; break;
            case 3: correctSprite = diamondSword; break;
            default: break;
        }
        GetComponent<SpriteRenderer>().sprite = correctSprite;
    }

    override public string GetItemName(int itemID)
    {
        switch (itemID)
        {
            case 0: return "Wooden Sword";
            case 1: return "Stone Sword";
            case 2: return "Iron Sword";
            case 3: return "Diamond Sword";
        }

        return "";
    }
}