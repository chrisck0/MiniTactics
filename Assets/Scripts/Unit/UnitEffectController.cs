using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitEffectController : MonoBehaviour
{
    [SerializeField] private ProjectileObjectPool _projectileObjectPool;
    [SerializeField] private UnitBase _owner;
    [SerializeField] private GameObject _firstWeaponEffect;
    [SerializeField] private GameObject _secondWeaponEffect;
    [SerializeField] private int _firstWeaponDelayTime;
    [SerializeField] private int _secondWeaponDelayTime;
    [SerializeField] private InputActionReference _lmbAction;
    [SerializeField] private InputActionReference _rmbAction;

    private void OnEnable() => BindUnitEvents();
    private void OnDisable() => UnbindUnitEvents();

    private void Attack(int weaponId)
    {
        ShowAttack(weaponId).Forget();
    }

    private async UniTask ShowAttack(int weaponId)
    {
        if (weaponId == //_owner.Stat.Data.WeaponIdList[0])
            1)
        {
            await ShowAttackWeapon1();
        }
        else
        {
            await ShowAttackWeapon2();
        }

    }

    private async UniTask ShowAttackWeapon1()
    {
        ProjectileMover projectile = _firstWeaponEffect.GetComponent<ProjectileMover>();

        if (projectile == null)
        {
            await NormalEffect(_firstWeaponDelayTime, _firstWeaponEffect);
        }
        else
        {
            await ProjectileEffect(_firstWeaponDelayTime, projectile);
        }
    }

    private async UniTask ShowAttackWeapon2()
    {
        ProjectileMover projectile = _secondWeaponEffect.GetComponent<ProjectileMover>();

        if (projectile == null)
        {
            await NormalEffect(_secondWeaponDelayTime, _secondWeaponEffect);
        }
        else
        {
            await ProjectileEffect(_secondWeaponDelayTime, projectile);
        }
    }


    private async UniTask NormalEffect(int delayTime, GameObject effect)
    {
        await UniTask.Delay(delayTime, DelayType.DeltaTime);
        effect.SetActive(true);
        await UniTask.Delay(500, DelayType.DeltaTime);
        effect.SetActive(false);
    }

    private async UniTask ProjectileEffect(int delayTime, ProjectileMover projectile)
    {
        await UniTask.Delay(delayTime, DelayType.DeltaTime);
        _projectileObjectPool.Shoot(projectile, transform);
    }

    private void AttackWeapon1(InputAction.CallbackContext context)
    {
        ShowAttackWeapon1().Forget();
    }

    private void AttackWeapon2(InputAction.CallbackContext context)
    {
        ShowAttackWeapon2().Forget();
    }

    private void BindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
        _lmbAction.action.Enable();
        _lmbAction.action.performed += AttackWeapon1;
        _rmbAction.action.Enable();
        _rmbAction.action.performed += AttackWeapon2;
    }

    private void UnbindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
        _lmbAction.action.Disable();
        _lmbAction.action.performed -= AttackWeapon1;
        _rmbAction.action.Disable();
        _rmbAction.action.performed -= AttackWeapon2;
    }
}
