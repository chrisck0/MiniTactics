using UnityEngine;
using System;

[Serializable]
public class UnitSaveData
{
    [SerializeField] private UnitPosition _unitPosition;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;

    public UnitPosition UnitPosition => _unitPosition;
    public int Health => _health;
    public int Energy => _energy;

    public UnitSaveData(UnitPosition unitPosition, int health, int energy)
    {
        _unitPosition = unitPosition;
        _health = health;
        _energy = energy;
    }
}
