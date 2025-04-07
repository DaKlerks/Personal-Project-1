using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float attackDistance;
    public Transform target;

    public Animator eyeAnimator;
    private float charge;
    private Shoot shoot;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shoot = GetComponent<Shoot>();
    }

    protected override void Update()
    {
        if (target)
        {
            offset = target.position - transform.position;
            sqrLen = offset.sqrMagnitude;

            LookTowards();
            Movement();
            Attack();
        }
    }

    protected override void Attack()
    {
        if (sqrLen > attackDistance)
        {
            charge -= Time.deltaTime / 5f;
            charge = Mathf.Clamp(charge, 0f, 1f);
            eyeAnimator.SetFloat("Charge", charge);
        }
        else if (sqrLen < attackDistance)
        {
            charge += Time.deltaTime / 5f;
            charge = Mathf.Clamp(charge, 0f, 1f);
            eyeAnimator.SetFloat("Charge", charge);
        }

        if (charge >= 0.9f)
        {
            shoot.Fire();
        }
    }

    protected override void LookTowards()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, offset, rotateSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected override void Movement()
    {
        if (sqrLen > attackDistance)
        {
            rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (sqrLen < attackDistance / 2)
        {
            rb.AddForce(-transform.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        RaycastHit rayCastHit;
        if (Physics.Raycast(transform.position, -transform.up, out rayCastHit, 1.5f))
        {
            rb.AddForce(transform.up * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
