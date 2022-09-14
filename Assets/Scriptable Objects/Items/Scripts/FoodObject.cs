using UnityEngine;

[CreateAssetMenu(fileName ="New Food Object", menuName = "Inventory System/Items/Food")]
public class FoodObject : ItemObject
{
    public float restoreHealth;
    public float restoreHunger;
    private void Awake() {
        type = ItemType.Food;
    }
}
