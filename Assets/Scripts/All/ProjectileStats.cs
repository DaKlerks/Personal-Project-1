using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ProjectileStats", order = 1)]
public class ProjectileStats : ScriptableObject
{
    public float projectileSpeed;
    public float projectileDamage;
    public float spread;

    public float fireRate;
}
