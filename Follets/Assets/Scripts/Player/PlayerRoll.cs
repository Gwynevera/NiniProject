using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRoll : MonoBehaviour
{
    PlayerManager pManager;
    Rigidbody rb;

    bool rollOnce;
    float rollSpeed = 20f;
    float rollFriction = 0.875f;
    Vector3 rollDir;

    float rollDuration = 0.45f;
    float rollInvencible = 0.2f;
    float rollTimer = 0f;

    public bool invencible = false;

    void Awake()
    {
        pManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GetRollInput() && pManager.CanDoAction(PlayerAction.Roll))
        {
            StartRoll();
        }
    }

    void FixedUpdate()
    {
        if (pManager.myState == PlayerState.Rolling)
        {
            if (rollOnce)
            {
                rollOnce = false;

                rb.linearVelocity = Vector3.zero;
                rb.AddForce(rollDir * rollSpeed, ForceMode.VelocityChange);
            }

            rollTimer += Time.fixedDeltaTime;
            invencible = rollTimer < rollInvencible;

            if (!invencible)
            {
                rb.linearVelocity *= rollFriction;
            }

            if (rollTimer >= rollDuration)
            {
                pManager.myState = PlayerState.Idle;
                invencible = false;
            }
        }
    }

    void StartRoll()
    {
        pManager.myState = PlayerState.Rolling;

        rollTimer = 0f;
        invencible = true;

        rollDir = transform.forward.normalized;
        rollOnce = true;

        rb.linearVelocity = Vector3.zero;
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
}
