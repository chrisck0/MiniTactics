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
    [SerializeField] private Button _bestRecordButton;
    [SerializeField] private BestRecordPopupUI _bestRecordPopupUI;
    [SerializeField] private RectTransform _saveDataStage;
    private SaveData _saveData;
    private GameObject _currentPopup;

    public SaveLoader SaveLoader => _saveLoader;

    private void OnEnable()
    {
        BindInputActions();
        BindButtonEvents();
        BindControllerEvents();
        DeactivatePopups();
        ShowSaveDataStage();
        UnlockStages();
    }

    private void OnDisable()
    {
        UnbindInputActions();
        UnbindButtonEvents();
        UnbindControllerEvents();
    }

    private void ShowStageSelectPopup(StageSelectController stageSelectController)
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
            _currentPopup = _stageSameDataSelectPopupUI.gameObject;
        }
        else
        {
            _stageDifferentDataSelectPopupUI.Show(this, stageSelectController);
            _currentPopup = _stageDifferentDataSelectPopupUI.gameObject;
        }
    }

    public void HidePopup()
    {
        if (_currentPopup == null) return;
        _currentPopup.SetActive(false);
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

    private void UnlockStages()
    {
        ClearData clearData = _saveLoader.LoadClearData();

        if (clearData == null)
        {
            _stageSelectControllers[0].Unlock();
        }
        else
        {
            int clearedStages = clearData.StageClearDataList.Count;

            for (int i = 0; i <= clearedStages; i++)
            {
                if (i >= _stageSelectControllers.Count) break;
                _stageSelectControllers[i].Unlock();
            }
        }
    }

    private void ShowBestRecordPopup()
    {
        _bestRecordPopupUI.Show(_saveLoader.LoadClearData());
        _currentPopup = _bestRecordPopupUI.gameObject;
        _popupBackgroundButton.gameObject.SetActive(true);
    }

    private void DeactivatePopups()
    {
        _stageSameDataSelectPopupUI.gameObject.SetActive(false);
        _stageDifferentDataSelectPopupUI.gameObject.SetActive(false);
        _bestRecordPopupUI.gameObject.SetActive(false);
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        HidePopup();
    }

    private void BindControllerEvents()
    {
        foreach (StageSelectController stageSelectController in _stageSelectControllers)
        {
            stageSelectController.OnButtonClicked += ShowStageSelectPopup;
        }
    }

    private void UnbindControllerEvents()
    {
        foreach (StageSelectController stageSelectController in _stageSelectControllers)
        {
            stageSelectController.OnButtonClicked -= ShowStageSelectPopup;
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
        _bestRecordButton.onClick.AddListener(ShowBestRecordPopup);
    }

    private void UnbindButtonEvents()
    {
        _popupBackgroundButton.onClick.RemoveListener(HidePopup);
        _bestRecordButton.onClick.RemoveListener(ShowBestRecordPopup);
    }
}
