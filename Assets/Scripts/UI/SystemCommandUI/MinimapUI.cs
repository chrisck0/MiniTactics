using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _minimapTilePrefab;
    [SerializeField] private List<Sprite> _tileSprites;
    [SerializeField] private List<Sprite> _unitSprites;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private RectTransform _mapTransform;
    [SerializeField] private float _maxRectLength;

    private TileBase[,] _tileMap;
    private UnitBase[,] _unitMap;
    private Dictionary<TileType, Sprite> _tileSpriteDict = new();
    private Dictionary<UnitType, Sprite> _unitSpriteDict = new();
    private Dictionary<Vector2Int, Image> _minimapDict = new();

    private void CreateDictionaries()
    {
        for (int i = 0; i < _tileSprites.Count; i++)
        {
            _tileSpriteDict[(TileType)(i + 1)] = _tileSprites[i];
        }

        for (int i = 0; i < _unitSprites.Count; i++)
        {
            _unitSpriteDict[(UnitType)i] = _unitSprites[i];
        }
    }

    public void SetMap()
    {
        Debug.Log("SetMap");
        _tileMap = _spawnManager.TileMap;
        _unitMap = _spawnManager.UnitMap;

        int maxLength = Mathf.Max(_tileMap.GetLength(0), _tileMap.GetLength(1));
        float cellSize = _maxRectLength / maxLength;
        _gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        _mapTransform.sizeDelta = new Vector2(cellSize * _tileMap.GetLength(0), cellSize * _tileMap.GetLength(1));
        
        CreateDictionaries();
        SetInitialMap();
        LoadMinimap();
    }

    private void SetInitialMap()
    {
        for (int i = 0; i < _unitMap.GetLength(1); i++)
        {
            for (int j = 0; j < _unitMap.GetLength(0); j++)
            {
                GameObject minimapTile = Instantiate(_minimapTilePrefab, _mapTransform);
                Vector2Int coord = new Vector2Int(j, _unitMap.GetLength(1) - i - 1);
                _minimapDict[coord] = minimapTile.GetComponent<Image>();
            }
        }

        LoadMinimap();
    }

    public void LoadMinimap()
    {
        for (int i = 0; i < _unitMap.GetLength(1); i++)
        {
            for (int j = 0;  j < _unitMap.GetLength(0); j++)
            {
                Vector2Int coord = new Vector2Int(j, i);

                if (_unitMap[j, i] == null)
                    _minimapDict[coord].sprite = _tileSpriteDict[_tileMap[j, i].TileData.TileType];
                else
                    _minimapDict[coord].sprite = _unitSpriteDict[_unitMap[j, i].UnitType];
            }
        }
    }
}
