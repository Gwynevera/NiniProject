using UnityEngine;

public enum KnockbackType
{
    Small,
    Big
}

enum KnockbackState
{
    Impulse,
    Recover
}

public class PlayerKnockback : MonoBehaviour
{
    Rigidbody rb;

    KnockbackType knockbackType;
    KnockbackState knockbackState;

    float knockbackTimeSmall = 0.5f;
    float knockbackTimeBig = 1f;
    float knockbackTimer = 0f;

    float knockbackSpeedSmall = 20f;
    float knockbackSpeedBig = 30f;
    float knockbackFrictionSmall = 0.8f;
    float knockbackFrictionBig = 0.9f;

    Vector3 knockbackDir;
    Vector3 recoverDir;

    float recoverTime = 0.5f;
    float recoverTimer = 0f;

    float recoverSpeed = 5f;

    float lerpSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<PlayerManager>().myState == PlayerState.Knockbacking)
        {
            if (knockbackState == KnockbackState.Impulse)
            {
                if (knockbackTimer == 0)
                {
                    rb.AddForce(knockbackDir * (knockbackType == KnockbackType.Small ? knockbackSpeedSmall : knockbackSpeedBig), ForceMode.VelocityChange);
                }

                knockbackTimer += Time.fixedDeltaTime;

                rb.linearVelocity *= knockbackType == KnockbackType.Small ? knockbackFrictionSmall : knockbackFrictionBig;

                if (knockbackTimer >= (knockbackType == KnockbackType.Small ? knockbackTimeSmall : knockbackTimeBig))
                {
                    knockbackTimer = 0f;

                    if (knockbackType == KnockbackType.Small)
                    {
                        GetComponent<PlayerManager>().myState = PlayerState.Idle;
                        GetComponent<SphereCollider>().enabled = true;
                    }
                    else
                    {
                        recoverDir = GetComponent<PlayerMovement>().GetMovementInput();
                        knockbackState = KnockbackState.Recover;
                        recoverTimer = recoverDir == Vector3.zero ? recoverTime/2 : 0;
                    }
                }
            }
            else if (knockbackState == KnockbackState.Recover)
            {
                if (recoverTimer == 0)
                {
                    rb.AddForce(recoverDir * recoverSpeed, ForceMode.VelocityChange);
                }

                transform.forward = Vector3.Lerp(transform.forward, recoverDir, lerpSpeed * Time.fixedDeltaTime);

                recoverTimer += Time.fixedDeltaTime;
                if (recoverTimer >= recoverTime)
                {
                    recoverTimer = 0f;
                    knockbackState = KnockbackState.Impulse;

                    GetComponent<PlayerManager>().myState = PlayerState.Idle;
                    GetComponent<SphereCollider>().enabled = true;
                }
            }
        }
    }

    public void SetKnockback(Vector3 dir, KnockbackType type)
    {
        knockbackType = type;

        knockbackTimer = 0f;
        knockbackDir = dir;
        knockbackState = KnockbackState.Impulse;

        GetComponent<PlayerManager>().myState = PlayerState.Knockbacking;
        GetComponent<PlayerManager>().health--;
    }
}
