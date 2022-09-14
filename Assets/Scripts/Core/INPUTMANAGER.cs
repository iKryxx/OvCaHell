using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static DataManager.DataManagement;

public class INPUTMANAGER : MonoBehaviour
{
    public InputActionAsset input;

    //Classes
    public PlayerMovement playerMovement;

    //Variables
    int x = 0;
    int y = 0;

    int tbc = 0;

    //Constants
    System.Type backpack = typeof(BackPackObject);
    System.Type equipment = typeof(EquipmentObject);
    System.Type food = typeof(FoodObject);
    System.Type tool = typeof(ToolObject);

    public BackPackManager backpackManager;
    //---------------------------------------------------------

    void MoveInput()
    {
        x = 0;
        y = 0;

        string[] all_move = new string[4] {"MoveUp","MoveLeft","MoveDown","MoveRight"};
        List<int> inputs = new List<int>();


        for (int i = 0; i < all_move.Length; i++)
        {
            string curr = all_move[i];
            string inp = input.FindActionMap("Player").FindAction(curr).bindings[0].ToString();
            inp = inp.Remove(0, inp.IndexOf("/") + 1);


            if (Input.GetKey(inp))
            {
                if (inp == "w")
                    y += 1;
                if (inp == "a")
                    x -= 1;
                if (inp == "s")
                    y -= 1;
                if (inp == "d")
                    x += 1;
            }
        }

        playerMovement.input.x = x;
        playerMovement.input.y = y;
    }

    void OnShift()
    {
        InventoryManager.instance.shift = !InventoryManager.instance.shift;
    }
    
    void OnPickUp()
    {
        InventoryManager.instance.tryPickUp();
    }
    
    void OnInteract()
    {
        ItemObject obj = A_D_getCurrentItem();
        if (obj == null)
            return;
        if (obj.GetType() == backpack && !backpackManager.isUIopen) {
            backpackManager.OpenUI(obj as BackPackObject);
        }
            

        
    }

    void OnAttack()
    {
        ItemObject obj = A_D_getCurrentItem();

        if (!AnimationManager.instance.anim.isPlaying || obj.GetType() != tool)
            playerMovement.triggerInput = true;
        if (obj == null)
            return;
        if (obj.GetType() == tool)
        {
            AnimationManager.instance.tryPlayAnim();
        }
    }
    void OnDrop()
    {
        if (InventoryManager.instance.inventory.getCurrentItem() != null)
        {

            if (InventoryManager.instance.shift)
                InventoryManager.instance.enviromentManager.DropAll(InventoryManager.instance.inventory.getCurrentSlot());
            else
                InventoryManager.instance.enviromentManager.DropItem(InventoryManager.instance.inventory.getCurrentSlot());

        }
    }
    
    void OnEscape()
    {
        if(backpackManager.isUIopen)
        {
            backpackManager.CloseUI();
            return;
        }
    }
    private void Update()
    {
        if (A_D_getCurrentItem() != null && A_D_getCurrentItem().GetType() == backpack)
        {
            BackPackObject obj = A_D_getCurrentItem() as BackPackObject;
            Debug.Log(obj.id);
        }


        if (tbc == 3)
        {
            playerMovement.triggerInput = false;
            tbc = 0;
        }
        if (playerMovement.triggerInput && (A_D_getCurrentItem() == null || A_D_getCurrentItem().GetType() != tool) || tbc > 0)
            tbc += 1;


        
        //if (!AnimationManager.instance.anim.isPlaying && playerMovement.triggerInput)
        //    playerMovement.triggerInput = false; 
        MoveInput();
    }
}