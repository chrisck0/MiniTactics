using UnityEngine;
using UnityEngine.UI;

public class StageSelectPopupUI : MonoBehaviour
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    private StageSelectUI _stageSelectUI;
    private StageSelectController _stageSelectController;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public void Show(StageSelectUI stageSelectUI, StageSelectController stageSelectController)
    {
        _stageSelectUI = stageSelectUI;
        _stageSelectController = stageSelectController;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _stageSelectUI.HidePopup();
    }

    public void LoadNewStage()
    {
        _stageSelectUI.SaveLoader.ClearSaveData();
        _stageSelectController.LoadScene();
    }

    public void LoadDataStage()
    {
        _stageSelectController.LoadScene();
    }

    private void PlayUISound()
    {
        AudioManager.Instance.PlayUISfx();
    }

    private void BindButtonEvents()
    {
        _yesButton.onClick.AddListener(PlayUISound);
        _noButton.onClick.AddListener(PlayUISound);
    }

    private void UnbindButtonEvents()
    {
        _yesButton.onClick.RemoveListener(PlayUISound);
        _noButton.onClick.RemoveListener(PlayUISound);
    }
}
