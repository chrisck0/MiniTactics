using UnityEngine;
using UnityEngine.UI;

public class EnemyDangerRangeUI : MonoBehaviour
{
    [SerializeField] private Image _dangerRangeImage;
    [SerializeField] private UnitAI _unitAI;

    private void Awake() => ToggleDangerRangeImage(false);
    private void OnEnable() => BindEnemyAIEvents();
    private void OnDisable() => UnbindEnemyAIEvents();

    private void ToggleDangerRangeImage(bool isShowingDanger)
    {
        _dangerRangeImage.gameObject.SetActive(isShowingDanger);
    }

    private void BindEnemyAIEvents()
    {
        _unitAI.OnDangerRangeVisibilityChanged += ToggleDangerRangeImage;
    }

    private void UnbindEnemyAIEvents()
    {
        _unitAI.OnDangerRangeVisibilityChanged -= ToggleDangerRangeImage;
    }
}
