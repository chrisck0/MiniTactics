using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemCommandUI : MonoBehaviour
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private GameSceneController _gameSceneController;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _activeUnits;
    [SerializeField] private Button _phaseEndButton;
    [SerializeField] private Button _playerUnitButton;
    [SerializeField] private Button _autoButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _retreatButton;
    [SerializeField] private YesNoPopupUI _yesNoPopup;

    private PopupUI _currentPopup;
    public bool IsPopupOpen => _currentPopup != null;

    private void Awake() => Init();
    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public void Show()
    {
        _turnText.text = _phaseStateController.Turn.ToString();
        _activeUnits.text = _phaseStateController.TotalActivePlayerUnit.ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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

    private void BindButtonEvents()
    {
        _phaseEndButton.onClick.AddListener(ShowPhaseEndPopup);
        _autoButton.onClick.AddListener(ShowAutoPopup);
        _retreatButton.onClick.AddListener(ShowRetreatPopup);
    }

    private void UnbindButtonEvents()
    {
        _phaseEndButton.onClick.RemoveListener(ShowPhaseEndPopup);
        _autoButton.onClick.RemoveListener(ShowAutoPopup);
        _retreatButton.onClick.RemoveListener(ShowRetreatPopup);
    }

    private void Init()
    {
        _yesNoPopup.gameObject.SetActive(false);
    }
}
