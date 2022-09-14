using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance{ get; private set; }
    private SettingsManager() { }
    public Settings settings;
    
    private void Awake()
    {
        instance = this;
    }
    
}
[System.Serializable]public class Settings
{
    public GameObject ItemObjectPrefab;
    public List<ItemObject> AllItems;
}
