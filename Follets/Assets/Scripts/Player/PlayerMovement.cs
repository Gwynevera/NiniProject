using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    float moveSpeed = 7.5f;
    float slownMoveSpeed = 4f;
    float lerpSpeed = 15f;
    float fastLerpSpeed = 20f;

    float minStickMovement = 0.15f;

    Vector3 movementInput;

    Vector3 desiredForward;
    public Vector3 DesiredForward
    {
        get { return desiredForward; }
        set 
        { 
            desiredForward = value;
            Debug.Log("Set " + DesiredForward);
        }
    }


    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();

        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (playerManager.CanDoAction(PlayerAction.Move))
        {
            movementInput = GetMovementInput();
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector3.zero)
        {
            if (playerManager.CanDoAction(PlayerAction.Move))
            {
                float speed = playerManager.MyState == PlayerState.Charging || playerManager.MyState == PlayerState.Throwing ? slownMoveSpeed : moveSpeed;

                Vector3 desiredVelocity = movementInput * speed;
                Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                transform.forward = Vector3.Lerp(transform.forward, movementInput, lerpSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (playerManager.CanDoAction(PlayerAction.None))
            {
                rb.linearVelocity = Vector3.zero;
            }
        }

        if (playerManager.MyState == PlayerState.Attacking
            || playerManager.MyState == PlayerState.Rolling
            || playerManager.MyState == PlayerState.Throwing
            || playerManager.MyState == PlayerState.Parrying)
        {
            transform.forward = Vector3.Lerp(transform.forward, desiredForward, fastLerpSpeed * Time.fixedDeltaTime);
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

        // Marcos
        ///movement.x += Input.GetAxis("Horizontal");
        ///movement.z += Input.GetAxis("Vertical");

        //if (movement.magnitude < minStickMovement)
            //movement = Vector3.zero;

        return movement;
    }
}
