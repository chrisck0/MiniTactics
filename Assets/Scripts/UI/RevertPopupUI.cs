using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RevertPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _revertTurn;
    [SerializeField] private TextMeshProUGUI _leftRevertTime;
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private InputActionReference _leftArrowAction;
    [SerializeField] private InputActionReference _rightArrowAction;
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private InputActionReference _cancelAction;
    [SerializeField] private GameObject _revertBackground;

    private SystemCommandUI _systemCommandUI;
    private List<UnitBase> _unitList = new();
    private List<UnitSaveData> _unitSaveDataList = new(); 
    private int _turn;
    private bool _isRevertingTime;

    private void OnEnable() => BindInputActions();
    private void OnDisable() => UnbindInputActions();

    public void Show(SystemCommandUI systemCommandUI)
    {
        _phaseStateController.ResetDanger();
        _unitSaveDataList.Clear();
        _systemCommandUI = systemCommandUI;
        _systemCommandUI.HideCommandUIInfo();
        _systemCommandUI.HideCommandButtons();
        _revertBackground.SetActive(false);

        foreach (UnitBase unit in _phaseStateController.PlayerUnitList)
        {
            _unitList.Add(unit);
        }
        
        foreach (UnitBase unit in _phaseStateController.EnemyUnitList)
        {
            _unitList.Add(unit);
        }

        _turn = _phaseStateController.Turn;
        SetTurnText();
        SetLeftRevertTimeText();
        _isRevertingTime = false;
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        _systemCommandUI.ShowCommandUIInfo();
        _systemCommandUI.ShowCommandButtons();
        gameObject.SetActive(false);
    }

    private void SetTurnText()
    {
        _revertTurn.text = _turn.ToString();
    }

    private void SetLeftRevertTimeText()
    {
        _leftRevertTime.text = GameManager.Instance.GetLeftRevertTime().ToString();
    }

    private void ShowPreviousTurn(InputAction.CallbackContext context)
    {
        if (!_isRevertingTime)
        {
            if (GameManager.Instance.GetLeftRevertTime() != 0)
            {
                GameManager.Instance.SetLeftRevertTime(GameManager.Instance.GetLeftRevertTime() - 1);
                _isRevertingTime = true;
                ShowTurnSituation();
                SetLeftRevertTimeText();
                _revertBackground.SetActive(true);
                _cancelAction.action.Disable();
            }
        }
        else
        {
            if (_turn == 1) return;
            _turn--;
            SetTurnText();
            ShowTurnSituation();
        }
    }

    private void ShowNextTurn(InputAction.CallbackContext context)
    {
        if (!_isRevertingTime || _turn == _phaseStateController.Turn) return;
        _turn++;
        SetTurnText();
        ShowTurnSituation();
    }

    private void ShowTurnSituation()
    {
        if (!_isRevertingTime) return;
        SetTurnText();
        _phaseStateController.RevertTurn(_unitList, _turn);
    }

    private void SetRevertTurn(InputAction.CallbackContext context)
    {
        if (!_isRevertingTime) return;
        _phaseStateController.SetRevertTurn(_unitList, _turn);
        Hide();
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        Hide();
    }

    private void BindInputActions() 
    {
        _leftArrowAction.action.Enable();
        _leftArrowAction.action.performed += ShowPreviousTurn;
        _rightArrowAction.action.Enable();
        _rightArrowAction.action.performed += ShowNextTurn;
        _selectAction.action.performed += SetRevertTurn;
        _cancelAction.action.performed += Cancel;
    }

    private void UnbindInputActions()
    {
        _leftArrowAction.action.Disable();
        _leftArrowAction.action.performed -= ShowPreviousTurn;
        _rightArrowAction.action.Disable();
        _rightArrowAction.action.performed -= ShowNextTurn;
        _selectAction.action.performed -= SetRevertTurn;
        _cancelAction.action.Enable();
        _cancelAction.action.performed -= Cancel;
    }
}
