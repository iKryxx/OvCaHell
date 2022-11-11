using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

using System.Threading;
using static DataManager.DataManagement;
using System.Linq;

public class SaveLoadmanager : MonoBehaviour
{
    public string SaveString;

    [ContextMenu("SaveBP")]
    void SaveBackPacks(World world)
    {
        SaveString = A_D_BackpacksToString();
        PlayerPrefs.SetString(world.getSavePrefix() + "BackPacks",SaveString);
    }

    [ContextMenu("LoadBP")]
    void LoadBackPacks(World world)
    {


        int Amount = 0;
        int index = 0;
        SaveString = PlayerPrefs.GetString(world.getSavePrefix() + "BackPacks","");
        if (SaveString == "" || SaveString == "{}")
            return;
        JArray a = JArray.Parse(SaveString);
        foreach (JObject BackPack in a)
        {
            int id = (int)BackPack["ID"];
            foreach (BackPackObject obj in SavedBackpacks.backPacks)
            {
                if (id != obj.id)
                    continue;
                foreach (JObject item in BackPack["Items"])
                {



                    switch (A_D_stringToItem(item["Item"].ToString().Replace("(Clone)", "")).type)
                    {
                        //case ItemType.Tool:
                        //    break;
                        case ItemType.BackPack:
                            BackPackObject currb = Instantiate((BackPackObject)A_D_stringToItem(item["Item"].ToString().Replace("(Clone)", "")));
                            Amount = (int)item["Amount"];
                            index = (int)item["Index"];
                            obj.Inventory.AddItem(currb, Amount, index);
                            currb.id = (int)item["ItemData"]["ID"];
                            break;

                        default:
                            ItemObject c = A_D_stringToItem(item["Item"].ToString().Replace("(Clone)", ""));
                            Amount = (int)item["Amount"];
                            index = (int)item["Index"];
                            obj.Inventory.AddItem(c, Amount, index);
                            break;
                    }
                }
            }
        }


    }
    [ContextMenu("SaveINV")]
    void SaveInventory(World world)
    {
        SaveString = A_D_InventoryToString();
        PlayerPrefs.SetString(world.getSavePrefix() + "Inventory", SaveString);
    }
    [ContextMenu("LoadINV")]
    void LoadInventory(World world)
    {
        InventoryManager.instance.inventory.Container.Clear();
        SaveString = PlayerPrefs.GetString(world.getSavePrefix() + "Inventory", "");
        if (SaveString == "")
            return;
        int Amount = 0;
        int index = 0;
        JArray a = JArray.Parse(SaveString);

        foreach (JObject Item in a)
        {
            if (A_D_stringToItem(Item["Item"].ToString().Replace("(Clone)", "")) == null)
                continue;
            switch (A_D_stringToItem(Item["Item"].ToString().Replace("(Clone)","")).type)
            {
                //case ItemType.Tool:
                //    break;
                case ItemType.BackPack:
                    BackPackObject currb = Instantiate((BackPackObject)A_D_stringToItem(Item["Item"].ToString().Replace("(Clone)","")));
                    Amount = (int)Item["Amount"];
                    index = (int)Item["Index"];                
                    InventoryManager.instance.inventory.AddItem(currb, Amount, index);
                    currb.id = (int)Item["ItemData"]["ID"];
                    break;

                default:
                    ItemObject c = A_D_stringToItem(Item["Item"].ToString().Replace("(Clone)", ""));
                    Amount = (int)Item["Amount"];
                    index = (int)Item["Index"];
                    InventoryManager.instance.inventory.AddItem(c, Amount, index);
                    break;
            }
        }

        LoadBackPacks(world);
    }

    [ContextMenu("Save World")]
    public void SaveWorld(World world)
    {
       tryAddWorldToList(world);


        SaveString = A_D_WorldToString();
        PlayerPrefs.SetString(world.getSavePrefix()+"World", SaveString);
        SaveString = A_D_PlayerPosToString();
        PlayerPrefs.SetString(world.getSavePrefix() + "Player", SaveString);
    }
    public IEnumerator loadWorld(World world)
    {

        SaveString = PlayerPrefs.GetString(world.getSavePrefix() + "World");

        JArray JA = JArray.Parse(SaveString);
        foreach (JObject cData in JA)
        {
            Chunk chunk = new Chunk((int)cData["x"], (int)cData["y"], cData["biome"].ToString().toBiome());
            chunk.isChunkLoadedFromFile = true;
            
            

            //Enviroment
            
            List<EnviromentObject> objs = OverworldGeneration.instance.envo.enviromentObjects;
            int ChunkSize = OverworldGeneration.instance.ChunkSize;

            foreach (JObject envo in cData["enviroment"])
            {
                EnviromentObject obj = null;

                foreach (var e in objs)
                {
                    if (e.prefab.name == envo["name"].ToString())
                        obj = e;
                }
                int x = (int)envo["x"];
                int y = (int)envo["y"];


                GameObject nin = Instantiate(obj.prefab, new Vector3(x, y, 0), Quaternion.identity);
                if (nin.transform.GetComponentInChildren<CircleCollider2D>() != null)
                {

                    //Debug.Log(nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(filter, results) + " " + nin.transform.position.x + " "+ nin.transform.position.y);

                    if (nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(OverworldGeneration.instance.villageFilter, OverworldGeneration.instance.results) > 0)
                        Destroy(nin);

                }
                else
                {
                    nin.AddComponent<CircleCollider2D>();
                    if (nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(OverworldGeneration.instance.houseFilter, OverworldGeneration.instance.results) > 0)
                        Destroy(nin);
                    else
                        Destroy(nin.GetComponent<CircleCollider2D>());
                }
                OverworldGeneration.instance.RefreshChunks();
                nin.transform.parent = chunk.thisObject.transform.Find("Enviroment");
                if (obj.mineable)
                {
                    nin.GetComponentInChildren<EnvoObject>().setValues(obj.prefab, obj.type, obj.mineable, obj.bestTool, obj.HP, obj.mineableWithFist);
                    nin.GetComponentInChildren<EnvoObject>().setDrops(obj.drops);
                }
                else
                    nin.isStatic = true;

            }
        }
        SaveString = PlayerPrefs.GetString(world.getSavePrefix() + "Player");
        if(SaveString == "")
            yield return null;
        JObject JO = JObject.Parse(SaveString);

        float px = (float)JO["x"]/10;
        float py = (float)JO["y"]/10;
        GameObject.FindGameObjectWithTag("Player").transform.position =new Vector3(px, py, 0);
        yield return null;
    }

    void tryAddWorldToList(World world)
    {
        if (!PlayerPrefs.HasKey("Worlds"))
        {
            PlayerPrefs.SetString("Worlds",world.Name);
            return;
        }

        SaveString = PlayerPrefs.GetString("Worlds");
        string[] list = SaveString.Split(' ');


        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == world.Name)
                return;
        }
        SaveString += " " + world.Name;
        PlayerPrefs.SetString("Worlds", SaveString);
    }



    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SaveInventory(OverworldGeneration.instance.currWorld);
            SaveBackPacks(OverworldGeneration.instance.currWorld);
        }
    }
    public void start()
    {
        LoadInventory(OverworldGeneration.instance.currWorld);
        StartCoroutine(AutoSave());
    }


}
