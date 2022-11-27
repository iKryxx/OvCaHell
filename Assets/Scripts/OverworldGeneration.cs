using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtensions;
using System.Linq;
using static DataManager.DataManagement;

public class OverworldGeneration : MonoBehaviour
{
    public static OverworldGeneration instance;
    public EnviromentManager envo;
    public SaveLoadmanager SaveLoad;

    public LayerMask forbiddenVillage;
    public LayerMask forbiddenHouse;

    public Dictionary<Vector2, Chunk> chunksToGenerate = new Dictionary<Vector2, Chunk>();
    public Dictionary<Vector2, string> chunksToRegenerate = new Dictionary<Vector2, string>();

    //WorldSettings
    public World currWorld;
    public int chunksPerFrame;
    public int BiomeSize;
    bool LockWorldSize = true;
    public int ChunkSize = 20;
    public GameObject ChunkParent;
    public Dictionary<Vector2,Chunk> allChunks = new Dictionary<Vector2,Chunk>();
    public Dictionary<Vector2,Chunk> activeChunks = new Dictionary<Vector2,Chunk>();
    public GameObject player;
    Chunk currentChunk;
    Chunk _currentChunk;
    public int renderDistance;
    public bool resetWorld;

    //Sprites
    public GameObject prefab;

    public BiomeTypes OverworldBiomes;

    //Village gen
    public float frequency; //the lower the less stretched hills
    public float threshhold; //the lower the fewer hills

    //Colliders for anti Enviroment Overlapping
    public ContactFilter2D villageFilter = new ContactFilter2D();
    public ContactFilter2D houseFilter = new ContactFilter2D();
    public List<Collider2D> results = new List<Collider2D>();

