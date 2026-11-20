using UnityEngine;
using UnityEngine.UI;

public class UnitListPopupUI : PopupUI
{
    [SerializeField] private Button _playerUnitButton;
    [SerializeField] private Button _enemyUnitButton;
    [SerializeField] private UnitContainerListUI _containerList;
    private SystemCommandUI _systemCommandUI;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public override void Show(IPopupController popupController)
    {
        base.Show(popupController);
        _systemCommandUI = popupController as SystemCommandUI;
        ShowPlayerUnit();
        _systemCommandUI.HideCommandUIInfo();
    }

    public override void Hide()
    {
        base.Hide();
        _systemCommandUI.ShowCommandUIInfo();
    }

    private void ShowPlayerUnit()
    {
        _containerList.HideAllUnitContainers();
        _containerList.ShowUnitContainers(_systemCommandUI.GetPlayerList());
    }

    private void ShowEnemyUnit()
    {
        _containerList.HideAllUnitContainers();
        _containerList.ShowUnitContainers(_systemCommandUI.GetEnemyList());
    }

    private void BindButtonEvents()
    {
        _playerUnitButton.onClick.AddListener(ShowPlayerUnit);
        _enemyUnitButton.onClick.AddListener(ShowEnemyUnit);
    }

    private void UnbindButtonEvents()
    {
        _playerUnitButton.onClick.RemoveListener(ShowPlayerUnit);
        _enemyUnitButton.onClick.RemoveListener(ShowEnemyUnit);
    }
}
