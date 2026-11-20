using System;
using UnityEngine;

[Serializable]
public struct UnitPosition
{
    public int Id;
    public Vector2Int Position;
    public Vector3 Direction;

    public UnitPosition(int id, Vector2Int position, Vector3 direction)
    {
        Id = id;
        Position = position;
        Direction = direction;
    }
}