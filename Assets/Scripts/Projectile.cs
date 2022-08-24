using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float timer;
    private float lifetime;

    public void SetConfigurations(float lifetime, float speed, Vector3 direction)
    {
        this.direction = direction;
        this.timer = 0;
        this.lifetime = lifetime;
        this.speed = speed;
    }

    protected void FixedUpdate()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (gameObject.activeInHierarchy)
        {
            LifeTime();
        }
    }

    protected void LifeTime()
    {
        timer += Time.deltaTime;

        if (timer * speed >= lifetime)
        {
            ReturnToPool();
            return;
        }
    }

    protected void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
