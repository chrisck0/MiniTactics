using UnityEngine;
using System;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private UnitBase _unit;
    [SerializeField] private UnitData _data;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;
    private BehaviorType _behaviorType;

    public event Action<int> OnHealthChanged;

    public UnitData Data => _data;
    public int Health => _health;
    public int Energy => _energy;
    public BehaviorType BehaviorType => _behaviorType;

    private void Awake() => CacheComponents();

    public void InitStat(UnitData data)
    {
        _behaviorType = data.BehaviorType;
        _data = data;
        _health = _data.Health;
        _energy = _data.Energy;
        OnHealthChanged?.Invoke(_health);
    }

    public void InitStat(UnitData data, int health, int energy)
    {
        _behaviorType = data.BehaviorType;
        _data = data;
        _health = health;
        _energy = energy;
        OnHealthChanged?.Invoke(_health);
    }

    public void AddEnergy(int num)
    {
        _energy += num;

        _energy = Mathf.Clamp(_energy, 0, _data.Energy);
    }

    public void AddHealth(int num)
    {
        _health += num;
        _health = Mathf.Clamp(_health, 0, _data.Health);

        if (_health == 0)
        {
            _unit.Die();
        }
        OnHealthChanged?.Invoke(_health);
    }

    public void ChangeBehaviorType(BehaviorType behaviorType)
    {
        _behaviorType = behaviorType;
    }

    private void CacheComponents()
    {
        _unit = GetComponent<UnitBase>();
    }
}
