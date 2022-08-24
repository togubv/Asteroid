using UnityEngine;

public class ProjectileUFO : Projectile
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Kill(Sender.None);
            ReturnToPool();
            return;
        }

        if (collider.gameObject.TryGetComponent<Asteroid>(out Asteroid asteroid))
        {
            asteroid.Kill(Sender.None);
            ReturnToPool();
            return;
        }
    }
}
