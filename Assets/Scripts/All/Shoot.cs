using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public ProjectileStats projectileStats;

    public Transform projectileSpawn;

    public Transform rayCastOrigin; //entity head location
    public Transform aimDirection; //how the player or enemy aims at targets (like camera or transform.forward)

    public GameObject projectilePrefab;

    private float projectileSpeed;
    private float projectileDamage;
    private float fireRate;
    private float spread;
    private Vector3 targetPoint;

    [Tooltip("Only Assign for the player")]
    public PlayerInput playerInput;
    private InputAction moveAction;

    void Start()
    {
        projectileSpeed = projectileStats.projectileSpeed;
        projectileDamage = projectileStats.projectileDamage;
        spread = projectileStats.spread;
        fireRate = projectileStats.fireRate;

        if (playerInput != null)
        {
            moveAction = playerInput.actions["Attack"];
        }
    }

    void Update()
    {
        fireRate -= Time.deltaTime;

        if (moveAction != null && moveAction.ReadValue<float>() != 0)
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (fireRate <= 0)
        {
            RaycastHit rayCastHit;

            if (Physics.Raycast(rayCastOrigin.position, aimDirection.forward, out rayCastHit, Mathf.Infinity))
            {
                targetPoint = rayCastHit.point;
            }
            else
            {
                targetPoint = rayCastOrigin.position + aimDirection.forward * 100f;
            }

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);
            Vector3 direction = (targetPoint - projectileSpawn.position).normalized;

            direction.x += Random.Range(-spread, spread);
            direction.y += Random.Range(-spread, spread);
            direction.z += Random.Range(-spread, spread);

            projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);

            fireRate = projectileStats.fireRate;
        }
    }
}
