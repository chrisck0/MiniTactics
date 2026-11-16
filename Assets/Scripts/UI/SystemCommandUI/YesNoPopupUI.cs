using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class YesNoPopupUI : PopupUI
{
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private InputActionReference _anyAction;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    private SystemCommandUI _systemCommandUI;

    public override void Show(IPopupController popupController)
    {
        base.Show(popupController);
        _systemCommandUI = popupController as SystemCommandUI;
    }

    private void PhaseEnd()
    {
        _systemCommandUI.HideCursor();
        _systemCommandUI.PhaseEnd();
        _systemCommandUI.Hide();
        ClosePopup();
    }

    private void AutoOn()
    {
        GameManager.Instance.LockCursor();
        Debug.Log("Auto on");
        _systemCommandUI.ToggleAuto(true);
        _systemCommandUI.HideCursor();
        _systemCommandUI.Hide();
        ClosePopup();
        BindInputEvents();
    }

    private void AutoOff(InputAction.CallbackContext context)
    {
        Debug.Log("Auto off");
        _systemCommandUI.ToggleAuto(false);
        GameManager.Instance.UnlockCursor();
        UnbindInputEvents();
    }

    private void Retreat()
    {
        _systemCommandUI.LoadMainMenu();
        ClosePopup();
    }

    public override void Hide()
    {
        base.Hide();
        UnbindEvents();
    }

    public void BindPhaseEndEvents()
    {
        _descriptionText.text = "페이즈를 종료하시겠습니까?";
        _yesButton.onClick.AddListener(PhaseEnd);
        _noButton.onClick.AddListener(ClosePopup);
    }

    public void BindAutoEvents()
    {
        _descriptionText.text = "오토를 활성화하시겠습니까?";
        _yesButton.onClick.AddListener(AutoOn);
        _noButton.onClick.AddListener(ClosePopup);
    }

    public void BindRetreatEvents()
    {
        _descriptionText.text = "퇴각하시겠습니까?\n(현재 진행 상황은 저장되지 않습니다)";
        _yesButton.onClick.AddListener(Retreat);
        _noButton.onClick.AddListener(ClosePopup);
    }

    public void UnbindEvents()
    {
        _yesButton.onClick.RemoveAllListeners();
        _noButton.onClick.RemoveAllListeners();
    }

    private void BindInputEvents()
    {
        _selectAction.action.actionMap.Disable();
        _anyAction.action.Enable();
        _anyAction.action.performed += AutoOff;
    }

    private void UnbindInputEvents()
    {
        _selectAction.action.actionMap.Enable();
        _anyAction.action.Disable();
        _anyAction.action.performed -= AutoOff;
    }
}
