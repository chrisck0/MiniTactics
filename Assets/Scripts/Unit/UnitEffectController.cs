using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitEffectController : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private GameObject _firstWeaponEffect;
    [SerializeField] private GameObject _secondWeaponEffect;
    [SerializeField] private int _firstWeaponDelayTime;
    [SerializeField] private int _secondWeaponDelayTime;
    [SerializeField] private InputActionReference _lmbAction;
    [SerializeField] private InputActionReference _rmbAction;

    private void OnEnable() => BindUnitEvents();
    private void OnDisable() => UnbindUnitEvents();

    private void Attack(AttackScore attackScore)
    {
        ShowAttack(attackScore).Forget();
    }

    private async UniTask ShowAttack(AttackScore attackScore)
    {
        if (attackScore.AttackWeaponData.Id == //_owner.Stat.Data.WeaponIdList[0])
            1)
        {
            await ShowAttackWeapon1(attackScore);
        }
        else
        {
            await ShowAttackWeapon2(attackScore);
        }

    }

    private async UniTask ShowAttackWeapon1(AttackScore attackScore)
    {
        Projectile projectile = _firstWeaponEffect.GetComponent<Projectile>();

        if (projectile == null)
        {
            await NormalEffect(_firstWeaponDelayTime, _firstWeaponEffect);
        }
        else
        {
            await ProjectileEffect(_firstWeaponDelayTime, projectile, attackScore);
        }
    }

    private async UniTask ShowAttackWeapon2(AttackScore attackScore)
    {
        Projectile projectile = _secondWeaponEffect.GetComponent<Projectile>();

        if (projectile == null)
        {
            await NormalEffect(_secondWeaponDelayTime, _secondWeaponEffect);
        }
        else
        {
            await ProjectileEffect(_secondWeaponDelayTime, projectile, attackScore);
        }
    }


    private async UniTask NormalEffect(int delayTime, GameObject effect)
    {
        await UniTask.Delay(delayTime, DelayType.DeltaTime);
        effect.SetActive(true);
        await UniTask.Delay(500, DelayType.DeltaTime);
        effect.SetActive(false);
    }

    private async UniTask ProjectileEffect(int delayTime, Projectile projectile, AttackScore attackScore)
    {
        await UniTask.Delay(delayTime, DelayType.DeltaTime);
        projectile.Shoot(transform.position, transform.forward, attackScore);
    }

    private void AttackWeapon1(InputAction.CallbackContext context)
    {
        ShowAttackWeapon1(new AttackScore(
            transform.position.WorldToCoord(),
            null,
            DirectionType.None,
            transform.position.WorldToCoord() + new Vector2Int(0, 4),
            0
            )).Forget();
    }

    private void AttackWeapon2(InputAction.CallbackContext context)
    {
        ShowAttackWeapon2(new AttackScore(
            transform.position.WorldToCoord(),
            null,
            DirectionType.Up,
            transform.position.WorldToCoord(),
            0
            )).Forget();
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
