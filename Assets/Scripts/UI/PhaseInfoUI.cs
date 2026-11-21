using TMPro;
using UnityEngine;

public class PhaseInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _playerPhase;
    [SerializeField] private GameObject _enemyPhase;

    public void SetPlayerPhase()
    {
        _playerPhase.SetActive(true);
        _enemyPhase.SetActive(false);
    }

    public void SetEnemyPhase()
    {
        _playerPhase.SetActive(false);
        _enemyPhase.SetActive(true);
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
