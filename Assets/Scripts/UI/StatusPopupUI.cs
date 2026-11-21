using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusPopupUI : PopupUI
{
    [SerializeField] private GameWinLoseConditionUI _gameWinLoseConditionUI;
    [SerializeField] private MinimapUI _minimapUI;
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private TextMeshProUGUI _playerCountText;
    [SerializeField] private TextMeshProUGUI _enemyCountText;

    public override void Show(IPopupController popupController)
    {
        base.Show(popupController);
        _gameWinLoseConditionUI.Show();
        SetText();
        LoadMinimap();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SetMiniMap()
    {
        _minimapUI.SetMap();
    }

    private void LoadMinimap()
    {
        _minimapUI.LoadMinimap();
    }

    private void SetText()
    {
        _playerCountText.text = _phaseStateController.PlayerUnitList.Count.ToString();
        _enemyCountText.text = _phaseStateController.EnemyUnitList.Count.ToString();
    }
}
