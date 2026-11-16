using TMPro;
using UnityEngine;

public class UnitContainer : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _moveText;
    [SerializeField] private TextMeshProUGUI _accuracyText;
    [SerializeField] private TextMeshProUGUI _avoidText;
    [SerializeField] private TextMeshProUGUI _weaponText;

    public void SetData(UnitBase unit)
    {
        _nameText.text = unit.Stat.Data.Name.ToString();
        _healthText.text = $"{unit.Stat.Health} / {unit.Stat.Data.Health}";
        _energyText.text = $"{unit.Stat.Energy} / {unit.Stat.Data.Energy}";
        _moveText.text = unit.Stat.Data.Move.ToString();
        _accuracyText.text = unit.Stat.Data.Accuracy.ToString();
        _avoidText.text = unit.Stat.Data.Avoid.ToString();

        string weaponList = "";
        for (int i = 0; i < unit.Weapon.WeaponDataList.Count; i++)
        {
            if (i == 0) weaponList += unit.Weapon.WeaponDataList[i].Name.ToString();
            else weaponList += $" / {unit.Weapon.WeaponDataList[i].Name}";
        }
        _weaponText.text = weaponList;
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
    }
}
