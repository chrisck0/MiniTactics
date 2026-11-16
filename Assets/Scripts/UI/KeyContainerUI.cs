using TMPro;
using UnityEngine;

public class KeyContainerUI : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI _keyText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void SetData(string key, string description)
    {
        _keyText.text = key;
        _descriptionText.text = description;
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
