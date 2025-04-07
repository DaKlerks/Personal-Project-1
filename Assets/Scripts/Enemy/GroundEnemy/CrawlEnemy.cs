using UnityEngine;
using UnityEngine.AI;


public class CrawlEnemy : Enemy
{
    public float attackDistance;
    public Transform target;

    private NavMeshAgent agent;
    private Shoot shoot;

    public Animator attackAnimator;
    private bool attack;
    private bool moving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shoot = GetComponent<Shoot>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        LookTowards();
        Movement();
        Attack();

        Debug.Log(sqrLen);
        Debug.Log(attackDistance);
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.transform == target)
            {
                Debug.DrawRay(transform.position, (target.position - transform.position), Color.green);
                shoot.Fire();
            }
        }
    }

    protected override void LookTowards()
    {
        if (target)
        {
            Vector3 targetPos = new Vector3(target.position.x, 0f, target.position.z);
            Vector3 thisPos = new Vector3(transform.position.x, 0f, transform.position.z);

            offset = targetPos - thisPos;
            sqrLen = offset.sqrMagnitude;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, offset, rotateSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    protected override void Movement()
    {
        if (sqrLen > attackDistance)
        {
            agent.destination = transform.position;
            moving = false;
            attackAnimator.SetBool("Moving", moving);
        }
        else if (sqrLen < attackDistance)
        {
            agent.destination = target.position;
            moving = true;
            attackAnimator.SetBool("Moving", moving);
        }
    }

    protected override void Attack()
    {
        if (sqrLen > attackDistance)
        {
            Shoot();
            attack = false;
            agent.angularSpeed = rotateSpeed;
            attackAnimator.SetBool("Attack", attack);
        }
        else if (sqrLen < attackDistance / 5)
        {
            attack = true;
            agent.angularSpeed = 0;
            attackAnimator.SetBool("Attack", attack);
        }
        else
        {
            attack = false;
            agent.angularSpeed = rotateSpeed;
            attackAnimator.SetBool("Attack", attack);
        }
    }
}