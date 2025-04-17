using System.Collections;
using TMPro;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
	private Vector3 targetPos = Vector3.zero;
	private Vector3 startPosition;
	[HideInInspector] public Vector3 disappearancePos;

	private PlayerUI playerUI;

	private float moveDuration;
	private float moveSpeed;
	private float startTime;
	private void Start()
	{
		playerUI = GameObject.Find("Player").GetComponent<PlayerUI>();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		playerUI.RemoveMessageFromMessageList(this);
		playerUI.AddMessageToPool(this);

		disappearancePos.y = transform.position.y;
		transform.position = disappearancePos;
		gameObject.SetActive(false);
	}

	private void FixedUpdate()
	{
		if (transform.position != targetPos)
		{
			MoveTowardsTarget();
		}
		else if (transform.position == disappearancePos)
		{
			gameObject.SetActive(false);
		}
	}

	public void SetTextMessage(string messageText)
	{
		transform.Find("MessageText").GetComponent<TMP_Text>().text = messageText;
	}

	public void AddMoving(Vector2 targetPos, float moveSpeed, float moveDuration)
	{
		this.targetPos = targetPos;
		this.moveSpeed = moveSpeed;
		this.moveDuration = moveDuration;
		startTime = Time.time;
		startPosition = transform.position;
	}

	private void MoveTowardsTarget()
	{
		float completedDistance = ((Time.time - startTime) * moveSpeed) / moveDuration;
		transform.position = Vector2.Lerp(startPosition, targetPos, completedDistance);
	}

	public void SetMessageLifetime(float messageLifeTime)
	{
		StartCoroutine(MessageLife(messageLifeTime));
	}

	private IEnumerator MessageLife(float messageLifeTime)
	{
		yield return new WaitForSeconds(messageLifeTime);
		HideMessage();
	}

	public void HideMessage()
	{
		StopAllCoroutines();
		playerUI.RemoveMessageFromMessageList(this);
		startTime = Time.time;
		disappearancePos.y = transform.position.y;
		startPosition = transform.position;
		targetPos = disappearancePos;
	}
}
