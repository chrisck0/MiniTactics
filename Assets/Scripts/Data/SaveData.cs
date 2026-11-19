using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    [SerializeField] private int _stageId;
    [SerializeField] private int _turn;
    [SerializeField] private List<UnitSaveData> _unitSaveDatas;
    [SerializeField] private Vector3 _cameraPosition;

    public int StageId => _stageId;
    public int Turn => _turn;
    public List<UnitSaveData> UnitSaveDatas => _unitSaveDatas;
    public Vector3 CameraPosition => _cameraPosition;

    public SaveData(int stageId, int turn, List<UnitSaveData> unitSaveDatas, Vector3 cameraPosition)
    {
        _stageId = stageId;
        _turn = turn;
        _unitSaveDatas = unitSaveDatas;
        _cameraPosition = cameraPosition;
    }
}
