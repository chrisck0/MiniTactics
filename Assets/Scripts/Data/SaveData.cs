using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [SerializeField] private int _initialSeed;
    [SerializeField] private int _stageId;
    [SerializeField] private int _leftRevertTime;
    [SerializeField] private List<TurnSaveData> _turnSaveDataList;

    public int InitialSeed => _initialSeed;
    public int StageId => _stageId;
    public int LeftRevertTime => _leftRevertTime;
    public List<TurnSaveData> TurnSaveDataList => _turnSaveDataList;

    public SaveData(int initialSeed, int stageId, int leftRevertTime, List<TurnSaveData> turnSaveDataList)
    {
        _initialSeed = initialSeed;
        _stageId = stageId;
        _leftRevertTime = leftRevertTime;
        _turnSaveDataList = turnSaveDataList;
    }
}
