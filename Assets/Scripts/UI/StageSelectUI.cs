using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] private InputActionReference _cancelInput;
    [SerializeField] private SaveLoader _saveLoader;
    [SerializeField] private List<StageSelectController> _stageSelectControllers;
    [SerializeField] private Button _popupBackgroundButton;
    [SerializeField] private StageSelectPopupUI _stageSameDataSelectPopupUI;
    [SerializeField] private StageSelectPopupUI _stageDifferentDataSelectPopupUI;
    [SerializeField] private RectTransform _saveDataStage;
    private SaveData _saveData;
    private StageSelectPopupUI _currentPopup;

    public SaveLoader SaveLoader => _saveLoader;

    private void OnEnable()
    {
        BindInputActions();
        BindButtonEvents();
        BindControllerEvents();
        DeactivatePopups();
        ShowSaveDataStage();
    }

    private void OnDisable()
    {
        UnbindInputActions();
        UnbindButtonEvents();
        UnbindControllerEvents();
    }

    private void ShowPopup(StageSelectController stageSelectController)
    {
        if (_saveData == null)
        {
            stageSelectController.LoadScene();
            return;
        }

        _popupBackgroundButton.gameObject.SetActive(true);
        if (_saveData.StageId == stageSelectController.StageData.StageId)
        {
            _stageSameDataSelectPopupUI.Show(this, stageSelectController);
            _currentPopup = _stageSameDataSelectPopupUI;
        }
        else
        {
            _stageDifferentDataSelectPopupUI.Show(this, stageSelectController);
            _currentPopup = _stageDifferentDataSelectPopupUI;
        }
    }

    public void HidePopup()
    {
        if (_currentPopup == null) return;
        _currentPopup.gameObject.SetActive(false);
        _popupBackgroundButton.gameObject.SetActive(false);
    }

    private void ShowSaveDataStage()
    {
        _saveData = _saveLoader.LoadSaveData();
        if (_saveData == null)
        {
            _saveDataStage.gameObject.SetActive(false);
        }
        else
        {
            _saveDataStage.gameObject.SetActive(false);

            foreach (StageSelectController stageSelectController in _stageSelectControllers)
            {
                if (stageSelectController.StageData.StageId == _saveData.StageId)
                {
                    _saveDataStage.transform.SetParent(stageSelectController.transform);
                    _saveDataStage.anchoredPosition = Vector2.zero;
                    _saveDataStage.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    private void DeactivatePopups()
    {
        _stageSameDataSelectPopupUI.gameObject.SetActive(false);
        _stageDifferentDataSelectPopupUI.gameObject.SetActive(false);
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        HidePopup();
    }

    private void BindControllerEvents()
    {
        foreach (StageSelectController stageSelectController in _stageSelectControllers)
        {
            stageSelectController.OnButtonClicked += ShowPopup;
        }
    }

    private void UnbindControllerEvents()
    {
        foreach (StageSelectController stageSelectController in _stageSelectControllers)
        {
            stageSelectController.OnButtonClicked -= ShowPopup;
        }
    }

    private void BindInputActions()
    {
        _cancelInput.action.Enable();
        _cancelInput.action.performed += Cancel;
    }

    private void UnbindInputActions()
    {
        _cancelInput.action.Disable();
        _cancelInput.action.performed -= Cancel;
    }

    private void BindButtonEvents()
    {
        _popupBackgroundButton.onClick.AddListener(HidePopup);
    }

    private void UnbindButtonEvents()
    {
        _popupBackgroundButton.onClick.RemoveListener(HidePopup);
    }
}
