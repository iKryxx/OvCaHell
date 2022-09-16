using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();
    public List<Recipe> CTrecipes = new List<Recipe>();

}

[System.Serializable]
public class Recipe
{ 
    public List<ItemAmount> requiredItems = new List<ItemAmount>();
    public ItemObject result;
    public int count;


}


[System.Serializable]
public class ItemAmount
{
    public ItemObject item;
    public int amount;

    public ItemAmount(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
