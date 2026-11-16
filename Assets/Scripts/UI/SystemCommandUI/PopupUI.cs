using System;
using UnityEngine;

public abstract class PopupUI : MonoBehaviour
{
    protected SystemCommandUI _systemCommandUI;

    public virtual void Show(SystemCommandUI systemCommandUI)
    {
        _systemCommandUI = systemCommandUI;
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected void ClosePopup()
    {
        _systemCommandUI.HidePopup();
    }
}