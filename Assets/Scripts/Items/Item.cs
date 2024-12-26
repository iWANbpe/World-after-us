using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [HideInInspector] public Rigidbody itemRigidbody;
    [HideInInspector] public GameObject targetObj;
    [HideInInspector] public Collider itemCollider;
    private const float dragSpeed = 5f;

    private void Awake()
    {
        targetObj = null;
        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (targetObj) 
        {
            itemRigidbody.MovePosition(Vector3.Lerp(itemRigidbody.transform.position, targetObj.transform.position, dragSpeed * Time.fixedDeltaTime));
        }
    }

}
