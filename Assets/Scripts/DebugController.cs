using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static DataManager.DataManagement;

public class DebugController : MonoBehaviour
{
    public GameObject player;
    public static DebugController main;

    bool showConsole;
    string input = "";
    List<string> lastInput = new List<string>();
    int curLast = 0;
    string output = "";

    public static DebugCommand<string, int> GIVE_ITEM_AMOUNT;
    public static DebugCommand<string> GIVE_ITEM;
    public static DebugCommand HELP;
    public static DebugCommand<int, int> TP;
    public static DebugCommand<string,string> DATA_GET;
    public static DebugCommand CLEAR;

    public List<DebugCommandBase> commandList;

    Vector2 scroll;

    public void OnReturn()
    {
        if (input == "")
            output = "";

        if (showConsole)
            HandleInput();
    }
    public void OnExit()
    {
        output = "";
        HandleInput();
    }
    public void OnConsoleUp()
    {
        

        if (curLast != 0)
            curLast -= 1;

        Debug.Log("Up");

        if (lastInput.Count != 0)
            input = lastInput[curLast];
    }
    public void OnConsoleDown()
    {
        
        if (lastInput.Count != 0 && curLast < lastInput.Count - 1)
            curLast += 1;

        Debug.Log("Down");

        if (lastInput.Count != 0)
            input = lastInput[curLast];
    }

    public void OnConsole()
    {
        showConsole = true;
    }
    private void OnGUI()
    {
        if (input != "")
        {
            List<DebugCommandBase> sd = new List<DebugCommandBase>();
            string data = "";
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                if(input.Contains(" ") && input.Length > 0)
                {
                    if (command.commandFormat.StartsWith(input.Remove(input.IndexOf(" "))))
                        sd.Add(command);
                }
                else if (command.commandFormat.StartsWith(input))
                    sd.Add(command);
            }
            for (int i = 0; i < sd.Count; i++)
            {
                DebugCommandBase command = sd[i] as DebugCommandBase;

                data += $"{command.commandFormat} - {command.commandDescription}\n";
            }

            output = data;
        }



        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.normal.background = MakeTex(600, 1, new Color(0, 0, 0, 0.5f));
        
        style.fontSize = 16;

        GUIStyle st = new GUIStyle(style);
        st.normal.background = MakeTex(600, 1, new Color(0, 0, 0, 0));


        GUIStyle style2 = new GUIStyle();
        style2.normal.textColor = Color.yellow;
        style2.normal.background = MakeTex(600, 1, new Color(0, 0, 0, 0));
        style2.fontSize = 16;


        if (!showConsole) { return; }
        float y = 0f;

        float h = 0f;

        GUI.Box(new Rect(0, y, Screen.width, 30), "", style);

        GUI.SetNextControlName("Console");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input, style2);
        GUI.FocusControl("Console");
        y += 30;
        if (output.Length > 0)
        {

            string[] trueOutput = output.Split("\n");

            h = Mathf.Min(Screen.height / 2, trueOutput.Length * 20);
            Debug.Log(h);
            GUI.Box(new Rect(0, y, Screen.width, h), "", style);
            Rect viewport = new Rect(0, y, Screen.width - 30, 20 * trueOutput.Length);


            for (int i = 0; i < trueOutput.Length; i++)
            {

                Rect labelRect = new Rect(10, y + 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, trueOutput[i], st);
            }

            GUI.EndScrollView();

            y += h;
        }






        
    }

    void HandleInput()
    {
        if(lastInput.Count == 0 || lastInput[lastInput.Count - 1] != input)
            lastInput.Add(input);
        curLast = lastInput.Count;
        string[] properties = input.Split(' ');
        DebugCommandBase aCommand = null;

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.commandId) && commandBase.commandFormat.Split(' ').Length == properties.Length)
            {
                if (commandList[i] as DebugCommand != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    try { (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1])); }
                    catch (FormatException)
                    { }

                }
                else if (commandList[i] as DebugCommand<string> != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    try { (commandList[i] as DebugCommand<string>).Invoke(properties[1]); }
                    catch (FormatException)
                    { }

                }
                else if (commandList[i] as DebugCommand<int, int> != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    try { (commandList[i] as DebugCommand<int, int>).Invoke(int.Parse(properties[1]), int.Parse(properties[2])); }
                    catch (FormatException)
                    { }

                }
                else if (commandList[i] as DebugCommand<string, int> != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    try { (commandList[i] as DebugCommand<string, int>).Invoke(properties[1], int.Parse(properties[2])); }
                    catch (FormatException)
                    { }

                }
                else if (commandList[i] as DebugCommand<string, string> != null)
                {
                    aCommand = commandList[i] as DebugCommandBase;
                    try { (commandList[i] as DebugCommand<string, string>).Invoke(properties[1], properties[2]); }
                    catch (FormatException)
                    { }

                }

            }
        }
        if (((aCommand != null && aCommand.commandId != "help") || aCommand == null ) &&output == "")
            showConsole = false;
        
        input = "";
    }

    private void Awake()
    {
        main = this;

        GIVE_ITEM_AMOUNT = new DebugCommand<string,int>("give", "Give an item with specified amount to the player", "give <string:item> <int:amount>", (i,a) =>
        {
            output = A_C_GIVEITEM(i,a);
        });

        GIVE_ITEM = new DebugCommand<string>("give", "Give an item to the player", "give <string:item>", (i) =>
        {
            output = A_C_GIVEITEM(i, 1);
        });

        HELP = new DebugCommand("help", "Shows a list of commands","help", ()=>
        {
            output = A_C_HELPCOMMAND();
        });

        TP = new DebugCommand<int, int>("tp", "Teleports the player to the specified coordinates", "tp <int:x> <int:y>", (x, y) =>
        {
            output = A_C_TELEPORTCOMMAND(x,y);
        });

        DATA_GET = new DebugCommand<string, string>("data", "gets data of the specified object", "data <string:[get]> <string:[current_item,player]>", (x, y) =>
        {
            output = A_C_DATACOMMAND(x,y);
        });

        CLEAR = new DebugCommand("clear", "clears your inventory", "clear", () =>
        {
            output = A_C_CLEARCOMMAND();
        });


        commandList = new List<DebugCommandBase>
        {
            GIVE_ITEM_AMOUNT,
            GIVE_ITEM,
            HELP,
            TP,
            DATA_GET,
            CLEAR
        };


        commandList = commandList.OrderBy(x => x.commandId).ToList();
    }



    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}


public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; } }
    public string commandFormat { get { return _commandFormat; } }

    public DebugCommandBase(string commandId, string commandDescription, string commandFormat)
    {
        _commandId = commandId;
        _commandDescription = commandDescription;
        _commandFormat = commandFormat;
    }

}

public class DebugCommand : DebugCommandBase
{
    public Action command;
    public DebugCommand(string id, string desc, string format, Action command) : base (id, desc, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}
public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;
    public DebugCommand(string id, string desc, string format, Action<T1> command) : base(id, desc, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}
public class DebugCommand<T1,T2> : DebugCommandBase
{
    private Action<T1, T2> command;
    public DebugCommand(string id, string desc, string format, Action<T1, T2> command) : base(id, desc, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value1, T2 value2)
    {
        command.Invoke(value1, value2);
    }
}
