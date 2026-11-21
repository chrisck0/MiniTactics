using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class UnitCommandStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileCursor _tileCursor;
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private TrajectoryController _trajectoryController;
    [SerializeField] private CommandUIController _commandUIController;
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private InputActionReference _infoAction;
    [SerializeField] private InputActionReference _cancelAction;
    [SerializeField] private InputActionReference _cursorPosition;
    [SerializeField] private LayerMask _ground;

    private StateBase _unitSelectState;
    private StateBase _unitMoveState;
    private StateBase _unitCommandState;
    private StateBase _targetSelectState;
    private StateBase _commandEndState;
    private PlayerUnit _selectedPlayerUnit;
    private Vector2Int _previousPlayerCoord;
    private Vector3 _previousPlayerDirection;
    private WeaponData _selectedWeaponData;
    private int _currentTurnCount;
    private List<EnemyUnit> _showingDangerEnemyList = new();

    public Action<StateBase> OnStateChanged { get; set; }
    public Action<PlayerUnit> OnUnitCommandEnd;

    public MapManager MapManager => _mapManager;
    public StateBase UnitSelectState => _unitSelectState;
    public StateBase UnitMoveState => _unitMoveState;
    public StateBase UnitCommandState => _unitCommandState;
    public StateBase TargetSelectState => _targetSelectState;
    public StateBase CommandEndState => _commandEndState;
    public PlayerUnit SelectedPlayerUnit => _selectedPlayerUnit;
    public Vector2Int PreviousPlayerCoord => _previousPlayerCoord;
    public Vector3 PreviousPlayerDirection => _previousPlayerDirection;
    public WeaponData SelectedWeaponData => _selectedWeaponData;
    public int Turn => _phaseStateController.Turn;
    public int TotalPlayerUnits => _phaseStateController.TotalPlayerUnits;
    public int CurrentTurnCount => _currentTurnCount;
    public List<EnemyUnit> ShowingDangerEnemyList => _showingDangerEnemyList;
    public bool IsSystemCommandPopupOpen => _commandUIController.IsSystemCommandPopupOpen;
    public bool IsAuto => _phaseStateController.IsAuto;

    private void Awake() => InitUnitCommandStates();
    private void OnEnable() => BindPhaseEvents();
    private void Start() => InitUI();
    private void OnDisable() => UnbindPhaseEvents();

    private void InitUnitCommandStates()
    {
        _unitSelectState = new UnitSelectState(
            this, _selectAction, _infoAction, _cancelAction, _cursorPosition, _ground);
        _unitMoveState = new UnitMoveState(
            this, _selectAction, _infoAction, _cancelAction, _cursorPosition, _ground);
        _unitCommandState = new UnitCommandState(
            this, _cancelAction, _commandUIController);
        _targetSelectState = new TargetSelectState(
            this, _selectAction, _cancelAction, _cursorPosition, _ground);
        _commandEndState = new CommandEndState(this);
    }

    private void InitUI()
    {
        _tileCursor.gameObject.SetActive(false);
        _commandUIController.DeactivateAll();
    }

    public void InitUnitCommand()
    {
        _currentTurnCount = 0;
        StartUnitCommand();
    }

    public void SetCurrentTurnCount(int num)
    {
        _currentTurnCount = num;
    }

    public void StartUnitCommand()
    {
        _currentTurnCount++;
        SetState(_unitSelectState);
    }

    private void EndUnitCommand()
    {
        SetState(_commandEndState);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }

    public void SetSelectedPlayerUnit(PlayerUnit playerUnit)
    {
        _selectedPlayerUnit = playerUnit;

        if (playerUnit == null) return;
        _previousPlayerCoord = playerUnit.transform.position.WorldToCoord();
        _previousPlayerDirection = playerUnit.transform.forward;
    }

    public void SetSelectedWeaponData(WeaponData weaponData)
    {
        _selectedWeaponData = weaponData;
    }

    public void ShowCursor(Vector2Int coord)
    {
        _tileCursor.SetPosition(coord);
        _tileCursor.Show();
    }

    public void CameraTrackTarget(Vector2Int coord)
    {
        _cameraMove.SetCameraTrackTarget(coord);
    }

    public void CanMoveCamera(bool canMove)
    {
        _cameraMove.CanMoveCamera(canMove);
    }

    public void ShowSystemCommandUI()
    {
        _commandUIController.SystemCommandUI.Show();
    }

    public void ShowTileInfo(TileBase tile)
    {
        _commandUIController.TileInfoUI.Show();
        _commandUIController.TileInfoUI.SetData(tile);
    }

    public void ShowUnitMoveandAttack(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            tile.HideRange();
            return;
        }
    }

    public void ShowUnitInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _commandUIController.UnitInfoUI.Hide();
            _commandUIController.AttackInfoUI.Hide();
            return;
        }

        _commandUIController.UnitInfoUI.Show();
        _commandUIController.UnitInfoUI.SetData(unit);

        EnemyUnit enemy = unit as EnemyUnit;
        if (enemy == null || enemy.UnitAI.MaxAttackScore.Score == 0)
        {
            _commandUIController.AttackInfoUI.Hide();
            return;
        }

        _commandUIController.AttackInfoUI.Show();
        _commandUIController.AttackInfoUI.SetData(enemy.UnitAI.MaxAttackScore.AttackWeaponData);
    }

    public void ShowUnitDetailedInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _commandUIController.UnitDetailedInfoUI.Hide();
            return;
        }

        _commandUIController.UnitDetailedInfoUI.Show();
        _commandUIController.UnitDetailedInfoUI.SetData(unit);
    }

    public void ShowBattleInfo(UnitStat playerStat, UnitStat enemyStat,
        WeaponData playerWeaponData, WeaponData enemyWeaponData,
        bool canReturnAttack, bool isPlayerPhase)
    {
        _commandUIController.BattleInfoUI.SetData(playerStat, enemyStat, playerWeaponData, enemyWeaponData,
            canReturnAttack, isPlayerPhase);
        _commandUIController.BattleInfoUI.Show();
    }

    public void ShowKeyInfo()
    {
        _commandUIController.KeyInfoUI.Show();
    }

    public void ShowSelectAndDangerKeyInfo(int siblingIndex)
    {
        _commandUIController.KeyInfoUI.SetSelectAndDangerKey(siblingIndex);
    }

    public void ShowSelectKeyInfo(int siblingIndex)
    {
        _commandUIController.KeyInfoUI.SetSelectKey(siblingIndex);
    }
    
    public void ShowInfoKeyInfo(int siblingIndex)
    {
        _commandUIController.KeyInfoUI.SetInfoKey(siblingIndex);
    }
    
    public void ShowCancelKeyInfo(int siblingIndex)
    {
        _commandUIController.KeyInfoUI.SetCancelKey(siblingIndex);
    }

    private void GameEnd()
    {
        _stateMachine.enabled = false;
    }

    public void ShowTileUI(TileBase tile)
    {
        if (tile == null) return;

        Vector2Int coord = tile.transform.position.WorldToCoord();

        ShowUnitInfo(tile);
        ShowTileInfo(tile);
        ShowCursor(coord);
    }

    public void HideCursor()
    {
        _tileCursor.Hide();
    }

    public void HideSystemCommandUI()
    {
        _commandUIController.SystemCommandUI.Hide();
    }

    public void HideSystemCommandPopup()
    {
        _commandUIController.SystemCommandUI.HidePopup();
    }

    public void HideTileUI()
    {
        HideTileInfo();
        HideUnitInfo();
    }

    public void HideTileInfo()
    {
        _commandUIController.TileInfoUI.Hide();
    }

    public void HideUnitInfo()
    {
        _commandUIController.UnitInfoUI.Hide();
    }

    public void HideUnitDetailedInfo()
    {
        _commandUIController.UnitDetailedInfoUI.Hide();
    }

    public void HideBattleInfo()
    {
        _commandUIController.BattleInfoUI.Hide();
    }

    public void HideKeyInfo()
    {
        _commandUIController.KeyInfoUI.Hide();
    }

    public void ShowAllDanger()
    {
        foreach (EnemyUnit enemyUnit in _showingDangerEnemyList)
        {
            enemyUnit.UnitAI.ShowDanger();
        }
    }

    public void InitDanger()
    {
        for (int i = 0; i < _mapManager.UnitMap.GetLength(1); i++)
        {
            for (int j = 0; j < _mapManager.UnitMap.GetLength(0); j++)
            {
                _mapManager.TileMap[j, i].HideDanger();
            }
        }
    }

    public void ToggleDanger(EnemyUnit enemyUnit)
    {
        if (enemyUnit.UnitAI.IsShowingDanger)
        {
            enemyUnit.UnitAI.SetIsShowingDanger(false);
            ShowingDangerEnemyList.Remove(enemyUnit);
            InitDanger();
            ShowAllDanger();
        }

        else
        {
            enemyUnit.UnitAI.SetIsShowingDanger(true);
            ShowingDangerEnemyList.Add(enemyUnit);
            InitDanger();
            ShowAllDanger();
        }
    }

    public void CalculateEnemyScore()
    {
        foreach (EnemyUnit enemyUnit in _phaseStateController.EnemyUnitList)
        {
            enemyUnit.UnitAI.CalculateScore();

            if (enemyUnit.UnitAI.MaxAttackScore.Score == 0) continue;
            enemyUnit.UnitAI.SetTargets(enemyUnit.UnitAI.MaxAttackScore.CurrentCoord);
        }
    }

    public void ShowTrajectory(EnemyUnit enemy)
    {
        _trajectoryController.ShowTrajectories(enemy, _mapManager);
    }

    public void ShowAllTrajectories()
    {
        foreach (EnemyUnit enemyUnit in _phaseStateController.EnemyUnitList)
        {
            _trajectoryController.ShowTrajectories(enemyUnit, _mapManager);
        }
    }

    public void HideAllTrajectories()
    {
        _trajectoryController.HideAllTrajectories();
    }

    private void BindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted += InitUnitCommand;
        _phaseStateController.OnEnemyPhaseStarted += EndUnitCommand;
        _phaseStateController.OnGameEnd += GameEnd;
    }

    private void UnbindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted -= InitUnitCommand;
        _phaseStateController.OnEnemyPhaseStarted -= EndUnitCommand;
        _phaseStateController.OnGameEnd -= GameEnd;
    }
}
