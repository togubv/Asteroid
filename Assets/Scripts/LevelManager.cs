using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Projectiles pools")]
    [SerializeField] private ProjectilePoolPlayer projectilePoolPlayer;
    [SerializeField] private ProjectilePoolUFO projectilePoolUFO;

    [Header("Asteroids prefabs")]
    [SerializeField] private Asteroid large;
    [SerializeField] private Asteroid medium;
    [SerializeField] private Asteroid small;

    [Header("UFO prefab")]
    [SerializeField] private UFO prefabUFO;

    [Header("Player transform")]
    [SerializeField] private Player player;

    [Header("Game settings")]
    [SerializeField] private int countStartLifes;
    [SerializeField] private int increaseCountAsteroidPerLevel;
    public int rewardUFO;
    public int rewardAsteroidLarge;
    public int rewardAsteroidMedium;
    public int rewardAsteroidSmall;

    [Header("Asteroids")]
    [SerializeField] private int countStartAsteroids;
    [SerializeField] private float asteroidSpeedMin;
    [SerializeField] private float asteroidSpeedMax;
    [SerializeField] private float splitAngle;

    [Header("UFO spawn interval")]
    [SerializeField] private float ufoIntervalMin;
    [SerializeField] private float ufoIntervalMax;

    public delegate void ChangeScoreCountHandler(int count);
    public event ChangeScoreCountHandler ChangeScoreCountHandlerEvent;
    public delegate void ChangeLifesHandler(int count);
    public event ChangeLifesHandler ChangeLifesHandlerEvent;

    public int Lifes => lifes;
    public int Score => score;
    public bool IsGame => isGame;

    private int lifes;
    private int score;

    private Transform asteroidsInWorld;
    private Transform ufoProjectiles;
    private int currentLevel;
    private int countAsteroids;
    private float sizeX, sizeY;
    private List<Asteroid> asteroidsInLevel;
    private List<Asteroid> asteroidsPool;
    private List<ProjectileUFO> projectileUFOPool;
    private List<UFO> ufoPool;
    private List<UFO> ufoInLevel;
    private float UFOtimer, UFOcooldown;
    private bool isUFO;
    private Transform playerTransform;
    private bool isGame;

    public void ChangeScoreCount(int count)
    {
        this.score = count;
        ChangeScoreCountHandlerEvent?.Invoke(this.score);
    }

    public void ChangeLifeCount(int count)
    {
        this.lifes = count;
        ChangeLifesHandlerEvent?.Invoke(this.lifes);
    }

    private void Start()
    {
        sizeX = 9;
        sizeY = 5;
        asteroidsInLevel = new List<Asteroid>();
        asteroidsPool = new List<Asteroid>();
        projectileUFOPool = new List<ProjectileUFO>();
        ufoPool = new List<UFO>();
        ufoInLevel = new List<UFO>();
        GameObject projectiles = new GameObject("UFOProjectiles");
        ufoProjectiles = projectiles.transform;
        GameObject asteroids = new GameObject("Asteroids");
        asteroidsInWorld = asteroids.transform;
        playerTransform = player.GetComponent<Transform>();
        player.PlayerDiedHandlerEvent += PlayerDied;
        UFOcooldown = RandomUFOCooldown();

        currentLevel = 1;
        countAsteroids = countStartAsteroids;
    }

    private void FixedUpdate()
    {
        if (isUFO == false)
        {
            UFOTimer();
        }
    }

    public void DelayedStartNewGame(float delay)
    {
        StartCoroutine(DelayedStartLevel(delay));
    }

    public void ClearWorld()
    {
        ChangeLifeCount(countStartLifes);
        ChangeScoreCount(0);
        countAsteroids = countStartAsteroids;
        RestartUFOTimer();
        player.gameObject.SetActive(false);

        for (int i = 0; i < asteroidsInLevel.Count; i++)
        {
            asteroidsInLevel[i].gameObject.SetActive(false);
        }

        asteroidsInLevel = new List<Asteroid>();

        for (int i = 0; i < ufoInLevel.Count; i++)
        {
            RemoveAndOffFromUFOInLevel(ufoInLevel[i]);
        }

        for (int i = 0; i < projectileUFOPool.Count; i++)
        {
            if (projectileUFOPool[i] != null)
                Destroy(projectileUFOPool[i].gameObject);
        }

        projectileUFOPool = new List<ProjectileUFO>();
        projectilePoolPlayer.DisableAllProjectiles();
        projectilePoolUFO.DisableAllProjectiles();
    }

    public void StartNewGame()
    {
        isGame = true;
        StartCoroutine(PlayerSpawn(0f));
        SpawnAsteroids(this.countStartAsteroids);
    }

    private void PlayerDied()
    {
        ChangeLifeCount(lifes - 1);
        if (lifes > -1)
        {
            StartCoroutine(PlayerSpawn(2.0f));
        }
    }

    private IEnumerator PlayerSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.gameObject.transform.position = new Vector3(0, 0, 0);
        player.gameObject.SetActive(true);
        player.RefreshPlayer();
    }
    private void SpawnAsteroids(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-sizeX, sizeX), Random.Range(-sizeY, sizeY), 0);
            Asteroid asteroid = AsteroidGetFromPool(large);
            AsteroidRefresh(asteroid, randomPosition, asteroid.transform.eulerAngles, RandomAsteroidSpeed(), Random.Range(0, 180));
            asteroidsInLevel.Add(asteroid);
        }

        currentLevel += 1;
        countAsteroids += increaseCountAsteroidPerLevel;
    }

    private void RemoveAndOffFromAsteroidsInLevel(Asteroid asteroid)
    {
        if (asteroidsInLevel.Contains(asteroid))
        {
            asteroid.gameObject.SetActive(false);
            asteroidsInLevel.Remove(asteroid);
        }

        if (asteroidsInLevel.Count <= 0)
        {
            StartCoroutine(DelayedSpawnAsteroidsLevel());
        }
    }

    private IEnumerator DelayedSpawnAsteroidsLevel()
    {
        yield return new WaitForSeconds(2.0f);
        SpawnAsteroids(this.countAsteroids);
    }

    private IEnumerator DelayedStartLevel(float delay)
    {
        ClearWorld();
        yield return new WaitForSeconds(delay);
        StartNewGame();
    }

    #region Asteroids

    public void SplitAsteroid(Asteroid parentAsteroid)
    {
        void GetAndRefreshFromPoolSplittedAsteroid(Asteroid prefab, float angle, float speed)
        {
            Asteroid newAsteroid = AsteroidGetFromPool(prefab);
            asteroidsInLevel.Add(newAsteroid);
            AsteroidRefresh(newAsteroid, parentAsteroid.transform.position, parentAsteroid.transform.eulerAngles, speed, angle);
        }

        if (parentAsteroid.Size == AsteroidSize.Large)
        {
            float speed = RandomAsteroidSpeed();
            GetAndRefreshFromPoolSplittedAsteroid(medium, splitAngle, speed);
            GetAndRefreshFromPoolSplittedAsteroid(medium, -splitAngle, speed);
            RemoveAndOffFromAsteroidsInLevel(parentAsteroid);
            return;
        }

        if (parentAsteroid.Size == AsteroidSize.Medium)
        {
            float speed = RandomAsteroidSpeed();
            GetAndRefreshFromPoolSplittedAsteroid(small, splitAngle, speed);
            GetAndRefreshFromPoolSplittedAsteroid(small, -splitAngle, speed);
            RemoveAndOffFromAsteroidsInLevel(parentAsteroid);
            return;
        }

        RemoveAndOffFromAsteroidsInLevel(parentAsteroid);
    }

    public void KillAsteroid(Asteroid asteroid)
    {
        RemoveAndOffFromAsteroidsInLevel(asteroid);
        return;
    }

    private int AddNewAsteroidToPool(Asteroid prefab)
    {
        Asteroid asteroid = Instantiate(prefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 180)), asteroidsInWorld);
        asteroidsPool.Add(asteroid);
        int i = asteroidsPool.Count - 1;
        return i;
    }

    private Asteroid AsteroidGetFromPool(Asteroid prefab)
    {
        if (asteroidsPool.Count == 0)
        {
            int i = AddNewAsteroidToPool(prefab);
            return asteroidsPool[i];
        }

        for (int i = 0; i < asteroidsPool.Count; i++)
        {
            if (asteroidsPool[i].Size == prefab.Size && asteroidsPool[i].gameObject.activeInHierarchy == false)
            {
                return asteroidsPool[i];
            }
        }
        return asteroidsPool[AddNewAsteroidToPool(prefab)];
    }

    private void AsteroidRefresh(Asteroid asteroid, Vector3 position, Vector3 rotation, float speed, float angle)
    {
        asteroid.gameObject.SetActive(true);
        asteroid.SetConfiguration(this, position, rotation, speed, angle);
    }

    private float RandomAsteroidSpeed()
    {
        return Random.Range(asteroidSpeedMin, asteroidSpeedMax);
    }

    #endregion

    #region UFO

    public void AddProjectileUFOToPool(ProjectileUFO projectile)
    {
        projectileUFOPool.Add(projectile);
    }

    private void UFOTimer()
    {
        UFOtimer += Time.deltaTime;

        if (UFOtimer >= UFOcooldown)
        {
            isUFO = true;
            UFOcooldown = RandomUFOCooldown();
            SpawnUFO();
        }  
    }

    public void RestartUFOTimer()
    {
        UFOtimer = 0;
        isUFO = false;
        UFOcooldown = RandomUFOCooldown();
    }

    private void SpawnUFO()
    {
        UFO ufo = GetFromUFOPool();
        RefreshUFO(ufo);
        ufoInLevel.Add(ufo);
    }

    private void RefreshUFO(UFO ufo)
    {
        int direction = RandomUFODirection();
        float randomY = Random.Range(-(sizeY - (sizeY * 0.4f)), sizeY - (sizeY * 0.4f));
        ufo.transform.position = new Vector3(sizeX * direction, randomY, 0);
        ufo.SetConfigurations(this, projectilePoolUFO, direction * -1, 1.75f, sizeX * 1.2f, playerTransform);
        ufo.gameObject.SetActive(true);
    }

    private UFO AddNewUFOToPool()
    {
        UFO newUFO = Instantiate(prefabUFO, new Vector3(0, 0, 0), Quaternion.identity);        
        ufoPool.Add(newUFO);
        return newUFO;
    }

    private UFO GetFromUFOPool()
    {
        if (ufoPool.Count == 0)
        {
            UFO newUFO = AddNewUFOToPool();
            ufoPool.Add(newUFO);
            return newUFO;
        }

        for (int i = 0; i < ufoPool.Count; i++)
        {
            if (ufoPool[i].gameObject.activeInHierarchy == false)
            {
                return ufoPool[i];
            }
        }
        return AddNewUFOToPool();
    }

    private void RemoveAndOffFromUFOInLevel(UFO ufo)
    {
        if (ufoInLevel.Contains(ufo))
        {
            ufo.gameObject.SetActive(false);
            ufoInLevel.Remove(ufo);
        }
    }

    private int RandomUFODirection()
    {
        int random = 0;
        while (random == 0)
        {
            random = Random.Range(-1, 2);
        }
        return random;
    }

    private float RandomUFOCooldown()
    {
        return Random.Range(ufoIntervalMin, ufoIntervalMax);
    }

    #endregion

}
