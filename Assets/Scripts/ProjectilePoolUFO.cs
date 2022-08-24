using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolUFO : MonoBehaviour
{
    [SerializeField] private ProjectileUFO prefab;

    private List<ProjectileUFO> pool;

    private void Start()
    {
        pool = new List<ProjectileUFO>();
    }

    public ProjectileUFO GetFromPool(Transform UFOTransform, Vector3 direction, float lifetime, float speed)
    {
        if (pool.Count == 0)
        {
            ProjectileUFO projectile = pool[AddNewToPool(UFOTransform, direction, lifetime, speed)];
            ProjectileRefresh(0, UFOTransform, direction, lifetime, speed);
            return projectile;
        }

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeInHierarchy == false)
            {
                ProjectileRefresh(i, UFOTransform, direction, lifetime, speed);
                return pool[i];
            }
        }

        return pool[AddNewToPool(UFOTransform, direction, lifetime, speed)];
    }

    public void DisableAllProjectiles()
    {
        foreach (ProjectileUFO projectile in pool)
        {
            projectile.gameObject.SetActive(false);
        }
    }

    private int AddNewToPool(Transform UFOTransform, Vector3 direction, float lifetime, float speed)
    {
        ProjectileUFO projectile = Instantiate(prefab, transform);
        pool.Add(projectile);
        ProjectileRefresh(pool.Count - 1, UFOTransform, direction, lifetime, speed);
        return pool.Count - 1;
    }

    private void ProjectileRefresh(int i, Transform UFOTransform, Vector3 direction, float lifetime, float speed)
    {
        pool[i].gameObject.transform.rotation = Quaternion.Euler(direction);
        pool[i].gameObject.transform.position = UFOTransform.position;
        pool[i].SetConfigurations(lifetime, speed, pool[i].gameObject.transform.right);
    }
}
