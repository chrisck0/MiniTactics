using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    private MapManager _mapManager;
    private TileCost[,] _tileCostMap;
    private HashSet<Vector2Int> _reachableCoords;
    private Stack<Vector2Int> _pathCoordStack = new();
    private AttackScore _maxAttackScore;
    private bool _isMoving;
    private bool _canAttack;
    private Vector2Int _moveTargetCoord;
    private int _turn;
    private int _totalUnitTypeUnits;
    private int _currentTurnCount;
    private bool _isShowingDanger;
    private UnitType _targetType;
    private CameraMove _cameraMove;

    public event Action<bool> OnDangerRangeVisibilityChanged;
    public AttackScore MaxAttackScore => _maxAttackScore;
    public bool IsShowingDanger => _isShowingDanger;

    private void Update()
    {
        if (_isMoving) MoveUnit();
        else if (_canAttack) Attack().Forget();
    }

    public void SetData(MapManager mapManager, UnitType targetType)
    {
        _mapManager = mapManager;
        _targetType = targetType;
    }

    public void InitDanger()
    {
        foreach (Vector2Int coord in _owner.DistanceRange.TargetableCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].InitDanger();
        }
        OnDangerRangeVisibilityChanged?.Invoke(false);
    }

    public void ShowDanger()
    {
        foreach (Vector2Int coord in _owner.DistanceRange.TargetableCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].ShowDanger();
        }
        _isShowingDanger = true;
        OnDangerRangeVisibilityChanged?.Invoke(true);
    }

    public void HideDanger()
    {
        foreach (Vector2Int coord in _owner.DistanceRange.TargetableCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].HideDanger();
        }
        _isShowingDanger = false;
        OnDangerRangeVisibilityChanged?.Invoke(false);
    }

    public void CalculateScore()
    {
        Vector2Int currentCoord = transform.position.WorldToCoord();

        _maxAttackScore = AttackScoreCalculator.CalculateScore(_owner, _mapManager, currentCoord);

        Debug.Log($"Attack Score {_maxAttackScore.CurrentCoord} {_maxAttackScore.AttackWeaponData} " +
            $"{_maxAttackScore.WeaponDirection} {_maxAttackScore.TargetCoord} {_maxAttackScore.Score}");
    }

    public void Behave(int turn, int totalUnitTypeUnits, int currentTurnCount, CameraMove cameraMove)
    {
        CalculateScore();
        _turn = turn;
        _totalUnitTypeUnits = totalUnitTypeUnits;
        _currentTurnCount = currentTurnCount;
        _cameraMove = cameraMove;

        Vector2Int currentCoord = transform.position.WorldToCoord();

        _tileCostMap = _mapManager.TileRange.UnitMoveRangeAndCalculateTotal(_owner, currentCoord);

        switch(_owner.Stat.Data.BehaviorType)
        {
            case BehaviorType.Aggressive:
                if (_maxAttackScore.Score == 0) MoveToTarget();
                else AttackTarget();
                break;

            case BehaviorType.Defensive:
                if (_maxAttackScore.Score != 0) AttackTarget();
                else Wait();
                break;

            case BehaviorType.Stationary:
                if (_owner.Stat.Data.Move != 0) Debug.LogError("Stationary Move Stat Error");
                if (_maxAttackScore.Score != 0) AttackTarget();
                else Wait();
                break;
        }
    }

    private void MoveToTarget()
    {
        Vector2Int targetCoord = FindClosestTargetCoord();
        MoveToClosestTarget(targetCoord);
    }

    private void AttackTarget()
    {
        Vector2Int currentCoord = transform.position.WorldToCoord();

        _cameraMove.SetCameraTrackTarget(_maxAttackScore.CurrentCoord);
        MoveToDestination(currentCoord, _maxAttackScore.CurrentCoord);
        _canAttack = true;
    }

    private void Wait()
    {
        _owner.OnUnitActioned?.Invoke();
    }

    private Vector2Int FindClosestTargetCoord()
    {
        int currentToTargetMinDistance = int.MaxValue;
        Vector2Int targetCoord = Vector2Int.zero;

        for (int i = 0; i < _tileCostMap.GetLength(1); i++)
        {
            for (int j = 0; j < _tileCostMap.GetLength(0); j++)
            {
                UnitBase unit = _mapManager.UnitMap[j, i];
                if (unit == null || unit.UnitType != _targetType) continue;

                Vector2Int newCoord = new Vector2Int(j, i);
                int newDistance = _mapManager.TileRange.GetUnreachableDistance(newCoord);
                if (newDistance < currentToTargetMinDistance)
                {
                    currentToTargetMinDistance = newDistance;
                    targetCoord = newCoord;
                }
            }
        }

        return targetCoord;
    }

    private void MoveToClosestTarget(Vector2Int targetCoord)
    {
        int destinationToTargetMinDistance = int.MaxValue;
        Vector2Int destinationCoord = Vector2Int.zero;
        _reachableCoords = _owner.DistanceRange.ReachableCoords;

        foreach (Vector2Int coord in _reachableCoords)
        {
            TileCost tileCost = _mapManager.TileRange.CalculateTileCost(coord, targetCoord, _targetType);
            if (destinationToTargetMinDistance > tileCost.Distance)
            {
                destinationToTargetMinDistance = tileCost.Distance;
                destinationCoord = coord;
            }
        }

        _cameraMove.SetCameraTrackTarget(destinationCoord);

        Vector2Int currentCoord = transform.position.WorldToCoord();
        _mapManager.TileRange.CalculateTileCost(currentCoord, destinationCoord, _targetType);
        MoveToDestination(transform.position.WorldToCoord(), destinationCoord);
    }

    private void MoveToDestination(Vector2Int currentCoord, Vector2Int destinationCoord)
    {
        _isMoving = true;

        _mapManager.UnitMap[currentCoord.x, currentCoord.y] = null;

        Vector2Int pathCoord = destinationCoord;

        int distance = VectorExtensions.CoordDistance(currentCoord, destinationCoord);

        int cnt = distance;

        while (pathCoord != currentCoord)
        {
            _pathCoordStack.Push(pathCoord);

            pathCoord = _tileCostMap[pathCoord.x, pathCoord.y].PrevTileCost.Coord;
        }

        _moveTargetCoord = currentCoord;
    }

    private void MoveUnit()
    {
        if (Vector3.Distance(transform.position, _moveTargetCoord.CoordToWorld()) >= 0.1f)
        {
            transform.Translate(Vector3.forward
                * Time.deltaTime * _owner.MoveSpeed);
            return;
        }

        if (_pathCoordStack.Count == 0)
        {
            _mapManager.UnitMap[_moveTargetCoord.x, _moveTargetCoord.y] = _owner;
            _isMoving = false;
            if (!_canAttack) WaitCamera().Forget();
            return;
        }

        _moveTargetCoord = _pathCoordStack.Pop();
        transform.LookAt(_moveTargetCoord.CoordToWorld());
        transform.Translate(Vector3.forward
            * Time.deltaTime * _owner.MoveSpeed);
    }

    private async UniTask WaitCamera()
    {
        await UniTask.WaitUntil(() => _cameraMove.IsTrackingTarget == false);
        _owner.OnUnitActioned?.Invoke();
    }

    public void SetTargets(Vector2Int coord)
    {
        WeaponData weaponData = _maxAttackScore.AttackWeaponData;
        Weapon weapon = _owner.Weapon;

        weapon.SetRange(coord, weaponData, _mapManager);
        weapon.SelectWeaponRange(coord, _maxAttackScore.TargetCoord, _targetType, _mapManager,
            _maxAttackScore.WeaponDirection, weaponData);
    }

    private async UniTask Attack()
    {
        _canAttack = false;

        WeaponData weaponData = _maxAttackScore.AttackWeaponData;
        Weapon weapon = _owner.Weapon;

        SetTargets(transform.position.WorldToCoord());
        _owner.DistanceRange.ShowRange();

        if (!weapon.CanAttackTargets(weaponData)) return;

        _owner.Rotation.SetDirection(
            _maxAttackScore.TargetCoord,
            _maxAttackScore.WeaponDirection
            );

        await UniTask.WaitUntil(() => _owner.Rotation.IsRotating == false);

        await weapon.AttackTargets(
            weaponData,
            _targetType,
            _turn,
            _totalUnitTypeUnits,
            _currentTurnCount
            );

        await weapon.GetAttackReturn(
            _targetType,
            _turn,
            _totalUnitTypeUnits,
            _currentTurnCount
            );

        _owner.OnUnitActioned?.Invoke();
    }
}
