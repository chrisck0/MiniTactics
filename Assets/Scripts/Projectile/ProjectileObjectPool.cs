using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] private List<ProjectileMover> _projectileMoverPrefabs;
    private ObjectPool<ProjectileMover> _projectileObjectPool;

    private void Awake() => CreatePool();

    private void CreatePool()
    {
        _projectileObjectPool = new ObjectPool<ProjectileMover>(_projectileMoverPrefabs, transform, false, 5);
    }

    public void Shoot(ProjectileMover projectilePrefab, Vector3 firePosition, Vector3 direction, int lifeTime,
        bool isProjectileMove, Vector3 projectileVelocity)
    {
        ProjectileMover projectile = _projectileObjectPool.Take(projectilePrefab);
        projectile.SetObjectPool(_projectileObjectPool, transform);
        projectile.SetLifeTime(lifeTime);
        projectile.SetIsProjectileMove(isProjectileMove, projectileVelocity);

        Transform projectileTransform = projectile.transform;
        projectileTransform.SetParent(null, false);
        projectileTransform.position = firePosition;
        projectileTransform.forward = direction;
        projectile.gameObject.SetActive(true);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
