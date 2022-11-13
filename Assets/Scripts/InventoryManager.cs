using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataManager.DataManagement;

public class InventoryManager : MonoBehaviour
{
    public TextMeshProUGUI currentItemText;
    public InventoryObject inventory;
    public static InventoryManager instance;

    private void Awake(){instance = this;}

    public ItemObject currentItem;
    public GameObject holdingItem;
    public bool focus = false;
    public Item _item = null;
    int _i = 0;
    public EnviromentManager enviromentManager;
    void ItemCollect()
    {
        /*if (focus)
            return;
        if (_i != 1)
        {
            _i++;
            return;
        }
        _i = 0;
        var item = other.GetComponent<Item>();
        if(item){
            focus = true;
            _item = item;
            other.gameObject.transform.Find("Canvas").gameObject.SetActive(true);

        }*/
        return;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            focus = false;
            _item = null;
        }
    }
    void OnApplicationQuit()
    {
        inventory.Container.Clear();
        inventory.currentSelection = 0;
    }
    private void Update() {
        currentItem = inventory.getCurrentItem();
        if (currentItem)
        {
            currentItemText.gameObject.SetActive(true);
            currentItemText.text = currentItem.name.Replace("_", " ").Replace("(Clone)", "");
        }
        else
            currentItemText.gameObject.SetActive(false);

        ItemCollect();
        showTool();
        Slot temp = null;
        foreach (var slot in inventory.Container)
        {
            if(slot.amount == 0)
                temp = slot;
        }
        if(temp != null)inventory.Container.Remove(temp);

        List<int> used = new List<int>();
        foreach (var slot in inventory.Container)
        {
            used.Add(slot.index);
            Transform parent = GameObject.Find("Slots").transform.GetChild(slot.index - 1);
            parent.Find("Icon").gameObject.SetActive(true);
            parent.Find("Item Amount").gameObject.SetActive(true);
            parent.Find("Icon").GetComponent<RawImage>().texture = slot.item.Icon;
            parent.Find("Item Amount").GetComponent<TextMeshProUGUI>().text = $"{slot.amount}";
        }
        for (int i = 1; i <= inventory.maxSize; i++)
        {
            Transform parent = GameObject.Find("Slots").transform.GetChild(i - 1);
            parent.Find("Selection").gameObject.SetActive(false);
            if (!used.Contains(i))
            {
                parent.Find("Icon").gameObject.SetActive(false);
                parent.Find("Item Amount").gameObject.SetActive(false);
            }
        }
        inventory.getCurrentSlotObject().transform.Find("Selection").gameObject.SetActive(true);

    }
    void showTool()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        holdingItem.transform.parent.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        ItemObject item = inventory.getCurrentItem();
        if (item == null)
        {
            holdingItem.GetComponent<SpriteRenderer>().color = new Color(255,255,255,0); return;
        }
        ItemType type = item.type;
        if (type != ItemType.Tool && type != ItemType.Equipment)
        {
            holdingItem.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0); return;
        }

        holdingItem.GetComponent<SpriteRenderer>().sprite = Sprite.Create(item.Icon, new Rect(new Vector2(0, 0), new Vector2(66, 66)), new Vector2(.5f, .5f), 15f);
        holdingItem.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);

    }

    [HideInInspector] public bool shift = false;

    public void tryPickUp()
    {
        if (_item != null)
        {
            
            int rem = 0;
            if ((A_D_getCurrentItem() != null && A_D_getCurrentItem().GetType() == typeof(BackPackObject)))
            {
                BackPackObject obj = A_D_getCurrentItem() as BackPackObject;
                rem = obj.Inventory.AddItem(_item.item, _item.amount);
            }
            else
                rem = inventory.AddItem(_item.item, _item.amount);
            if (rem > 0)
            {
                _item.amount = rem;
            }
            else Destroy(_item.gameObject);
        }
    }
    
}
