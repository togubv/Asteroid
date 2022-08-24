using System.Collections;
using UnityEngine;

public class UFO : MonoBehaviour, IKillable
{
    [SerializeField] private ProjectileUFO projectileUFO;
    [SerializeField] private float projectileSpeed;

    private LevelManager levelManager;
    private ProjectilePoolUFO projectilePoolUFO;
    private float speed;
    private Vector3 direction;
    private float timer;
    private float lifetime;
    private Transform playerTransform;
    private float projectileLifeTime;
    private bool isCooldown = true;

    public void Kill(Sender sender)
    {
        if (sender == Sender.Player)
        {
            levelManager.ChangeScoreCount(levelManager.Score + 200);
        }

        levelManager.RestartUFOTimer();
        gameObject.SetActive(false);
    }

    public void SetConfigurations(LevelManager manager, ProjectilePoolUFO projectilePoolUFO, int direction, float speed, float lifetime, Transform playerTransform)
    {
        if (levelManager == null)
            this.levelManager = manager;

        if (this.projectilePoolUFO == null)
            this.projectilePoolUFO = projectilePoolUFO;

        if (this.playerTransform == null)
            this.playerTransform = playerTransform;

        if (projectileLifeTime == 0)
            projectileLifeTime = Screen.width / 100;

        this.speed = speed;
        this.direction = new Vector3(direction, 0, 0);
        this.timer = 0;
        this.lifetime = lifetime;
    }

    private void OnEnable()
    {
        StartCoroutine(AttackCooldown());
    }

    private void FixedUpdate()
    {
        transform.position += speed * direction * Time.deltaTime;
        Lifetime();

        if(isCooldown == false)
        {
            Shoot();
        }
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
        var dir = playerTransform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 direction = new Vector3(0, 0, angle);
        ProjectileUFO projectile = projectilePoolUFO.GetFromPool(transform, direction, lifetime, projectileSpeed);
        projectile.gameObject.SetActive(true);
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(RandomShootCooldown());
        if(gameObject.activeInHierarchy)
            isCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (player.IsInvulnerable == false)
            {
                Kill(Sender.Player);
            }
        }

        if (collider.gameObject.TryGetComponent<Asteroid>(out Asteroid asteroid))
        {
            Kill(Sender.None);
        }
    }

    private float RandomShootCooldown()
    {
        return Random.Range(1.0f, 3.0f);
    }
}
