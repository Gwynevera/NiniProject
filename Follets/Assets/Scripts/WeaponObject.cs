using Unity.VisualScripting;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider c;

    public Weapon weapon;
    public GameObject player;

    float throwForce;
    float baseThrowForce = 10f;
    float minThrowForce = 5f;
    float maxThrowForce = 20f;
    float throwTorqueMult = 1.15f;

    float throwDuration = 0.65f;
    float throwTime;
    float throwTimer;
    float throwTimeMult = 0.25f;
    float throwMinTime = 0.35f;
    float throwMaxTime = 0.875f;

    float throwFriction = 0.975f;
    float minVel = 0.25f;

    bool dropping;
    float timeToDrop = 0.5f;
    float dropTimer;
    float dropForce = 8f;
    Vector3 dropDir;

    bool timestopped;
    float timestopTimer;

    Vector3 prevSpeed;
    Vector3 prevTorque;

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
        if (timestopped)
        {
            timestopTimer -= Time.fixedDeltaTime;
            if (timestopTimer < 0)
            {
                timestopped = false;

                rb.linearVelocity = prevSpeed;
                rb.angularVelocity = prevTorque;
            }
            return;
        }

        if (rb.linearVelocity != Vector3.zero)
        {
            throwTimer += Time.fixedDeltaTime;

            if (throwTimer > throwTime)
            {
                rb.linearVelocity *= throwFriction;
                rb.angularVelocity *= throwFriction;

                if (rb.linearVelocity.magnitude < minVel)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    Physics.IgnoreCollision(c, player.GetComponent<Collider>(), false);

                    player = null;
                    c.enabled = true;
                    c.isTrigger = true;
                }
            }
        }
        else if (dropping)
        {
            dropTimer += Time.fixedDeltaTime;
            if (dropTimer > timeToDrop)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.constraints = RigidbodyConstraints.FreezeAll;

                player = null;
                c.enabled = true;
                c.isTrigger = true;

                dropping = false;
            }
        }
    }

    public void ThrowWeapon(Vector3 pos, Vector3 dir, GameObject p, float extraForce)
    {
        this.gameObject.SetActive(true);
        player = p;

        Physics.IgnoreCollision(c, p.GetComponent<Collider>());

        c.enabled = true;
        c.isTrigger = false;

        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        transform.position = pos;
        transform.rotation = Quaternion.identity;
        transform.forward = dir.normalized;
        transform.parent = null;

        throwTime = weapon.weight * throwDuration;
        float forceDiff = throwDuration - throwTime;
        throwTime = throwDuration + (forceDiff * throwTimeMult);
        if (throwTime < throwMinTime) throwTime = throwMinTime;
        if (throwTime > throwMaxTime) throwTime = throwMaxTime;

        //extraForce /= weapon.weight;
        throwForce += extraForce;

        if (throwForce < minThrowForce) throwForce = minThrowForce;
        if (throwForce > maxThrowForce) throwForce = maxThrowForce;
        
        rb.AddForce(dir * throwForce, ForceMode.VelocityChange);
        rb.AddTorque(new Vector3(0, -throwForce * throwTorqueMult, 0), ForceMode.Impulse);

        throwTimer = 0;
    }

    public void DropWeapon(Transform t)
    {
        gameObject.SetActive(true);

        transform.parent = null;
        transform.position = t.position;
        transform.rotation = t.rotation;

        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        c.enabled = false;
        c.isTrigger = true;

        dropping = true;
        dropTimer = 0;

        float dropX = Random.Range(-1, 1);
        float dropZ = Random.Range(-1, 1);
        dropDir = new Vector3(dropX, 0, dropZ);
        rb.AddForce(dropDir.normalized * dropForce, ForceMode.VelocityChange);
        rb.AddTorque(new Vector3(0, dropForce, 0), ForceMode.Impulse);
    }

    public void ResetWeapon()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        rb.constraints = RigidbodyConstraints.FreezeAll;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        c.enabled = false;

        dropping = false;
        dropTimer = 0;

        timestopped = false;
        timestopTimer = 0;
    }

    public void SetTimestop(float t)
    {
        timestopped = true;
        timestopTimer = t;

        prevSpeed = rb.linearVelocity;
        prevTorque = rb.angularVelocity;

        rb.linearVelocity = rb.angularVelocity = Vector3.zero;
    }
}
