using UnityEngine;
using UnityEngine.UI;

public class StatusPopupUI : PopupUI
{
    [SerializeField] private MinimapUI _minimapUI;

    public override void Show(IPopupController popupController)
    {
        base.Show(popupController);
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
}
