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

        EnemyUnit enemyUnit = selectedUnit as EnemyUnit;

        if (enemyUnit == null || enemyUnit.EnemyAI.MaxAttackScore.Score == 0)
        {
            _unitWeaponNameText.text = selectedUnit.Weapon.WeaponDataList[0].Name;
        }
        else
        {
            _unitWeaponNameText.text = enemyUnit.EnemyAI.MaxAttackScore.AttackWeaponData.Name;
        }
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
