using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public int maxSize;
    public int currentSelection = 0;
    public List<Slot> Container = new List<Slot>();
    
    public int AddItem(ItemObject _item, int _amount){
        int left = _amount;


        if((_item.GetType() == typeof(ToolObject) || _item.GetType() == typeof(BackPackObject) && !_item.name.Contains("(Clone)"))) _item = Instantiate(_item);
        //_item = CreateInstance(_item.GetType()) as ItemObject;
        Debug.Log(_item.GetType());


        bool full = fullInventory(_item);
        int hi = hasItem(_item);
        int l = left;
        int i = 0;

        while (!fullInventory(_item))
        {
            if (i == 100)
                break;
            i++;
            if (hasItem(_item) != -1)
                left = Container[hasItem(_item)].AddAmount(_amount);
            else if (!fullInventory(_item))
            {
                Container.Add(new Slot(_item, Mathf.Min(_item.maxStack, left), getNextIndex()));
                left = Mathf.Max(left - _item.maxStack, 0);
            }
            if (left <= 0)
                break;

        }
        return Mathf.Max(left, 0);        
    }


    public void RemoveItem(ItemObject _item, int Amount) {

        int am = Amount;
        foreach (var item in Container)
        {
            if (am == 0 )
                return;
            if (item.item != _item)
                continue;
            int toreduce = Mathf.Min(am,item.amount);

            am -= toreduce;
            item.amount -= toreduce;
        }
    }
    public int AddItem(ItemObject _item, int _amount, int index)
    {
        Debug.Log($"{Container == null}");



        Container.Add(new Slot(_item, _amount, index));

        
        return 0;
    }
    public int getNextIndex(){
        List<int> indexes = new List<int>();
        foreach (var slot in Container)
        {
            indexes.Add(slot.index);
        }
        for (int i = 1; i <= maxSize; i++)
        {
            if(!indexes.Contains(i))
                return i;
        }
        return -1;
    }
    public int hasItem(ItemObject _item){
        for (int i = 0; i < Container.Count; i++)
        {
            if(Container[i].item == _item && Container[i].amount != Container[i].item.maxStack)
                return i;
        }
        return -1;
    }

    public bool hasItem(ItemObject _item, int count)
    {
        int j = 0;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item.name.Replace("(Clone)", "") == _item.name.Replace("(Clone)",""))
                j += Container[i].amount;

            if(j >= count)
                return true;
        }
        return false;
    }

    public int getFreeSlot(ItemObject itemObject)
    {
        if (hasItem(itemObject) != -1)
            return hasItem(itemObject);
        else
        {
            for (int i = 0; i < maxSize; i++)
            {
                try
                {
                    if (Container[i] == null)
                    {
                        Container[i] = new Slot(itemObject, 1, getNextIndex()); return i;
                    }
                }
                catch { }
            }
        }
        return -1;
    }
    public int canAddItem(ItemObject obj)
    {
        if (Container.Count < maxSize)
            return obj.maxStack * (maxSize-Container.Count);

        int canAdd = 0;
        for (int i = 0; i < maxSize; i++)
        {
            try
            {
                if (Container[i].item == obj && Container[i].amount < obj.maxStack)
                    canAdd += obj.maxStack - Container[i].amount;
            }
            catch { }
        }
        
        return canAdd;
    }
    public bool fullInventory(ItemObject item){
        if (Container.Count < maxSize)
            return false;
        else
        {
            foreach (var slot in Container)
            {
                if (slot.item == item && slot.amount != item.maxStack)
                    return false;
            }
        }
        return true;
    }
    public void manageSelection(float type)
    {
        if(type > 0) //forward (--)
        {
            if(currentSelection == 0)
                currentSelection = maxSize - 1;
            else
                currentSelection--;
        }
        else if(type < 0)
        {
            if(currentSelection == maxSize - 1)
                currentSelection = 0;
            else
                currentSelection++;
        }
    }
    public GameObject getCurrentSlotObject()
    {
        return GameObject.Find("Slots").transform.GetChild(currentSelection).gameObject;
    }
    public Slot getCurrentSlot()
    {
        foreach (var slot in Container)
        {
            if (slot.index == currentSelection + 1)
                return slot;
        }
        return null;
    }
    public ItemObject getCurrentItem()
    {
        foreach (var slot in Container)
        {
            if (slot.index == currentSelection + 1)
                return slot.item;
        }
        return null;
    }
}
[System.Serializable]
public class Slot{
    public ItemObject item;
    public int amount;
    public int index;

    public Slot(ItemObject item, int amount, int index)
    {
        this.item = item;
        this.amount = amount;
        this.index = index;
    }
    public int AddAmount(int value){
        int toAdd = Mathf.Min(value, this.item.maxStack - this.amount);
        this.amount = Mathf.Min(this.amount + value, this.item.maxStack);
        return value - toAdd;
    }
}
