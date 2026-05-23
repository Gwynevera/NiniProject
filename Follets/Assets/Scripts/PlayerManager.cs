using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Rolling,
    Knockbacking,
}

public enum PlayerAction
{
    Move,
    Attack,
    Roll,
    Knockback,
    None
}

public class PlayerManager : MonoBehaviour
{
    public PlayerState myState;

    public int health = 3;

    public bool CanDoAction(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Move:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Attack:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Roll:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.Knockback:
                if (myState == PlayerState.Knockbacking)
                    return false;
                return true;

            case PlayerAction.None:
                if (myState == PlayerState.Attacking
                    || myState == PlayerState.Rolling
                    || myState == PlayerState.Knockbacking)
                    return false;
                return true;

            default:
                return false;
        }
    }
}
