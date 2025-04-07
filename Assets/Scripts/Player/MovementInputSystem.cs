using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MovementInputSystem : MonoBehaviour
{
    [Header("Character Controller")]
    public CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private new Transform camera;

    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;
    public Vector3 currentVelocity;
    private float originalSpeed;
    private bool isSprinting;
    private Vector3 velocity;

    private Vector3 moveDir;

    [Header("Jumping")]
    public float jumpHeight = 3f;
    private bool isJumping;

    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask ground;

    [Header("Crouching")]
    public Vector3 crouchingCenter;
    public float crouchHeight = 1f;
    private Vector3 standCenter;
    private float standHeight;
    private bool isCrouching;
    private bool forcedCrouch;

    [Header("Sliding")]
    public float slideSpeed = 15f;
    private float originalSlideSpeed;
    private bool isSliding;

    [HideInInspector] public Vector2 currentInputVector;
    [HideInInspector] public Vector2 smoothInputVelocity;

    public float smoothInputSpeed = 0.1f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        camera = transform.GetChild(0);
        originalSpeed = speed;
        originalSlideSpeed = slideSpeed;
        standHeight = controller.height;
        standCenter = controller.center;
    }

    void Update()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        jumpBufferCounter -= Time.deltaTime;

        if (forcedCrouch)
        {
            if (!Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
            {
                forcedCrouch = false;
                isCrouching = false;
            }
        }

        HeightHandler();

        SpeedHandler();

        MovementHandler();

        Gravity();
    }

    public void MovementHandler()
    {
        if (!isSliding)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);

            moveDir = transform.right * currentInputVector.x + transform.forward * currentInputVector.y;

            if (moveDir.magnitude > 1)
            {
                moveDir /= moveDir.magnitude;
            }

            controller.Move(moveDir * speed * Time.deltaTime);
            currentVelocity = controller.velocity;
        }
        else
        {
            controller.Move(moveDir * slideSpeed * Time.deltaTime);
            currentVelocity = controller.velocity;
        }
    }

    void SpeedHandler()
    {
        if (isSprinting)
        {
            speed = Mathf.Lerp(speed, sprintSpeed, Time.deltaTime * 5);
        }
        else if (isCrouching)
        {
            speed = Mathf.Lerp(speed, crouchSpeed, Time.deltaTime * 5);
        }
        else
        {
            speed = Mathf.Lerp(speed, originalSpeed, Time.deltaTime * 5);
        }

        if (isSliding)
        {
            slideSpeed = Mathf.Lerp(slideSpeed, 0f, Time.deltaTime);
            speed = slideSpeed;
            slideSpeed = Mathf.Clamp(slideSpeed, 0f, originalSlideSpeed);
            if (slideSpeed < 5f)
            {
                isSliding = false;
                if (Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
                {
                    isCrouching = true;
                    isSprinting = false;
                }
            }
        }
        else
        {
            slideSpeed = Mathf.Lerp(slideSpeed, originalSlideSpeed, Time.deltaTime * 5);
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !isSprinting)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            if (!Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
            {
                isCrouching = false;
            }
            else
            {
                forcedCrouch = true;
            }
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed && !isCrouching)
        {
            isSprinting = true;
        }
        else if (context.canceled)
        {
            isSprinting = false;
        }
    }

    public void Slide(InputAction.CallbackContext context)
    {
        if (context.performed && isSprinting && !isSliding && IsGrounded() && slideSpeed > 5f && currentInputVector.y > 0)
        {
            isSliding = true;
        }

        if (context.canceled)
        {
            if (!Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
            {
                isSliding = false;
            }
            else
            {
                forcedCrouch = true;
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping && !Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
        {
            jumpBufferCounter = 0f;

            isSliding = false;

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            StartCoroutine(JumpCooldown());
        }
    }

    void HeightHandler()
    {
        if (isCrouching || isSliding)
        {
            controller.height = crouchHeight;
            controller.center = crouchingCenter;
            camera.localPosition = Vector3.Lerp(camera.localPosition, new Vector3(camera.localPosition.x, 0f, camera.localPosition.z), Time.deltaTime * 5);
        }
        else
        {
            controller.height = standHeight;
            controller.center = standCenter;
            camera.localPosition = Vector3.Lerp(camera.localPosition, new Vector3(camera.localPosition.x, 0.7f, camera.localPosition.z), Time.deltaTime * 5);
        }
    }

    void Gravity()
    {
        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.3f);
        isJumping = false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheck.position, groundDistance, ground);
    }
}
