using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SystemCommandUI : MonoBehaviour, IPopupController
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private GameSceneController _gameSceneController;
    [SerializeField] private SaveLoader _saveLoader;
    [SerializeField] private GameObject _turn;
    [SerializeField] private GameObject _activeUnits;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _activeUnitsText;
    [SerializeField] private Button _phaseEndButton;
    [SerializeField] private Button _unitListButton;
    [SerializeField] private Button _autoButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _suspendButton;
    [SerializeField] private Button _retreatButton;
    [SerializeField] private Button _popupBackgroundButton;
    [SerializeField] private YesNoPopupUI _yesNoPopup;
    [SerializeField] private UnitListPopupUI _unitListPopup;
    [SerializeField] private SettingsPopupUI _settingsPopup;
    [SerializeField] private GameObject _autoTextObject;

    private PopupUI _currentPopup;
    public bool IsPopupOpen => _currentPopup != null;

    private void Awake() => Init();
    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public void Show()
    {
        _turnText.text = _phaseStateController.Turn.ToString();
        _activeUnitsText.text = _phaseStateController.TotalActivePlayerUnit.ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowCommandUIInfo()
    {
        _turn.gameObject.SetActive(true);
        _activeUnits.gameObject.SetActive(true);
    }

    public void HideCommandUIInfo()
    {
        _turn.gameObject.SetActive(false);
        _activeUnits.gameObject.SetActive(false);
    }

    public void HidePopup()
    {
        if (_currentPopup == null) return;

        _popupBackgroundButton.gameObject.SetActive(false);
        _currentPopup.Hide();
        _currentPopup = null;
    }

    private void ShowPhaseEndPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _yesNoPopup.Show(this);
        _yesNoPopup.BindPhaseEndEvents();
        _currentPopup = _yesNoPopup;
    }

    private void ShowUnitListPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _unitListPopup.Show(this);
        _currentPopup = _unitListPopup;
    }

    private void ShowAutoPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _yesNoPopup.Show(this);
        _yesNoPopup.BindAutoEvents();
        _currentPopup = _yesNoPopup;
    }

    private void ShowSettingsPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _settingsPopup.Show(this);
        _currentPopup = _settingsPopup;
    }

    private void ShowSuspendPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _yesNoPopup.Show(this);
        _yesNoPopup.BindSuspendEvents();
        _currentPopup = _yesNoPopup;
    }

    private void ShowRetreatPopup()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _yesNoPopup.Show(this);
        _yesNoPopup.BindRetreatEvents();
        _currentPopup = _yesNoPopup;
    }

    public void HideCursor()
    {
        _unitCommandStateController.HideCursor();
    }

    public void PhaseEnd()
    {
        _phaseStateController.ClearActivePlayerUnit();
        _unitCommandStateController.SetState(_unitCommandStateController.CommandEndState);
    }

    public void SaveDataAndLoadMainMenu()
    {
        _saveLoader.SaveNewData();
        LoadMainMenu();
    }

    public void ClearSaveDataAndLoadMainMenu()
    {
        _saveLoader.ClearSaveData();
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        _unitCommandStateController.SetState(_unitCommandStateController.CommandEndState);
        _phaseStateController.SetState(_phaseStateController.PhaseEndState);
        _gameSceneController.LoadMainMenu();
    }

    public void ToggleAuto(bool isAuto)
    {
        _phaseStateController.ToggleAuto(isAuto);
        _autoTextObject.SetActive(isAuto);
    }

    public List<UnitBase> GetPlayerList()
    {
        return _phaseStateController.PlayerUnitList;
    }

    public List<UnitBase> GetEnemyList()
    {
        return _phaseStateController.EnemyUnitList;
    }

    private void BindButtonEvents()
    {
        _popupBackgroundButton.onClick.AddListener(HidePopup);
        _phaseEndButton.onClick.AddListener(ShowPhaseEndPopup);
        _unitListButton.onClick.AddListener(ShowUnitListPopup);
        _autoButton.onClick.AddListener(ShowAutoPopup);
        _settingsButton.onClick.AddListener(ShowSettingsPopup);
        _suspendButton.onClick.AddListener(ShowSuspendPopup);
        _retreatButton.onClick.AddListener(ShowRetreatPopup);
    }

    private void UnbindButtonEvents()
    {
        _popupBackgroundButton.onClick.RemoveListener(HidePopup);
        _phaseEndButton.onClick.RemoveListener(ShowPhaseEndPopup);
        _unitListButton.onClick.RemoveListener(ShowUnitListPopup);
        _autoButton.onClick.RemoveListener(ShowAutoPopup);
        _settingsButton.onClick.RemoveListener(ShowSettingsPopup);
        _suspendButton.onClick.RemoveListener(ShowSuspendPopup);
        _retreatButton.onClick.RemoveListener(ShowRetreatPopup);
    }

    private void Init()
    {
        _popupBackgroundButton.gameObject.SetActive(false);
        _yesNoPopup.gameObject.SetActive(false);
        _unitListPopup.gameObject.SetActive(false);
        _settingsPopup.gameObject.SetActive(false);
        _autoTextObject.SetActive(false);
    }
}
