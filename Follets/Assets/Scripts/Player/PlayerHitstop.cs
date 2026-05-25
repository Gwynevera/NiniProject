using UnityEngine;

public class PlayerHitstop : MonoBehaviour
{
    PlayerManager pManager;
    PlayerAttack pAttack;
    Rigidbody rb;

    bool hitstopped;
    bool damaged;

    Vector3 originalPos;
    float maxShakeDist = 0.35f;
    float minShakeDist = 0.1f;

    float hitstopTimer;
    float bigHitstopTime = 1; //0.25f;
    float smallHitstopTime = 0.5f; //0.1f;

    PlayerState prevState;
    Vector3 prevSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        pManager = GetComponent<PlayerManager>();
        pAttack = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (hitstopped)
        {
            rb.linearVelocity = Vector3.zero;

            hitstopTimer -= Time.fixedDeltaTime;
            if (hitstopTimer <= 0)
            {
                hitstopped = false;

                if (damaged)
                {
                    damaged = false;
                    transform.position = originalPos;
                    pManager.myState = PlayerState.Knockbacking;
                }
                else
                {
                    rb.linearVelocity = prevSpeed;
                    pManager.myState = prevState;
                }
            }
            else if (damaged)
            {
                float shakeDist = Random.Range(minShakeDist, maxShakeDist);
                transform.position = originalPos + new Vector3(Random.Range(-shakeDist, shakeDist), 0, Random.Range(-shakeDist, shakeDist));
            }
        }
    }

    public void StartVictimHitstop(bool bigHit)
    {
        pManager.myState = PlayerState.Hitstopping;
        
        originalPos = transform.position;
        damaged = true;
        
        hitstopped = true;
        hitstopTimer = bigHit ? bigHitstopTime : smallHitstopTime;
    }

    public void StartBullyHitstop(PlayerState preState, Vector3 preSpeed, bool bigHit)
    {
        pManager.myState = PlayerState.Hitstopping;

        originalPos = transform.position;

        prevState = preState;
        prevSpeed = preSpeed;
        rb.linearVelocity = Vector3.zero;

        if (prevSpeed == Vector3.zero)
        {
            float speed = pAttack.Charged ? pAttack.ChargeMoveSpeed : pAttack.AttackMoveSpeed;
            prevSpeed = transform.forward * speed;
        }

        hitstopped = true;
        hitstopTimer = bigHit ? bigHitstopTime : smallHitstopTime;
    }
}
