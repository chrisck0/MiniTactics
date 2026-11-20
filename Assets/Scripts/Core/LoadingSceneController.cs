using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private SceneReference _gameScene;
    [SerializeField] private DataLoader _dataLoader;

    private void OnEnable() => BindDataLoadEvents();
    private void OnDisable() => UnbindDataLoadEvents();

    public void LoadGameScene()
    {
        if (!_gameScene.IsValid)
        {
            Debug.LogWarning("GameSceneController : 씬이 연결되지 않았음");
            return;
        }

        SceneManager.LoadScene(_gameScene.SceneName);
    }

    private void BindDataLoadEvents()
    {
        _dataLoader.OnDataLoaded += LoadGameScene;
    }

    private void UnbindDataLoadEvents()
    {
        _dataLoader.OnDataLoaded -= LoadGameScene;
    }
}