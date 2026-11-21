using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTransitionUI : MonoBehaviour
{
    [SerializeField] private GameObject _playerPhase;
    [SerializeField] private GameObject _enemyPhase;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerPhaseText()
    {
        _playerPhase.SetActive(true);
        _enemyPhase.SetActive(false);
    }

    public void SetEnemyPhaseText()
    {
        _playerPhase.SetActive(false);
        _enemyPhase.SetActive(true);
    }
}
