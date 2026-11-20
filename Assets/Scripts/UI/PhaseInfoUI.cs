using TMPro;
using UnityEngine;

public class PhaseInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerPhaseText;
    [SerializeField] private TextMeshProUGUI _enemyPhaseText;

    public void SetPlayerPhase()
    {
        _playerPhaseText.gameObject.SetActive(true);
        _enemyPhaseText.gameObject.SetActive(false);
    }

    public void SetEnemyPhase()
    {
        _playerPhaseText.gameObject.SetActive(false);
        _enemyPhaseText.gameObject.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
