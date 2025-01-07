using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [HideInInspector] public string itemName;
    [HideInInspector] public bool isIntractable;
    [HideInInspector] public Rigidbody itemRigidbody;

    public ItemInfo itemInfo;

    protected GameObject targetObj;
    protected Collider itemCollider;
    private const float dragSpeed = 5f;

    private void FixedUpdate()
    {
        if (targetObj) 
        {
            itemRigidbody.MovePosition(Vector3.Lerp(itemRigidbody.transform.position, targetObj.transform.position, dragSpeed * Time.fixedDeltaTime));
        }
    }

    
    public void SetTarget(GameObject target) 
    {
        targetObj = target;
    }

    public void DisableCollisionLayer(LayerMask layerMask) 
    {
        itemCollider.excludeLayers = layerMask;
    }
}
