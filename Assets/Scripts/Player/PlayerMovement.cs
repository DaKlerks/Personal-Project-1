using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Character Controller")]
    public CharacterController controller;
    private Rigidbody rb;

    private new Transform camera;

    [Header("Movement")]
    public float speed = 5f;
    private float originalSpeed;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;
    private bool isSprinting;
    private Vector3 velocity;

    private float x;
    private float z;
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
    public float crouchHeight;
    private Vector3 standCenter;
    private float standHeight;
    private bool isCrouching;

    [Header("Sliding")]
    public float slideSpeed;
    private float originalSlideSpeed;
    private bool isSliding;

    void Start()
    {
        camera = transform.GetChild(0);
        originalSpeed = speed;
        originalSlideSpeed = slideSpeed;
        standHeight = controller.height;
        standCenter = controller.center;

        //skin hooks on ceilings slowing movement, lowering step height stops this
    }

    void Update()
    {

        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Sprint();

        Crouch();

        Slide();

        HeightHandler();

        SpeedHandler();

        MovementHandler();

        Jump();

        //gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void MovementHandler()
    {
        if (!isSliding)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            moveDir = transform.right * x + transform.forward * z;

            if (moveDir.magnitude > 1)
            {
                moveDir /= moveDir.magnitude;
            }

            controller.Move(moveDir * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(moveDir * slideSpeed * Time.deltaTime);
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
        }
        else
        {
            slideSpeed = Mathf.Lerp(slideSpeed, originalSlideSpeed, Time.deltaTime * 5);
        }
    }

    public void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && IsGrounded() && !isSprinting || Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground) && !isSliding)
        {
            isCrouching = true;
        }
        else if (!Physics.CheckSphere(transform.position + new Vector3(0f,0.5f,0f), 0.5f, ground) && !isSliding)
        {
            isCrouching = false;
        }
    }

    public void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    public void Slide()
    {
        if (isSprinting && Input.GetKey(KeyCode.LeftControl) && IsGrounded() && speed > 5f && z > 0)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
            if (Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground) || Input.GetKey(KeyCode.LeftControl))
            {
                isSprinting = false;
                Crouch();
            }
        }
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping && !Physics.CheckSphere(transform.position + new Vector3(0f, 0.5f, 0f), 0.5f, ground))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferCounter = 0f;

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

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.3f);
        isJumping = false;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheck.position, groundDistance, ground);
    }
}
