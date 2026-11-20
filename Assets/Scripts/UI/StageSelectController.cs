using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private SceneReference _nextScene;
    [SerializeField] private StageData _stageData;
    [SerializeField] private Button _stageButton;
    [SerializeField] private GameObject _lockedBackground;

    public event Action<StageSelectController> OnButtonClicked;
    public StageData StageData => _stageData;

    private void OnEnable()
    {
        BindButtonEvents();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();
    }

    public void Unlock()
    {
        _lockedBackground.SetActive(false);
    }

    private void ButtonClick()
    {
        OnButtonClicked?.Invoke(this);
    }

    public void LoadScene()
    {
        GameManager.Instance.SetStageData(_stageData);
        SceneManager.LoadScene(_nextScene.SceneName);
    }

    private void BindButtonEvents()
    {
        _stageButton.onClick.AddListener(ButtonClick);
    }

    private void UnbindButtonEvents()
    {
        _stageButton.onClick.RemoveListener(ButtonClick);
    }
}
