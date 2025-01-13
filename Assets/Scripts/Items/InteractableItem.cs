using UnityEngine;

public class InteractableItem : Item
{
    protected override void ItemInitialization() 
    {
        isIntractable = true;
        itemName = itemInfo.itemName;
    }
}
