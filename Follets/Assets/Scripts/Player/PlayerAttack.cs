using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    bool buffered;
    float bufferTime = 0.25f;
    float bufferTimer = 0;

    Vector3 attackDirection;
    float attackTimer;

    float attackPrepareTime = 0.1f;
    float attackActiveTime = 0.15f;
    float attackRecoverTime = 0.2f;

    float attackMoveSpeed = 10f;
    float attackFriction = 0.65f;
    public float AttackMoveSpeed => attackMoveSpeed;

    bool hitboxActive;
    float hitboxOffset = 0.75f;
    Vector3 hitboxSize = new Vector3(1, 1, 1);
    Vector3 chargeHitboxSize = new Vector3(1.5f, 1, 2.5f);
    float chargeHitboxOffset = 1;

    [SerializeField]
    bool charged;
    public bool Charged => charged;

    float chargeMinTime = 0.25f;
    float chargeTime = 0.75f;
    [SerializeField]
    float chargeTimer;

    float chargeActiveTime = 0.25f;
    float chargeRecoveryTime = 0.65f;

    float chargeMoveSpeed = 20f;
    float chargeFriction = 0.85f;
    public float ChargeMoveSpeed => chargeMoveSpeed;

    [SerializeField]
    bool tooMuchHold;
    float holdTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        playerManager.OnResetAttack += ResetAttack;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnResetAttack -= ResetAttack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetAttackInput() || buffered)
        {
            if (!tooMuchHold)
            {
                if (playerManager.CanDoAction(PlayerAction.Attack))
                {
                    playerManager.MyState = PlayerState.Attacking;

                    attackTimer = 0;
                    buffered = false;

                    attackDirection = GetComponent<PlayerMovement>().GetMovementInput();
                    if (attackDirection != Vector3.zero)
                    {
                        GetComponent<PlayerMovement>().DesiredForward = attackDirection;
                    }
                    else
                    {
                        attackDirection = transform.forward;
                    }

                    if (charged)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.AddForce(attackDirection * chargeMoveSpeed, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    buffered = true;
                    bufferTimer = 0;
                }
            }

            chargeTimer = 0;
            tooMuchHold = false;
            holdTimer = 0;
        }

        if (IsAttackHold())
        {
            if (playerManager.CanDoAction(PlayerAction.Charge))
            {
                if (playerManager.MyState != PlayerState.Charging)
                {
                    chargeTimer += Time.fixedDeltaTime;
                    if (chargeTimer >= chargeMinTime)
                    {
                        playerManager.MyState = PlayerState.Charging;
                    }
                }
                else
                {
                    if (!charged)
                    {
                        chargeTimer += Time.fixedDeltaTime;
                        if (chargeTimer >= chargeTime)
                        {
                            charged = true;
                        }
                    }
                }
            }
            else
            {
                holdTimer += Time.fixedDeltaTime;
                if (holdTimer >= chargeMinTime)
                {
                    tooMuchHold = true;
                }
            }
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
        if (playerManager.MyState == PlayerState.Attacking)
        {
            attackTimer += Time.fixedDeltaTime;

            if (charged)
            {
                if (attackTimer < chargeActiveTime)
                {
                    HitboxCheck(KnockbackType.Big, chargeHitboxOffset, chargeHitboxSize);
                }
                else
                {
                    rb.linearVelocity *= chargeFriction;

                    if (attackTimer >= chargeActiveTime + chargeRecoveryTime)
                    {
                        charged = false;
                        chargeTimer = 0;
                        playerManager.MyState = PlayerState.Idle;
                    }
                }
            }
            else
            {
                if (attackTimer >= attackPrepareTime)
                {
                    if (attackTimer < attackPrepareTime + attackActiveTime)
                    {
                        if (!hitboxActive)
                        {
                            hitboxActive = true;

                            rb.linearVelocity = Vector3.zero;
                            rb.AddForce(attackDirection * attackMoveSpeed, ForceMode.VelocityChange);
                        }

                        HitboxCheck(KnockbackType.Small, hitboxOffset, hitboxSize);
                    }
                    else if (attackTimer >= attackPrepareTime + attackActiveTime)
                    {
                        hitboxActive = false;
                        rb.linearVelocity *= attackFriction;
                    }

                    if (attackTimer >= attackPrepareTime + attackActiveTime + attackRecoverTime)
                    {
                        playerManager.MyState = PlayerState.Idle;
                    }
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

    bool IsAttackHold()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().attackKey].isPressed)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().attackButton].isPressed)
            {
                return true;
            }
        }
        return false;
    }

    void HitboxCheck(KnockbackType knockType, float boxOffset, Vector3 boxSize)
    {
        Vector3 boxCenter = transform.position + (boxOffset * attackDirection);
        Collider[] objects = Physics.OverlapBox(boxCenter, boxSize, transform.rotation);

        // Dibujar la caja del OverlapBox
        DrawOverlapBox(boxCenter, boxSize, transform.rotation, knockType == KnockbackType.Big ? Color.red : Color.blue);

        if (objects != null && objects.Length > 0)
        {
            foreach (Collider obj in objects)
            {
                if (obj.gameObject != this.gameObject)
                {
                    if (obj.tag == "Player")
                    {
                        if (obj.GetComponent<PlayerManager>().CanDoAction(PlayerAction.Knockback)
                            && !obj.GetComponent<PlayerRoll>().invencible)
                        {
                            Vector3 dir = obj.transform.position - transform.position;
                            obj.GetComponent<PlayerKnockback>().SetKnockbackDamage(dir.normalized, knockType, playerManager.myWeapon != null);

                            GetComponent<PlayerHitstop>().StartBullyHitstop(playerManager.MyState, rb.linearVelocity, knockType == KnockbackType.Big);
                        }
                    }
                }
            }
        }
    }

    private void ResetAttack()
    {
        hitboxActive = false;
        charged = false;
        buffered = false;
        tooMuchHold = false;

        attackTimer = 0;
        chargeTimer = 0;
        bufferTimer = 0;
        holdTimer = 0;
    }


    private void DrawOverlapBox(Vector3 center, Vector3 size, Quaternion rotation, Color color)
    {
        // Calcula los 8 vértices de la caja
        Vector3 halfSize = size * 0.5f;
        Vector3[] corners = new Vector3[8]
        {
            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(halfSize.x, halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, halfSize.z)
        };

        // Aplica rotación y posición a cada esquina
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = center + rotation * corners[i];
        }

        // Dibuja las 12 líneas (bordes de la caja)
        // Cara frontal
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[2], color);
        Debug.DrawLine(corners[2], corners[3], color);
        Debug.DrawLine(corners[3], corners[0], color);

        // Cara trasera
        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[6], color);
        Debug.DrawLine(corners[6], corners[7], color);
        Debug.DrawLine(corners[7], corners[4], color);

        // Conexiones entre caras
        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

    private void OnDrawGizmos()
    {
        //if (playerManager.myState == PlayerState.Attacking)
        {
            //Vector3 boxSize = charged ? chargeHitboxSize : hitboxSize;
            //float boxOffset = charged ? chargeHitboxOffset : hitboxOffset;
            //Gizmos.DrawWireCube(transform.position + (boxOffset * transform.forward), boxSize);
        }
    }
}
