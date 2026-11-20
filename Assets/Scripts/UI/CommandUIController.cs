using UnityEngine;

public class CommandUIController : MonoBehaviour
{
    [SerializeField] private PhaseInfoUI _phaseInfoUI;
    [SerializeField] private SystemCommandUI _systemCommandUI;
    [SerializeField] private UnitCommandUI _unitCommandUI;
    [SerializeField] private AttackCommandUI _attackCommandUI;
    [SerializeField] private TileInfoUI _tileInfoUI;
    [SerializeField] private UnitInfoUI _unitInfoUI;
    [SerializeField] private AttackInfoUI _attackInfoUI;
    [SerializeField] private UnitDetailedInfoUI _unitDetailedInfoUI;
    [SerializeField] private BattleInfoUI _battleInfoUI;
    [SerializeField] private KeyInfoUI _keyInfoUI;

    public PhaseInfoUI PhaseInfoUI => _phaseInfoUI;
    public SystemCommandUI SystemCommandUI => _systemCommandUI;
    public UnitCommandUI UnitCommandUI => _unitCommandUI;
    public AttackCommandUI AttackCommandUI => _attackCommandUI;
    public TileInfoUI TileInfoUI => _tileInfoUI;
    public UnitInfoUI UnitInfoUI => _unitInfoUI;
    public AttackInfoUI AttackInfoUI => _attackInfoUI;
    public UnitDetailedInfoUI UnitDetailedInfoUI => _unitDetailedInfoUI;
    public BattleInfoUI BattleInfoUI => _battleInfoUI;
    public KeyInfoUI KeyInfoUI => _keyInfoUI;
    public bool IsSystemCommandPopupOpen => _systemCommandUI.IsPopupOpen;

    public void DeactivateAll()
    {
        _phaseInfoUI.gameObject.SetActive(false);
        _systemCommandUI.gameObject.SetActive(false);
        _unitCommandUI.gameObject.SetActive(false);
        _attackCommandUI.gameObject.SetActive(false);
        _tileInfoUI.gameObject.SetActive(false);
        _unitInfoUI.gameObject.SetActive(false);
        _attackInfoUI.gameObject.SetActive(false);
        _unitDetailedInfoUI.gameObject.SetActive(false);
        _battleInfoUI.gameObject.SetActive(false);
        _keyInfoUI.gameObject.SetActive(false);
    }
}
