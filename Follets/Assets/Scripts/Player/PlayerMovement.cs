using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    PlayerManager pManager;
    Rigidbody rb;

    float moveSpeed = 7.5f;
    float lerpSpeed = 15f;

    float minStickMovement = 0.05f;

    Vector3 movementInput;

    float chargeMoveSpeed = 4f;

    void Awake()
    {
        pManager = GetComponent<PlayerManager>();

        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (pManager.CanDoAction(PlayerAction.Move))
        {
            movementInput = GetMovementInput();
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector3.zero && pManager.CanDoAction(PlayerAction.Move))
        {
            float speed = pManager.myState == PlayerState.Charging ? chargeMoveSpeed : moveSpeed;

            pManager.myState = PlayerState.Moving;

            Vector3 desiredVelocity = movementInput * speed;
            Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            transform.forward = Vector3.Lerp(transform.forward, movementInput, lerpSpeed * Time.fixedDeltaTime);
        }
        else if (pManager.CanDoAction(PlayerAction.None))
        {
            pManager.myState = PlayerState.Idle;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public Vector3 GetMovementInput()
    {
        Vector3 movement = Vector3.zero;

        if (Keyboard.current != null && GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            float xMov = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
            float yMov = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
            movement = new Vector3(xMov, 0, yMov);
        }

        if (Gamepad.current != null && GetComponent<PlayerButtons>().playerInput == PlayerInput.Gamepad)
        {
            Vector2 gamepadMove = Vector2.zero;
            gamepadMove += Gamepad.current.leftStick.ReadValue();

            if (gamepadMove.magnitude < minStickMovement)
                gamepadMove = Vector3.zero;

            movement = new Vector3(gamepadMove.x, 0, gamepadMove.y);
        }

        return movement;
    }
}
