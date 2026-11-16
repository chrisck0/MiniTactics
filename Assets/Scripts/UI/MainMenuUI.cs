using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour, IPopupController
{
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _popupBackgroundButton;
    [SerializeField] private SettingsPopupUI _settingsPopupUI;

    private void Awake() => HidePopup();
    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    private void GameStart()
    {
        gameObject.SetActive(false);
        _stageSelectUI.SetActive(true);
    }

    private void Settings()
    {
        _popupBackgroundButton.gameObject.SetActive(true);
        _settingsPopupUI.Show(this);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HidePopup()
    {
        _popupBackgroundButton.gameObject.SetActive(false);
        _settingsPopupUI.Hide();
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
}
