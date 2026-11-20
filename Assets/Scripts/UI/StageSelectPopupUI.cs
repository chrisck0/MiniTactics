using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageSelectPopupUI : MonoBehaviour
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    private StageSelectUI _stageSelectUI;
    private StageSelectController _stageSelectController;

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
}
