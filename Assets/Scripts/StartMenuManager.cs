using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public int ActiveScreen;
    public GameObject ButtonPrefab;
    public GameObject Error;
    public Dictionary<int,Transform> Menus = new Dictionary<int, Transform>();

    //Only For Serializing!
    [System.Serializable]
    public struct Menu
    {
        public int Id;
        public Transform MenuTrans;
    }
    public List<Menu> menus = new List<Menu>();

    private void Deleted(int k)
    {
        
        string[] Worlds = PlayerPrefs.GetString("Worlds").Split(' ');
        Transform menu;
        Menus.TryGetValue(1, out menu);
        if(k == 0)
        {
            foreach (Transform transform in menu.Find("Main Buttons"))
            {
                if (transform.gameObject.name != "Foot")
                    Destroy(transform.gameObject);
            }
            return;
        }


        foreach (Transform transform in menu.Find("Main Buttons"))
        {
            if (transform.gameObject.name != "Foot")
                Destroy(transform.gameObject);
        }

        for (int i = 0; i < Worlds.Length; i++)
        {
            GameObject curr = Instantiate(ButtonPrefab, menu.Find("Main Buttons"));
            curr.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Play\n'" + Worlds[i] + "'";
            string arg = Worlds[i];
            curr.GetComponent<Button>().onClick.AddListener(delegate { PlayWorld(arg); });
        }
        
        
    }
    private void Awake()
    {
        foreach (var menu in menus)
        {
            Menus.Add(menu.Id, menu.MenuTrans);
        }

        if (!PlayerPrefs.HasKey("Worlds"))
            return;
        string[] Worlds = PlayerPrefs.GetString("Worlds").Split(' ');

        for(int i = 0; i < Worlds.Length; i++)
        {
            Transform menu = null;
            Menus.TryGetValue(1, out menu);
            GameObject curr = Instantiate(ButtonPrefab, menu.Find("Main Buttons"));
            curr.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Play\n'"+Worlds[i]+"'";
            string arg = Worlds[i];
            curr.GetComponent<Button>().onClick.AddListener(delegate { PlayWorld(arg); });
        }
        for (int i = 0; i < Worlds.Length; i++)
        {
            Transform menu = null;
            Menus.TryGetValue(2, out menu);
            GameObject curr = Instantiate(ButtonPrefab, menu.Find("Main Buttons"));
            curr.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Delete\n'" + Worlds[i] + "'";
            string arg = Worlds[i];
            curr.GetComponent<Button>().onClick.AddListener(delegate { DeleteWorld(arg); });
        }
        Transform foot = null;
        Menus.TryGetValue(1,out foot);
        foot = foot.GetChild(0);
        foot.GetChild(0).SetSiblingIndex(foot.childCount-1);
        
        
    }
    //


    public void GenerateWorld()
    {
        string name = GameObject.Find("NewWorldName").GetComponent<TextMeshProUGUI>().text.Replace(" ","_").Replace("\u200B","");
        string[] Worlds = PlayerPrefs.GetString("Worlds").Split(' ');

        for (int i = 0; i < Worlds.Length; i++)
        {
            if (Worlds[i] == name.Replace(" ",""))
            {
                Error.gameObject.SetActive(true);
                Error.transform.SetSiblingIndex(1);
                return;
            }
        }
        PlayWorld(name);
    }
    public void DeleteWorld(string name)
    {
        if (!PlayerPrefs.HasKey("Worlds"))
            return;
        string[] Worlds = PlayerPrefs.GetString("Worlds").Split(' ');

        Transform menu;

        Menus.TryGetValue(2, out menu);
        if (Worlds.Length == 1)
        {

            PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Inventory");
            PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-World");
            PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Backpacks");
            PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Player");
            PlayerPrefs.DeleteKey("Worlds");

            foreach (Transform transform in menu.Find("Main Buttons"))
            {
                if (transform.gameObject.name != "Back")
                    Destroy(transform.gameObject);
            }
            Deleted(0);
            return;
        }

        int numIdx = Array.IndexOf(Worlds, name.Replace("\u200B", ""));
        List<string> tmp = new List<string>(Worlds);
        tmp.RemoveAt(numIdx);
        Worlds = tmp.ToArray();

        Menus.TryGetValue(2, out menu);
        string str = String.Join(" ", Worlds);
        PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Inventory");
        PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-World");
        PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Backpacks");
        PlayerPrefs.DeleteKey($"Save-{name.Replace("\u200B", "")}-Player");
        PlayerPrefs.SetString("Worlds",str);

        foreach (Transform transform in menu.Find("Main Buttons"))
        {
            if(transform.gameObject.name != "Back")
                Destroy(transform.gameObject);
        }
        Debug.Log("MEEEEEEEEEEEEEEEEEEEEEEEEEP");
        Worlds = PlayerPrefs.GetString("Worlds").Replace("\u200B","").Split(' ');
        for (int i = 0; i < Worlds.Length; i++)
        {
            Transform menu__ = null;
            Menus.TryGetValue(2, out menu__);
            GameObject curr = Instantiate(ButtonPrefab, menu__.Find("Main Buttons"));
            curr.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Delete\n'" + Worlds[i] + "'";
            string arg = Worlds[i];
            curr.GetComponent<Button>().onClick.AddListener(delegate { DeleteWorld(arg); });
        }
        Deleted(1);
    }


    public void SetScreen(int i)
    {
        //0:Start Screen, 1:World List, 2: Delete Worlds, 3: Quit 'Are you Sure??', 4: New World, 5: Options
        ActiveScreen = i;
        UpdateScreen();
    }

    public void PlayWorld(string Name)
    {
        crossSceneVariables.World = Name;
        SceneManager.LoadScene(1);
    }
    public void Leave()
    {
        Application.Quit();
    }

    void UpdateScreen()
    {
        foreach (var Menu in Menus)
        {
            Menu.Value.gameObject.SetActive(false);
        }
        Transform menu = null;
        Menus.TryGetValue(ActiveScreen, out menu);
        menu.gameObject.SetActive(true);
    }
}
