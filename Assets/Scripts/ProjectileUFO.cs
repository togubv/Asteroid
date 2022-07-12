using UnityEngine;

public class ProjectileUFO : Projectile
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.GetComponent<IKillable>().Kill(Sender.None);
            Destroy(gameObject);
        }

        if (collider.gameObject.CompareTag("Asteroid"))
        {
            collider.gameObject.GetComponent<IKillable>().Kill(Sender.None);
            Destroy(gameObject);
        }
    }
}
