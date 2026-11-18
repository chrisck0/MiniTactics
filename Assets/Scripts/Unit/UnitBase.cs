using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[RequireComponent(typeof(UnitStat), typeof(Weapon), typeof(UnitDistanceRange))]
public abstract class UnitBase : MonoBehaviour, IPoolable, IDamageable
{
    [SerializeField] private UnitStat _stat;
    [SerializeField] private UnitDistanceRange _distanceRange;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private UnitAI _unitAI;
    [SerializeField] private UnitRotation _rotation;
    [SerializeField] private UnitAnimationController _animationController;
    [SerializeField] private float _moveSpeed = 8f;
    private bool _isMoving;
    private ObjectPool<UnitBase> _objectPool;

    public Action OnUnitCanAction;
    public Action OnUnitActioned;
    public Action<int> OnAttackWeapon;
    public Action OnSelect;
    public Action<bool> OnMove;
    public Action OnVictory;
    public Action OnAvoid;
    public Action OnDamage;
    public event Action<UnitBase> OnDead;

    public UnitStat Stat => _stat;
    public UnitDistanceRange DistanceRange => _distanceRange;
    public Weapon Weapon => _weapon;
    public UnitAI UnitAI => _unitAI;
    public UnitRotation Rotation => _rotation;
    public float MoveSpeed => _moveSpeed;
    public abstract UnitType UnitType { get; }

    public void ConsumeEnergy(int num)
    {
        _stat.AddEnergy(-num);
    }

    public void TakeDamage(int num)
    {
        _stat.AddHealth(-num);
        if (Stat.Health > num) OnDamage?.Invoke();
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void UnSelect()
    {
        Debug.Log("Unselect");
    }

    public void ToggleMove()
    {
        _isMoving = !_isMoving;
        OnMove?.Invoke(_isMoving);
    }

    public void Avoid()
    {
        Debug.Log($"{_stat.Data.Name} avoided");
        OnAvoid?.Invoke();
    }

    public void Die()
    {
        OnDead?.Invoke(this);
        WaitAndReturnToPool().Forget();
    }

    private async UniTask WaitAndReturnToPool()
    {
        await UniTask.Delay(2000, DelayType.Realtime);
        ReturnToPool();
    }

    public void SetObjectPool(ObjectPool<UnitBase> objectPool)
    {
        _objectPool = objectPool;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        _objectPool.Return(this);
    }
}
