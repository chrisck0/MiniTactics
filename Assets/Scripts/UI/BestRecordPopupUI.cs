using System.Collections.Generic;
using UnityEngine;

public class BestRecordPopupUI : MonoBehaviour
{
    [SerializeField] private List<StageRecordUI> _stageRecordList;

    public void Show(ClearData clearData)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < _stageRecordList.Count; i++)
        {
            if (clearData != null && i < clearData.StageClearDataList.Count)
                _stageRecordList[i].SetData(clearData.StageClearDataList[i]);
            else
                _stageRecordList[i].SetData();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
