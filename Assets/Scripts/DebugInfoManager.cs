using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugInfoManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Transform Player;

    bool shouldUp = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Info());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            shouldUp = !shouldUp;
        text.gameObject.SetActive(shouldUp);
    
    }

    IEnumerator Info()
    {

        while (true)
        {
            if (shouldUp)
            {
                float FPS = Mathf.Round(1f / Time.unscaledDeltaTime * 100) / 100;
                Vector3 Position_Normal = Player.transform.position.Sorted(1);
                Vector2Int Position_Chunk = Player.transform.position.ToChunkCoords();
                Chunk chunk = Position_Chunk.ToChunk();
                string biome = chunk.biome.ToString();
                var _holdingItem = InventoryManager.instance.currentItem;
                string holdingItem = "";
                if (_holdingItem != null)
                    holdingItem = InventoryManager.instance.currentItem.name;

                text.text =
                    ($"FPS: {FPS}\n" +
                    $"XYZ: {Position_Normal.x}, {Position_Normal.y}\n" +
                    $"XYZC: {Position_Chunk.x}, {Position_Chunk.y}\n" +
                    $"Biome: {biome.ToUpper()}\n" +
                    $"Selected Item: {holdingItem.Replace("_", " ").Replace("(Clone)","")}");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public static class vec3
{
    public static Vector3 Sorted(this Vector3 original, int steps)
    {
        int mult = (int)Mathf.Pow(10, steps);

        float x = Mathf.Round(original.x * Mathf.Pow(10, steps)) / mult;
        float y = Mathf.Round(original.y * Mathf.Pow(10, steps)) / mult;
        float z = Mathf.Round(original.z * Mathf.Pow(10, steps)) / mult;

        return new Vector3(x, y, z);
    }
}
