using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private SaveLoader _saveLoader;
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private List<UnitBase> _unitPrefabs;
    [SerializeField] private List<TileBase> _tilePrefabs;
    [SerializeField] private ProjectileObjectPool _projectileObjectPool;

    private StageData _stageData;
    private ObjectPool<UnitBase> _unitObjectPool;
    private ObjectPool<TileBase> _tileObjectPool;
    private Dictionary<int, UnitBase> _unitPrefabDict = new();
    private Dictionary<TileData, TileBase> _tilePrefabDict = new();
    private Dictionary<UnitType, List<UnitBase>> _spawnedUnitDict = new();
    private TileBase[,] _tileMap;
    private UnitBase[,] _unitMap;
    private int _turn = 1;
    private List<TurnSaveData> _turnSaveDataList;
    private Vector3 _cameraPosition;
    private int _initialSeed;

    public StageData StageData => _stageData;
    public Dictionary<UnitType, List<UnitBase>> SpawnedUnitDict => _spawnedUnitDict;
    public TileBase[,] TileMap => _tileMap;
    public UnitBase[,] UnitMap => _unitMap;
    public int Turn => _turn;
    public List<TurnSaveData> TurnSaveDataList => _turnSaveDataList;
    public Vector3 CameraPosition => _cameraPosition;
    public int InitialSeed => _initialSeed;

    public event Action OnStageLoaded;
    public event Action OnUnitSpawned;

    private void Awake()
    {
        CreatePools();
        CreatePrefabDict();
    }

    private void Start() => SpawnStage();
    private void CreatePools()
    {
        _unitObjectPool = new ObjectPool<UnitBase>(_unitPrefabs, new GameObject("UnitObjectPool").transform);
        _tileObjectPool = new ObjectPool<TileBase>(_tilePrefabs, new GameObject("TileObjectPool").transform);
    }
    
    private void CreatePrefabDict()
    {
        foreach (UnitBase unitPrefab in _unitPrefabs)
        {
            _unitPrefabDict[unitPrefab.Stat.Data.Id] = unitPrefab;
        }

        foreach (TileBase tilePrefab in _tilePrefabs)
        {
            _tilePrefabDict[tilePrefab.TileData] = tilePrefab;
        }
    }

    private void SpawnStage()
    {
        _stageData = GameManager.Instance.GetStageData();
        SaveData saveData = _saveLoader.LoadSaveData();

        if (saveData != null && saveData.StageId != _stageData.StageId)
        {
            _saveLoader.ClearSaveData();
            saveData = null;
        }

        CreateMapAndSpawnMap(_stageData.Map);

        _cameraPosition = _stageData.CameraInitialPosition;
        if (saveData == null)
        {
            CreateMapAndSpawnUnits(_stageData.UnitPositionList);
        }
        else
        {
            CreateMapAndSpawnUnitsFromSave(saveData);
            TurnSaveData turnSaveData = saveData.TurnSaveDataList[saveData.TurnSaveDataList.Count - 1];
            _cameraPosition = turnSaveData.CameraPosition;
        }

        OnStageLoaded?.Invoke();
    }

    private void CreateMapAndSpawnMap(TileType[,] map)
    {
        _tileMap = new TileBase[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(1); i++)
        {
            for (int j = 0; j < map.GetLength(0); j++)
            {
                if (!_dataManager.TileDataDict.TryGetValue(map[j, i], out TileData tileData)) continue;

                if (!_tilePrefabDict.TryGetValue(tileData, out TileBase tilePrefab)) continue;

                TileBase tile = _tileObjectPool.Take(tilePrefab);
                Vector2Int coord = new Vector2Int(j, i);
                _tileMap[j, i] = tile;

                tile.transform.position = coord.CoordToWorld(-1f);
            }
        }
    }

    private void CreateMapAndSpawnUnits(List<UnitPosition> unitPositionList)
    {
        _turnSaveDataList = new();

        Dictionary<int, UnitData> unitDataDict = _dataManager.UnitDataDict;
        _unitMap = new UnitBase[_tileMap.GetLength(0), _tileMap.GetLength(1)];
        
        GameManager.Instance.SetInitialSeed();
        GameManager.Instance.SetLeftRevertTime();

        foreach (UnitPosition unitPosition in unitPositionList)
        {
            if (!unitDataDict.TryGetValue(unitPosition.Id, out UnitData unitData)) continue;

            if (!_unitPrefabDict.TryGetValue(unitData.Id, out UnitBase unitPrefab)) continue;

            UnitBase unit = _unitObjectPool.Take(unitPrefab);
            unit.SetObjectPool(_unitObjectPool);
            unit.EffectController.SetProjectileObjectPool(_projectileObjectPool);

            Vector2Int coord = unitPosition.Position;
            unit.transform.position = coord.CoordToWorld();
            unit.transform.forward = unitPosition.Direction;
            unit.Stat.InitStat(unitData);
            unit.DistanceRange.SetMap(_tileMap);

            Dictionary<int, WeaponData> weaponDataDict = _dataManager.WeaponDataDict;

            unit.Weapon.ClearWeapons();
            foreach (int weaponId in unitData.WeaponIdList)
            {
                if (weaponDataDict.TryGetValue(weaponId, out WeaponData weaponData))
                {
                    unit.Weapon.AddWeapon(weaponData);
                }
            }

            _unitMap[coord.x, coord.y] = unit;
            AddSpawnedUnit(unitData.UnitType, unit);
        }
    }

    public void ResetAndSpawnUnits(List<UnitBase> unitList, List<UnitSaveData> unitSaveDataList)
    {
        unitList.Clear();

        for (int i = 0; i < _unitMap.GetLength(1); i++)
        {
            for (int j = 0; j < _unitMap.GetLength(0); j++)
            {
                _unitMap[j, i] = null;
            }
        }

        Dictionary<int, UnitData> unitDataDict = _dataManager.UnitDataDict;

        foreach (UnitSaveData unitSaveData in unitSaveDataList)
        {
            if (!unitDataDict.TryGetValue(unitSaveData.UnitPosition.Id, out UnitData unitData)) continue;

            if (!_unitPrefabDict.TryGetValue(unitData.Id, out UnitBase unitPrefab)) continue;

            UnitBase unit = _unitObjectPool.Take(unitPrefab);
            unit.SetObjectPool(_unitObjectPool);
            unit.EffectController.SetProjectileObjectPool(_projectileObjectPool);

            Vector2Int coord = unitSaveData.UnitPosition.Position;
            unit.transform.position = coord.CoordToWorld();
            unit.transform.forward = unitSaveData.UnitPosition.Direction;
            unit.Stat.InitStat(unitData, unitSaveData.Health, unitSaveData.Energy);
            unit.DistanceRange.SetMap(_tileMap);

            Dictionary<int, WeaponData> weaponDataDict = _dataManager.WeaponDataDict;

            unit.Weapon.ClearWeapons();
            foreach (int weaponId in unitData.WeaponIdList)
            {
                if (weaponDataDict.TryGetValue(weaponId, out WeaponData weaponData))
                {
                    unit.Weapon.AddWeapon(weaponData);
                }
            }

            unitList.Add(unit);

            _unitMap[coord.x, coord.y] = unit;
        }
    }

    public void SetRevertTurn(List<UnitBase> unitList)
    {
        _spawnedUnitDict[UnitType.Player].Clear();
        _spawnedUnitDict[UnitType.Enemy].Clear();

        foreach (UnitBase unit in unitList)
        {
            AddSpawnedUnit(unit.UnitType, unit);
        }
    }

    private void CreateMapAndSpawnUnitsFromSave(SaveData saveData)
    {
        TurnSaveData turnSaveData = saveData.TurnSaveDataList[saveData.TurnSaveDataList.Count - 1];
        _turn = turnSaveData.Turn;
        _turnSaveDataList = saveData.TurnSaveDataList;
        GameManager.Instance.SetInitialSeed(saveData.InitialSeed);
        GameManager.Instance.SetLeftRevertTime(saveData.LeftRevertTime);

        Dictionary<int, UnitData> unitDataDict = _dataManager.UnitDataDict;
        _unitMap = new UnitBase[_tileMap.GetLength(0), _tileMap.GetLength(1)];

        foreach (UnitSaveData unitSaveData in turnSaveData.UnitSaveDatas)
        {
            if (!unitDataDict.TryGetValue(unitSaveData.UnitPosition.Id, out UnitData unitData)) continue;

            if (!_unitPrefabDict.TryGetValue(unitData.Id, out UnitBase unitPrefab)) continue;

            UnitBase unit = _unitObjectPool.Take(unitPrefab);
            unit.SetObjectPool(_unitObjectPool);
            unit.EffectController.SetProjectileObjectPool(_projectileObjectPool);

            Vector2Int coord = unitSaveData.UnitPosition.Position;
            unit.transform.position = coord.CoordToWorld();
            unit.transform.forward = unitSaveData.UnitPosition.Direction;
            unit.Stat.InitStat(unitData, unitSaveData.Health, unitSaveData.Energy);
            unit.DistanceRange.SetMap(_tileMap);

            Dictionary<int, WeaponData> weaponDataDict = _dataManager.WeaponDataDict;

            unit.Weapon.ClearWeapons();
            foreach (int weaponId in unitData.WeaponIdList)
            {
                if (weaponDataDict.TryGetValue(weaponId, out WeaponData weaponData))
                {
                    unit.Weapon.AddWeapon(weaponData);
                }
            }

            _unitMap[coord.x, coord.y] = unit;
            AddSpawnedUnit(unitData.UnitType, unit);
        }
    }

    private void AddSpawnedUnit(UnitType unitType, UnitBase unit)
    {
        if (_spawnedUnitDict.ContainsKey(unitType))
        {
            _spawnedUnitDict[unitType].Add(unit);
        }
        else
        {
            List<UnitBase> list = new List<UnitBase>();
            _spawnedUnitDict[unitType] = list;
            _spawnedUnitDict[unitType].Add(unit);
        }
    }
}
