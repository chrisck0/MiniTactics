using UnityEngine;
using UnityEngine.InputSystem;

public sealed class UnitSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _infoAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private UnitBase _prevUnit;
    private bool _isInfoOpen;
    private bool _isSystemOpen;
    private bool _isPopupOpen => _controller.IsSystemCommandPopupOpen;
    private bool _isBinded;

    public UnitSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference infoAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground)
    {
        _controller = controller;
        _selectAction = selectAction;
        _infoAction = infoAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
    }

    public override void Enter()
    {
        BindInputActions();
        _controller.CanMoveCamera(true);
        _controller.CalculateEnemyScore();
        _currentTile = null;
        _isInfoOpen = false;
        _isSystemOpen = false;
        _controller.SetSelectedPlayerUnit(null);
        if (!_controller.IsAuto) ShowKeyInfo();
    }

    public override void Update()
    {
        if (_controller.IsAuto && _isBinded)
        {
            _controller.CanMoveCamera(false);
            UnbindInputActions();
        }

        if (_isInfoOpen || _isSystemOpen) return;

        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            // _currentTile = null;
            // _controller.HideCursor();
            // _controller.HideTileUI();
            // HideDistanceRange();
            // _controller.HideAllTrajectories();
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;
        _currentTile = tile;
        _controller.ShowCursor(tile.transform.position.WorldToCoord());
        _controller.ShowTileUI(tile);
        ShowDistanceRange(tile);
    }

    public override void Exit()
    {
        _currentTile = null;
        HideSystemCommand();
        HideDistanceRange();
        UnbindInputActions();
        _controller.HideKeyInfo();
        _controller.CanMoveCamera(false);
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null || _isInfoOpen || _isSystemOpen) return;

        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            ShowSystemCommand();
            return;
        }

        switch (unit.UnitType)
        {
            case UnitType.Player:
                PlayerUnit playerUnit = (unit as PlayerUnit);

                if (playerUnit == null || !playerUnit.CanAction) return;

                _controller.SetSelectedPlayerUnit(playerUnit);
                playerUnit.Select();
                _controller.SetState(_controller.UnitMoveState);
                break;

            case UnitType.Enemy:
                EnemyUnit enemyUnit = (unit as EnemyUnit);

                if (enemyUnit == null) return;
                
                _controller.ToggleDanger(enemyUnit);
                break;
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (_isPopupOpen)
        {
            _controller.HideSystemCommandPopup();
            return;
        }

        if (_isInfoOpen) HideDetailedInfo();
        if (_isSystemOpen) HideSystemCommand();
    }

    private void ShowDistanceRange(TileBase tile)
    {
        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == _prevUnit) return;

        HideDistanceRange();
        _controller.HideAllTrajectories();

        if (unit == null
            || unit.UnitType == UnitType.Player && !(unit as PlayerUnit).CanAction) return;

        _prevUnit = unit;

        unit.DistanceRange.ShowTargetable();
        unit.DistanceRange.ShowReachable();

        EnemyUnit enemy = unit as EnemyUnit;
        if (enemy != null) _controller.ShowTrajectory(enemy);
    }

    private void HideDistanceRange()
    {
        if (_prevUnit == null) return;

        _prevUnit.DistanceRange.HideTargetable();
        _prevUnit.DistanceRange.HideReachable();
        _prevUnit = null;
    }

    private void ShowKeyInfo()
    {
        _controller.ShowKeyInfo();
        _controller.ShowSelectKeyInfo(1);
        _controller.ShowInfoKeyInfo(2);
        _controller.ShowCancelKeyInfo(3);
    }

    private void ShowDetailedInfo(InputAction.CallbackContext context)
    {
        if (_currentTile == null) return;

        if (_isInfoOpen) HideDetailedInfo();
        else
        {
            _controller.CanMoveCamera(false);
            Vector2Int coord = _currentTile.transform.position.WorldToCoord();

            UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

            if (unit == null) return;

            _controller.HideTileUI();
            _controller.ShowUnitDetailedInfo(_currentTile);
            _isInfoOpen = true;
        }
    }

    private void HideDetailedInfo()
    {
        _controller.CanMoveCamera(true);
        _controller.HideUnitDetailedInfo();
        _controller.ShowTileUI(_currentTile);
        _isInfoOpen = false;
    }

    private void ShowSystemCommand()
    {
        _controller.CanMoveCamera(false);
        _controller.HideTileUI();
        _controller.ShowSystemCommandUI();
        _isSystemOpen = true;
    }

    private void HideSystemCommand()
    {
        _controller.CanMoveCamera(true);
        _controller.HideSystemCommandUI();
        _controller.ShowTileUI(_currentTile);
        _isSystemOpen = false;
    }

    private void BindInputActions()
    {
        _isBinded = true;
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _infoAction.action.Enable();
        _infoAction.action.performed += ShowDetailedInfo;
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _isBinded = false;
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _infoAction.action.Disable();
        _infoAction.action.performed -= ShowDetailedInfo;
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
        _cursorPosition.action.Disable();
    }
}
