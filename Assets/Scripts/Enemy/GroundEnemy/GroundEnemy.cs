using UnityEngine;
using UnityEngine.AI;


public class GroundEnemy : Enemy
{
    public float attackDistance;
    public Transform target;

    private NavMeshAgent agent;

    public Animator attackAnimator;
    private bool attack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    protected override void Update()
    {
        if (target)
        {
            Vector3 targetPos = new Vector3(target.position.x, 0f, target.position.z); ;
            Vector3 thisPos = new Vector3(transform.position.x, 0f, transform.position.z);
            
            offset = targetPos - thisPos;
            sqrLen = offset.sqrMagnitude;

            LookTowards();
            Movement();
            Attack();
        }
    }

    protected override void Attack()
    {
        if (sqrLen < attackDistance)
        {
            attack = true;
        }
        else
        {
            attack = false;
        }

        attackAnimator.SetBool("Attack", attack);
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
            agent.destination = target.position;
        }
        else if (sqrLen < attackDistance)
        {
            agent.destination = transform.position;
        }
    }
}