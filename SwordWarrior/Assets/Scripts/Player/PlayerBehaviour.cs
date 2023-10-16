using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour Instance;

    #region Private Variables
        #region Player Components
            private Animator playerAnimator;
            private Rigidbody2D playerRigidBody;
            private SpriteRenderer playerSpriteRenderer;
        #endregion
        private PlayerControls playerControls;
        private Vector2 moveDirection;
        private bool isMoving = false;
        #region Animator Variables
            private int isMovingAnimationHash;
        #endregion
    #endregion

    #region Serialized Field Variables
        [SerializeField] private float moveVelocity = 5f;
    #endregion

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        #endregion
        GetPlayerComponents();
        SetInputParameters();
        GetAnimatorParametersHash();
    }

    private void Update()
    {
        MovePlayer();
        AnimatePlayer();
    }

    private void GetPlayerComponents()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void GetInputInfo(InputAction.CallbackContext inputContext)
    {
        moveDirection.x = inputContext.ReadValue<float>();
        isMoving = moveDirection.x != 0;
    }

    private void GetAnimatorParametersHash()
    {
        isMovingAnimationHash = Animator.StringToHash("isMoving");
    }

    private void SetInputParameters()
    {
        playerControls = new PlayerControls();
        playerControls.Movement.Walk.started += GetInputInfo;
        playerControls.Movement.Walk.performed += GetInputInfo;
        playerControls.Movement.Walk.canceled += GetInputInfo;
    }

    private void MovePlayer()
    {
        if (moveDirection.x > 0)
        {
            playerSpriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            playerSpriteRenderer.flipX = true;
        }
        transform.Translate(moveDirection * moveVelocity * Time.deltaTime);
    }

    private void AnimatePlayer()
    {
        if (isMoving && !playerAnimator.GetBool(isMovingAnimationHash)) 
        {
            playerAnimator.SetBool(isMovingAnimationHash, true);
        }
        else if (!isMoving && playerAnimator.GetBool(isMovingAnimationHash))
        {
            playerAnimator.SetBool(isMovingAnimationHash, false);
        }
    }

    #region OnEnable/Disable Functions
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Movement.Walk.started -= GetInputInfo;
        playerControls.Movement.Walk.performed -= GetInputInfo;
        playerControls.Movement.Walk.canceled -= GetInputInfo;
        playerControls.Disable();
    }
    #endregion  
}
