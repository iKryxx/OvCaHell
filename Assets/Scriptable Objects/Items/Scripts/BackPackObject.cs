using UnityEngine;

[CreateAssetMenu(fileName = "New Backpack Object", menuName = "Inventory System/Items/Backpack")]
public class BackPackObject : ItemObject
{
    /*[System.NonSerialized]*/ public InventoryObject Inventory;
    public int size;
    public int id = -1;
    //public bool HASINIT = false;

    private void Awake()
    {
        
        type = ItemType.BackPack;

        id = this.GetID();
        this.AddToBackPack();

        Inventory = new InventoryObject();
        Inventory.maxSize = size;


        /*if (id == 1) { 
            Inventory.AddItem(Items.returnItemByName("Wood"), 1); 
            Inventory.AddItem(Items.returnItemByName("Wooden_Axe"), 2); 
            Inventory.AddItem(Items.returnItemByName("Apple"), 3);
            Inventory.AddItem(Items.returnItemByName("Wooden_Axe"), 2);
        }*/
        /*if (id == 2)
        {
            Inventory.AddItem(Items.returnItemByName("Wood"), 4);
            Inventory.AddItem(Items.returnItemByName("Wooden_Axe"), 5);
            Inventory.AddItem(Items.returnItemByName("Apple"), 6);
            Inventory.AddItem(Items.returnItemByName("Wooden_Axe"), 2);
        }*/
    }
}
