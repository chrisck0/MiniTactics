using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour, IPopupController
{
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _popupBackgroundButton;
    [SerializeField] private SettingsPopupUI _settingsPopupUI;
    [SerializeField] private InputActionReference _cancelAction;

    private void Awake() => HidePopup();
    private void OnEnable()
    {
        BindButtonEvents();
        BindSoundEvents();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();
        UnbindSoundEvents();
    }

    private void GameStart()
    {
        gameObject.SetActive(false);
        _stageSelectUI.SetActive(true);
    }

    private void Settings()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _settingsPopupUI.Show(this);
        BindInputActions();
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Hide(InputAction.CallbackContext context)
    {
        HidePopup();
    }

    public void HidePopup()
    {
        _popupBackgroundButton.gameObject.SetActive(false);
        _settingsPopupUI.Hide();
        UnbindInputActions();
    }

    private void PlayUISound()
    {
        AudioManager.Instance.PlayUISfx();
    }

    private void BindButtonEvents()
    {
        _popupBackgroundButton.onClick.AddListener(HidePopup);
        _gameStartButton.onClick.AddListener(GameStart);
        _settingsButton.onClick.AddListener(Settings);
        _quitButton.onClick.AddListener(Quit);
    }

    private void UnbindButtonEvents()
    {
        _popupBackgroundButton.onClick.AddListener(HidePopup);
        _gameStartButton.onClick.RemoveListener(GameStart);
        _settingsButton.onClick.RemoveListener(Settings);
        _quitButton.onClick.RemoveListener(Quit);
    }

    private void BindSoundEvents()
    {
        _gameStartButton.onClick.AddListener(PlayUISound);
        _settingsButton.onClick.AddListener(PlayUISound);
        _quitButton.onClick.AddListener(PlayUISound);
    }

    private void UnbindSoundEvents()
    {
        _gameStartButton.onClick.RemoveListener(PlayUISound);
        _settingsButton.onClick.RemoveListener(PlayUISound);
        _quitButton.onClick.RemoveListener(PlayUISound);
    }

    private void BindInputActions()
    {
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Hide;
    }

    private void UnbindInputActions()
    {
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Hide;
    }
}
