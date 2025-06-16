using UnityEngine;

public class Padlock : MonoBehaviour, IOnShoot
{
	[SerializeField] private Door lockedDoor;
	private Rigidbody rb;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
		lockedDoor.canBeOpened = false;
	}

	public void OnShoot()
	{
		lockedDoor.canBeOpened = true;
		rb.isKinematic = false;
	}
}