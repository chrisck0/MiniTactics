using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    private const string SAVE_DATA_NAME = "save.json";
    private const string CLEAR_DATA_NAME = "clear.json";

    [SerializeField] private PhaseStateController _phaseStateController;

    private string _savePath;
    private string _clearPath;

    private void Awake() => CachePath();

    public void SaveNewData()
    {
        SaveData saveData = new SaveData(
            GameManager.Instance.GetInitialSeed(),
            GameManager.Instance.GetStageData().StageId, 
            GameManager.Instance.GetLeftRevertTime(),
            _phaseStateController.TurnSaveDataList
            );

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(_savePath, json);
    }

    public void SaveClearData(StageClearData stageClearData)
    {
        ClearData clearData = LoadClearData();

        if (clearData == null)
        {
            List<StageClearData> stageClearDataList = new();
            clearData = new ClearData(stageClearDataList);
        }

        if (clearData.StageClearDataList.Count >= stageClearData.StageId)
        {
            StageClearData prevStageClearData = clearData.StageClearDataList[stageClearData.StageId - 1];

            if (CheckIsNewRecord(prevStageClearData, stageClearData))
                clearData.StageClearDataList[stageClearData.StageId - 1] = stageClearData;
        }
        else
            clearData.StageClearDataList.Add(stageClearData);

        string json = JsonUtility.ToJson(clearData, true);
        File.WriteAllText(_clearPath, json);
    }

    private bool CheckIsNewRecord(StageClearData prevData, StageClearData newData)
    {
        if (prevData.Turn > newData.Turn) return true;
        if (prevData.Turn < newData.Turn) return false;

        if (prevData.LostPlayer > newData.LostPlayer) return true;
        if (prevData.LostPlayer < newData.LostPlayer) return false;

        if (prevData.KilledEnemy > newData.KilledEnemy) return false;
        if (prevData.KilledEnemy < newData.KilledEnemy) return true;

        return false;
    }

    public void ClearSaveData()
    {
        if (!File.Exists(_savePath))
        {
            return;
        }

        File.Delete(_savePath);
    }

    public SaveData LoadSaveData()
    {
        if (!File.Exists(_savePath))
        {
            Debug.Log("세이브 파일 없음");
            return null;
        }

        string json = File.ReadAllText(_savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.Log("SaveLoader: 저장 파일이 비어 있음");
            return null;
        }

        return data;
    }

    public ClearData LoadClearData()
    {
        if (!File.Exists(_clearPath))
        {
            Debug.Log("클리어 파일 없음");
            return null;
        }

        string json = File.ReadAllText(_clearPath);
        ClearData data = JsonUtility.FromJson<ClearData>(json);

        if (data == null)
        {
            Debug.Log("SaveLoader: 클리어 파일이 비어 있음");
            return null;
        }

        return data;
    }

    private void CachePath()
    {
        _savePath = Path.Combine(Application.persistentDataPath, SAVE_DATA_NAME);
        _clearPath = Path.Combine(Application.persistentDataPath, CLEAR_DATA_NAME);
    }
}
