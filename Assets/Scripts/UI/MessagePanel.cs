using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagePanel : MonoBehaviour
{
    [HideInInspector] public Vector3 targetPos = Vector3.zero;
    [HideInInspector] public float moveSpeed;

    private void FixedUpdate()
    {
        if(transform.position != targetPos) 
        {
            MoveTowards(targetPos, moveSpeed);
            print(targetPos + "\n" + transform.position);
        }
    }

    public void SetTextMessage(string messageText) 
    {
        transform.Find("MessageText").GetComponent<TMP_Text>().text = messageText;
    }

    private void MoveTowards(Vector2 targetPos, float speed) 
    {
        transform.position = Vector2.Lerp(transform.position, targetPos, speed);
    }
}
