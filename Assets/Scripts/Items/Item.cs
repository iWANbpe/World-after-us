using UnityEngine;

public class Item : MonoBehaviour
{
	[HideInInspector] public Rigidbody itemRigidbody;
	[HideInInspector] public ItemInfo itemInfo;

	protected GameObject targetObj;
	protected Collider itemCollider;
	private const float dragSpeed = 5f;

	private void Awake()
	{
		targetObj = null;
		itemRigidbody = GetComponent<Rigidbody>();
		itemCollider = GetComponent<Collider>();
	}

	public void SetTarget(GameObject target)
	{
		targetObj = target;
	}

	public void DisableCollisionLayer(LayerMask layerMask)
	{
		itemCollider.excludeLayers = layerMask;
	}

	private void FixedUpdate()
	{
		if (targetObj)
		{
			itemRigidbody.MovePosition(Vector3.Lerp(itemRigidbody.transform.position, targetObj.transform.position, dragSpeed * Time.fixedDeltaTime));
		}
	}
}