using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataManager
{
    public static class DataManagement
    {


        //Commands
        public static string A_C_GIVEITEM(string arg1, int arg2)
        {
            ItemObject i = A_D_stringToItem(arg1);

            if (i == null)
                return $"Syntax Error: <{arg1}> is not a valid item";
            if (arg2 < 1)
                return $"Syntax Error: Please insert a number greater than 0";

            InventoryManager.instance.inventory.AddItem(i, arg2);
            return $"successfully gave {arg2}x {i.name}";
        }
        public static string A_C_DATACOMMAND(string arg1, string arg2)
        {
            string data = "An unknown error has occured, please try again or Submit a bug report";
            switch (arg1.ToLower())
            {
                case "get":
                    switch (arg2.ToLower())
                    {
                        case "current_item":
                            data = A_D_getDataOfItem(A_D_getCurrentItem());
                            if (data == null)
                                data = "";
                            break;

                        case "player":
                            data = "Not Yet Implemented";
                            break;
                        default:
                            data = "Mistake on Argument 2: not correct Syntax";
                            break;
                    }
                    break;
                default:
                    data = "Mistake on Argument 1: not correct Syntax";
                    break;
            }
            return data;
        }
        public static string A_C_TELEPORTCOMMAND(int arg1, int arg2)
        {
            DebugController.main.player.transform.position = new Vector3(arg1, arg2, 0);
            return $"Successfully teleported to x:{arg1} y:{arg2}";
        }
        public static string A_C_HELPCOMMAND()
        {
            string data = "";
            for (int i = 0; i < DebugController.main.commandList.Count; i++)
            {
                DebugCommandBase command = DebugController.main.commandList[i] as DebugCommandBase;

                data += $"{command.commandFormat} - {command.commandDescription}\n";
            }
            return data.Remove(data.Length - 2);
        }

        public static string A_C_CLEARCOMMAND()
        {
            int c = InventoryManager.instance.inventory.Container.Count;
            InventoryManager.instance.inventory.Container.Clear();
            return $"successfully cleared {c} items.";
        }
        //Data
        public static string A_D_getDataOfItem(ItemObject item)
        {
            if (item == null)
                return null;

            Debug.Log(item.getItemData());
            return item.getItemData();
        }

        public static Vector2 A_D_getChunkPosition()
        {
            return OverworldGeneration.instance.player.transform.position.ToChunkCoords();
        }

        public static Vector2 A_D_getGlobalPosition()
        {
            return OverworldGeneration.instance.player.transform.position;
        }

        public static ItemObject A_D_getCurrentItem()
        {
            return InventoryManager.instance.currentItem;
        }

        public static ItemObject A_D_stringToItem(string str)
        {
            return Items.returnItemByName(str);
        }

        public static string A_D_BackpacksToString()
        {
            string str = "[";
            List<BackPackObject> list = SavedBackpacks.backPacks;

            if (list.Count == 0)
                return "{}";
            foreach (var BP in list)
            {
                bool b = false;
                str +=
                    $"{{" +
                    $"\"size\":{BP.size}," +
                    $"\"ID\":{BP.id}," +
                    $"\"Items\":[";

                foreach (var Slot in BP.Inventory.Container)
                {
                    b = true;
                    str +=
                        $"{{" +
                        $"\"Item\":\"{Slot.item.name}\"," +
                        $"\"Amount\":{Slot.amount}," +
                        $"\"Index\":{Slot.index}" +
                        $"}},";
                }
                if(b) str = str.Remove(str.LastIndexOf(","),1);
                str += "]},";
            }
            str = str.Remove(str.LastIndexOf(","), 1);
            return str + "]";
        }
        public static string A_D_InventoryToString()
        {
            string str = "[";

            foreach (var Slot in InventoryManager.instance.inventory.Container)
            {
                switch (Slot.item.type)
                {
                    //case ItemType.Tool:
                        //break;
                    case ItemType.BackPack:
                        BackPackObject obj = Slot.item as BackPackObject;
                        str +=
                        $"{{" +
                        $"\"Item\":\"{Slot.item.name}\"," +
                        $"\"Amount\":{Slot.amount}," +
                        $"\"Index\":{Slot.index}," +
                        $"\"ItemData\":{obj.A_D_BackpackToString()}" +
                        $"}},";
                        break;
                    default:
                        str +=
                        $"{{" +
                        $"\"Item\":\"{Slot.item.name}\"," +
                        $"\"Amount\":{Slot.amount}," +
                        $"\"Index\":{Slot.index}" +
                        $"}},";
                        break;
                }


                
            }

            try
            {
                str = str.Remove(str.LastIndexOf(","), 1);
            }
            catch {}
            return str + "]";
        }

        public static string A_D_BackpackToString(this BackPackObject BP)
        {
            string str = "";
            
            bool b = false;
            str +=
                $"{{" +
                $"\"size\":{BP.size}," +
                $"\"ID\":{BP.id}," +
                $"\"Items\":[";

            foreach (var Slot in BP.Inventory.Container)
            {
                b = true;
                str +=
                    $"{{" +
                    $"\"Item\":\"{Slot.item.name}\"," +
                    $"\"Amount\":{Slot.amount}," +
                    $"\"Index\":{Slot.index}" +
                    $"}},";
            }
            if (b) str = str.Remove(str.LastIndexOf(","), 1);
            str += "]},";
            
            str = str.Remove(str.LastIndexOf(","), 1);
            return str;
        }
    }
}
