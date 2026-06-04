using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    PlayerManager playerManager;

    float grabRadius = 1;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetGrabInput() && playerManager.CanDoAction(PlayerAction.Move))
        {
            Collider[] objs = Physics.OverlapSphere(transform.position, grabRadius);

            if (objs != null && objs.Length > 0)
            {
                foreach (Collider c in objs)
                {
                    if (c.CompareTag("Weapon") && c.isTrigger)
                    {
                        GetComponent<PlayerCollision>().HandleWeapon(c.gameObject);
                    }
                }
            }
        }
    }

    bool GetGrabInput()
    {
        if (GetComponent<PlayerButtons>().playerInput == PlayerInput.Keyboard)
        {
            if (Keyboard.current != null && Keyboard.current[GetComponent<PlayerButtons>().grabKey].wasPressedThisFrame)
            {
                return true;
            }
        }
        else
        {
            if (Gamepad.current != null && Gamepad.current[GetComponent<PlayerButtons>().grabButton].wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
}
