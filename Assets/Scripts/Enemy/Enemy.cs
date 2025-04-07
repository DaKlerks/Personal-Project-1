using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float health;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float rotateSpeed;
    [SerializeField]
    protected float damage;

    //required for setting rotation and checking distance
    protected Vector3 offset;
    protected float sqrLen;

    protected virtual void Attack()
    {
        Debug.Log("Enemy class: Attack method called");
    }

    protected virtual void LookTowards()
    {
        Debug.Log("Enemy class: LookTowards method called");
    }

    protected virtual void Movement()
    {
        Debug.Log("Enemy class: Movement method called");
    }

    protected abstract void Update();
}
