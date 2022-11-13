using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    private void OnMouseOver()
    {
        Vector2 playerPos = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y);
        Vector2 itemPos = new Vector2(transform.position.x, transform.position.y);
        var item = gameObject.GetComponent<Item>();
        if (item)
            ActionManager.instance.SetAction(gameObject.GetComponent<SpriteRenderer>().sprite.texture, $"{item.amount}x {item.item.name.Replace("(Clone)", "")}\nPress F to pick up");
        if (InventoryManager.instance.focus && Vector2.Distance(playerPos, itemPos) <= 22)
            return;

        if(Vector2.Distance(playerPos,itemPos) > 22)
        {
            ActionManager.instance.ResetAction();
            InventoryManager.instance.focus = false;
            InventoryManager.instance._item = null;
            return;
        }
        
        if (item)
        {
            InventoryManager.instance.focus = true;
            InventoryManager.instance._item = item;

        }


    }
    private void OnMouseExit()
    {
        var item = gameObject.GetComponent<Item>();
        if (item)
        {
            ActionManager.instance.ResetAction();
            InventoryManager.instance.focus = false;
            InventoryManager.instance._item = null;
        }
    }
}
