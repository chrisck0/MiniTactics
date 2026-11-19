using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] private List<ProjectileMover> _projectileMoverPrefabs;
    private ObjectPool<ProjectileMover> _projectileObjectPool;

    private void Awake() => CreatePool();

    private void CreatePool()
    {
        _projectileObjectPool = new ObjectPool<ProjectileMover>(_projectileMoverPrefabs, transform, false);
    }

    public void Shoot(ProjectileMover projectilePrefab, Transform firePoint)
    {
        ProjectileMover projectile = _projectileObjectPool.Take(projectilePrefab);
        projectile.SetObjectPool(_projectileObjectPool, transform);

        Transform projectileTransform = projectile.transform;
        projectileTransform.SetParent(null, false);
        projectileTransform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        projectile.gameObject.SetActive(true);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
