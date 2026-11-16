using System.Collections.Generic;
using UnityEngine;

public class UnitContainerListUI : MonoBehaviour
{
    [SerializeField] private UnitContainer _unitContainerPrefab;
    [SerializeField] private Transform _contentTransform;
    private ObjectPool<UnitContainer> _unitContainerObjectPool;
    private List<UnitContainer> _activatedUnitContainers = new();

    private void Awake() => CreatePool();

    private void CreatePool()
    {
        List<UnitContainer> unitContainers = new();
        unitContainers.Add(_unitContainerPrefab);
        _unitContainerObjectPool = new ObjectPool<UnitContainer>(unitContainers, _contentTransform, false);
    }

    public void ShowUnitContainers(List<UnitBase> unitList)
    {
        foreach (UnitBase unit in unitList)
        {
            UnitContainer unitContainer = _unitContainerObjectPool.Take(_unitContainerPrefab);
            _activatedUnitContainers.Add(unitContainer);
            unitContainer.SetData(unit);
        }
    }

    public void HideAllUnitContainers()
    {
        foreach (UnitContainer unitContainer in _activatedUnitContainers)
        {
            _unitContainerObjectPool.Return(unitContainer);
        }

        _activatedUnitContainers.Clear();
    }
}
