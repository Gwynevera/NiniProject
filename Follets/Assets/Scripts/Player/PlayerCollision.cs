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
        if (c != null && c.collider.CompareTag("Attack"))
        {
            Vector3 knockDir = transform.position - c.transform.position;
            knockDir.y = 0;

            SetupKnockback(knockDir);
        }
    }

    void OnTriggerEnter(Collider t)
    {
        if (t != null && t.CompareTag("Weapon"))
        {
            if (t.gameObject.GetComponent<WeaponObject>().player == null)
            {
                playerManager.GetWeapon(t.gameObject.GetComponent<WeaponObject>());

                t.gameObject.SetActive(false);
                t.transform.parent = this.gameObject.transform;
                t.transform.localPosition = Vector3.zero;

                GetComponent<PlayerThrow>().weapon = t.gameObject;
                
            }
            else if (t.gameObject.GetComponent<WeaponObject>().player != gameObject)
            {
                Vector3 knockDir = transform.position - t.transform.position;
                knockDir.y = 0;

                SetupKnockback(knockDir);
            }
        }
    }

    void SetupKnockback(Vector3 d)
    {
        if (playerManager.CanDoAction(PlayerAction.Knockback)
            && !GetComponent<PlayerRoll>().invencible)
        {
            transform.forward = -d.normalized;
            GetComponent<PlayerKnockback>().SetKnockbackDamage(d, KnockbackType.Big);
        }
    }
}
