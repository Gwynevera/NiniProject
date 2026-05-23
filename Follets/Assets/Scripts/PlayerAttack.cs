using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    Rigidbody rb;

    bool buffered;
    float bufferTime = 0.25f;
    float bufferTimer = 0;

    public GameObject attackBox;

    float attackPrepareTime = 0.45f;
    float attackActiveTime = 0.15f;
    float attackRecoverTime = 0.55f;
    float attackTimer;

    Vector3 attackDirection;
    float attackMoveSpeed = 10f;
    float attackFriction = 0.9f;

    float lerpSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetAttackInput() || buffered)
        {
            if (GetComponent<PlayerManager>().CanDoAction(PlayerAction.Attack))
            {
                GetComponent<PlayerManager>().myState = PlayerState.Attacking;

                attackTimer = 0;
                buffered = false;
                rb.linearVelocity = Vector3.zero;

                attackDirection = GetComponent<PlayerMovement>().GetMovementInput();
            }
            else
            {
                buffered = true;
                bufferTimer = 0;

            }
        }
        else if (GetComponent<PlayerManager>().myState != PlayerState.Attacking)
        {
            attackBox.SetActive(false);
        }

        if (buffered)
        {
            bufferTimer += Time.fixedDeltaTime;
            if (bufferTimer >= bufferTime)
            {
                buffered = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<PlayerManager>().myState == PlayerState.Attacking)
        {
            attackTimer += Time.fixedDeltaTime;

            transform.forward = Vector3.Lerp(transform.forward, attackDirection, lerpSpeed * Time.fixedDeltaTime);

            if (attackTimer >= attackPrepareTime)
            {
                if (attackTimer < attackPrepareTime + attackActiveTime)
                {
                    if (!attackBox.activeSelf)
                    {
                        rb.AddForce(transform.forward * attackMoveSpeed, ForceMode.VelocityChange);
                    }

                    attackBox.SetActive(true);
                }
                else if (attackTimer >= attackPrepareTime + attackActiveTime)
                {
                    attackBox.SetActive(false);
                    rb.linearVelocity *= attackFriction;
                }

                if (attackTimer >= attackPrepareTime + attackActiveTime + attackRecoverTime)
                {
                    GetComponent<PlayerManager>().myState = PlayerState.Idle;
                }
            }
        }
    }

    bool GetAttackInput()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().attackKey].wasReleasedThisFrame)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().attackButton].wasReleasedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
}
