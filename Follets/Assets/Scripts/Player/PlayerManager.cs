using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Charging,
    Rolling,
    Hitstopping,
    Knockbacking,
}

public enum PlayerAction
{
    Move,
    Attack,
    Charge,
    Roll,
    Hitstop,
    Knockback,
    None
}

public class PlayerManager : MonoBehaviour
{
    public PlayerState myState;

    public int health = 3;

    public HitstopManager hitstop;

    private void Awake()
    {
        hitstop = FindAnyObjectByType<HitstopManager>();
    }

    public bool CanDoAction(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Move:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Attack:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Charge:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Hitstopping
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Roll:
                if (myState == PlayerState.Attacking
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
                    || myState == PlayerState.Charging
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
