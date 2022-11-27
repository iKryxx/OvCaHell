using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

using static DataManager.DataManagement;
using System.Linq;

public class SaveLoadmanager : MonoBehaviour
{
    public string SaveString;
    public SaveLoadmanager instance;

    [ContextMenu("SaveBP")]
    void SaveBackPacks(World world)
    {
        SaveString = A_D_BackpacksToString();
        SaveStates.SetKey(world.getSavePrefix() + "/BackPacks",SaveString);
    }

    [ContextMenu("LoadBP")]
    void LoadBackPacks(World world)
    {


        int Amount = 0;
        int index = 0;
        SaveStates.GetKey(world.getSavePrefix() + "/BackPacks",out var SaveString);
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
        SaveStates.SetKey(world.getSavePrefix() + "/Inventory", SaveString);
    }
    [ContextMenu("LoadINV")]
    void LoadInventory(World world)
    {
        InventoryManager.instance.inventory.Container.Clear();
        SaveStates.GetKey(world.getSavePrefix() + "/Inventory", out var SaveString);
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

        foreach (var ch in OverworldGeneration.instance.allChunks.Values)
            ch.Save();


        SaveString = A_D_PlayerPosToString();
        SaveStates.SetKey(world.getSavePrefix() + "/Player", SaveString);
    }
    public IEnumerator loadWorld(World world)
    {
        SaveString = SaveStates.GetKey(world.getSavePrefix() + "/Player");
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
        if (!SaveStates.GetKey("/Worlds",out var s))
        {
            SaveStates.SetKey("/Worlds",world.Name);
            return;
        }

        SaveString = SaveStates.GetKey("/Worlds");
        string[] list = SaveString.Split(' ');


        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == world.Name)
                return;
        }
        SaveString += " " + world.Name;
        SaveStates.SetKey("/Worlds", SaveString);
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
