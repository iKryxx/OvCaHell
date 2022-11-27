using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class WorldIndex
{
    public static List<World> worlds = new List<World>() { new World("Debug", 0) };


    public static World stringToWorld(this string w)
    {
        foreach (var world in worlds)
        {
            if (world.Name == w)
                return world;
        }

        World cworld = new World(w, Random.Range(int.MinValue, int.MaxValue));
        worlds.Add(cworld);
        return cworld;
    }
}
public class World
{
    public string Name;
    public int Seed;

    public World(string name, int seed)
    {
        this.Name = name;
        this.Seed = seed;
    }

    public string getSavePrefix()
    {
        return $"/Saves/Save-{this.Name}";
    }
}


public static class crossSceneVariables
{
    public static string World { get; set; } = "";
    public static int Seed { get; set; }
}
