using UnityEngine;

public class KeyInfoUI : MonoBehaviour
{
    [SerializeField] private KeyContainerUI _keyContainerPrefab;
    private ObjectPool<KeyContainerUI> _containerObjectPool;
}
