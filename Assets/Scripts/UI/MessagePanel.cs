using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagePanel : MonoBehaviour
{
    private Vector3 targetPos = Vector3.zero;
    private Vector3 startPosition;
    [HideInInspector] public Vector3 disappearancePos;

    private PlayerUI playerUI;

    private float moveDuration;
    private float moveSpeed;
    private float startTime;

    private float messageLifeTime;

    private void Start()
    {
        playerUI = GameObject.Find("Player").GetComponent<PlayerUI>();
        StartCoroutine(MessageLife());
    }
    private void FixedUpdate()
    {
        if(transform.position != targetPos) 
        {
            MoveTowardsTarget();
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
        this.messageLifeTime = messageLifeTime;
    }

    private IEnumerator MessageLife() 
    {
        yield return new WaitForSeconds(messageLifeTime);

        playerUI.RemoveMessageFromMessageList(gameObject);
        startTime = Time.time;
        disappearancePos.y = transform.position.y;
        startPosition = transform.position;
        targetPos = disappearancePos;
    }
}
