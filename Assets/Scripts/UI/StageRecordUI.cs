using TMPro;
using UnityEngine;

public class StageRecordUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _killedEnemyText;
    [SerializeField] private TextMeshProUGUI _lostPlayerText;
    [SerializeField] private TextMeshProUGUI _noRecordText;

    public void SetData()
    {
        _noRecordText.gameObject.SetActive(true);
        _turnText.gameObject.SetActive(false);
        _killedEnemyText.gameObject.SetActive(false);
        _lostPlayerText.gameObject.SetActive(false);
    }

    public void SetData(StageClearData stageClearData)
    {
        _turnText.text = stageClearData.Turn.ToString();
        _killedEnemyText.text = stageClearData.KilledEnemy.ToString();
        _lostPlayerText.text = stageClearData.LostPlayer.ToString();

        _noRecordText.gameObject.SetActive(false);
        _turnText.gameObject.SetActive(true);
        _killedEnemyText.gameObject.SetActive(true);
        _lostPlayerText.gameObject.SetActive(true);
    }
}
