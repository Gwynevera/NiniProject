using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    bool parrying;
    bool porrying;

    float parryParryTime = 0.25f;
    float parryPorryTime = 0.55f;
    float parryTimer;

    float parryKnockback = 5f;

    float parryFriction = 0.7755f;

    public GameObject parry;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetParryInput() && playerManager.CanDoAction(PlayerAction.Parry))
        {
            playerManager.MyState = PlayerState.Parrying;

            parrying = true;
            parryTimer = 0;
            porrying = false;

            parry.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (playerManager.MyState == PlayerState.Parrying)
        {
            if (rb.linearVelocity != Vector3.zero)
            {
                rb.linearVelocity *= parryFriction;
            }

            if (parrying)
            {
                parryTimer += Time.fixedDeltaTime;

                if (parryTimer >= parryParryTime)
                {
                    parrying = false;
                    parryTimer = 0;
                    porrying = true;

                    parry.SetActive(false);
                }
            }

            if (porrying)
            {
                parryTimer += Time.fixedDeltaTime;

                if (parryTimer >= parryPorryTime)
                {
                    parrying = false;
                    parryTimer = 0;
                    porrying = false;

                    playerManager.MyState = PlayerState.Idle;
                }
            }
        }
    }

    bool GetParryInput()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().parryKey].wasPressedThisFrame)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().parryButton].wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
}
