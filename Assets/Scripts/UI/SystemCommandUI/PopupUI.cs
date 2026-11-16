using System;
using UnityEngine;

public abstract class PopupUI : MonoBehaviour
{
    protected IPopupController _popupController;

    public virtual void Show(IPopupController popupController)
    {
        _popupController = popupController;
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected void ClosePopup()
    {
        _popupController.HidePopup();
    }
}