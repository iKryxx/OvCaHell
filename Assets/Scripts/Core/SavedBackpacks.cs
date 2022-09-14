using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SavedBackpacks
{
    public static List<BackPackObject> backPacks = new List<BackPackObject>();


    public static void AddToBackPack(this BackPackObject BP)
    {
        backPacks.Add(BP);
    }

    public static int GetID(this BackPackObject obj)
    {
        if(obj.HasID() != -1)
            return backPacks[obj.HasID()].id;
        else
            return SmallestID();
    }



    static int HasID(this BackPackObject obj)
    {
        foreach (var item in backPacks)
        {
            if(item == obj)
                return backPacks.IndexOf(item);
        }
        return -1;
    }
    static int SmallestID()
    {
        List<int> list = new List<int>();
        foreach (var item in backPacks)
        {
            list.Add(item.id);
        }

        if (list.Count == 0)
            return 1;
        list.Sort();
        for (int i = 1; i < list[list.Count-1]; i++)
        {
            if(!list.Contains(i))
                return i;
        }
        return list.Count + 1;
    }
}