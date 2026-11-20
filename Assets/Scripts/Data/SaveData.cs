using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [SerializeField] private int _initialSeed;
    [SerializeField] private int _stageId;
    [SerializeField] private List<TurnSaveData> _turnSaveDataList;

    public int InitialSeed => _initialSeed;
    public int StageId => _stageId;
    public List<TurnSaveData> TurnSaveDataList => _turnSaveDataList;

    public SaveData(int initialSeed, int stageId, List<TurnSaveData> turnSaveDataList)
    {
        _initialSeed = initialSeed;
        _stageId = stageId;
        _turnSaveDataList = turnSaveDataList;
    }
}
