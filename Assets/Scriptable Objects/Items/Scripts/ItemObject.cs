using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Food,
    Equipment,
    Tool,
    BackPack,
    Default
}
public enum ToolType
{
    Axe,
    Pickaxe,
    Shovel,
    Hoe
}
public enum EquipmentType
{
    Sword,
    Bow,
    Wand
}
public abstract class ItemObject : ScriptableObject
{
    public Texture2D Icon;
    public ItemType type;
    [TextArea(15,20)]
    public string description;
    public int maxStack;
}
