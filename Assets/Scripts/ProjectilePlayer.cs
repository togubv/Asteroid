using UnityEngine;

public class ProjectilePlayer : Projectile
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<Asteroid>(out Asteroid asteroid))
        {
            asteroid.Kill(Sender.Player);
            ReturnToPool();
            return;
        }

        if (collider.gameObject.TryGetComponent<UFO>(out UFO ufo))
        {
            ufo.Kill(Sender.Player);
            ReturnToPool();
            return;
        }
    }
}
