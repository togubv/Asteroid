using UnityEngine;

public class UFO : MonoBehaviour, IKillable
{
    [SerializeField] private ProjectileUFO projectileUFO;
    [SerializeField] private float projectileSpeed;

    private LevelManager levelManager;
    private float speed;
    private Vector3 direction;
    private float timer;
    private float lifetime;
    private float shootCooldown;
    private float shootTimer;
    private Transform playerTransform;
    private float projectileLifeTime;

    public void Kill(Sender sender)
    {
        if (sender == Sender.Player)
        {
            levelManager.ChangeScoreCount(levelManager.Score + 200);
        }

        levelManager.RestartUFOTimer();
        gameObject.SetActive(false);
    }

    public void SetConfigurations(LevelManager manager, int direction, float speed, float lifetime, Transform playerTransform)
    {
        if (levelManager == null)
            this.levelManager = manager;

        if (this.playerTransform == null)
            this.playerTransform = playerTransform;

        if (projectileLifeTime == 0)
            projectileLifeTime = Screen.width / 100;

        this.speed = speed;
        this.direction = new Vector3(direction, 0, 0);
        this.timer = 0;
        this.lifetime = lifetime;
        this.shootCooldown = RandomShootCooldown();
    }

    private void FixedUpdate()
    {
        transform.position += speed * direction * Time.deltaTime;
        Lifetime();
        ShootCooldown();
    }

    private void Lifetime()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            Kill(Sender.None);
        }
    }

    private void Shoot()
    {
        var direction = playerTransform.position - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ProjectileUFO projectile = Instantiate(projectileUFO, transform.position, Quaternion.Euler(0, 0, angle));
        projectile.SetConfigurations(projectileLifeTime, projectileSpeed, projectile.transform.right, true);
        levelManager.AddProjectileUFOToPool(projectile);
    }

    private void ShootCooldown()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer > shootCooldown)
        {
            shootTimer = 0;
            shootCooldown = RandomShootCooldown();
            Shoot();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.gameObject.GetComponent<Player>().IsInvulnerable == false)
            {
                Kill(Sender.Player);
            }
        }

        if (collider.gameObject.CompareTag("Asteroid"))
        {
            Kill(Sender.None);
        }
    }

    private float RandomShootCooldown()
    {
        return Random.Range(2.0f, 5.0f);
    }
}
