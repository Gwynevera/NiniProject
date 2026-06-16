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

    float attackPrepareTime;
    float minAttackPrepareTime = 0.1f;
    float maxAttackPrepareTime = 0.35f;
    float attackPrepareTimeBase = 0.2f;
    float attackPrepareMult = 0.55f;

    float attackActiveTime;
    float minAttackActiveTime = 0.1f;
    float maxAttackActiveTime = 0.25f;
    float attackActiveTimeBase = 0.15f;
    float attackActiveMult = 0.5f;

    float attackRecoverTime;
    float minAttackRecoverTime = 0.1f;
    float maxAttackRecoverTime = 0.5f;
    float attackRecoverTimeBase = 0.25f;
    float attackRecoverMult = 0.35f;

    float attackMoveSpeed;
    float minAttackSpeed = 8.75f;
    float maxAttackSpeed = 15f;
    float attackMoveSpeedBase = 10f;
    float attackMoveSpeedMult = 0.75f;

    float attackFriction = 0.65f;
    /// float attackRotate = 0.5f;
    
    public float AttackMoveSpeed => attackMoveSpeed;

    bool hitboxActive;
    Weapon weaponBase;
    Vector3 hitBox;
    float swingOffset;
    Vector3 swingHitBoxBase = new Vector3(2, 1, 1.75f);
    Vector2 swingHitBoxMult = new Vector2(0.25f, 0.65f);
    Vector3 chargeHitBox;
    float chargeOffset;
    Vector3 chargeHitBoxBase = new Vector3(2, 1, 3);
    Vector3 chargeHitBoxMult = new Vector2(0.15f, 0.45f);

    [SerializeField]
    bool charged;
    public bool Charged => charged;

    float chargeMinTime = 0.25f;
    float chargeTime = 0.75f;
    [SerializeField]
    float chargeTimer;

    float chargeActiveTime;
    float chargeActiveTimeBase = 0.25f;
    float chargeActiveMult = 0.5f;

    float chargeRecoveryTime;
    float chargeRecoveryTimeBase = 0.65f;
    float chargeRecoveryMult = 0.15f;

    float chargeMoveSpeed;
    float minChargeSpeed = 12f;
    float chargeMoveSpeedBase = 20f;
    float chargeMoveSpeedMult = 0.5f;

    float chargeFriction = 0.85f;

    public float ChargeMoveSpeed => chargeMoveSpeed;

    [SerializeField]
    bool tooMuchHold;
    float holdTimer;

    bool parried;
    public bool Parried
    {
        set
        {
            parried = value;
        }
    }

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        weaponBase = new Weapon();

        attackPrepareTime = attackPrepareTimeBase;
        attackActiveTime = attackActiveTimeBase;
        attackRecoverTime = attackRecoverTimeBase;

        attackMoveSpeed = attackMoveSpeedBase;
        attackDirection = transform.forward;

        playerManager.OnResetAttack += ResetAttack;
        playerManager.OnGetWeapon += UpdateAttackStats;
        playerManager.OnDropWeapon += ResetAttackStats;
        GetComponent<PlayerThrow>().OnThrowWeapon += ResetAttackStats;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnResetAttack -= ResetAttack;
            playerManager.OnGetWeapon -= UpdateAttackStats;
            playerManager.OnDropWeapon -= ResetAttackStats;
            GetComponent<PlayerThrow>().OnThrowWeapon -= ResetAttackStats;
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
                    parried = false;

                    attackDirection = GetComponent<PlayerMovement>().GetMovementInput();
                    if (attackDirection.magnitude < GetComponent<PlayerMovement>().minStickMovement)
                    {
                        attackDirection = transform.forward;
                    }
                    else
                    {
                        attackDirection = attackDirection.normalized;
                    }

                    GetComponent<PlayerMovement>().DesiredForward = attackDirection;

                    if (charged)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.AddForce(attackDirection.normalized * chargeMoveSpeed, ForceMode.VelocityChange);
                    }
                }
                else if (attackTimer >= attackPrepareTime + (attackActiveTime/2))
                {
                    buffered = true;
                    bufferTimer = 0;
                }
            }

            tooMuchHold = false;
            holdTimer = 0;
            chargeTimer = 0;
            if (playerManager.MyState != PlayerState.Attacking) charged = false;
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
                    HitboxCheck(KnockbackType.Big, chargeOffset, chargeHitBox);
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

                        HitboxCheck(KnockbackType.Small, playerManager.myWeapon == null ? 1 : swingOffset, playerManager.myWeapon == null ? Vector3.one : hitBox);
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
                else
                {
                    rb.linearVelocity *= attackFriction;
                    if (rb.linearVelocity.magnitude < 0) rb.linearVelocity = Vector3.zero;
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
        if (parried) return;

        Vector3 boxCenter = transform.position + (boxOffset * attackDirection);
        Collider[] objects = Physics.OverlapBox(boxCenter, boxSize/2, transform.rotation);

        // Dibujar la caja del OverlapBox
        Utils.DrawOverlapBox(boxCenter, boxSize, transform.rotation, knockType == KnockbackType.Big ? Color.red : Color.blue);

        if (objects != null && objects.Length > 0)
        {
            foreach (Collider obj in objects)
            {
                if (obj.gameObject != this.gameObject)
                {
                    if (obj.name == "Parry")
                    {
                        if (obj.transform.parent != null)
                        {
                            Vector3 parryDir = obj.transform.position - transform.position;
                            parryDir.y = 0;
                            obj.GetComponentInParent<PlayerParry>().ParrySuccessful(parryDir.normalized, charged ? 2 : 1);
                            
                            obj.GetComponentInParent<PlayerHitstop>().StartBullyHitstop(obj.GetComponentInParent<PlayerManager>().MyState, obj.GetComponentInParent<Rigidbody>().linearVelocity, PlayerHitstop.smallHitstopTime);
                        }

                        parried = true;
                        GetComponent<PlayerHitstop>().StartBullyHitstop(playerManager.MyState, rb.linearVelocity, PlayerHitstop.smallHitstopTime, true);
                        
                        return;
                    }

                    if (obj.tag == "Player")
                    {
                        if (obj.GetComponent<PlayerManager>().CanDoAction(PlayerAction.Knockback)
                            && !obj.GetComponent<PlayerRoll>().invencible
                            && !obj.GetComponent<PlayerParry>().parry.activeSelf)
                        {
                            float hitTime = charged ? PlayerHitstop.bigHitstopTime : PlayerHitstop.smallHitstopTime;

                            Vector3 dir = obj.transform.position - transform.position;
                            obj.GetComponent<PlayerKnockback>().SetKnockbackDamage(dir.normalized, knockType, hitTime);

                            GetComponent<PlayerHitstop>().StartBullyHitstop(playerManager.MyState, rb.linearVelocity, hitTime);
                        }
                    }
                    
                }
            }
        }
    }

    private void ResetAttackStats()
    {
        attackPrepareTime = attackPrepareTimeBase;
        attackActiveTime = attackActiveTimeBase;
        attackRecoverTime = attackRecoverTimeBase;
        attackMoveSpeed = attackMoveSpeedBase;
    }

    private void UpdateAttackStats()
    {
        float wWidth = playerManager.myWeapon.GetComponent<WeaponObject>().weapon.width;
        float wLength = playerManager.myWeapon.GetComponent<WeaponObject>().weapon.length;
        float wWeight = playerManager.myWeapon.GetComponent<WeaponObject>().weapon.weight;

        attackPrepareTime = wWeight * attackPrepareTimeBase / weaponBase.weight;
        float prepDiff = attackPrepareTime - attackPrepareTimeBase;
        attackPrepareTime = attackPrepareTimeBase + (prepDiff * attackPrepareMult);
        if (attackPrepareTime < minAttackPrepareTime) attackPrepareTime = minAttackPrepareTime;
        if (attackPrepareTime > maxAttackPrepareTime) attackPrepareTime = maxAttackPrepareTime;

        attackActiveTime = wWeight * attackActiveTimeBase / weaponBase.weight;
        float activeDiff = attackActiveTime - attackActiveTimeBase;
        attackActiveTime = attackActiveTimeBase + (activeDiff * attackActiveMult);
        if (attackActiveTime < minAttackActiveTime) attackActiveTime = minAttackActiveTime;
        if (attackActiveTime > maxAttackActiveTime) attackActiveTime = maxAttackActiveTime;

        attackRecoverTime = wWeight * attackRecoverTimeBase / weaponBase.weight;
        float recoverDiff = attackRecoverTime - attackRecoverTimeBase;
        attackRecoverTime = attackRecoverTimeBase + (recoverDiff * attackRecoverMult);
        if (attackRecoverTime < minAttackRecoverTime) attackRecoverTime = minAttackRecoverTime;
        if (attackRecoverTime > maxAttackRecoverTime) attackRecoverTime = maxAttackRecoverTime;

        attackMoveSpeed = wWeight * attackMoveSpeedBase / weaponBase.weight;
        float moveDiff = attackMoveSpeedBase - attackMoveSpeed;
        attackMoveSpeed = attackMoveSpeedBase + (moveDiff * attackMoveSpeedMult);
        if (attackMoveSpeed < minAttackSpeed) attackMoveSpeed = minAttackSpeed;
        if (attackMoveSpeed > maxAttackSpeed) attackMoveSpeed = maxAttackSpeed;

        chargeActiveTime = wWeight * chargeActiveTimeBase / weaponBase.weight;
        float chActiveDiff = chargeActiveTime - chargeActiveTimeBase;
        chargeActiveTime = chargeActiveTimeBase + (chActiveDiff * chargeActiveMult);

        chargeRecoveryTime = wWeight * chargeRecoveryTimeBase / weaponBase.weight;
        float chRecoverDiff = chargeRecoveryTime - chargeRecoveryTimeBase;
        chargeRecoveryTime = chargeRecoveryTimeBase + (chRecoverDiff * chargeRecoveryMult);

        chargeMoveSpeed = wWeight * chargeMoveSpeedBase / weaponBase.weight;
        float chMoveDiff = chargeMoveSpeedBase - chargeMoveSpeed;
        chargeMoveSpeed = chargeMoveSpeedBase + (chMoveDiff * chargeMoveSpeedMult);
        if (chargeMoveSpeed < minChargeSpeed) chargeMoveSpeed = minChargeSpeed;

        hitBox = new Vector3(wWidth * swingHitBoxBase.x / weaponBase.width, 1, wLength * swingHitBoxBase.z / weaponBase.length);
        Vector3 swingDiff = hitBox - swingHitBoxBase;
        hitBox = swingHitBoxBase + new Vector3(swingDiff.x * swingHitBoxMult.x, 0, swingDiff.z * swingHitBoxMult.y);
        swingOffset = (playerManager.width / 2) + (hitBox.z / 2);

        chargeHitBox = new Vector3(wWidth * chargeHitBoxBase.x / weaponBase.width, 1, wLength * chargeHitBoxBase.z / weaponBase.length);
        Vector3 chargeDiff = chargeHitBox - chargeHitBoxBase;
        chargeHitBox = chargeHitBoxBase + new Vector3(chargeDiff.x * chargeHitBoxMult.x, 0, chargeDiff.z * chargeHitBoxMult.y);
        chargeOffset = (playerManager.width / 2) + (chargeHitBox.z / 2);
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

        parried = false;
        
        attackDirection = transform.forward;
        GetComponent<PlayerMovement>().DesiredForward = transform.forward;
    }
}
