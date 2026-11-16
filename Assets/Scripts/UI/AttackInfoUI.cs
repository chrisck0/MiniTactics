using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AttackInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _weaponText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _energyText;

    public void SetData(WeaponData weaponData)
    { 
        _weaponText.text = weaponData.Name.ToString();
        _attackText.text = weaponData.Attack.ToString();
        _energyText.text = weaponData.EnergyCost.ToString();
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
