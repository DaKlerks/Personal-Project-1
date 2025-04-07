using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{

    public Transform playerBody;

    private PlayerInput playerInput;
    private InputAction lookAction;

    float xRotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = transform.parent.GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = lookAction.ReadValue<Vector2>();

        float mouseX = input.x * Time.deltaTime;
        float mouseY = input.y * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
