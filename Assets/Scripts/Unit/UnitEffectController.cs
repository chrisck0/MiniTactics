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

    private void Attack(int weaponId)
    {
        ShowAttack(weaponId).Forget();
    }

    private async UniTask ShowAttack(int weaponId)
    {
        if (weaponId == 1)
        {
            await UniTask.Delay(_firstWeaponDelayTime, DelayType.DeltaTime);
            _firstWeaponEffect.SetActive(true);
            await UniTask.Delay(500, DelayType.DeltaTime);
            _firstWeaponEffect.SetActive(false);
        }
        else
        {
            await UniTask.Delay(_secondWeaponDelayTime, DelayType.DeltaTime);
            _secondWeaponEffect.SetActive(true);
            await UniTask.Delay(500, DelayType.DeltaTime);
            _secondWeaponEffect.SetActive(false);
        }

    }

    private void AttackWeapon1(InputAction.CallbackContext context)
    {
        ShowAttack(1).Forget();
    }

    private void AttackWeapon2(InputAction.CallbackContext context)
    {
        ShowAttack(2).Forget();
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
