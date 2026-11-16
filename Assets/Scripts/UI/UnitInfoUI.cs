using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitNameText;
    [SerializeField] private TextMeshProUGUI _unitHealthText;
    [SerializeField] private TextMeshProUGUI _unitEnergyText;
    [SerializeField] private TextMeshProUGUI _unitWeaponNameText;

    public void SetData(UnitBase selectedUnit)
    {
        _unitNameText.text = selectedUnit.Stat.Data.Name;
        _unitHealthText.text = $"{selectedUnit.Stat.Health} / {selectedUnit.Stat.Data.Health}";
        _unitEnergyText.text = $"{selectedUnit.Stat.Energy} / {selectedUnit.Stat.Data.Energy}";

        string weaponList = "";
        for (int i = 0; i < selectedUnit.Weapon.WeaponDataList.Count; i++)
        {
            if (i == 0) weaponList += selectedUnit.Weapon.WeaponDataList[i].Name.ToString();
            else weaponList += $" / {selectedUnit.Weapon.WeaponDataList[i].Name}";
        }
        _unitWeaponNameText.text = weaponList;
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
