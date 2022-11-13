using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance = null;
    gameAction CurrentAction = null;
    public GameObject ActionCanvas;
    public void SetAction(Texture2D icon, string Text)
    {
        CurrentAction = new gameAction(icon, Text);
    }
    public void SetAction(Texture2D icon, string text, float max_value, float curr_value)
    {
        CurrentAction = new gameAction(icon, text, max_value, curr_value);
    }
    public void ResetAction()
    {
         CurrentAction = null;
    }

    private void Update()
    {
        ActionCanvas.SetActive(CurrentAction != null);
        if(CurrentAction != null)
        {
            ActionCanvas.SetActive(true);
            ActionCanvas.transform.Find("Background").Find("Icon").GetComponent<RawImage>().texture = CurrentAction.Icon;
            ActionCanvas.transform.Find("Background").Find("Text").GetComponent<TextMeshProUGUI>().text = CurrentAction.Text;
            if(CurrentAction.type == gameAction.ActionType.progress)
            {
                ActionCanvas.transform.Find("Background").Find("HP").gameObject.SetActive(true);
                ActionCanvas.transform.Find("Background").Find("HP").GetComponent<Slider>().maxValue = CurrentAction.max_value;
                ActionCanvas.transform.Find("Background").Find("HP").GetComponent<Slider>().value = CurrentAction.curr_value;

            }
        }
        else
        {
            ActionCanvas.SetActive(false);
            ActionCanvas.transform.Find("Background").Find("HP").gameObject.SetActive(false);


        }
       
    }

    private void Awake()
    {
        instance = this;
    }
}
class gameAction
{
    public enum ActionType
    {
        item, interaction, progress
    }
    public ActionType type;
    public Texture2D Icon;
    public string Text;
    public float max_value;
    public float curr_value;

    public gameAction(Texture2D icon, string text)
    {
        Icon = icon;
        Text = text;
        type = ActionType.item;
    }

    public gameAction(Texture2D icon, string text, float max_value, float curr_value)
    {
        Text = text;
        this.max_value = max_value;
        this.curr_value = curr_value;
        type = ActionType.progress;
        Icon = icon;
    }
}
