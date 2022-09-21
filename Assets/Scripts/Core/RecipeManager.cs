using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RecipeManager : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();
    public List<Recipe> CTrecipes = new List<Recipe>();

    public GameObject Prefab;
    public GameObject ReqPrefab;

    public List<RecipeObject> recipeObjects = new List<RecipeObject>();

    public bool shouldDisplay;
    public GameObject RecipeParent;

    private void Update()
    {
        

        if(shouldDisplay)
            displayRecipes();
        else
            RecipeParent.SetActive(false);
    }


    void displayRecipes()
    {
        RecipeParent.SetActive(true);
        GLOBAL.ISINPUTBLOCKED = true;

        foreach (var recipe in recipes)
        {
            bool sd = true;
            foreach (var item in recipe.requiredItems)
            {
                if (!InventoryManager.instance.inventory.hasItem(item.item, item.amount))
                    sd = false;

            }

            if (sd && !recipeObjects.ContainsRecipe(recipe))
                AddRecipeButton(recipe);

            else if(!sd)
                RemoveRecipeButton(recipe);
        }
    }

    private void RemoveRecipeButton(Recipe recipe)
    {
        if(!recipeObjects.ContainsRecipe(recipe))
            return;
        foreach (var rec in recipeObjects)
        {
            if (rec.Recipe == recipe)
            {
                Destroy(rec.RecipeSlot);
                recipeObjects.Remove(rec);
                return;
            }

        }
    }

    void AddRecipeButton(Recipe recipe)
    {
        GameObject curr = Instantiate(Prefab, GameObject.Find("Recipes").transform);

        curr.transform.Find("Item").GetComponent<TextMeshProUGUI>().text = recipe.result.name.Replace("_", " ");
        curr.transform.Find("Icon").GetComponent<RawImage>().texture = recipe.result.Icon;
        curr.name = recipe.result.name.Replace("_", " ");

        foreach (var item in recipe.requiredItems)
        {
            GameObject curi = Instantiate(ReqPrefab, curr.transform.Find("Req"));
            curi.transform.GetComponent<RawImage>().texture = item.item.Icon;
        }
        List<GameObject> list = new List<GameObject>();
        foreach (Transform t in curr.transform.Find("Req"))
        {
            list.Add(t.gameObject);
        }

        RecipeObject c = new RecipeObject(curr, list, recipe);
        recipeObjects.Add(c);
        c.SetText();
    }


    public void craft(GameObject curr)
    {
        if (InventoryManager.instance.inventory.canAddItem(curr.toRecipe(recipeObjects).result) < curr.toRecipe(recipeObjects).count)
            return;

        foreach (var item in curr.toRecipe(recipeObjects).requiredItems)
        {
            InventoryManager.instance.inventory.RemoveItem(item.item, item.amount);
        }
        InventoryManager.instance.inventory.AddItem(curr.toRecipe(recipeObjects).result, curr.toRecipe(recipeObjects).count);


    }


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


public class RecipeObject
{
    public GameObject RecipeSlot;
    public List<GameObject> ItemSlots;
    public Recipe Recipe;

    public RecipeObject(GameObject recipeSlot, List<GameObject> itemSlots, Recipe recipe)
    {
        RecipeSlot = recipeSlot;
        ItemSlots = itemSlots;
        Recipe = recipe;
    }

    public void SetText(int multiplier = 1)
    {
        for (int i = 0; i < Recipe.requiredItems.Count; i++)
        {
            Debug.Log($"{Recipe.requiredItems[i].amount * multiplier}");
            ItemSlots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{Recipe.requiredItems[i].amount * multiplier}";
            RecipeSlot.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{Recipe.count * multiplier}";
        }
    }

    
}
public static class RecipeExtensions
{
    public static bool ContainsRecipe(this List<RecipeObject> obj, Recipe rec)
    {
        foreach (var recipe in obj)
        {
            if (recipe.Recipe == rec)
                return true;
        }
        return false;
    }

    public static Recipe toRecipe(this GameObject curr, List<RecipeObject> objs)
    {
        foreach (var item in objs)
        {
            if (item.RecipeSlot == curr)
                return item.Recipe;
        }
        return null;
    }
}
