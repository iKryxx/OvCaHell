using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment Object", menuName = "Inventory System/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    public float defBonus;
    public float manaBonus;
    public float atkBonus;
    public float speedBonus;

    public EquipmentType equipType;
    private void Awake() {
        type = ItemType.Equipment;
    }
}