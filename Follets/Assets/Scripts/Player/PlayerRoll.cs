using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRoll : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    float rollSpeed = 20f;
    float rollFriction = 0.875f;
    Vector3 rollDir;

    float rollDuration = 0.5f;
    float rollInvencible = 0.3f;
    float rollTimer = 0f;

    public bool invencible = false;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        playerManager.OnResetRoll += ResetRoll;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnResetRoll -= ResetRoll;
        }
    }

    void Update()
    {
        if (GetRollInput() && playerManager.CanDoAction(PlayerAction.Roll))
        {
            StartRoll();
        }
    }

    void FixedUpdate()
    {
        if (playerManager.MyState == PlayerState.Rolling)
        {
            rollTimer += Time.fixedDeltaTime;
            invencible = rollTimer < rollInvencible;

            if (invencible)
            {
                Utils.DrawOverlapBox(transform.position, Vector3.one, transform.rotation, Color.yellow);
            }
            else
            {
                rb.linearVelocity *= rollFriction;
            }

            if (rollTimer >= rollDuration)
            {
                playerManager.MyState = PlayerState.Idle;
            }
        }
    }

    void StartRoll()
    {
        playerManager.MyState = PlayerState.Rolling;

        rollDir = GetComponent<PlayerMovement>().GetMovementInput().normalized;
        if (rollDir != Vector3.zero)
        {
            GetComponent<PlayerMovement>().DesiredForward = rollDir;
        }
        else
        {
            rollDir = transform.forward;
        }

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(rollDir * rollSpeed, ForceMode.VelocityChange);

        invencible = true;
        rollTimer = 0f;
    }

    bool GetRollInput()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().rollKey].wasPressedThisFrame)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().rollButton].wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetRoll()
    {
        invencible = false;
        rollTimer = 0;
    }
}
