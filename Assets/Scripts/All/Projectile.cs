using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float destroyTimer;

    void Update()
    {
        destroyTimer -= Time.deltaTime;
        if ( destroyTimer < 0 )
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
