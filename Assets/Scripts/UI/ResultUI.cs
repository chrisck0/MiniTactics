using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private GameSceneController _gameSceneController;
    [SerializeField] private GameObject _gameClearText;
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _killedEnemyText;
    [SerializeField] private TextMeshProUGUI _lostPlayerText;
    [SerializeField] private Button _loadStageButton;
    [SerializeField] private TextMeshProUGUI _loadStageButtonText;
    [SerializeField] private Button _mainMenuButton;

    private bool _isGameClear;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public void SetData(bool isGameClear, int turn, int killedEnemy, int lostPlayer)
    {
        _isGameClear = isGameClear;

        if (isGameClear && _dataManager.StageDataDict.ContainsKey(GameManager.Instance.GetStageData().StageId + 1))
        {
            _gameClearText.SetActive(true);
            _gameOverText.SetActive(false);
            _loadStageButtonText.text = "다음 스테이지";
        }
        else
        {
            _gameClearText.SetActive(false);
            _gameOverText.SetActive(true);
            _loadStageButtonText.text = "다시하기";
        }

        _turnText.text = turn.ToString();
        _killedEnemyText.text = killedEnemy.ToString();
        _lostPlayerText.text = lostPlayer.ToString();

        gameObject.SetActive(true);
    }

    private void LoadStage()
    {
        if (_isGameClear && _dataManager.StageDataDict.ContainsKey(GameManager.Instance.GetStageData().StageId + 1))
            _gameSceneController.LoadNextStage();
        else
            _gameSceneController.LoadCurrentStage();
    }

    private void LoadMainMenu()
    {
        _gameSceneController.LoadMainMenu();
    }

    private void BindButtonEvents()
    {
        _loadStageButton.onClick.AddListener(LoadStage);
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void UnbindButtonEvents()
    {
        _loadStageButton.onClick.RemoveListener(LoadStage);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
    }
}
