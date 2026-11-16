using System.Collections.Generic;
using UnityEngine;

public class KeyInfoUI : MonoBehaviour
{
    [SerializeField] private KeyContainerUI _keyContainerPrefab;
    private ObjectPool<KeyContainerUI> _containerObjectPool;
    private List<KeyContainerUI> _activatedContainers = new();

    private void Awake() => CreatePool();
    private void OnDisable() => ReturnKeyContainers();

    private void CreatePool()
    {
        List<KeyContainerUI> keyContainers = new();
        keyContainers.Add(_keyContainerPrefab);
        _containerObjectPool = new ObjectPool<KeyContainerUI>(keyContainers, transform, false);
    }

    public void SetSelectKey(int siblingIndex)
    {
        KeyContainerUI keyContainer = _containerObjectPool.Take(_keyContainerPrefab);
        keyContainer.SetData("LMB", "선택");
        keyContainer.transform.SetSiblingIndex(siblingIndex);
        _activatedContainers.Add(keyContainer);
    }

    public void SetInfoKey(int siblingIndex)
    {
        KeyContainerUI keyContainer = _containerObjectPool.Take(_keyContainerPrefab);
        keyContainer.SetData("RMB", "상세 정보");
        keyContainer.transform.SetSiblingIndex(siblingIndex);
        _activatedContainers.Add(keyContainer);
    }

    public void SetCancelKey(int siblingIndex)
    {
        KeyContainerUI keyContainer = _containerObjectPool.Take(_keyContainerPrefab);
        keyContainer.SetData("ESC", "취소");
        keyContainer.transform.SetSiblingIndex(siblingIndex);
        _activatedContainers.Add(keyContainer);
    }

    private void ReturnKeyContainers()
    {
        foreach (KeyContainerUI keyContainer in _activatedContainers)
        {
            _containerObjectPool.Return(keyContainer);
        }

        _activatedContainers.Clear();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
