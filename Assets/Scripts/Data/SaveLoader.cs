using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    private const string FILE_NAME = "save.json";
    [SerializeField] private PhaseStateController _phaseStateController;

    private string _path;

    private void Awake() => CachePath();

    public void SaveNewData()
    {
        SaveData saveData = new SaveData(
            GameManager.Instance.GetInitialSeed(),
            GameManager.Instance.GetStageData().StageId, 
            _phaseStateController.TurnSaveDataList
            );

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(_path, json);
    }

    public void ClearSaveData()
    {
        if (!File.Exists(_path))
        {
            return;
        }

        File.Delete(_path);
    }

    public SaveData Load()
    {
        if (!File.Exists(_path))
        {
            Debug.Log("세이브 파일 없음");
            return null;
        }

        string json = File.ReadAllText(_path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.Log("SaveLoader: 저장 파일이 비어 있음");
            return null;
        }

        if (data.StageId != GameManager.Instance.GetStageData().StageId)
        {
            Debug.Log("세이브 데이터와 다른 스테이지");
            return data;
        }

        return data;
    }

    private void CachePath()
    {
        _path = Path.Combine(Application.persistentDataPath, FILE_NAME);
    }
}
