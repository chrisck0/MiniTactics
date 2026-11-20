using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TurnSaveData
{
    [SerializeField] private int _turn;
    [SerializeField] private List<UnitSaveData> _unitSaveDatas;
    [SerializeField] private Vector3 _cameraPosition;

    public int Turn => _turn;
    public List<UnitSaveData> UnitSaveDatas => _unitSaveDatas;
    public Vector3 CameraPosition => _cameraPosition;

    public TurnSaveData(int stageId, int turn, List<UnitSaveData> unitSaveDatas, Vector3 cameraPosition)
    {
        _turn = turn;
        _unitSaveDatas = unitSaveDatas;
        _cameraPosition = cameraPosition;
    }
}
