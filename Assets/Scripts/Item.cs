using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Item : MonoBehaviour
{
    public ItemObject item;
    public int amount;
    [HideInInspector] public TextMeshProUGUI nameTMP;


    public Item(ItemObject item, int amount, Vector2 position)
    {
        this.item = item;
        this.amount = amount;

        Vector2Int pos = position.ToChunkCoords();
        Chunk chunk = position.ToChunkCoords().ToChunk();
        if (chunk == null && !OverworldGeneration.instance.chunksToGenerate.HasChunk(chunk))
            new Chunk(pos.x, pos.y, biome.Null);


        //GameObject curr = Instantiate(SettingsManager.instance.settings.ItemObjectPrefab, new Vector3(position.x,position.y,0), Random.rotation,GameObject.Find("GroundItems").transform);
        GameObject curr = Instantiate(SettingsManager.instance.settings.ItemObjectPrefab, new Vector3(position.x,position.y,-1), Quaternion.Euler(0,0,Random.Range(0,360)),position.ToChunkCoords().ToChunk().thisObject.transform.Find("GroundItems"));

        Item i = curr.AddComponent<Item>();
        i.item = this.item;
        i.amount = this.amount;
        i.nameTMP = curr.transform.Find("Canvas").Find("Name").GetComponent<TextMeshProUGUI>();
        curr.transform.Find("Canvas").transform.rotation = new Quaternion(0,0,0,-curr.transform.rotation.z);

        curr.GetComponent<SpriteRenderer>().sprite = Sprite.Create(item.Icon,new Rect(new Vector2(0,0),new Vector2(66,66)),new Vector2(.5f,.5f),100f);
        curr.transform.localScale = Vector3.one * 6.6f;
        //curr.GetComponent<SpriteRenderer>().material.mainTexture = item.Icon;
    }
    private void Update()
    {
        if(nameTMP != null)
            nameTMP.text = $"{amount}x {item.name.Replace("(Clone)", "")}\nPress F to pick up";

    }
}
static class Items
{
    public static ItemObject returnItemByName(string name)
    {
        string _fixed = name.Replace("_", " ").ToLower();
        foreach(ItemObject obj in SettingsManager.instance.settings.AllItems)
        {
            if (obj.name.ToLower() == _fixed)
                return obj;
        }
        return null;
    }

    public static string getItemData(this ItemObject item)
    {
        var tobj = item as ToolObject;
        var eobj = item as EquipmentObject;
        var fobj = item as FoodObject;
        var dobj = item as DefaultObject;
        var bobj = item as BackPackObject;

        string data = "{";
        if (tobj)
        {
            data +=
                $"\"name\": \"{item.name.Replace("_", " ").Replace("(Clone)", "")}\"," +
                $"\"description\": \"{item.description}\"," +
                $"\"type\": \"{item.type.ToString()}\"," +
                $"\"max stack\": \"{item.maxStack}\"," +
                $"\"tool type\": \"{tobj.toolType}\"," +
                $"\"tool strength\": \"{tobj.toolStrength}\"";
        } else if (eobj)
        {
            data +=
                $"\"name\": \"{item.name.Replace("_", " ").Replace("(Clone)", "")}\"," +
                $"\"description\": \"{item.description}\"," +
                $"\"type\": \"{item.type.ToString()}\"," +
                $"\"max stack\": \"{item.maxStack}\"," +
                $"\"equipment type\": \"{eobj.equipType}\"," +
                $"\"defense bonus\": \"{eobj.defBonus}\"," +
                $"\"mana bonus\": \"{eobj.manaBonus}\"," +
                $"\"attack bonus\": \"{eobj.atkBonus}\"," +
                $"\"speed bonus\": \"{eobj.speedBonus}\"";
        } else if (fobj)
        {
            data +=
                $"\"name\": \"{item.name.Replace("_", " ").Replace("(Clone)", "")}\"," +
                $"\"description\": \"{item.description}\"," +
                $"\"type\": \"{item.type.ToString()}\"," +
                $"\"max stack\": \"{item.maxStack}\"," +
                $"\"restored health\": \"{fobj.restoreHealth}\"," +
                $"\"restored hunger\": \"{fobj.restoreHunger}\"";
        } else if (dobj)
        {
            data +=
                $"\"name\": \"{item.name.Replace("_", " ").Replace("(Clone)", "")}\"," +
                $"\"description\": \"{item.description}\"," +
                $"\"type\": \"{item.type.ToString()}\"," +
                $"\"max stack\": \"{item.maxStack}\"";
        } else if (bobj)
        {
            data +=
                $"\"name\": \"{item.name.Replace("_", " ").Replace("(Clone)", "")}\"," +
                $"\"description\": \"{item.description}\"," +
                $"\"ItemData\":" +
                $"{{" +
                $"\"ID\":{bobj.id}" +
                $"}}";
        }
        data += "}";
        return data;
    }
}
