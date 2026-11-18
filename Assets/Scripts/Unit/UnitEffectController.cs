using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitEffectController : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private GameObject _firstWeaponEffect;
    [SerializeField] private GameObject _secondWeaponEffect;
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
        if (weaponId == _owner.Stat.Data.WeaponIdList[0])
        {
            _firstWeaponEffect.SetActive(true);
            await UniTask.Delay(2000, DelayType.DeltaTime);
            _firstWeaponEffect.SetActive(false);
        }
        else
        {
            _secondWeaponEffect.SetActive(true);
            await UniTask.Delay(2000, DelayType.DeltaTime);
            _secondWeaponEffect.SetActive(false);
        }

    }

    private void AttackWeapon1(InputAction.CallbackContext context)
    {
        ShowAttack(_owner.Stat.Data.WeaponIdList[0]).Forget();
    }

    private void AttackWeapon2(InputAction.CallbackContext context)
    {
        ShowAttack(_owner.Stat.Data.WeaponIdList[1]).Forget();
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