    private void Awake()
    {

        instance = this;
        
        villageFilter.SetLayerMask(forbiddenVillage);
        houseFilter.SetLayerMask(forbiddenHouse);
    }
    void Start()
    {
        if (crossSceneVariables.World != "")
            currWorld = crossSceneVariables.World.stringToWorld();
        else
            currWorld = "Debug".stringToWorld();
        if (SaveStates.GetKey(currWorld.getSavePrefix() + "/Player", out var s) && !resetWorld)
            StartCoroutine(SaveLoad.loadWorld(currWorld));

        SaveLoad.start();

    }
    void Update()
    {
        //setting current chunk
        Vector2 pos = player.transform.position;
        Vector2Int curr = pos.ToChunkCoords();

        currentChunk = allChunks.GetChunkAt(curr.x, curr.y);

        if(currentChunk == null)
            new Chunk(curr.x, curr.y, biome.Null);
        //Generate new Chunks
        if (currentChunk != null && !currentChunk.hasEntered)
        {
            GenerateNeighbours();
            currentChunk.hasEntered = true;
        }
        //Debug.Log(chunksToRegenerate.Count);
        RegenerateChunks();
        RefreshChunks();
        //Hide other Chunks
        if (_currentChunk != currentChunk)
        {
            Dictionary<Vector2, Chunk> newActive = new Dictionary<Vector2, Chunk>();
            for (int x = curr.x - renderDistance ; x <= curr.x + renderDistance ; x++)
            {
                
                for (int y = curr.y - renderDistance ; y <= curr.y + renderDistance ; y++)
                {
                    if(allChunks.TryGetValue(new Vector2(x,y), out var chunk) && /*!activeChunks.ContainsKey(new Vector2(x, y)) &&*/ chunk != null)
                    {
                        newActive.Add(new Vector2(x, y), chunk);
                    } else if (SaveStates.GetKey(new Vector2(x, y).A_D_GetChunkSavePrefix(),out var s)) chunksToRegenerate.TryAdd(new Vector2(x, y), "");
                }
            }
            List<Vector2> toRemove = new List<Vector2>();
            foreach (var chunk in activeChunks.Values)
            {
                if (currentChunk == null)
                { break; }
                if (Mathf.Abs(Mathf.Max(chunk.x,currentChunk.x) - Mathf.Min(chunk.x, currentChunk.x)) > renderDistance || Mathf.Abs(Mathf.Max(chunk.y, currentChunk.y) - Mathf.Min(chunk.y, currentChunk.y)) > renderDistance)
                {
                    //if (chunk.thisObject == null) Debug.Log($"chunk null at {chunk.x} {chunk.y}");
                   
                    chunk.Save();
                    //Debug.Log("MEEEEEEEEEEEEp   " + GameObject.Find($"{chunk.x} {chunk.y}").name, GameObject.Find($"{chunk.x} {chunk.y}"));
                    Destroy(GameObject.Find($"{chunk.x} {chunk.y}"));
                    if (GameObject.Find($"{chunk.x} {chunk.y}") != null)
                    {
                        
                        //Destroy(GameObject.Find($"{chunk.x} {chunk.y}"));
                        //Debug.Break();
                    }
                    allChunks.Remove(chunk.GetPos());

                }

            }
            activeChunks = newActive;
        }
        _currentChunk = currentChunk;

        if(allChunks.Count > renderDistance * renderDistance)
        {
            Dictionary<Vector2, Chunk> allChunksCopy = new Dictionary<Vector2, Chunk>(allChunks);
            foreach (Chunk chunk in allChunksCopy.Values)
            {
                if (Mathf.Abs(Mathf.Max(chunk.x, currentChunk.x) - Mathf.Min(chunk.x, currentChunk.x)) > renderDistance || Mathf.Abs(Mathf.Max(chunk.y, currentChunk.y) - Mathf.Min(chunk.y, currentChunk.y)) > renderDistance)
                {
                    chunk.Save();
                    Destroy(GameObject.Find($"{chunk.x} {chunk.y}"));
                    allChunks.Remove(chunk.GetPos());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SaveLoad.SaveWorld(currWorld);
    }

    private void GenerateVillage(Vector2Int pos)
    {
        if (Mathf.PerlinNoise(pos.x * frequency/4 + currWorld.Seed, pos.y * frequency/4 + currWorld.Seed) < threshhold)
            VillageGen.instance.VillageGeneration(new Vector2Int(pos.x, pos.y));
    }

    void GenerateNeighbours(){
        //bool allowOpt = chunks.IsSorroundedByChunks(currentChunk.x, currentChunk.y);
        for (int x = -renderDistance + currentChunk.x; x <= renderDistance + currentChunk.x; x++)
        {
            if (-renderDistance + currentChunk.x + 1 > x && renderDistance + currentChunk.x - 1 < x)
                continue;
            for (int y = -renderDistance + currentChunk.y; y <= renderDistance + currentChunk.y; y++)
            {
                if (-renderDistance + currentChunk.y + 1 > y && renderDistance + currentChunk.y - 1 < y)
                    continue;
                if (allChunks.GetChunkAt(x, y) == null && !chunksToGenerate.HasChunk(allChunks.GetChunkAt(x, y)))
                {
                    if (SaveStates.GetKey(new Vector2(x, y).A_D_GetChunkSavePrefix(), out string s))
                        chunksToRegenerate.TryAdd(new Vector2(x, y), "");
                    else
                    {
                        Chunk chunk = new Chunk((int)x, (int)y, GetPerlinNoiseBiome.GenerateBiomeAt(new Vector2(x, y)));
                        chunksToGenerate.TryAdd(new Vector2(x, y), chunk);
                    }
                }
            }
        }


        for (int x = -renderDistance*2 + currentChunk.x; x <= renderDistance * 2 + currentChunk.x; x++)
        {
            if (x > -renderDistance + currentChunk.x && x < renderDistance + currentChunk.x)
                continue;
            for (int y = -renderDistance * 2 + currentChunk.y; y <= renderDistance * 2 + currentChunk.y; y++)
            {
                if (y > -renderDistance + currentChunk.y && y < renderDistance + currentChunk.y)
                    continue;

                //if (Mathf.Abs(x - y) > renderDistance + currentChunk.y + currentChunk.x || Mathf.Abs(x-y) < -renderDistance - currentChunk.y - currentChunk.x)
                //continue;
                //Debug.Log(chunksToGenerate.HasChunk(chunks.GetChunkAt(x, y)));
                if (allChunks.GetChunkAt(x, y) == null && !chunksToGenerate.HasChunk(allChunks.GetChunkAt(x, y)))
                {
                    GenerateVillage(new Vector2Int(x, y));

                }
            }
        }


    }

    //Render Chunks and Generate Enviroment
    public void RefreshChunks(){
        for (int o = 0; o <= Mathf.Min(chunksPerFrame,chunksToGenerate.Count-1); o++)
        {
            Chunk chunk = null;
            if (chunksToGenerate.Count > 0)
            {
                chunk = chunksToGenerate.ElementAt(o).Value;
            }
            else
                return;
            if (allChunks.TryGetValue(new Vector2(chunk.x, chunk.y), out var ch) == true)
            {
                chunksToGenerate.Remove(new Vector2(chunk.x, chunk.y));
                return;
            }
            if (chunksToRegenerate.TryGetValue(new Vector2(chunk.x, chunk.y), out var ch2) == true)
            {
                chunksToGenerate.Remove(new Vector2(chunk.x, chunk.y));
                return;
            }
            GameObject Parent = new GameObject($"{chunk.x} {chunk.y}");
            Parent.transform.position = chunk.GetPos() * ChunkSize;
            Parent.transform.parent = ChunkParent.transform;
            GameObject gItems = new GameObject("GroundItems");
            gItems.transform.parent = Parent.transform;

            GameObject EV = new GameObject("Enviroment");
            EV.transform.parent = Parent.transform;

            biome bi = chunk.biome;
            List<Texture2D> currSprites = ChunkInfo.getCorrectSprites(bi);
            int z = Random.Range(0, currSprites.Count);
            GameObject curr = Instantiate(prefab, new Vector3(chunk.x * ChunkSize + 10, chunk.y * ChunkSize + 10, 1), Quaternion.identity);
            curr.GetComponent<SpriteRenderer>().sprite = Sprite.Create(currSprites[z], new Rect(0, 0, currSprites[z].width, currSprites[z].height), new Vector2(0.5f, 0.5f));
            curr.transform.localScale = new Vector3(100, 100, 1);
            curr.transform.parent = Parent.transform;
            curr.isStatic = true;

            chunk.thisObject = Parent;
            //set chunks positin based of y position, because of 3d effect
            Parent.transform.position += new Vector3(0, 0, 0);

            //Debug.Log(chunk.isChunkLoadedFromFile);


            foreach (var ENV in chunk.toBiome().enviroment)
            {
                if (chunk.x.IsWithin(-5,5) && chunk.y.IsWithin(-5,5) && !chunk.isChunkLoadedFromFile)
                    continue;

                if (chunk.isChunkLoadedFromFile)
                    break;
                if (Random.Range(0, 101) > ENV.chance)
                    continue;

                List<EnviromentObject> objs = envo.enviromentObjects.getObjectOfType(ENV.type);


                //Debug.Log(objs.Count + " " + chunk.biome.ToString());
                if (objs.Count == 0)
                    continue;

                for (int i = 0; i < Random.Range(ENV.min_Amount, ENV.max_Amount + 1); i++)
                {
                    //Debug.Log(objs.Count);
                    EnviromentObject obj = objs[Random.Range(0, objs.Count)];
                    float x = Random.Range(0, ChunkSize + 1) + chunk.x * ChunkSize;
                    float y = Random.Range(0, ChunkSize + 1) + chunk.y * ChunkSize;

                    GameObject nin = Instantiate(obj.prefab, new Vector3(x, y, y / 10000 - 0.0033f), Quaternion.identity);

                    if (obj.mineable)
                    {
                        
                        nin.AddComponent<IgnoreCollisionScript>();
                        
                    }

                    else
                    {
                        nin.AddComponent<PolygonCollider2D>();
                        if (nin.transform.GetComponentInChildren<PolygonCollider2D>().OverlapCollider(houseFilter, results) > 0)
                            Destroy(nin);
                        else
                            Destroy(nin.GetComponent<PolygonCollider2D>());
                    }


                    nin.transform.parent = chunk.thisObject.transform.Find("Enviroment");
                    if (obj.mineable)
                    {
                        nin.GetComponentInChildren<EnvoObject>().setValues(nin, obj.type, obj.mineable, obj.bestTool, obj.HP, obj.mineableWithFist);
                        nin.GetComponentInChildren<EnvoObject>().setDrops(obj.drops);
                    }
                    else
                    {
                        nin.transform.position += new Vector3(0, 0, 0.05f);
                        nin.isStatic = true;
                    }

                }
            }


            chunk.generated = true;

            List<Sprite> sprites = new List<Sprite>();
            foreach (Transform children in Parent.transform)
            {
                if (children.name != "GroundItems" && children.name != "Enviroment")
                    sprites.Add(children.GetComponent<SpriteRenderer>().sprite);
            }

            try
            {
                allChunks.Add(new Vector2(chunk.x, chunk.y), chunk);
            }
            catch (System.ArgumentException) { }
            chunksToGenerate.Remove(new Vector2(chunk.x, chunk.y));
            chunk.enviroment = Parent.transform.A_D_EnviromentOfChunkToEnviroment();
        }
    }
    public void RegenerateChunks(){
        for (int o = 0; o <= Mathf.Min(chunksPerFrame,chunksToRegenerate.Count-1); o++)
        {
            Chunk chunk = null;
            if (chunksToRegenerate.Count > 0)
            {
               // Debug.Log("BING CHILLING");
                chunk = chunksToRegenerate.ElementAt(o).Key.A_D_StringToChunk();
                //Debug.Log((chunk.thisObject == null) + " " + chunk.x + " " + chunk.y);
                if (chunk == null || GameObject.Find($"{chunk.x} {chunk.y}") == true) { chunksToRegenerate.Remove(chunksToRegenerate.ElementAt(o).Key); continue; }
            }
            else
                return;

            

            GameObject Parent = new GameObject($"{chunk.x} {chunk.y}");
            chunk.thisObject = Parent;
            Parent.transform.position = chunk.GetPos() * ChunkSize;
            Parent.transform.parent = ChunkParent.transform;
            Parent.transform.position = chunk.GetPos() * ChunkSize;
            Parent.transform.parent = ChunkParent.transform;
            GameObject gItems = new GameObject("GroundItems");
            gItems.transform.parent = Parent.transform;

            GameObject EV = new GameObject("Enviroment");
            EV.transform.parent = Parent.transform;

            biome bi = chunk.biome;
            List<Texture2D> currSprites = ChunkInfo.getCorrectSprites(bi);
            int z = Random.Range(0, currSprites.Count);
            GameObject curr = Instantiate(prefab, new Vector3(chunk.x * ChunkSize + 10, chunk.y * ChunkSize + 10, 1), Quaternion.identity);
            curr.GetComponent<SpriteRenderer>().sprite = Sprite.Create(currSprites[z], new Rect(0, 0, currSprites[z].width, currSprites[z].height), new Vector2(0.5f, 0.5f));
            curr.transform.localScale = new Vector3(100, 100, 1);
            curr.transform.parent = Parent.transform;
            curr.isStatic = true;




            foreach (var ENV in chunk.enviroment)
            {
                

                //Debug.Log(objs.Count + " " + chunk.biome.ToString());
                float x = ENV.x;
                float y = ENV.y;

                GameObject nin = Instantiate(ENV.envo.prefab, new Vector3(x, y, y / 10000 - 0.0033f), Quaternion.identity);

                if (ENV.envo.mineable)
                {
                        
                    nin.AddComponent<IgnoreCollisionScript>();
                        
                }

                else
                {
                    nin.AddComponent<PolygonCollider2D>();
                    if (nin.transform.GetComponentInChildren<PolygonCollider2D>().OverlapCollider(houseFilter, results) > 0)
                        Destroy(nin);
                    else
                        Destroy(nin.GetComponent<PolygonCollider2D>());
                }

                if(!chunk.thisObject.transform.Find("Enviroment"))
                {
                    GameObject E = new GameObject("Enviroment");
                    E.transform.parent = chunk.thisObject.transform;
                }
                else
                    nin.transform.parent = chunk.thisObject.transform.Find("Enviroment");
                if (ENV.envo.mineable)
                {
                    nin.GetComponentInChildren<EnvoObject>().setValues(nin, ENV.envo.type, ENV.envo.mineable, ENV.envo.bestTool, ENV.envo.HP, ENV.envo.mineableWithFist);
                    nin.GetComponentInChildren<EnvoObject>().setDrops(ENV.envo.drops);
                }
                else
                {
                    nin.transform.position += new Vector3(0, 0, 0.05f);
                    nin.isStatic = true;
                }

                
            }


            chunk.generated = true;

            List<Sprite> sprites = new List<Sprite>();
            foreach (Transform children in Parent.transform)
            {
                if (children.name != "GroundItems" && children.name != "Enviroment")
                    sprites.Add(children.GetComponent<SpriteRenderer>().sprite);
            }

            try
            {
                allChunks.Add(new Vector2(chunk.x, chunk.y), chunk);
            }
            catch (System.ArgumentException) { }
            chunksToRegenerate.Remove(new Vector2(chunk.x, chunk.y));
        }
    }




    [ContextMenu("DELETE ALL KEYS")]
    void WIPE()
    {
        PlayerPrefs.DeleteAll();
        Debug.Break();
    }

    [ContextMenu("TestSave")]
    void TestSave()
    {
        SaveStates.SetKey(currentChunk.A_D_GetChunkSavePrefix(), currentChunk.A_D_ChunkToString());
    }
    [ContextMenu("TestLoad")]
    void TestLoad()
    {
        Debug.Log(SaveStates.GetKey(currentChunk.A_D_GetChunkSavePrefix()));
    }
    [ContextMenu("TestDelete")]
    void TestDelete()
    {
        SaveStates.DeleteKey(currentChunk.A_D_GetChunkSavePrefix());
    }
}
[System.Serializable] public class Chunk{
    public int x;
    public int y;
    public biome biome = biome.Null;
    public bool generated = false;
    public bool hasEntered = false;
    public bool isChunkLoadedFromFile = false;
    public GameObject thisObject;
    public List<Enviroment> enviroment = new List<Enviroment>();
    public Chunk(int x, int y, biome type){
        this.x = x;
        this.y = y;
        this.biome = type;
        if(biome == biome.Null && !this.generated)
            OverworldGeneration.instance.chunksToGenerate.TryAdd(new Vector2(x,y), new Chunk((int)x, (int)y, GetPerlinNoiseBiome.GenerateBiomeAt(new Vector2(x, y))));
    }
    public void Save()
    {
        SaveStates.SetKey(this.A_D_GetChunkSavePrefix(), this.A_D_ChunkToString());
    }
    

}

public class Enviroment
{
    public float x, y;
    public EnviromentObject envo;

    public Enviroment(float x, float y, EnviromentObject envo)
    {
        this.x = x;
        this.y = y;
        this.envo = envo;
    }
}
public enum biome
{
    Null, Plains, Woods, Desert
}

//hold all Biome Types in a Dimension
[System.Serializable] public class BiomeTypes
{
    public List<BiomeType> biomes = new List<BiomeType>();
}
[System.Serializable]
public class BiomeType
{
    public string NAME;
    public biome name;
    public List<Texture2D> sprites = new List<Texture2D>();
    public List<EnviromentData> enviroment = new List<EnviromentData>();
    
}



[System.Serializable]public class EnviromentData
{
    public EnviromentType type;
    [Range(0, 100)] public float chance;
    public int min_Amount;
    public int max_Amount;

}


static class ChunkInfo{
    public static Vector2 GetPos(this Chunk chunk){
        return new Vector2(chunk.x,chunk.y);
    }
    public static BiomeType toBiome(this Chunk chunk) {
        BiomeType type = null;
        foreach (var BIOME in OverworldGeneration.instance.OverworldBiomes.biomes)
        {
            if(BIOME.name == chunk.biome)
                type = BIOME;
        }
        return type;
    }
    public static List<Texture2D> getCorrectSprites(biome biome)
    {
        foreach (var item in OverworldGeneration.instance.OverworldBiomes.biomes)
        {
            if (biome == item.name)
                return item.sprites;
        }
        return null;
    }
    public static Chunk GetChunkAt(this Dictionary<Vector2,Chunk> chunks, int x, int y){
        Chunk chunk = null;
        if(chunks.TryGetValue(new Vector2(x, y), out chunk))
            return chunk;
        return null;
    }
    public static bool IsSorroundedByChunks(this List<Chunk> chunks, int x, int y) {
        bool sor = true;
        int rd = OverworldGeneration.instance.renderDistance;
        foreach (var chunk in chunks)
        {

            if ((chunk.x > x + rd || chunk.x < x - rd) && (chunk.y > y + rd || chunk.y < y - rd))
                continue;


            if (chunk.x == x-1 && chunk.y == y+1)
                continue;
            else if (chunk.x == x && chunk.y == y + 1)
                continue;
            else if (chunk.x == x + 1 && chunk.y == y + 1)
                continue;
            else if (chunk.x == x - 1 && chunk.y == y)
                continue;
            else if (chunk.x == x + 1 && chunk.y == y)
                continue;
            if (chunk.x == x - 1 && chunk.y == y - 1)
                continue;
            else if (chunk.x == x && chunk.y == y - 1)
                continue;
            else if (chunk.x == x + 1 && chunk.y == y - 1)
                continue;
            sor = false;
        }
        return sor;
    }
    public static Chunk ToChunk(this Vector2Int pos)
    {
        Chunk chunk = null;
        OverworldGeneration.instance.allChunks.TryGetValue(pos, out chunk);
        return chunk;
    }


    public static Vector2Int ToChunkCoords(this Vector2 position)
    {
        int x;
        int y;
        int ChunkSize = OverworldGeneration.instance.ChunkSize;
        if (position.x < 0)
            x = (int)((position.x - ChunkSize) / ChunkSize);
        else
            x = (int)(position.x / ChunkSize);

        if (position.y < 0)
            y = (int)((position.y - ChunkSize) / ChunkSize);
        else
            y = (int)(position.y / ChunkSize);

        return new Vector2Int(x, y);
    }
    public static Vector2Int ToChunkCoords(this Vector3 position)
    {
        int x;
        int y;
        int ChunkSize = OverworldGeneration.instance.ChunkSize;
        if (position.x < 0)
            x = (int)((position.x - ChunkSize) / ChunkSize);
        else
            x = (int)(position.x / ChunkSize);

        if (position.y < 0)
            y = (int)((position.y - ChunkSize) / ChunkSize);
        else
            y = (int)(position.y / ChunkSize);

        return new Vector2Int(x, y);
    }
    public static bool HasChunk(this Dictionary<Vector2, Chunk> chunks, Chunk chunk)
    {
        if (chunk == null)
            return false;
        return (chunks.ContainsKey(new Vector2(chunk.x, chunk.y)));
        
    }
}

public static class GetPerlinNoiseBiome
{

    public static biome GenerateBiomeAt(Vector2 pos)
    {
        //using seed
        pos = pos + new Vector2(pos.x + OverworldGeneration.instance.currWorld.Seed * 100, pos.y + OverworldGeneration.instance.currWorld.Seed * 100);
        float perlin = Mathf.PerlinNoise(pos.x / OverworldGeneration.instance.BiomeSize + .1f, pos.y / OverworldGeneration.instance.BiomeSize + .1f);
        biome bi = biome.Null;
        if (perlin < 0.2f)
            bi = biome.Desert;
        else if (perlin < 0.5f)
            bi = biome.Woods;
        else
            bi = biome.Plains;
        return bi;
    }
}