using System;
using UnityEngine;

[Serializable]
public struct StageClearData
{
    public int StageId;
    public int Turn;
    public int KilledEnemy;
    public int LostPlayer;

    public StageClearData(int stageId, int turn, int killedEnemy, int lostPlayer)
    {
        StageId = stageId;
        Turn = turn;
        KilledEnemy = killedEnemy;
        LostPlayer = lostPlayer;
    }
}
