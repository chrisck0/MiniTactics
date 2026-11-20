using System;

[Serializable]
public struct UnitSaveData
{
    public UnitPosition UnitPosition;
    public int Health;
    public int Energy;

    public UnitSaveData(UnitPosition unitPosition, int health, int energy)
    {
        UnitPosition = unitPosition;
        Health = health;
        Energy = energy;
    }
}
