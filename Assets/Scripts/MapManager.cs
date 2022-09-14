using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Prefab;
    public bool toggle = true;
    void Start()
    {
        Camera.GetComponent<Camera>().orthographicSize = OverworldGeneration.instance.ChunkSize * OverworldGeneration.instance.renderDistance;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            toggle = !toggle;      
        }
        if(!toggle)
        {
            Camera.SetActive(false);
            Prefab.SetActive(false);
        }
        else
        {
            Camera.SetActive(true);
            Prefab.SetActive(true);
        }

    }
}
