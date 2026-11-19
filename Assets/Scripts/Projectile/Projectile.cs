using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileObjectPool _projectileObjectPool;
    [SerializeField] private ProjectileMover _projectilePrefab;
    [SerializeField] private int _additionalProjectileNum;
    [SerializeField] private float _angle;
    [SerializeField] private int _lifeTime;

    public void Shoot(Vector3 fireDirection, AttackScore attackScore)
    {
        Debug.Log($"AttackScore {attackScore.AttackWeaponData} {attackScore.WeaponDirection} {attackScore.TargetCoord}");
        if (attackScore.WeaponDirection == DirectionType.None)
        {
            ShootTrajectory(transform.position, fireDirection, attackScore);
        }
        else
        {
            ShootAll(transform.position, fireDirection);
        }
    }

    public void ShootTrajectory(Vector3 firePosition, Vector3 fireDirection, AttackScore attackScore)
    {
        Vector3 targetPosition = attackScore.TargetCoord.CoordToWorld();
        Vector3 ownerPosition = attackScore.CurrentCoord.CoordToWorld();

        float distance = Vector3.Distance(targetPosition, ownerPosition);

        Vector3 projectileVelocity = new Vector3(
            (targetPosition - ownerPosition).normalized.x,
            1f,
            (targetPosition - ownerPosition).normalized.z
            ) * Mathf.Sqrt(-distance * Physics.gravity.y / 2);

        Debug.Log($"Projectile Velocity {projectileVelocity}");
        _projectileObjectPool.Shoot(_projectilePrefab, firePosition, fireDirection, 10000, true, projectileVelocity);
    }

    public void ShootAll(Vector3 firePosition, Vector3 fireDirection)
    {
        for (int i = -_additionalProjectileNum; i <= _additionalProjectileNum; i++)
        {
            Vector3 direction = fireDirection;
            int lifeTime = _lifeTime;

            if (i != 0)
            {
                direction = Quaternion.Euler(0, _angle / 2 / i, 0) * fireDirection;
                lifeTime = (int)(_lifeTime / Mathf.Cos(_angle / 2 / i * Mathf.Deg2Rad));
            }

            _projectileObjectPool.Shoot(_projectilePrefab, firePosition, direction, lifeTime, false, Vector3.zero);
        }
    }

    public void SetObjectPool(ProjectileObjectPool projectileObjectPool)
    {
        _projectileObjectPool = projectileObjectPool;
    }
}
