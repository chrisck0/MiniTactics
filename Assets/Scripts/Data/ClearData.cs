using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClearData
{
    [SerializeField] private List<StageClearData> _stageClearDataList;

    public List<StageClearData> StageClearDataList => _stageClearDataList;

    public ClearData(List<StageClearData> stageClearDataList)
    {
        _stageClearDataList = stageClearDataList;
    }
}
