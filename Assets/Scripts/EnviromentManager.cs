using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{
    public GameObject player;
    public List<EnviromentObject> enviromentObjects = new List<EnviromentObject>();
    int i;
    
    void AddStart()
    {
        Item item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(5, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(7, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(9, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(11, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(13, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(15, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(17, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(19, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(21, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(23, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(25, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(27, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(29, 0));
        item = new Item(Items.returnItemByName("Wood"), 7, new Vector2(31, 0));
        item = new Item(Items.returnItemByName("Wooden Axe"), 1, new Vector2(33, 0));


        GameObject nin = Instantiate(enviromentObjects[0].prefab,new Vector3(20,20,0),Quaternion.identity);
        nin.GetComponentInChildren<EnvoObject>().setValues(enviromentObjects[0].prefab, enviromentObjects[0].type, enviromentObjects[0].mineable, enviromentObjects[0].bestTool, enviromentObjects[0].HP, enviromentObjects[0].mineableWithFist);
        nin.GetComponentInChildren<EnvoObject>().setDrops(enviromentObjects[0].drops);
        nin.transform.parent = ChunkInfo.ToChunk(ChunkInfo.ToChunkCoords(new Vector2(20, 20))).thisObject.transform.Find("Enviroment"); 
    }
    public void DropItem(Slot slot)
    {
        Item closest = GetClosestItem(Physics2D.BoxCastAll(player.transform.position, new Vector2(5.0f, 5.0f), 45, new Vector2()), player.transform, slot.item.name);
        if(closest != null)
        {
            closest.amount += 1;
        }
        else
            new Item(slot.item, 1, player.transform.position + new Vector3(Random.Range(-2.4f,2.4f), Random.Range(-2.4f, 2.4f)));

        slot.amount -= 1;
    }
    public void DropAll(Slot slot)
    {        
        
        new Item(slot.item, slot.amount, player.transform.position + new Vector3(Random.Range(-2.4f, 2.4f), Random.Range(-2.4f, 2.4f)));

        slot.amount = 0;
    }


    Item GetClosestItem(RaycastHit2D[] itemObjects, Transform fromThis, string name)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < itemObjects.Length; i++)
        {
            if (itemObjects[i].transform.TryGetComponent<Item>(out Item iss) && iss.item.name == name && iss.amount < iss.item.maxStack)
                list.Add(itemObjects[i].transform);
        }

        if (list.Count == 0)
            return null;
        Item bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = fromThis.position;
        foreach (Transform potentialTarget in list)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.GetComponent<Item>();
            }
        }
        return bestTarget;
    }
    void Update()
    {
        if (i < 1)
            i++;
        else if (i == 1)
        {
            i++;
            AddStart();
        }
    }
}
public enum EnviromentType
{
    Tree,
    Stone,
    Flower
}
[System.Serializable]
public class EnviromentObject{
    public GameObject prefab;
    public EnviromentType type;
    public List<Drop> drops = new List<Drop>();
    public bool mineable;
    public bool mineableWithFist;
    public ToolType bestTool;
    public float HP;
}

public static class EnviromentInfo
{
    public static List<EnviromentObject> getObjectOfType(this List<EnviromentObject> obj, EnviromentType type){
        List<EnviromentObject> enviromentObject = new List<EnviromentObject>();
;
        foreach (var env in obj)
        {

            if(env.type == type)
                enviromentObject.Add(env);
        }
        return enviromentObject;
    }
}


[System.Serializable]
public class Drop
{
    public ItemObject Item;
    public int min_Amount;
    public int max_Amount;
    public bool guaranteed;
    public int chance;


    [HideInInspector] public int Amount;
}

