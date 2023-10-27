using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] float speed, mouseSensibility, jumpForce = 5f, gravity = -9.81f;
    [SerializeField] Vector2 xRotationLimits;

    [SerializeField] bool invertedYRotation;

    private CharacterController charCon;
    private Controls controls;
    private Vector2 eulerRotation = Vector2.zero;
    private Vector3 velocity;

    private void Awake()
    {
        controls = new();
        charCon = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        Rotation();
        Shoot();
    }

    private void Movement()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        Vector3 direction = (transform.forward * input.y + transform.right * input.x).normalized;
        charCon.Move(speed * Time.deltaTime * direction);

        // Jump logic
        if (charCon.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Small value to ensure we're grounded
        }

        if (controls.Player.Jump.triggered && charCon.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        charCon.Move(velocity * Time.deltaTime);
    }

    private void Rotation()
    {
        Vector2 delta = controls.Player.PointerDelta.ReadValue<Vector2>().normalized;
        transform.Rotate(new Vector3(0, delta.x * mouseSensibility, 0));
        eulerRotation.y = transform.rotation.y;
        eulerRotation.x = Mathf.Clamp(delta.y * mouseSensibility + eulerRotation.x, xRotationLimits.x, xRotationLimits.y);
        head.localRotation = Quaternion.Euler(invertedYRotation ? -eulerRotation.x : eulerRotation.x, 0, 0);
    }

    private void Shoot()
    {
        if (controls.Player.Shoot.triggered)
        {
            RaycastHit hit;
            if (Physics.Raycast(head.position, head.forward, out hit, Mathf.Infinity))
            {
                Debug.Log("Hit object: " + hit.transform.name);
            }
        }
    }
}
