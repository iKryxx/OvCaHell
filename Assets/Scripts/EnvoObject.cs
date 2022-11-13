using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BasicExtensions;

public class EnvoObject : MonoBehaviour
{
    private bool shouldShake = false;
    private Vector3 startingpos;
    public EnviromentObject thisObject;
    public Transform GuiParent;
    float originalHP;
    float size;
    float _HP;

    public void setValues(GameObject prefab, EnviromentType type, bool mineable, ToolType bestTool, float hP, bool mineableWithFist)
    {
        thisObject.prefab = prefab;
        thisObject.type = type;
        thisObject.mineable = mineable;
        thisObject.bestTool = bestTool;
        thisObject.HP = hP;
        thisObject.mineableWithFist = mineableWithFist;

        originalHP = hP;
        size = transform.localScale.x;
        _HP = hP;
        startingpos = thisObject.prefab.transform.position;
    }
    public void setDrops(List<Drop> drops)
    {
        foreach(Drop d in drops)
        {
            if (d.guaranteed == true || d.chance == 100) {
                Drop drop = new Drop();
                drop.Amount = Random.Range(d.min_Amount, d.max_Amount + 1);
                drop.Item = d.Item;
                thisObject.drops.Add(drop); 
            }
            else
            {
                if (Random.Range(0, 100) < d.chance)
                {
                    Drop drop = new Drop();
                    drop.Amount = Random.Range(d.min_Amount, d.max_Amount + 1);
                    drop.Item = d.Item;
                    thisObject.drops.Add(drop);
                }
            }
        }
    }


    private void Update()
    {
        if (_HP != thisObject.HP)
        {
            StartCoroutine("Shake");
            _HP = thisObject.HP;
        }
        if (shouldShake)
        {
            thisObject.prefab.transform.position = startingpos + new Vector3(Mathf.Sin(Time.time * 1.0f) * 1.0f, Mathf.Sin(Time.time * 40.0f) * .25f, 0);
        }

        if(thisObject.HP <= 0)
        {
            
            foreach (var drop in thisObject.drops)
            {
                float size = 20;

                Vector2 rpoc = transform.position + new Vector3(Random.Range(-size, size), Random.Range(-size, size), 0);

                Destroy(transform.parent.gameObject);
                Item item = new Item(drop.Item, drop.Amount, rpoc);
            }
            
        }
    }

    private IEnumerator Shake()
    {
        shouldShake = true;
        yield return new WaitForSeconds(0.2f);
        shouldShake = false;
        thisObject.prefab.transform.position = startingpos;
    }

    public static Vector3 RandomPointOnUnitCircle(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;

        return new Vector3(x, y, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ToolObject currentItem = null;
        if (InventoryManager.instance.currentItem != null)
        {
            try
            {
                currentItem = (ToolObject)InventoryManager.instance.currentItem;
            }
            catch { currentItem = null;  }
        }

        //currentItem = (ToolObject)InventoryManager.instance.currentItem;


        //Debug.Log(collision.tag != "Holding" || !PlayerMovement.instance.triggerInput || (currentItem == null && !thisObject.mineableWithFist));

        if (collision.tag != "Holding" || !PlayerMovement.instance.triggerInput || (currentItem == null && !thisObject.mineableWithFist))
        {
            if (currentItem == null)
                PlayerMovement.instance.triggerInput = false; 
            return; 
        }

        if (currentItem == null)
        {
            thisObject.HP -= 1;
            //float newSize = Mathf.Lerp(size / 2, size, size * (thisObject.HP / originalHP));
            //transform.localScale = new Vector3(newSize, newSize, 1);
            PlayerMovement.instance.triggerInput = false;
        }
        else if (thisObject.bestTool == currentItem.toolType && AnimationManager.instance.anim.isPlaying)
        {
            thisObject.HP -= currentItem.toolStrength;
            //float newSize = Mathf.Lerp(size / 2, size, size * (thisObject.HP / originalHP));
            //transform.localScale = new Vector3(newSize, newSize, 1);
            PlayerMovement.instance.triggerInput = false;
        }
    }

    private void OnMouseOver()
    {
        string text = $"{thisObject.prefab.name.toFormat()}\n Attack to damage";
        
        Texture2D tex = thisObject.prefab.transform.Find("Image").GetComponent<SpriteRenderer>().sprite.texture;
        ActionManager.instance.SetAction(tex, text, originalHP, thisObject.HP);
        
    }
    private void OnMouseExit()
    {
        ActionManager.instance.ResetAction();
    }

}
