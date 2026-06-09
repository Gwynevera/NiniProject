using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerManager playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision c)
    {
        if (c != null)
        {
            bool yes = false;

            float t = 0;

            if (c.collider.CompareTag("Attack"))
            {
                yes = true;
                t = PlayerHitstop.bigHitstopTime;
            }
            else if (c.collider.CompareTag("Weapon"))
            {
                if (c.collider.gameObject.GetComponent<WeaponObject>().player != this.gameObject)
                {
                    yes = true;
                    t = PlayerHitstop.smallHitstopTime;
                    c.collider.gameObject.GetComponent<WeaponObject>().SetTimestop(t);
                }
                else
                {
                    HandleWeapon(c.gameObject);
                }
            }

            if (yes)
            {
                Vector3 knockDir = transform.position - c.transform.position;
                knockDir.y = 0;

                SetupKnockback(knockDir, KnockbackType.Small, t);
            }
        }
    }

    void OnTriggerEnter(Collider t)
    {
        if (t != null
            && t.CompareTag("Weapon")
            && playerManager.CanDoAction(PlayerAction.Move)
            && playerManager.myWeapon == null)
        {
            HandleWeapon(t.gameObject);
        }
    }

    void OnTriggerStay(Collider t)
    {
        if (t != null
            && t.CompareTag("Weapon")
            && playerManager.CanDoAction(PlayerAction.Move)
            && playerManager.myWeapon == null)
        {
            HandleWeapon(t.gameObject);
        }
    }

    public void HandleWeapon(GameObject g)
    {
        if (g.GetComponent<WeaponObject>().player == null)
        {
            playerManager.GetWeapon(g);

            g.transform.parent = playerManager.weaponHandle;
            g.GetComponent<WeaponObject>().ResetWeapon();
            g.GetComponent<WeaponObject>().player = this.gameObject;
        }
    }

    void SetupKnockback(Vector3 d, KnockbackType k, float t)
    {
        if (playerManager.CanDoAction(PlayerAction.Knockback)
            && !GetComponent<PlayerRoll>().invencible)
        {
            transform.forward = -d.normalized;
            GetComponent<PlayerKnockback>().SetKnockbackDamage(d, k, t);
        }
    }
}
