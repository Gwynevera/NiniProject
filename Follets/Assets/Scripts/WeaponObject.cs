using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider c;

    public Weapon weapon;
    public GameObject player;

    float throwForce;
    float baseThrowForce = 10f;
    float throwForceMultiplier = 0.25f;
    float minThrowForce = 5f;
    float maxThrowForce = 20f;

    float throwDuration = 0.65f;
    float throwTimer;

    float throwFriction = 0.925f;
    float minVel = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        c = GetComponent<SphereCollider>();

        c.radius = weapon.length;
        c.isTrigger = true;

        throwForce = baseThrowForce;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.linearVelocity != Vector3.zero)
        {
            throwTimer += Time.fixedDeltaTime;

            if (throwTimer > throwDuration)
            {
                rb.linearVelocity *= throwFriction;
                rb.angularVelocity *= throwFriction;

                if (rb.linearVelocity.magnitude < minVel)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    player = null;
                }
            }
        }
    }

    public void ThrowWeapon(Vector3 pos, Vector3 dir, GameObject p, float extraForce)
    {
        this.gameObject.SetActive(true);
        player = p;

        transform.position = pos;
        transform.rotation = Quaternion.identity;
        transform.forward = dir.normalized;
        transform.parent = null;

        throwForce = weapon.weight * baseThrowForce;
        float forceDiff = baseThrowForce - throwForce;
        throwForce = baseThrowForce + (forceDiff * throwForceMultiplier);

        extraForce /= weapon.weight;
        throwForce += extraForce;

        if (throwForce < minThrowForce) throwForce = minThrowForce;
        if (throwForce > maxThrowForce) throwForce = maxThrowForce;
        
        rb.AddForce(dir * throwForce, ForceMode.VelocityChange);
        rb.AddTorque(new Vector3(0, throwForce, 0), ForceMode.Impulse);

        throwTimer = 0;
    }

    public void DrowWeapon()
    {

    }
}
