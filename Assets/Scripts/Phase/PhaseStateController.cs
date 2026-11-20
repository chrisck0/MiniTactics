using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private SaveLoader _saveLoader; // 디버깅
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private GameUIController _gameUIController;
    [SerializeField] private SystemCommandUI _systemCommandUI;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;
    private StateBase _phaseEndState;
    private int _turn;
    private int _initialPlayerNum;
    private int _initialEnemyNum;
    private List<UnitBase> _playerUnitList;
    private List<UnitBase> _enemyUnitList;
    private bool _isAuto;
    private List<TurnSaveData> _turnSaveDataList;

    public event Action OnGameEnd;

    public Action<StateBase> OnStateChanged { get; set; }
    public event Action OnPlayerPhaseStarted;
    public event Action OnEnemyPhaseStarted;
    public MapManager MapManager => _mapManager;
    public int Turn => _turn;
    public int TotalPlayerUnits => _playerUnitList.Count;
    public int TotalEnemyUnits => _enemyUnitList.Count;
    public int TotalActivePlayerUnit => (_playerPhaseState as PlayerPhaseState).TotalActivePlayerUnit;
    public bool IsAuto => _isAuto;
    public List<UnitBase> PlayerUnitList => _playerUnitList;
    public List<UnitBase> EnemyUnitList => _enemyUnitList;
    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;
    public StateBase PhaseEndState => _phaseEndState;
    public CameraMove CameraMove => _cameraMove;
    public List<TurnSaveData> TurnSaveDataList => _turnSaveDataList;

    private void OnEnable()
    {
        BindSpawnEvents();
        InitUI();
    }

    private void OnDisable()
    {
        UnbindSpawnEvents();
    }

    private void InitUI()
    {
        _gameUIController.DeactivateAll();
    }

    private void ShowGameCondition()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += StartPhase;
        _gameUIController.GameWinLoseConditionUI.Show();
    }

    private void StartPhase(InputAction.CallbackContext context)
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= StartPhase;
        _gameUIController.GameWinLoseConditionUI.Hide();

        InitPhaseStates(
            _spawnManager.TurnSaveDataList,
            _spawnManager.SpawnedUnitDict, 
            _spawnManager.Turn,
            _spawnManager.CameraPosition
            );
        StartPlayerPhase();
    }

    private void InitPhaseStates(
        List<TurnSaveData> turnSaveDataList, Dictionary<UnitType, List<UnitBase>> spawnedUnitDict,
        int turn, Vector3 cameraPosition)
    {
        _turn = turn - 1; // 시작하자마자 플레이어 페이즈이므로 1 줄이기
        _turnSaveDataList = turnSaveDataList;

        StageData stageData = GameManager.Instance.StageData;

        _cameraMove.InitPosition(
            cameraPosition,
            new Vector3(0.5f, 0f, stageData.Map.GetLength(0) - .5f),
            new Vector3(0.5f, 0f, .5f),
            new Vector3(stageData.Map.GetLength(1) - .5f, 0f, stageData.Map.GetLength(0) - .5f),
            new Vector3(stageData.Map.GetLength(1) - .5f, 0f, 0.5f)
            );

        _playerPhaseState = new PlayerPhaseState(
            this, _unitCommandStateController, _mapManager, spawnedUnitDict);
        _enemyPhaseState = new EnemyPhaseState(this, spawnedUnitDict);
        _phaseEndState = new PhaseEndState(this);

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;
        _playerUnitList = playerUnitList;
        _initialPlayerNum = playerUnitList.Count;

        foreach (UnitBase playerUnitBase in _playerUnitList)
        {
            PlayerUnit playerUnit = playerUnitBase as PlayerUnit;

            if (playerUnit == null) continue;
            BindUnitEvents(playerUnit);
            playerUnit.UnitAI.SetData(_mapManager, UnitType.Enemy);
        }

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;
        _enemyUnitList = enemyUnitList;
        _initialEnemyNum = enemyUnitList.Count;

        foreach (UnitBase enemyUnitBase in _enemyUnitList)
        {
            EnemyUnit enemyUnit = enemyUnitBase as EnemyUnit;

            if (enemyUnit == null) continue;
            BindUnitEvents(enemyUnit);
            enemyUnit.UnitAI.SetData(_mapManager, UnitType.Player);
        }
    }

    private void SaveTurnSaveData()
    {
        List<UnitSaveData> unitSaveDatas = new();

        foreach (UnitBase unit in _playerUnitList)
        {
            UnitPosition unitPosition = new UnitPosition(
                unit.Stat.Data.Id, unit.transform.position.WorldToCoord(), unit.transform.forward);

            unitSaveDatas.Add(new UnitSaveData(unitPosition, unit.Stat.Health, unit.Stat.Energy));
        }

        foreach (UnitBase unit in _enemyUnitList)
        {
            UnitPosition unitPosition = new UnitPosition(
                unit.Stat.Data.Id, unit.transform.position.WorldToCoord(), unit.transform.forward);

            unitSaveDatas.Add(new UnitSaveData(unitPosition, unit.Stat.Health, unit.Stat.Energy));
        }

        TurnSaveData saveData = new TurnSaveData(
            GameManager.Instance.StageData.StageId,
            _turn + 1,
            unitSaveDatas,
            _cameraMove.GetCameraPosition()
            );

        if (_turnSaveDataList.Count == 0
            || _turnSaveDataList[_turnSaveDataList.Count - 1].Turn != _turn + 1)
            _turnSaveDataList.Add(saveData);

        Debug.Log("TurnSaveData Saved");
    }

    public void StartPlayerPhase()
    {
        SaveTurnSaveData();
        WaitPlayerPhaseUI().Forget();
    }

    public void StartEnemyPhase()
    {
        WaitEnemyPhaseUI().Forget();
    }

    private async UniTask WaitPlayerPhaseUI()
    {
        _turn++;
        _gameUIController.PhaseUI.SetPlayerPhaseText();
        _gameUIController.PhaseUI.Show();
        SetEnemyUnitActive();

        await UniTask.Delay(3000, DelayType.Realtime);

        _gameUIController.PhaseUI.Hide();
        SetState(_playerPhaseState);
        OnPlayerPhaseStarted?.Invoke();
    }

    private async UniTask WaitEnemyPhaseUI()
    {
        _gameUIController.PhaseUI.SetEnemyPhaseText();
        _gameUIController.PhaseUI.Show();
        _unitCommandStateController.InitAllDanger();
        SetPlayerUnitActive();

        await UniTask.Delay(3000, DelayType.Realtime);

        _gameUIController.PhaseUI.Hide();
        SetState(_enemyPhaseState);
        OnEnemyPhaseStarted?.Invoke();
    }

    private void SetPlayerUnitActive()
    {
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            playerUnit.InitAction();
        }
    }

    private void SetEnemyUnitActive()
    {
        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            enemyUnit.OnUnitCanAction?.Invoke();
        }
    }

    private void UnitDead(UnitBase unit)
    {
        Debug.Log($"{unit.Stat.Data.Name} Died");
        switch (unit.UnitType)
        {
            case UnitType.Player:
                _playerUnitList.Remove(unit);
                break;

            case UnitType.Enemy:
                _enemyUnitList.Remove(unit);

                EnemyUnit enemyUnit = unit as EnemyUnit;
                if (enemyUnit == null) return;
                _unitCommandStateController.ShowingDangerEnemyList.Remove(enemyUnit);
                enemyUnit.UnitAI.HideDanger();
                break;
        }

        Vector2Int coord = unit.transform.position.WorldToCoord();
        _mapManager.UnitMap[coord.x, coord.y] = null;

        if (CheckGameLose(unit)) ShowGameResult(false);
        if (CheckGameWin(unit)) ShowGameResult(true);
        UnbindUnitEvents(unit);
    }

    private bool CheckGameWin(UnitBase unit)
    {
        switch(_spawnManager.StageData.StageWinCondition)
        {
            case StageWinCondition.Rout:
                if (_enemyUnitList.Count == 0) return true;
                break;

            case StageWinCondition.Targets:
                if (_spawnManager.StageData.TargetIds[0] == unit.Stat.Data.Id) return true;
                break;
        }

        return false;
    }

    private bool CheckGameLose(UnitBase unit)
    {
        foreach (int mainUnitId in _spawnManager.StageData.MainUnitIds)
        {
            if (mainUnitId == unit.Stat.Data.Id) return true;
        }
        return false;
    }

    private void ShowGameResult(bool isGameWin)
    {
        _gameUIController.ResultUI.SetData(isGameWin, _turn,
            _initialEnemyNum - _enemyUnitList.Count, _initialPlayerNum - _playerUnitList.Count);
        _stateMachine.enabled = false;
        // _mapManager.TileRange.InitTiles();
        OnGameEnd?.Invoke();
    }

    public void ClearActivePlayerUnit()
    {
        (_playerPhaseState as PlayerPhaseState).ClearActivePlayerUnit();
    }

    public void ToggleAuto(bool isAuto)
    {
        _isAuto = isAuto;
    }

    public void TurnOffAutoDuringPlayerPhase()
    {
        _unitCommandStateController.SetState(_unitCommandStateController.CommandEndState);
    }

    public void CameraTrackTarget(Vector2Int coord)
    {
        _cameraMove.SetCameraTrackTarget(coord);
    }

    public int GetPlayerCurrentTurnCount()
    {
        return _unitCommandStateController.CurrentTurnCount;
    }

    public void SetPlayerCurrentTurnCount(int num)
    {
        _unitCommandStateController.SetCurrentTurnCount(num);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }

    private void BindSpawnEvents()
    {
        _spawnManager.OnStageLoaded += ShowGameCondition;
    }

    private void UnbindSpawnEvents()
    {
        _spawnManager.OnStageLoaded -= ShowGameCondition;
    }

    private void BindUnitEvents(UnitBase unit)
    {
        unit.OnDead += UnitDead;
    }

    private void UnbindUnitEvents(UnitBase unit)
    {
        unit.OnDead -= UnitDead;
    }
}
