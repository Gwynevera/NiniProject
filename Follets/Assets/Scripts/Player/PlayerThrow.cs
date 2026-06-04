using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerThrow : MonoBehaviour
{
    PlayerManager playerManager;
    Rigidbody rb;

    public event Action OnThrowWeapon;

    bool thrown;
    public bool Thrown => thrown;

    float throwRecoverTime = 0.5f;
    float throwTimer;

    Vector3 throwDir;
    float throwOffset = 1f;
    float throwSpeed = 3f;

    float throwHoldTime = 1.5f;
    float throwHoldExtraForce = 5f;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        playerManager.OnResetThrow += ResetThrow;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnResetThrow -= ResetThrow;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetThrowInput())
        {
            if (playerManager.CanDoAction(PlayerAction.Throw))
            {
                thrown = true;

                if (throwTimer > throwHoldTime)
                {
                    throwTimer = throwHoldTime;
                }

                float throwExtraSpeed = throwTimer * throwHoldExtraForce / throwHoldTime;

                throwTimer = 0;

                throwDir = GetComponent<PlayerMovement>().GetMovementInput().normalized;
                if (throwDir != Vector3.zero)
                {
                    GetComponent<PlayerMovement>().DesiredForward = throwDir;
                }
                else
                {
                    throwDir = transform.forward;
                }

                rb.linearVelocity = Vector3.zero;
                rb.AddForce(throwDir * throwSpeed, ForceMode.VelocityChange);

                playerManager.myWeapon.GetComponent<WeaponObject>().ThrowWeapon(transform.position + (throwDir * throwOffset), throwDir, this.gameObject, throwExtraSpeed);
                playerManager.myWeapon = null;

                OnThrowWeapon?.Invoke();
            }
        }

        if (IsThrowHold())
        {
            if (playerManager.myWeapon != null)
            {
                if (playerManager.CanDoAction(PlayerAction.Move))
                {
                    playerManager.MyState = PlayerState.Throwing;

                    GetComponent<PlayerMovement>().DesiredForward = GetComponent<PlayerMovement>().GetMovementInput().normalized;
                }

                throwTimer += Time.fixedDeltaTime;
            }
            else
            {

            }
        }

        if (thrown)
        {
            throwTimer += Time.fixedDeltaTime;
            if (throwTimer >= throwRecoverTime)
            {
                playerManager.MyState = PlayerState.Idle;
                throwTimer = 0;
                thrown = false;
            }
        }
    }

    bool GetThrowInput()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().throwKey].wasReleasedThisFrame)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().throwButton].wasReleasedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    bool IsThrowHold()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().throwKey].isPressed)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().throwButton].isPressed)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetThrow()
    {
        throwTimer = 0;
        thrown = false;
    }
}
