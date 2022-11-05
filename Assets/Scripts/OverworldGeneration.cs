using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGeneration : MonoBehaviour
{
    public static OverworldGeneration instance;
    public EnviromentManager envo;
    public SaveLoadmanager SaveLoad;

    public LayerMask forbiddenVillage;
    public LayerMask forbiddenHouse;


    //WorldSettings
    public World currWorld;
    public int BiomeSize;
    bool LockWorldSize = true;
    public int ChunkSize = 20;
    public GameObject ChunkParent;
    public List<Chunk> chunks = new List<Chunk>();
    public GameObject player;
    Chunk currentChunk;
    Chunk _currentChunk;
    public int renderDistance;

    //Sprites
    public GameObject prefab;

    public BiomeTypes OverworldBiomes;
    //public List<Texture2D> PlainsGroundSpritesMiddle = new List<Texture2D>();
    public List<Texture2D> WoodsGroundSprites = new List<Texture2D>();
    public List<Texture2D> DesertGroundSprites = new List<Texture2D>();

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
        if (PlayerPrefs.HasKey(currWorld.getSavePrefix() + "World"))
            StartCoroutine(SaveLoad.loadWorld(currWorld));

        SaveLoad.start();

    }
    void Update()
    {


        //setting current chunk
        Vector2 pos = player.transform.position;
        Vector2Int curr = pos.ToChunkCoords();

        currentChunk = chunks.GetChunkAt(curr.x, curr.y);

        if(currentChunk == null)
            chunks.Add(new Chunk(curr.x, curr.y, GetPerlinNoiseBiome.GenerateBiomeAt(new Vector2(curr.x, curr.y))));
        //Generate new Chunks
        if (currentChunk != null && !currentChunk.hasEntered)
        {
            GenerateNeighbours();
            RefreshChunks();
            currentChunk.hasEntered = true;
        }
        //Hide other Chunks
        if(_currentChunk != currentChunk)
        {
            foreach (var chunk in chunks)
            {
                if(currentChunk == null)
                    break;
                if(Mathf.Abs(chunk.x - currentChunk.x) > renderDistance || Mathf.Abs(chunk.y - currentChunk.y) > renderDistance)
                    chunk.thisObject.SetActive(false);
                else
                    chunk.thisObject.SetActive(true);
            }
        }


        _currentChunk = currentChunk;

        if (Input.GetKeyDown(KeyCode.Escape))
            SaveLoad.SaveWorld(currWorld);

        //Generating Villages, one per frame
    }

    private void GenerateVillage(Vector2Int pos)
    {
        if (Mathf.PerlinNoise(pos.x * frequency + currWorld.Seed, pos.y * frequency + currWorld.Seed) < threshhold)
            VillageGen.instance.VillageGeneration(new Vector2Int(pos.x, pos.y));
    }

    void GenerateNeighbours(){
        
        for (int x = -renderDistance + currentChunk.x; x <= renderDistance + currentChunk.x; x++)
        {
            for (int y = -renderDistance + currentChunk.y; y <= renderDistance + currentChunk.y; y++)
            {
                //if (Mathf.Abs(x - y) > renderDistance + currentChunk.y + currentChunk.x || Mathf.Abs(x-y) < -renderDistance - currentChunk.y - currentChunk.x)
                //continue;
                if (chunks.GetChunkAt(x, y) == null)
                {
                    chunks.Add(new Chunk(x, y, GetPerlinNoiseBiome.GenerateBiomeAt(new Vector2(x, y))));
                    GenerateVillage(new Vector2Int(x, y));
                }
            }
        }
    }

    //Render Chunks and Generate Enviroment
    public void RefreshChunks(){
        foreach (var chunk in chunks)
        {
            if(chunk.generated)
                continue;
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
            GameObject curr = Instantiate(prefab,new Vector3(chunk.x*ChunkSize + 10, chunk.y*ChunkSize + 10, 1), Quaternion.identity);
            curr.GetComponent<SpriteRenderer>().sprite = Sprite.Create(currSprites[z],new Rect(0,0, currSprites[z].width, currSprites[z].height),new Vector2(0.5f,0.5f));
            curr.transform.localScale = new Vector3(100,100,1);
            curr.transform.parent = Parent.transform;
            curr.isStatic = true;

            chunk.thisObject = Parent;

            //Debug.Log(chunk.isChunkLoadedFromFile);


            foreach (var ENV in chunk.toBiome().enviroment)
            {

                if (chunk.isChunkLoadedFromFile)
                    break;
                if (Random.Range(0, 100) > ENV.chance)
                    continue;

                List<EnviromentObject> objs = envo.enviromentObjects.getObjectOfType(ENV.type);
                

                //Debug.Log(objs.Count + " " + chunk.biome.ToString());
                if (objs.Count == 0)
                    continue;
                
                for (int i = 0; i < Random.Range(ENV.min_Amount, ENV.max_Amount + 1); i++)
                {
                    EnviromentObject obj = objs[Random.Range(0, objs.Count - 1)];
                    int x = Random.Range(0, ChunkSize + 1) + chunk.x * ChunkSize;
                    int y = Random.Range(0, ChunkSize + 1) + chunk.y * ChunkSize;


                    GameObject nin = Instantiate(obj.prefab, new Vector3(x, y, 0), Quaternion.identity);
                    if(nin.transform.GetComponentInChildren<CircleCollider2D>() != null)
                    {

                        //Debug.Log(nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(filter, results) + " " + nin.transform.position.x + " "+ nin.transform.position.y);

                        if (nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(villageFilter,results) > 0)
                            Destroy(nin);

                    }
                    else
                    {
                        nin.AddComponent<CircleCollider2D>();
                        if (nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(houseFilter, results) > 0)
                            Destroy(nin);
                        else
                            Destroy(nin.GetComponent<CircleCollider2D>());
                    }


                    nin.transform.parent = chunk.thisObject.transform.Find("Enviroment");
                    if (obj.mineable)
                    {
                        nin.GetComponentInChildren<EnvoObject>().setValues(obj.prefab, obj.type, obj.mineable, obj.bestTool, obj.HP, obj.mineableWithFist);
                        nin.GetComponentInChildren<EnvoObject>().setDrops(obj.drops);
                    }
                    else
                        nin.isStatic = true;

                }
            }
            

            chunk.generated = true;
            
            List<Sprite> sprites = new List<Sprite>();
            foreach (Transform children in Parent.transform)
            {
                if(children.name != "GroundItems" && children.name != "Enviroment")
                    sprites.Add(children.GetComponent<SpriteRenderer>().sprite);
            }

            

        }  
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
    public Chunk(int x, int y, biome type){
        this.x = x;
        this.y = y;
        this.biome = type;
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
    [Range(0, 100)] public int chance;
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
    public static Chunk GetChunkAt(this List<Chunk> chunks, int x, int y){
        foreach (var chunk in chunks)
        {
            if(chunk.x == x && chunk.y == y)
                return chunk;
        }
        return null;
    }
    public static Chunk ToChunk(this Vector2Int pos)
    {


        foreach (var chunk in OverworldGeneration.instance.chunks)
        {
            if (chunk.x == pos.x && chunk.y == pos.y)
                return chunk;
        }
        return null;
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