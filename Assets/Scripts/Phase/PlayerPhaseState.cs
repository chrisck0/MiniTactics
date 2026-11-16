using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPhaseState : StateBase
{
    private PhaseStateController _controller;
    private UnitCommandStateController _unitCommandStateController;
    private MapManager _mapManager;
    private List<UnitBase> _playerUnitList;
    private List<UnitBase> _enemyUnitList;
    private List<PlayerUnit> _activePlayerUnit = new();
    private PlayerUnit _currentPlayerUnit;
    private bool _isAuto;
    private bool _isStateEnd;

    public int TotalActivePlayerUnit => _activePlayerUnit.Count;

    public PlayerPhaseState(PhaseStateController controller,
        UnitCommandStateController unitCommandStateController,
        MapManager mapManager,
        Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _controller = controller;
        _unitCommandStateController = unitCommandStateController;
        _mapManager = mapManager;

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;
        _playerUnitList = playerUnitList;

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;
        _enemyUnitList = enemyUnitList;
    }

    public override void Enter()
    {
        _isAuto = _controller.IsAuto;
        _isStateEnd = false;
        _activePlayerUnit.Clear();
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            _activePlayerUnit.Add(playerUnit);
            playerUnit.Stat.AddEnergy(5);
            AttackScoreCalculator.CalculateScore(
                playerUnit, _mapManager, playerUnit.transform.position.WorldToCoord());
        }

        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            enemyUnit.Stat.AddEnergy(5);

            // Danger 영역 표시를 위해 계산
            AttackScoreCalculator.CalculateScore(
                enemyUnit, _mapManager, enemyUnit.transform.position.WorldToCoord());
        }
        _unitCommandStateController.ShowAllDanger();

        BindCommandEvents();
    }

    public override void Update()
    {
        if (_controller.IsAuto) _isAuto = true;

        if (!_isAuto || _isStateEnd) return;

        if (_currentPlayerUnit == null)
        {
            if (_activePlayerUnit.Count != 0)
            {
                if (!_controller.IsAuto)
                {
                    _isAuto = false;
                    _controller.TurnOffAutoDuringPlayerPhase();
                    return;
                }

                _controller.SetPlayerCurrentTurnCount(_controller.GetPlayerCurrentTurnCount() + 1);
                _currentPlayerUnit = _activePlayerUnit[_activePlayerUnit.Count - 1];
                _activePlayerUnit.RemoveAt(_activePlayerUnit.Count - 1);

                BindPlayerUnitEvents();

                _currentPlayerUnit.UnitAI.Behave(
                    _controller.Turn,
                    _controller.TotalPlayerUnits,
                    _controller.GetPlayerCurrentTurnCount()
                    );
            }
            else
            {
                PlayerPhaseEnd();
                _isStateEnd = true;
                _controller.SetPlayerCurrentTurnCount(0);
            }
        }
    }

    public override void Exit()
    {
        UnbindCommandEvents();
    }

    private void UnitCommandEnd(PlayerUnit actionedPlayerUnit)
    {
        if (_isAuto) return;
        if (actionedPlayerUnit != null) _activePlayerUnit.Remove(actionedPlayerUnit);

        if (_activePlayerUnit.Count == 0)
        {
            PlayerPhaseEnd();
        }
        else
        {
            foreach (PlayerUnit playerUnit in _activePlayerUnit)
            {
                AttackScoreCalculator.CalculateScore(
                    playerUnit, _mapManager, playerUnit.transform.position.WorldToCoord());
            }
            _unitCommandStateController.StartUnitCommand();
        }
    }

    private void PlayerAutoActioned()
    {
        UnbindPlayerUnitEvents();
        if (_activePlayerUnit.Count != 0) WaitForSeconds().Forget();
        else _currentPlayerUnit = null;
    }

    private async UniTask WaitForSeconds()
    {
        await UniTask.Delay(1000, DelayType.DeltaTime);
        _currentPlayerUnit = null;
    }

    private void PlayerPhaseEnd()
    {
        _controller.StartEnemyPhase();
    }

    private void BindCommandEvents()
    {
        _unitCommandStateController.OnUnitCommandEnd += UnitCommandEnd;
    }

    private void UnbindCommandEvents()
    {
        _unitCommandStateController.OnUnitCommandEnd -= UnitCommandEnd;
    }

    private void BindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnUnitActioned += PlayerAutoActioned;
    }

    private void UnbindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnUnitActioned -= PlayerAutoActioned;
    }
}
