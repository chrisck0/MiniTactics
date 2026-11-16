using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SystemCommandUI : MonoBehaviour
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private GameSceneController _gameSceneController;
    [SerializeField] private GameObject _turn;
    [SerializeField] private GameObject _activeUnits;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _activeUnitsText;
    [SerializeField] private Button _phaseEndButton;
    [SerializeField] private Button _unitListButton;
    [SerializeField] private Button _autoButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _retreatButton;
    [SerializeField] private YesNoPopupUI _yesNoPopup;
    [SerializeField] private UnitListPopupUI _unitListPopup;

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
        _currentPopup.Hide();
        _currentPopup = null;
    }

    private void ShowPhaseEndPopup()
    {
        _yesNoPopup.Show(this);
        _yesNoPopup.BindPhaseEndEvents();
        _currentPopup = _yesNoPopup;
    }

    private void ShowUnitListPopup()
    {
        _unitListPopup.Show(this);
        _currentPopup = _unitListPopup;
    }

    private void ShowAutoPopup()
    {
        _yesNoPopup.Show(this);
        _yesNoPopup.BindAutoEvents();
        _currentPopup = _yesNoPopup;
    }

    private void ShowRetreatPopup()
    {
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
        _unitCommandStateController.SetState(_unitCommandStateController.CommandEndState);
    }

    public void LoadMainMenu()
    {
        _unitCommandStateController.SetState(_unitCommandStateController.CommandEndState);
        _phaseStateController.SetState(_phaseStateController.PhaseEndState);
        _gameSceneController.LoadMainMenu();
    }

    public void ToggleAuto(bool isAuto)
    {
        _phaseStateController.ToggleAuto(isAuto);
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
        _phaseEndButton.onClick.AddListener(ShowPhaseEndPopup);
        _unitListButton.onClick.AddListener(ShowUnitListPopup);
        _autoButton.onClick.AddListener(ShowAutoPopup);
        _retreatButton.onClick.AddListener(ShowRetreatPopup);
    }

    private void UnbindButtonEvents()
    {
        _phaseEndButton.onClick.RemoveListener(ShowPhaseEndPopup);
        _unitListButton.onClick.RemoveListener(ShowUnitListPopup);
        _autoButton.onClick.RemoveListener(ShowAutoPopup);
        _retreatButton.onClick.RemoveListener(ShowRetreatPopup);
    }

    private void Init()
    {
        _yesNoPopup.gameObject.SetActive(false);
        _unitListPopup.gameObject.SetActive(false);
    }
}
