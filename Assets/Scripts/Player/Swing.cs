using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform weaponTip;
    public LayerMask ground;

    private LineRenderer lr;
    private Rigidbody rb;
    private Transform cam;
    private Transform GroundCheck;
    private float groundDistance;
    private float gravity;

    [Header("Swinging")]
    public float thrust;
    public float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;
    private bool swinging;
    private bool afterSwing;

    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    private MovementInputSystem moveScript;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector3 moveDir;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    private float smoothInputSpeed;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0);
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        moveScript = GetComponent<MovementInputSystem>();
        currentInputVector = moveScript.currentInputVector;
        smoothInputVelocity = moveScript.smoothInputVelocity;
        smoothInputSpeed = moveScript.smoothInputSpeed;
        GroundCheck = moveScript.GroundCheck;
        groundDistance = moveScript.groundDistance;
        gravity = moveScript.gravity;
    }

    void Update()
    {
        if (swinging)
        {
            moveScript.enabled = false;
            controller.enabled = false;
            AirMovement();
        }
        else if (afterSwing)
        {
            moveScript.enabled = false;
            controller.enabled = false;
            AirMovement();
        }
        
        if (IsGrounded() && !swinging)
        {
            afterSwing = false;
            moveScript.enabled = true;
            rb.isKinematic = true;
            controller.enabled = true;
        }

        DrawRope();

        CheckForSwingPoints();

        Gravity();
    }

    void CheckForSwingPoints()
    {
        if (joint != null)
        {
            return;
        }
        else
        {
            RaycastHit sphereCastHit;
            Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, ground);

            RaycastHit rayCastHit;
            Physics.Raycast(cam.position, cam.forward, out rayCastHit, maxSwingDistance, ground);

            Vector3 realHitPoint;

            if (rayCastHit.point != Vector3.zero)
            {
                realHitPoint = rayCastHit.point;
            }
            else if (sphereCastHit.point != Vector3.zero)
            {
                realHitPoint = sphereCastHit.point;
            }
            else
            {
                realHitPoint = Vector3.zero;
            }

            if (realHitPoint != Vector3.zero)
            {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = realHitPoint;
            }
            else
            {
                predictionPoint.gameObject.SetActive(false);
            }
            predictionHit = rayCastHit.point == Vector3.zero ? sphereCastHit : rayCastHit;
        }
    }

    void AirMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);

        moveDir = transform.right * currentInputVector.x + transform.forward * currentInputVector.y;

        if (moveDir.magnitude > 1)
        {
            moveDir /= moveDir.magnitude;
        }

        rb.AddForce(moveDir * thrust * Time.deltaTime);
    }

    void DrawRope()
    {
        if (joint == null)
        {
            currentGrapplePosition = weaponTip.position;
            return;
        }
        else
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 5f);

            lr.SetPosition(0, weaponTip.position);
            lr.SetPosition(1, currentGrapplePosition);
        }
    }

    public void SwingHandler(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (predictionHit.point == Vector3.zero)
            {
                return;
            }
            else
            {
                lr.positionCount = 2;
                rb.isKinematic = false;

                if (!afterSwing)
                {
                    rb.linearVelocity = GetComponent<MovementInputSystem>().currentVelocity;
                }

                swinging = true;

                swingPoint = predictionHit.point;
                joint = this.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = swingPoint;

                float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

                joint.maxDistance = distanceFromPoint * 0.5f;
                joint.minDistance = distanceFromPoint * 0.25f;

            }
        }

        if (context.canceled && joint != null)
        {
            Destroy(joint);

            rb.isKinematic = false;
            
            swinging = false;
            afterSwing = true;
            lr.positionCount = 0;
        }
    }

    void Gravity()
    {
        if (rb.isKinematic == false)
        {
            if (joint == null && !IsGrounded())
            {
                rb.AddForce(new Vector3(0f, gravity * Time.deltaTime, 0f) * 100f);
            }
            else if (swinging)
            {
                rb.AddForce(new Vector3(0f, gravity * Time.deltaTime, 0f) * 50f);
            }
        }
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheck.position, groundDistance, ground);
    }
}
