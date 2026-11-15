using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private SceneReference _mainMenuScene;
    [SerializeField] private SceneReference _gameScene;

    public void LoadMainMenu()
    {
        if (!_mainMenuScene.IsValid)
        {
            Debug.LogWarning("GameSceneController : 씬이 연결되지 않았음");
            return;
        }

        SceneManager.LoadScene(_mainMenuScene.SceneName);
    }

    public void LoadNextStage()
    {
        if (!_gameScene.IsValid)
        {
            Debug.LogWarning("GameSceneController : 씬이 연결되지 않았음");
            return;
        }

        //GameManger.SetStageData();
        SceneManager.LoadScene(_gameScene.SceneName);
    }
}
