using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    public bool isUIopen = false;
    public GameObject STORAGE;
    public GameObject PREFAB;
    public BackPackObject currBP;


    public void OpenUI(BackPackObject obj)
    {
        if (GLOBAL.ISINPUTBLOCKED)
            return;

        GLOBAL.ISINPUTBLOCKED = true;

        currBP = obj;
        isUIopen = true;
        STORAGE.SetActive(true);

        for (int i = 0; i < obj.Inventory.maxSize; i++)
        {
            Instantiate(PREFAB, STORAGE.transform);
        }
    }
    public void CloseUI()
    {
        GLOBAL.ISINPUTBLOCKED = false;

        currBP = null;
        isUIopen =false;

        foreach (Transform item in STORAGE.transform)
        {
            Destroy(item.gameObject);
        }
        STORAGE.SetActive(false);
    }

    private void Update()
    {
        if (STORAGE.activeSelf && currBP != null)
        {
            List<int> used = new List<int>();
            foreach (var slot in currBP.Inventory.Container)
            {
                used.Add(slot.index);
                Transform parent = STORAGE.transform.GetChild(slot.index - 1);
                parent.Find("Icon").gameObject.SetActive(true);
                parent.Find("Item Amount").gameObject.SetActive(true);
                parent.Find("Icon").GetComponent<RawImage>().texture = slot.item.Icon;
                parent.Find("Item Amount").GetComponent<TextMeshProUGUI>().text = $"{slot.amount}";
            }
            for (int i = 1; i <= currBP.Inventory.maxSize; i++)
            {
                Transform parent = STORAGE.transform.GetChild(i - 1);
                parent.Find("Selection").gameObject.SetActive(false);
                if (!used.Contains(i))
                {
                    parent.Find("Icon").gameObject.SetActive(false);
                    parent.Find("Item Amount").gameObject.SetActive(false);
                }
            }
        }
    }

}
