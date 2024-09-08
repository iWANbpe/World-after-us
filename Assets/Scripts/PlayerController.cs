using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedUpKoef = 1f;
    [SerializeField] private float gravityKoef = 3f;
    [SerializeField] private float jumpForce = 1f;

    private InputActionsPlayer inputActions;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
