using System.Linq;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider c)
    {
        if (c != null && c.tag == "Attack")
        {
            Vector3 knockDir = transform.position - c.transform.position;
            knockDir.y = 0;

            if (GetComponent<PlayerManager>().CanDoAction(PlayerAction.Knockback)
                && !GetComponent<PlayerRoll>().invencible)
            {
                GetComponent<SphereCollider>().enabled = false;

                transform.forward = -knockDir.normalized;

                GetComponent<PlayerManager>().myState = PlayerState.Knockbacking;
                GetComponent<PlayerKnockback>().SetKnockback(knockDir, KnockbackType.Big);
            }
        }
    }
}
