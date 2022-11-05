using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance{ get; private set; }
    private SettingsManager() { }
    public Settings settings;
    
    private void Awake()
    {
        instance = this;
    }

    public void leave()
    {
        OverworldGeneration.instance.SaveLoad.SaveWorld(crossSceneVariables.World.stringToWorld());
        SceneManager.LoadScene(0);
    }
    
}
[System.Serializable]public class Settings
{
    public GameObject ItemObjectPrefab;
    public List<ItemObject> AllItems;
}
