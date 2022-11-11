using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public GameObject GuiParent;
    private void OnMouseOver()
    {
        Vector2 playerPos = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y);
        Vector2 itemPos = new Vector2(transform.position.x, transform.position.y);

        Debug.Log(Vector2.Distance(playerPos, itemPos));

        if (InventoryManager.instance.focus && Vector2.Distance(playerPos, itemPos) <= 22)
            return;

        if(Vector2.Distance(playerPos,itemPos) > 22)
        {
            gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            InventoryManager.instance.focus = false;
            InventoryManager.instance._item = null;
            return;
        }
        var item = gameObject.GetComponent<Item>();
        if (item)
        {
            InventoryManager.instance.focus = true;
            InventoryManager.instance._item = item;
            gameObject.transform.Find("Canvas").gameObject.SetActive(true);

        }


    }
    private void OnMouseExit()
    {
        var item = gameObject.GetComponent<Item>();
        if (item)
        {
            gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            InventoryManager.instance.focus = false;
            InventoryManager.instance._item = null;
        }
    }
}
