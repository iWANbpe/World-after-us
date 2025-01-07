using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : Item
{
    private void Awake()
    {
        targetObj = null;
        isIntractable = true;
        itemName = itemInfo.itemName;
        
        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }
}
