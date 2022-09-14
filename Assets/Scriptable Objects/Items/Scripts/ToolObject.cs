using UnityEngine;

[CreateAssetMenu(fileName ="New Tool Object", menuName = "Inventory System/Items/Tool")]
public class ToolObject : ItemObject
{
    public ToolType toolType;
    /// <summary>
    /// 1 = Wood
    /// 2 = Stone
    /// 3 = Iron
    /// 4 = Hardened Stone
    /// </summary>
    public int toolStrength;
    private void Awake() {
        type = ItemType.Tool;
    }
}
