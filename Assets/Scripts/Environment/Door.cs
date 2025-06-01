using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteract
{
	[SerializeField] private float doorOpenAngle;
	[SerializeField] private float doorOpenSpeed;

	private float doorStartAngle;
	public bool canBeOpened { set { _canBeOpened = value; } }
	private bool _canBeOpened = true;
	private bool isOpen = false;

	private void Awake()
	{
		doorStartAngle = transform.rotation.y;
	}

	public bool CanInteract()
	{
		return _canBeOpened;
	}

	public void Interact()
	{
		Open();
	}

	public void Open()
	{
		StartCoroutine(OpeningCoroutine());
	}

	private IEnumerator OpeningCoroutine()
	{
		_canBeOpened = false;
		float time = 0f;
		float endRotationAngle = isOpen ? doorStartAngle : doorOpenAngle;

		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = Quaternion.Euler(0f, endRotationAngle, 0f);

		while (time < 1)
		{
			transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
			yield return null;
			time += Time.deltaTime * doorOpenSpeed;
		}

		isOpen = !isOpen;
		_canBeOpened = true;
	}
}