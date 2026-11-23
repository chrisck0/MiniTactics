using TMPro;
using UnityEngine;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    private ObjectPool<TileBase> _objectPool;

    [SerializeField] private TileData _tileData;
    [SerializeField] private GameObject _blueTransparentTile;
    [SerializeField] private GameObject _redTransparentTile;
    [SerializeField] private GameObject _darkRedTransparentTile;
    private bool _canSelect;

    public TileData TileData => _tileData;
    public bool CanSelect => _canSelect;

    private void Awake() => InitTile();

    public void SetCanSelect()
    {
        _canSelect = true;
    }

    public void ShowDistance()
    {
        _blueTransparentTile.gameObject.SetActive(true);
        _redTransparentTile.gameObject.SetActive(false);
    }

    public void HideDistance()
    {
        _blueTransparentTile.gameObject.SetActive(false);
    }

    public void ShowRange()
    {
        _redTransparentTile.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        _redTransparentTile.gameObject.SetActive(false);
    }

    public void ShowDanger()
    {
        _darkRedTransparentTile.gameObject.SetActive(true);
    }

    public void HideDanger()
    {
        _darkRedTransparentTile.gameObject.SetActive(false);
    }

    public void InitTile()
    {
        _blueTransparentTile.gameObject.SetActive(false);
        _redTransparentTile.gameObject.SetActive(false);
        _canSelect = false;
    }

    public void InitSelect()
    {
        _canSelect = false;
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
        _objectPool.Return(this);
    }
}
