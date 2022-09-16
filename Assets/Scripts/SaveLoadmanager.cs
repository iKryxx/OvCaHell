using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

using System.Threading;
using static DataManager.DataManagement;

public class SaveLoadmanager : MonoBehaviour
{
    public string SaveString;

    [ContextMenu("SaveBP")]
    void SaveBackPacks()
    {
        SaveString = A_D_BackpacksToString();
        PlayerPrefs.SetString("BackPacks",SaveString);
    }

    [ContextMenu("LoadBP")]
    void LoadBackPacks()
    {


        int Amount = 0;
        int index = 0;
        SaveString = PlayerPrefs.GetString("BackPacks","");
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
    void SaveInventory()
    {
        SaveString = A_D_InventoryToString();
        PlayerPrefs.SetString("Inventory", SaveString);
    }
    [ContextMenu("LoadINV")]
    void LoadInventory()
    {
        SaveString = PlayerPrefs.GetString("Inventory", "");
        if (SaveString == "")
            return;
        int Amount = 0;
        int index = 0;
        JArray a = JArray.Parse(SaveString);

        foreach (JObject Item in a)
        {
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

        LoadBackPacks();
    }



    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SaveInventory();
            SaveBackPacks();
        }
    }
    private void Awake()
    {
        LoadInventory();
        StartCoroutine(AutoSave());
    }
}
