using UnityEngine;

public class NonInteractableItem : Item
{
    protected override void ItemInitialization()
    {
        isIntractable = false;
    }
}
