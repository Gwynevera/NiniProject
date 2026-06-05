using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    bool parrying;
    bool porrying;
    bool perrying;

    float parryTime = 0.5f;
    float porryTime = 1f;
    float perryTime = 0.45f;
    float parryTimer;

    float parryKnockback = 5f;

    float parryFriction = 0.7755f;
    float perryFriction = 0.957f;

    public GameObject parry;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        playerManager.OnResetParry += ResetParry;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnResetParry -= ResetParry;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetParryInput() && (playerManager.MyState != PlayerState.Parrying || perrying))
        {
            playerManager.MyState = PlayerState.Parrying;

            parryTimer = 0;
            parrying = true;
            porrying = false;
            perrying = false;

            parry.SetActive(true);

            GetComponent<PlayerMovement>().DesiredForward = GetComponent<PlayerMovement>().GetMovementInput().normalized;
        }
    }

    void FixedUpdate()
    {
        if (playerManager.MyState == PlayerState.Parrying)
        {
            if (rb.linearVelocity != Vector3.zero)
            {
                rb.linearVelocity *= perrying ? perryFriction : parryFriction;
            }

            // Parry is ACTIVE
            if (parrying)
            {
                parryTimer += Time.fixedDeltaTime;

                if (parryTimer >= parryTime)
                {
                    parryTimer = playerManager.myWeapon == null ? parryTime/2 : 0;
                    parrying = false;
                    porrying = true;
                    perrying = false;

                    parry.SetActive(false);
                }
            }

            // Parry is DISABLED
            if (porrying)
            {
                parryTimer += Time.fixedDeltaTime;

                if (parryTimer >= porryTime)
                {
                    ResetParry();
                    playerManager.MyState = PlayerState.Idle;
                }
            }

            // Parry was SUCCESSFUL
            if (perrying)
            {
                parryTimer += Time.fixedDeltaTime;

                if (parryTimer >= perryTime)
                {
                    ResetParry();
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

    public void ParrySuccessful(Vector3 dir, float mult)
    {
        parry.SetActive(false);

        parryTimer = 0;
        parrying = false;
        porrying = false;
        perrying = true;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir.normalized * parryKnockback * mult, ForceMode.VelocityChange);

        GetComponent<PlayerMovement>().DesiredForward = -dir;
    }

    void ResetParry()
    {
        parry.SetActive(false);
        parrying = false;
        porrying = false;
        perrying = false;
        parryTimer = 0;
    }
}
