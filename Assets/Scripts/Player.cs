using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    [SerializeField] private Core core;
    [SerializeField] private ProjectilePlayer prefabProjectile;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float attackCooldown;

    [SerializeField] private float racingForce = 5f;
    [SerializeField] private float speedLimit = 0.1f;
    [SerializeField] private float inertionDamping = 0.9f;

    public delegate void PlayerDiedHandler();
    public event PlayerDiedHandler PlayerDiedHandlerEvent;

    public bool IsInvulnerable => isInvulnerable;

    private float projectileLifeTime;
    private bool isCooldown;
    private Transform projectilesInWorld;
    private List<ProjectilePlayer> projectilePool;
    public List<ProjectilePlayer> ProjectilePool => projectilePool;

    private Vector3 inertion;
    private float racing;
    private bool isRacing;
    private float inputVertical;

    private Camera cameraMain;
    private PlayerControl playerControl;
    private bool isInvulnerable;
    private SpriteRenderer spriteR;

    public void Kill(Sender sender)
    {
        if (isInvulnerable == false)
        {
            PlayerDiedHandlerEvent?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void RefreshPlayer()
    {
        transform.rotation = Quaternion.identity;
        racing = 0;
        inputVertical = 0;
        inertion = new Vector3(0, 0, 0);
        isCooldown = false;
        SetInvulnerability(3.0f, 0.5f);
    }

    private void Start()
    {
        core.ChangePlayerControlHandlerEvent += ChanglePlayerControl;
        projectilePool = new List<ProjectilePlayer>();
        projectileLifeTime = Screen.width / 100;
        GameObject projectiles = new GameObject("Projectiles");
        projectilesInWorld = projectiles.transform;
        cameraMain = Camera.main;
        spriteR = GetComponent<SpriteRenderer>();

        if (attackCooldown <= 0)
        {
            attackCooldown = 0.33f;
        }
    }

    private void ChanglePlayerControl(PlayerControl type)
    {
        this.playerControl = type;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) && playerControl == PlayerControl.Keyboard)
        {
            PlayerRotateKeyboard(1);
        }

        if (Input.GetKey(KeyCode.D) && playerControl == PlayerControl.Keyboard)
        {
            PlayerRotateKeyboard(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCooldown == false)
            {
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && playerControl == PlayerControl.MouseAndKeyboard)
        {
            if (isCooldown == false)
            {
                Shoot();
            }
        }

        if ((Input.GetKey(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.UpArrow)) && playerControl == PlayerControl.MouseAndKeyboard)
        {
            isRacing = true;
        }

        if ((Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.UpArrow)) && playerControl == PlayerControl.MouseAndKeyboard)
        {
            isRacing = false;
            inputVertical = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            isRacing = true;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isRacing = false;
            inputVertical = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isRacing == true)
        {
            Racing();
        }

        Move();

        if (core.PlayerControl == PlayerControl.MouseAndKeyboard)
            PlayerRotateMouse();
    }

    private ProjectilePlayer ProjectileGetFromPool(Vector3 playerPosition)
    {
        if (projectilePool.Count == 0)
        {
            ProjectilePlayer projectile = projectilePool[AddNewProjectileToPool()];
            ProjectileRefresh(0, playerPosition);
            return projectile;
        }

        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (projectilePool[i].gameObject.activeInHierarchy == false)
            {
                ProjectileRefresh(i, playerPosition);
                return projectilePool[i];
            }
        }

        return projectilePool[AddNewProjectileToPool()];
    }

    private int AddNewProjectileToPool()
    {
        ProjectilePlayer projectile = Instantiate(prefabProjectile, transform.position, transform.rotation, projectilesInWorld);
        projectilePool.Add(projectile);
        return projectilePool.Count - 1;
    }

    private void ProjectileRefresh(int i, Vector3 playerPosition)
    {
        projectilePool[i].gameObject.transform.position = playerPosition;
        projectilePool[i].SetConfigurations(this.projectileLifeTime, this.projectileSpeed, transform.up, false);
    }

    private void PlayerRotateKeyboard(float direction)
    {
        transform.rotation *= Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotateSpeed * direction);
    }

    private void PlayerRotateMouse()
    {
        Vector3 cursor = cameraMain.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDir = cursor - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion dir = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);

        if (transform.rotation != dir)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, dir, rotateSpeed * 5 * Time.deltaTime);
        }

    }

    private void Racing()
    {
        if (inputVertical < 1)
        {
            inputVertical += 0.01f;
        }
    }

    private void Move()
    {
        racing = Mathf.Clamp(racing + inputVertical * racingForce * Time.deltaTime, 0, 1);
        inertion += transform.up * inputVertical * racing * Time.fixedDeltaTime;
        inertion = Vector3.ClampMagnitude(inertion, speedLimit);
        inertion *= inertionDamping;
        transform.Translate(inertion, Space.World);
    }

    private void Shoot()
    {
        ProjectileGetFromPool(transform.position).gameObject.SetActive(true);
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(this.attackCooldown);
        isCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("UFO") || collider.gameObject.CompareTag("Asteroid"))
        {
            Kill(Sender.None);
        }
    }

    private void SetInvulnerability(float duration, float animPeriod)
    {
        StartCoroutine(StartInvulnerability(duration, animPeriod));
    }

    private IEnumerator StartInvulnerability(float duration, float animPeriod)
    {
        isInvulnerable = true;
        float timer = 0;
        while (timer < duration)
        {
            yield return new WaitForSeconds(animPeriod / 2);
            spriteR.enabled = false;
            yield return new WaitForSeconds(animPeriod / 2);
            spriteR.enabled = true;
            timer += animPeriod;
        }
        isInvulnerable = false;
    }
}
