using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGen : MonoBehaviour
{
    /*
     * Todo:
     * Entering Houses:
     *  - implement World Saving (OOF)
     *      -> Proper Start Screen (Load World, New World, Delete World, Quit, Options)
     *  - Enter House => Save World, Load House, Load new Scene based on Loaded House
     *  - Leave House => Save House, Load World
     * 
     * Options:
     *  - Rebinding (pretty unnecessary for now, but make it automatic so no manually adding controlls)
     *  - Sounds
     *  - Help (List of all Commands)
     *  - Credits (Mention Helpers, Close Friends! <3 )
     */

    public static VillageGen instance;


    public int chunkSize;
    public int tileSize;

    public int minSize;
    public int maxSize;

    public GameObject Path;
    public List<VillageEnviroment> villageEnvos;

    public List<Village> Villages = new List<Village>();

    public List<House> Houses = new List<House>();

    void Update()
    {
        
    }
    private void Awake()
    {
        instance = this;
        chunkSize = OverworldGeneration.instance.ChunkSize;
    }


    //ONLY TESTING!
    [ContextMenu("Generate")] public void Gen()
    {
        foreach (Transform item in GameObject.Find("Test").transform)
        {
            Destroy(item.gameObject);
            Villages.Clear();
        }
        VillageGeneration(new Vector2Int(-1, 0));
    }


    public void VillageGeneration(Vector2Int ch)
    {

        foreach (var village in Villages)
        {
            if (village.getDistanceTo(ch * 20) < 500)
                return;
        }



        //Initializing Village and starting Position
        Village vill = new Village(Random.Range(minSize, maxSize + 1));
        GameObject cv = new GameObject();
        cv.transform.position = new Vector3(ch.x * 20, ch.y * 20, 0);
        


        (Vector2 coords, List<int> direction) currPos = (ch * chunkSize + new Vector2Int(Random.Range(0, chunkSize), Random.Range(0, chunkSize)), new List<int>());
        VillageCoords currentCoord;
        currentCoord = new VillageCoords((int)currPos.coords.x, (int)currPos.coords.y, villageEnvos[0]);
        vill.coords.Add(currentCoord);
        if (currPos.coords.ToChunkCoords().ToChunk() == null)
            OverworldGeneration.instance.chunks.Add(new Chunk(currPos.coords.ToChunkCoords().x, currPos.coords.ToChunkCoords().y, GetPerlinNoiseBiome.GenerateBiomeAt(currPos.coords.ToChunkCoords())));
        Villages.Add(vill);

        //Procedurally Generating Village via Loop
        for (int i = 0; i < vill.size; i++)
        {
            //checking if no Directions are available
            int j = 0;
            while (true)
            {
                if (j == 1000)
                { Debug.Log("Stack");break; }
                j ++;
                if (vill.getRandomFreeCoord(currPos.coords).coords == currPos.coords)
                {
                    currentCoord = vill.coords[Random.Range(0, vill.coords.Count)];
                    currPos.coords = new Vector2(currentCoord.x, currentCoord.y);
                    if (currPos.coords.ToChunkCoords().ToChunk() == null)
                        OverworldGeneration.instance.chunks.Add(new Chunk(currPos.coords.ToChunkCoords().x, currPos.coords.ToChunkCoords().y, GetPerlinNoiseBiome.GenerateBiomeAt(currPos.coords.ToChunkCoords())));
                }
                else
                    break;
            }
            //normal Generation:
            if (Random.Range(0, 13) < 6)
            {
                (Vector2 c, int d) currGen = vill.getRandomFreeCoord(currPos.coords);
                currPos.coords = currGen.c;
                currentCoord = new VillageCoords((int)currPos.coords.x, (int)currPos.coords.y, villageEnvos[0]);
                currentCoord.AddDir(currGen.d);
                vill.coords.Add(currentCoord);

                if (currPos.coords.ToChunkCoords().ToChunk() == null)
                    OverworldGeneration.instance.chunks.Add(new Chunk(currPos.coords.ToChunkCoords().x, currPos.coords.ToChunkCoords().y, GetPerlinNoiseBiome.GenerateBiomeAt(currPos.coords.ToChunkCoords())));
            }
            //Split Generation
            else
            {
                currentCoord = vill.coords[Random.Range(0, vill.coords.Count)];
                currPos.coords = new Vector2(currentCoord.x, currentCoord.y);

                (Vector2 c, int d) currGen = vill.getRandomFreeCoord(currPos.coords);
                currPos.coords = currGen.c;
                currentCoord = new VillageCoords((int)currPos.coords.x, (int)currPos.coords.y, villageEnvos[0]);
                currentCoord.AddDir(currGen.d);
                vill.coords.Add(currentCoord);

                if (currPos.coords.ToChunkCoords().ToChunk() == null)
                    OverworldGeneration.instance.chunks.Add(new Chunk(currPos.coords.ToChunkCoords().x, currPos.coords.ToChunkCoords().y, GetPerlinNoiseBiome.GenerateBiomeAt(currPos.coords.ToChunkCoords())));

            }


        }

        //Generating Houses
        foreach (var coord in vill.coords)
        {
            foreach (var dir in coord.directions)
            {
                List<int> possible = vill.getPossibleDirections(new Vector2(coord.x, coord.y));
                int neededDir = vill.getOppositeDir(dir);
                if (possible.Contains(neededDir) && Random.Range(0, 100) < 75 && neededDir != 2)
                    vill.Objects.Add((new Vector2(coord.x, coord.y), Houses[0].getObjectOfDir(neededDir)));


            }
        }
        

        //Calculating boundaries:
        int min_x = int.MaxValue;
        int min_y = int.MaxValue;
        int max_x = int.MinValue;
        int max_y = int.MinValue;
        foreach (var coord in vill.coords)
        {
            if(coord.x < min_x)
                min_x = coord.x;
            else if(coord.x > max_x)
                max_x = coord.x;
            if(coord.y < min_y)
                min_y = coord.y;
            else if(coord.y > max_y)
                max_y = coord.y;
        }
        vill.Bounderies.min = new Vector2(min_x, min_y);
        vill.Bounderies.max = new Vector2(max_x, max_y);
        vill.setMidPoint();
        cv.name = $"Village: {vill.MidPoint.x} {vill.MidPoint.y}";
        cv.transform.parent = GameObject.Find("Villages").transform;
        vill.setCollider();



        //Instantiating the Paths
        foreach (var c in vill.coords)
        {
            foreach (var dir in c.directions)
            {
                //Debug.Log($"{c.x} {c.y}: {dir}");
                GameObject curr = Instantiate(c.usedEnvo.prefab);

                if (dir == 0)
                {
                    curr.transform.GetChild(0).localScale = new Vector3(1, tileSize + 1f, 1);
                    curr.transform.position = new Vector3(c.x, c.y, 0);
                }
                else if (dir == 1)
                {
                    curr.transform.GetChild(0).localScale = new Vector3(1, tileSize + 1f, 1);
                    curr.transform.position = new Vector3(c.x, c.y, 0);
                    curr.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (dir == 2)
                {
                    curr.transform.GetChild(0).localScale = new Vector3(1, tileSize + 1f, 1);
                    curr.transform.position = new Vector3(c.x, c.y, 0);
                    curr.transform.rotation = Quaternion.Euler(0, 0, -180);

                }
                else if (dir == 3)
                {
                    curr.transform.GetChild(0).localScale = new Vector3(1, tileSize + 1f, 1);
                    curr.transform.position = new Vector3(c.x, c.y, 0);
                    curr.transform.rotation = Quaternion.Euler(0, 0, 90);

                }
                else
                {
                    Destroy(curr);
                }
                curr.transform.parent = cv.transform;
            }
        }
        //Instantiating Houses
        foreach (var house in vill.Objects)
        {
            GameObject curr = Instantiate(house.obj);
            curr.transform.position = new Vector3(house.pos.x, house.pos.y, 0) + (curr.transform.position - curr.transform.Find("offset").position);
        }
    }
}

[System.Serializable]public class House
{
    public string name;
    public GameObject front;
    public GameObject left;
    public GameObject right;

    public GameObject getObjectOfDir(int dir)
    {

        Debug.Log(dir);
        switch (dir)
        {
            case 0:
                return front;
            case 1:
                return left;
            case 3:
                return right;
            default:
                return null;
        }
    }
}

[System.Serializable]public class VillageEnviroment
{
    public EnvoType type;
    public GameObject prefab;
}

public class VillageCoords
{
    public int x;
    public int y;
    public List<int> directions = new List<int>();

    public VillageEnviroment usedEnvo;

    public VillageCoords(int x, int y, VillageEnviroment usedEnvo, int direction)
    {
        this.x = x;
        this.y = y;
        this.usedEnvo = usedEnvo;
        this.directions.Add(direction);
    }
    public VillageCoords(int x, int y, VillageEnviroment usedEnvo)
    {
        this.x = x;
        this.y = y;
        this.usedEnvo = usedEnvo;
    }
    public void AddDir(int direction)
    {
        this.directions.Add(direction);
    }
}
[System.Serializable]public class Village
{
    public List<VillageCoords> coords = new List<VillageCoords>();
    public int size;
    public (Vector2 min, Vector2 max) Bounderies = (Vector2.zero, Vector2.zero);
    public Vector2 MidPoint = Vector2.zero;

    public List<(Vector2 pos, GameObject obj)> Objects = new List<(Vector2 pos, GameObject obj)> ();

    public (Vector2 coords, int direction) getRandomFreeCoord(Vector2 currentPos) {
        int tileSize = VillageGen.instance.tileSize;
        List<int> possible = getPossibleDirections(currentPos);
        
        int i;
        if (possible.Count == 0)
            i = -1;
        else
            i =  possible[Random.Range(0, possible.Count)];

        switch (i)
        {
            case 0: return (currentPos + new Vector2Int(0, tileSize),2);
            case 1: return (currentPos + new Vector2Int(tileSize, 0),3);
            case 2: return (currentPos + new Vector2Int(0, -tileSize),0);
            case 3: return (currentPos + new Vector2Int(-tileSize, 0),1);
            default:
                //Debug.LogError("cannot get Random Direction!");
                return (currentPos,-1);
        }
    }
    public Vector2 getNewPosition(Vector2 currentPos, int dir)
    {
        int tileSize = VillageGen.instance.tileSize;
        List<int> possible = getPossibleDirections(currentPos);
        switch (dir)
        {
            case 0: return currentPos + new Vector2Int(0, tileSize);
            case 1: return currentPos + new Vector2Int(tileSize, 0);
            case 2: return currentPos + new Vector2Int(0, -tileSize);
            case 3: return currentPos + new Vector2Int(-tileSize, 0);
        }
        return currentPos;
    }
    public List<int> getPossibleDirections(Vector2 currentPos)
    {
        int tileSize = VillageGen.instance.tileSize;
        List<int> possible = new List<int>() { 0, 1, 2, 3 };//N E S W
        foreach (var coord in this.coords)
        {
            if (coord.y == (int)currentPos.y + tileSize && coord.x == currentPos.x)
                possible.Remove(0);
            if (coord.x == (int)currentPos.x + tileSize && coord.y == currentPos.y)
                possible.Remove(1);
            if (coord.y == (int)currentPos.y - tileSize && coord.x == currentPos.x)
                possible.Remove(2);
            if (coord.x == (int)currentPos.x - tileSize && coord.y == currentPos.y)
                possible.Remove(3);

        }
        return possible;
    }

    public float getDistanceTo(Vector2 coords)
    {
        return Mathf.Sqrt(Mathf.Pow(Mathf.Max(coords.x,MidPoint.x)-Mathf.Min(coords.x, MidPoint.x), 2) + Mathf.Pow(Mathf.Max(coords.y, MidPoint.y) - Mathf.Min(coords.y, MidPoint.y), 2));
    }

    public void setMidPoint()
    {
        this.MidPoint = new Vector2((Bounderies.min.x + Bounderies.max.x)/2, (Bounderies.min.y + Bounderies.max.y) / 2);
    }
    public void setCollider()
    {
        GameObject collider = new GameObject();
        CircleCollider2D coll = collider.AddComponent<CircleCollider2D>();
        collider.transform.position = this.MidPoint;
        collider.transform.parent = GameObject.Find($"Village: {this.MidPoint.x} {this.MidPoint.y}").transform;
        coll.radius = Mathf.Max(this.Bounderies.max.x - this.Bounderies.min.x, this.Bounderies.max.y - this.Bounderies.min.y);
        //coll.isTrigger = true;
        collider.layer = LayerMask.NameToLayer("NoEnvoSpawnVillage");


        List<Collider2D> results = new List<Collider2D>();
        if (coll.OverlapCollider(OverworldGeneration.instance.villageFilter, results) > 0)
        {
            foreach (var item in results)
            {
                Debug.Log("this", item.gameObject);
                GameObject.Destroy(item.gameObject);
            }
        }
        Physics2D.IgnoreCollision(coll, GameObject.Find("Character").GetComponent<Collider2D>());
    }

    internal int getOppositeDir(int dir)
    {
        return (dir + 2) % 4;
    }

    public Village(int size)
    {
        this.coords = new List<VillageCoords>();
        this.size = size;
    }
}

public enum EnvoType
{
    House,
    Path
}
