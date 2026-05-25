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

            if (playerManager.CanDoAction(PlayerAction.Knockback)
                && !GetComponent<PlayerRoll>().invencible)
            {
                //transform.forward = -knockDir.normalized;
                ///GetComponent<PlayerKnockback>().SetKnockbackDamage(knockDir, KnockbackType.Big);
                Debug.Log("Attacked");
            }
        }
    }

    private void OnTriggerEnter(Collider t)
    {
        if (t != null && t.CompareTag("Weapon"))
        {
            string weaponName = t.name;
            Weapon weapon = Resources.Load<Weapon>($"Weapons/{weaponName}");
            if (weapon != null)
            {
                playerManager.GetWeapon(weapon);
                Destroy(t.gameObject);
            }
            else
            {
                Debug.LogWarning($"Weapon resource not found: Weapons/{weaponName}");
            }
        }
    }
}
