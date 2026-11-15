using System.Collections.Generic;
using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    [SerializeField] private Trajectory _trajectoryPrefab;
    private ObjectPool<Trajectory> _trajectoryObjectPool;
    private List<Trajectory> _activatedTrajectories = new();

    private void Awake() => CreatePool();

    private void CreatePool()
    {
        List<Trajectory> trajectories = new();
        trajectories.Add(_trajectoryPrefab);
        _trajectoryObjectPool = new ObjectPool<Trajectory>(trajectories, transform, false);
    }

    public void ShowTrajectories(EnemyUnit enemy, MapManager mapManager)
    {
        Debug.Log(enemy.Weapon.GlobalTargetCoords.Count);
        foreach (Vector2Int targetCoord in enemy.Weapon.GlobalTargetCoords)
        {
            Debug.Log(targetCoord);

            UnitBase targetUnit = mapManager.UnitMap[targetCoord.x, targetCoord.y];

            if (targetUnit == null || targetUnit.UnitType != UnitType.Player) continue;

            Trajectory trajectory = _trajectoryObjectPool.Take(_trajectoryPrefab);
            _activatedTrajectories.Add(trajectory);
            trajectory.DrawTrajectory(enemy, targetUnit);
        }
    }

    public void HideAllTrajectories()
    {
        foreach (Trajectory trajectory in _activatedTrajectories)
        {
            _trajectoryObjectPool.Return(trajectory);
        }
    }
}
