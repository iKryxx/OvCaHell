using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public bool isPlayingAnim = false;
    public bool readyAnim = false;
    public Animation anim;

    public Sprite[] Walkingnimation;
    public bool readyWalking = false;
    int j = 0;


    int i;
    ItemObject currentItem;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {


        if (readyWalking)
        {
            j++;
            if (j == Walkingnimation.Length*20)
                j = 0;
        }
        else
            j = 0;
        GameObject.Find("Character").transform.Find("Body").Find("Feet").GetChild(0).GetComponent<SpriteRenderer>().sprite = Walkingnimation[Mathf.CeilToInt(j / 20)];

        isPlayingAnim = anim.isPlaying;


        if (InventoryManager.instance.inventory.getCurrentSlot() == null)
            return;
        if(InventoryManager.instance.inventory.getCurrentSlot().index != i)
        {
            anim.Stop();
            i = InventoryManager.instance.inventory.getCurrentSlot().index;
        }

        currentItem = InventoryManager.instance.inventory.getCurrentItem();

        if ((currentItem.type == ItemType.Tool || currentItem.type == ItemType.Equipment) && !isPlayingAnim)
            readyAnim = true; 
        if(isPlayingAnim)
            readyAnim = false;



       
    }

    public void tryPlayAnim()
    {
        if (!readyAnim)
            return;
        string _name = "";
        PlayerMovement.instance.triggerInput = true;
        switch (currentItem.type)
        {
            case ItemType.Tool:
                ToolObject tool = (ToolObject)currentItem;             
                _name = tool.toolType.ToString();
                break;
            case ItemType.Equipment:
                EquipmentObject equip = (EquipmentObject)currentItem;
                _name = equip.equipType.ToString();
                break;
            default:
                break;
        }
        if (anim.GetClip(_name) == null)  return;

        
        anim.Play(_name);
    }

    public void playWalkingAnimation()
    {
        readyWalking = true;
    }
    public void stopWalkingAnimation()
    {
        readyWalking = false;
    }
}