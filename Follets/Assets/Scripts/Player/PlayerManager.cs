using UnityEngine;
using System;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Charging,
    Throwing,
    Parrying,
    Rolling,
    Hitstopping,
    Knockbacking,
}

public enum PlayerAction
{
    Move,
    Attack,
    Charge,
    Throw,
    Parry,
    Roll,
    Hitstop,
    Knockback,
    None
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerState myState;
    public PlayerState MyState
    {
        get => myState;
        set
        {
            if (myState != value)
            {
                myState = value;

                switch (value)
                {
                    case PlayerState.Throwing:
                        OnResetAttack?.Invoke();
                        break;

                    case PlayerState.Rolling:
                        OnResetAttack?.Invoke();
                        OnResetThrow?.Invoke();
                        break;

                    case PlayerState.Hitstopping:
                    case PlayerState.Knockbacking:
                        if (GetComponent<PlayerHitstop>().Damaged)
                        {
                            OnResetAttack?.Invoke();
                            OnResetThrow?.Invoke();
                            OnResetRoll?.Invoke();
                            OnResetParry?.Invoke();
                        }
                        break;
                }
            }
        }
    }

    public event Action OnResetAttack;
    public event Action OnResetRoll;
    public event Action OnResetThrow;
    public event Action OnResetParry;
    public event Action OnGetWeapon;
    public event Action OnDropWeapon;

    public int health = 3;
    public float width = 1;

    public GameObject myWeapon;
    public Prop myProp;

    public Transform weaponHandle;

    private void Awake()
    {

    }

    public void GetWeapon(GameObject weapon)
    {
        if (myWeapon == null)
        {

        }
        else
        {
            // Drop current
            myWeapon.GetComponent<WeaponObject>().DropWeapon(myWeapon.transform);
            OnDropWeapon?.Invoke();
        }

        myWeapon = weapon;
        OnGetWeapon?.Invoke();
    }

    public bool CanDoAction(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Move:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || GetComponent<PlayerThrow>().Thrown
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Attack:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || myState == PlayerState.Throwing
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Charge:
                if (myWeapon == null
                    || myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || myState == PlayerState.Throwing
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Throw:
                if (myWeapon == null
                    || myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || myState == PlayerState.Throwing
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Parry:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || myState == PlayerState.Throwing
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Roll:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || GetComponent<PlayerThrow>().Thrown
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Hitstop:
                if (myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Knockback:
                if (myState == PlayerState.Knockbacking
                    || myState == PlayerState.Hitstopping)
                    return false;
                return true;

            case PlayerAction.None:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Parrying
                    || myState == PlayerState.Throwing
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            default:
                return false;
        }
    }
}
