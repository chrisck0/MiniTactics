using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private SceneReference _mainMenuScene;
    [SerializeField] private SceneReference _loadingScene;

    public void LoadMainMenu()
    {
        if (!_mainMenuScene.IsValid)
        {
            Debug.LogWarning("GameSceneController : 씬이 연결되지 않았음");
            return;
        }

        SceneManager.LoadScene(_mainMenuScene.SceneName);
    }

    public void LoadCurrentStage()
    {
        SceneManager.LoadScene(_loadingScene.SceneName);
    }

    public void LoadNextStage()
    {
        if (!_loadingScene.IsValid)
        {
            Debug.LogWarning("GameSceneController : 씬이 연결되지 않았음");
            return;
        }

        int currentStageId = GameManager.Instance.GetStageData().StageId;
        StageData nextStageData = _dataManager.StageDataDict[currentStageId + 1];

        GameManager.Instance.SetStageData(nextStageData);
        SceneManager.LoadScene(_loadingScene.SceneName);
    }
}
