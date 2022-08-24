using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolPlayer : MonoBehaviour
{
    [SerializeField] private ProjectilePlayer prefab;

    private List<ProjectilePlayer> pool;

    private void Start()
    {
        pool = new List<ProjectilePlayer>();
    }

    public ProjectilePlayer GetFromPool(Transform playerTransform, float lifetime, float speed)
    {
        if (pool.Count == 0)
        {
            ProjectilePlayer projectile = pool[AddNewToPool(playerTransform, lifetime, speed)];
            ProjectileRefresh(0, playerTransform, lifetime, speed);
            return projectile;
        }

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeInHierarchy == false)
            {
                ProjectileRefresh(i, playerTransform, lifetime, speed);
                return pool[i];
            }
        }

        return pool[AddNewToPool(playerTransform, lifetime, speed)];
    }

    public void DisableAllProjectiles()
    {
        foreach(ProjectilePlayer projectile in pool)
        {
            projectile.gameObject.SetActive(false);
        }
    }

    private int AddNewToPool(Transform playerTransform, float lifetime, float speed)
    {
        ProjectilePlayer projectile = Instantiate(prefab, transform);
        pool.Add(projectile);
        ProjectileRefresh(pool.Count - 1, playerTransform, lifetime, speed);
        return pool.Count - 1;
    }

    private void ProjectileRefresh(int i, Transform playerTransform, float lifetime, float speed)
    {
        pool[i].gameObject.transform.position = playerTransform.position;
        pool[i].SetConfigurations(lifetime, speed, playerTransform.up);
    }
}
