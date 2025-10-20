using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]    // will create a new option in the unity right-click menu
public class InventoryItemData : ScriptableObject
{
    public int ID;
    public string DisplayName;
    [TextArea(4, 4)]
    public string Description;
    public Sprite Icon;
    public int MaxStackSize;
    // public string Type       // is it a a ressource for the player, a material, something else...?
    public Resources type;

}
