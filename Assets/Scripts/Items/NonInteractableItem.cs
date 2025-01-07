using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonInteractableItem : Item
{
    private void Awake()
    {
        itemInfo = null;
        targetObj = null;
        isIntractable = false;

        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }
}
