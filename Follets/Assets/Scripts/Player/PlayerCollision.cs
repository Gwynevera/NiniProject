using System.Linq;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerManager pManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        pManager = GetComponent<PlayerManager>();
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

            if (pManager.CanDoAction(PlayerAction.Knockback)
                && !GetComponent<PlayerRoll>().invencible)
            {
                transform.forward = -knockDir.normalized;
                GetComponent<PlayerKnockback>().SetKnockbackDamage(knockDir, KnockbackType.Big);
            }
        }
    }
}
