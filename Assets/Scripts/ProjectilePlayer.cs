using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlayer : Projectile
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Asteroid"))
        {
            collider.gameObject.GetComponent<IKillable>().Kill(Sender.Player);
            ReturnToPool();
        }

        if (collider.gameObject.CompareTag("UFO"))
        {
            collider.gameObject.GetComponent<IKillable>().Kill(Sender.Player);
            ReturnToPool();
        }
    }
}
