using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesNoPopupUI : PopupUI
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    private void PhaseEnd()
    {
        _systemCommandUI.HideCursor();
        _systemCommandUI.PhaseEnd();
        _systemCommandUI.Hide();
        ClosePopup();
    }

    private void Auto()
    {
        Debug.Log("Auto on");
        ClosePopup();
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
        _yesButton.onClick.AddListener(Auto);
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
}
