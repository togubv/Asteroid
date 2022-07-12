using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IKillable
{
    [SerializeField] private AsteroidSize size;
    [SerializeField] private Asteroid meduim, small;

    public AsteroidSize Size => size;

    public float speed;
    public Vector3 direction;
    private LevelManager levelManager;

    public void SetConfiguration(LevelManager manager, Vector3 position, Vector3 rotation, float speed, float angle)
    {
        if (levelManager == null)
        {
            this.levelManager = manager;
        }

        this.transform.position = position;
        this.transform.eulerAngles = new Vector3(rotation.x, rotation.y, rotation.z + angle);
        this.speed = speed;
    }

    public void Kill(Sender sender)
    {
        if (sender == Sender.Player)
        {
            int scoreCount = 0;
            switch (size)
            {
                case AsteroidSize.Large:
                    scoreCount = levelManager.rewardAsteroidLarge;
                    break;
                case AsteroidSize.Medium:
                    scoreCount = levelManager.rewardAsteroidMedium;
                    break;
                case AsteroidSize.Small:
                    scoreCount = levelManager.rewardAsteroidSmall;
                    break;
            }
            levelManager.ChangeScoreCount(levelManager.Score + scoreCount);
        }

        levelManager.SplitAsteroid(this); 
    }

    public void KillWithoutSplit()
    {
        levelManager.KillAsteroid(this);
    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.up * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("UFO"))
        {
            KillWithoutSplit();
        }

        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.gameObject.GetComponent<Player>().IsInvulnerable == false)
            {
                KillWithoutSplit();
            }
        }
    }
}

public enum AsteroidSize
{
    Large,
    Medium,
    Small
}
