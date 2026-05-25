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

    float attackPrepareTime = 0.2f;
    float attackActiveTime = 0.25f;
    float attackRecoverTime = 0.2f;

    float attackMoveSpeed = 10f;
    float attackFriction = 0.65f;
    public float AttackMoveSpeed => attackMoveSpeed;

    bool hitboxActive;
    float hitboxOffset = 0.75f;
    Vector3 hitboxSize = new Vector3(1, 1, 1);
    Vector3 chargeHitboxSize = new Vector3(1.5f, 1, 2.5f);
    float chargeHitboxOffset = 1;

    float lerpSpeed = 10f;
    float chargedLerpSpeed = 25f;

    bool charged;
    public bool Charged => charged;

    float chargeMinTime = 0.25f;
    float chargeTime = 1.25f;
    float chargeTimer;

    float chargeActiveTime = 0.25f;
    float chargeRecoveryTime = 0.65f;

    float chargeMoveSpeed = 20f;
    float chargeFriction = 0.85f;
    public float ChargeMoveSpeed => chargeMoveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetAttackInput() || buffered)
        {
            if (playerManager.CanDoAction(PlayerAction.Attack))
            {
                playerManager.myState = PlayerState.Attacking;

                attackTimer = 0;
                buffered = false;
                rb.linearVelocity = Vector3.zero;

                attackDirection = GetComponent<PlayerMovement>().GetMovementInput();

                if (charged)
                {
                    rb.AddForce(transform.forward * chargeMoveSpeed, ForceMode.VelocityChange);
                }
            }
            else
            {
                buffered = true;
                bufferTimer = 0;
            }
        }

        if (IsAttackHold() && playerManager.CanDoAction(PlayerAction.Charge))
        {
            if (playerManager.myState != PlayerState.Charging)
            {
                chargeTimer += Time.fixedDeltaTime;
                if (chargeTimer >= chargeMinTime)
                {
                    playerManager.myState = PlayerState.Charging;
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
        if (playerManager.myState == PlayerState.Attacking)
        {
            attackTimer += Time.fixedDeltaTime;

            transform.forward = Vector3.Lerp(transform.forward, attackDirection, ((charged || attackTimer < attackPrepareTime) ? chargedLerpSpeed : lerpSpeed) * Time.fixedDeltaTime);

            if (charged)
            {
                if (attackTimer < chargeActiveTime)
                {
                    HitboxCheck(KnockbackType.Big, chargeHitboxOffset, chargeHitboxSize / 2);
                }
                else
                {
                    rb.linearVelocity *= chargeFriction;

                    if (attackTimer >= chargeActiveTime + chargeRecoveryTime)
                    {
                        charged = false;
                        chargeTimer = 0;
                        playerManager.myState = PlayerState.Idle;
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
                            rb.AddForce(transform.forward * attackMoveSpeed, ForceMode.VelocityChange);
                        }

                        HitboxCheck(KnockbackType.Small, hitboxOffset, hitboxSize / 2);
                    }
                    else if (attackTimer >= attackPrepareTime + attackActiveTime)
                    {
                        hitboxActive = false;
                        rb.linearVelocity *= attackFriction;
                    }

                    if (attackTimer >= attackPrepareTime + attackActiveTime + attackRecoverTime)
                    {
                        playerManager.myState = PlayerState.Idle;
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
        Collider[] objects = Physics.OverlapBox(transform.position + (boxOffset * transform.forward), boxSize, transform.rotation);

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
                            obj.GetComponent<PlayerKnockback>().SetKnockbackDamage(dir.normalized, knockType);

                            GetComponent<PlayerHitstop>().StartBullyHitstop(playerManager.myState, rb.linearVelocity, knockType == KnockbackType.Big);
                        }
                    }
                }
            }
        }
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
